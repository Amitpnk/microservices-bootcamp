using AutoMapper;
using EvenTicket.Services.Discount.Models;
using EvenTicket.Services.Discount.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EvenTicket.Services.Discount.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscountController(ICouponRepository couponRepository, IMapper mapper) : ControllerBase
{
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetDiscountForCode(string code)
    {
        var coupon = await couponRepository.GetCouponByCode(code);

        if (coupon == null)
            return NotFound();

        return Ok(mapper.Map<CouponDto>(coupon));
    }

    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [HttpGet("{couponId}")]
    public async Task<IActionResult> GetDiscountForCode(Guid couponId)
    {

        var coupon = await couponRepository.GetCouponById(couponId);

        if (coupon == null)
            return NotFound();

        return Ok(mapper.Map<CouponDto>(coupon));
    }

    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [HttpGet("error/{couponId}")]
    public async Task<IActionResult> GetDiscountForCode2(Guid couponId)
    {

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        var coupon = await couponRepository.GetCouponById(couponId);

        if (coupon == null)
            return NotFound();

        return Ok(mapper.Map<CouponDto>(coupon));
    }

    [HttpPut("use/{couponId}")]
    public async Task<IActionResult> UseCoupon(Guid couponId)
    {
        await couponRepository.UseCoupon(couponId);
        return Ok();
    }
}