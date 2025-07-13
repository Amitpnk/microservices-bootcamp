using EvenTicket.Services.ShoppingBasket.Models;
using System;
using System.Threading.Tasks;

namespace EvenTicket.Services.ShoppingBasket.Services;

public interface IDiscountService
{
    Task<Coupon> GetCoupon(Guid couponId);
    //Task<Coupon> GetCouponWithError(Guid couponId);
}