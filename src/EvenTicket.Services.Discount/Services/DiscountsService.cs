using AutoMapper;
using EvenTicket.Grpc;
using EvenTicket.Services.Discount.Repositories;
using Grpc.Core;

namespace EvenTicket.Services.Discount.Services;

public class DiscountsService : Discounts.DiscountsBase
{
    private readonly ICouponRepository couponRepository;
    private readonly IMapper mapper;

    public DiscountsService(IMapper mapper, ICouponRepository couponRepository)
    {
        this.mapper = mapper;
        this.couponRepository = couponRepository;
    }

    public override async Task<GetCouponByIdResponse> GetCoupon(GetCouponByIdRequest request, ServerCallContext context)
    {
        var response = new GetCouponByIdResponse();

        if (!Guid.TryParse(request.CouponId, out var couponGuid))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid CouponId"));

        var coupon = await couponRepository.GetCouponById(Guid.Parse(request.CouponId));
        if (coupon == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found"));

        response.Coupon = new Coupon
        {
            Code = coupon.Code,
            AlreadyUsed = coupon.AlreadyUsed,
            Amount = coupon.Amount,
            CouponId = coupon.CouponId.ToString()
        };
        return response;
    }
}

