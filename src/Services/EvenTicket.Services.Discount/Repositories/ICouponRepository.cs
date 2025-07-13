using EvenTicket.Services.Discount.Entities;

namespace EvenTicket.Services.Discount.Repositories;

public interface ICouponRepository
{
    Task<Coupon> GetCouponByCode(string couponCode);
    Task UseCoupon(Guid couponId);
    Task<Coupon> GetCouponById(Guid couponId);
}