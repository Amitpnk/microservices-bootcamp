using AutoMapper;
using EvenTicket.Services.ShoppingBasket.Models;
using EvenTicket.Services.ShoppingBasket.Repositories;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using EvenTicket.Grpc;
using EvenTicket.Services.ShoppingBasket.Services;
using static Azure.Core.HttpHeader;
using Coupon = EvenTicket.Services.ShoppingBasket.Models.Coupon;
using Microsoft.Extensions.Logging;

namespace EvenTicket.Services.ShoppingBasket.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketsController(
        IBasketRepository basketRepository,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<BasketsController> logger,
        ILogger<DiscountService> discountLogger) 
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

                int total = 0;

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
                    total = total - coupon.Amount;
                }
                else
                {
                    total = total;
                }

                await basketRepository.ClearBasket(basketCheckout.BasketId);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.StackTrace);
            }
        }
    }
}
