using AutoMapper;
using EvenTicket.Grpc;
using EvenTicket.Services.ShoppingBasket.Models;
using EvenTicket.Services.ShoppingBasket.Repositories;
using EvenTicket.Services.ShoppingBasket.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using EvenTicket.Infrastructure.MessagingBus;
using EvenTicket.Services.ShoppingBasket.Messages;
using Coupon = EvenTicket.Services.ShoppingBasket.Models.Coupon;

namespace EvenTicket.Services.ShoppingBasket.Controllers;

[Route("api/baskets")]
[ApiController]
public class BasketsController(
    IBasketRepository basketRepository,
    IMapper mapper,
    IConfiguration configuration,
    ILogger<BasketsController> logger,
    ILogger<DiscountService> discountLogger,
    IMessageBus messageBus)
    : ControllerBase
{
    private readonly string _grpcServiceAddress = configuration["GrpcService:Address"];

    [HttpGet("{basketId}", Name = "GetBasket")]
    public async Task<ActionResult<Basket>> Get(Guid basketId)
    {
        var basket = await basketRepository.GetBasketById(basketId);
        if (basket == null)
        {
            return NotFound();
        }

        var result = mapper.Map<Basket>(basket);
        result.NumberOfItems = basket.BasketLines.Sum(bl => bl.TicketAmount);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Basket>> Post(BasketForCreation basketForCreation)
    {
        var basketEntity = mapper.Map<Entities.Basket>(basketForCreation);

        basketRepository.AddBasket(basketEntity);
        await basketRepository.SaveChanges();

        var basketToReturn = mapper.Map<Basket>(basketEntity);

        return CreatedAtRoute(
            "GetBasket",
            new { basketId = basketEntity.BasketId },
            basketToReturn);
    }

    [HttpPut("{basketId}/coupon")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ApplyCouponToBasket(Guid basketId, Coupon coupon)
    {
        var basket = await basketRepository.GetBasketById(basketId);

        if (basket == null)
        {
            return BadRequest();
        }

        basket.CouponId = coupon.CouponId;
        await basketRepository.SaveChanges();

        return Accepted();
    }


    [HttpPost("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> CheckoutBasketAsync([FromBody] BasketCheckout basketCheckout)
    {
        try
        {
            //based on basket checkout, fetch the basket lines from repo
            var basket = await basketRepository.GetBasketById(basketCheckout.BasketId);

            if (basket == null)
            {
                return BadRequest();
            }

            var basketCheckoutMessage = mapper.Map<BasketCheckoutMessage>(basketCheckout);
            basketCheckoutMessage.BasketLines = new List<BasketLineMessage>();
            basketCheckoutMessage.CreationDateTime = DateTime.UtcNow;
            basketCheckoutMessage.Id = Guid.NewGuid();

            int total = 0;

            foreach (var b in basket.BasketLines)
            {
                var basketLineMessage = new BasketLineMessage
                {
                    BasketLineId = b.BasketLineId,
                    Price = b.Price,
                    TicketAmount = b.TicketAmount
                };

                total += b.Price * b.TicketAmount;

                basketCheckoutMessage.BasketLines.Add(basketLineMessage);
            }

            //apply discount by talking to the discount service
            Coupon coupon = null;

            var channel = GrpcChannel.ForAddress(_grpcServiceAddress);
            var discountService = new DiscountService(new Discounts.DiscountsClient(channel), discountLogger);

            if (basket.CouponId.HasValue)
            {
                coupon = await discountService.GetCoupon(basket.CouponId.Value);
            }


            if (coupon != null)
            {
                basketCheckoutMessage.BasketTotal = total - coupon.Amount;
            }
            else
            {
                basketCheckoutMessage.BasketTotal = total;
            }


          
            try
            {
                await messageBus.PublishMessage(basketCheckoutMessage, "checkoutmessage");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //await basketRepository.ClearBasket(basketCheckout.BasketId);
            return Accepted(basketCheckoutMessage);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
        }
    }
}