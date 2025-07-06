using EvenTicket.Grpc;
using Coupon = EvenTicket.Services.ShoppingBasket.Models.Coupon;

namespace EvenTicket.Services.ShoppingBasket.Services
{
    public class DiscountService(Discounts.DiscountsClient discountsService, ILogger<DiscountService> logger) : IDiscountService
    {
        public async Task<Coupon> GetCoupon(Guid couponId)
        {
            try
            {
                var getCouponByIdRequest = new GetCouponByIdRequest { CouponId = couponId.ToString() };

                var getCouponByIdResponse = await discountsService.GetCouponAsync(getCouponByIdRequest);

                var coupon = new Coupon
                {
                    Code = getCouponByIdResponse.Coupon.Code,
                    Amount = getCouponByIdResponse.Coupon.Amount,
                    AlreadyUsed = getCouponByIdResponse.Coupon.AlreadyUsed,
                    CouponId = Guid.Parse(getCouponByIdResponse.Coupon.CouponId)
                };

                return coupon;
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                throw;
            }

        }


    }
}
