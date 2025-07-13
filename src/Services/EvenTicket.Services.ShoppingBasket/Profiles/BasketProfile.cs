using AutoMapper;

namespace EvenTicket.Services.ShoppingBasket.Profiles;

public class BasketProfile : Profile
{
    public BasketProfile()
    {
        CreateMap<Models.BasketForCreation, Entities.Basket>();
        CreateMap<Entities.Basket, Models.Basket>().ReverseMap();
    }
}