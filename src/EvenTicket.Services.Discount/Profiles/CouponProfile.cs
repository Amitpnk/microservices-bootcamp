using AutoMapper;
using EvenTicket.Services.Discount.Entities;
using EvenTicket.Services.Discount.Models;

namespace EvenTicket.Services.Discount.Profiles;

public class CouponProfile : Profile
{
    public CouponProfile()
    {
        CreateMap<Coupon, CouponDto>().ReverseMap();
    }
}

