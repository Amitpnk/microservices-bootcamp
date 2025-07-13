using AutoMapper;
using EvenTicket.Services.ShoppingBasket.Messages;
using EvenTicket.Services.ShoppingBasket.Models;

namespace EvenTicket.Services.ShoppingBasket.Profiles;

public class BasketCheckoutProfile : Profile
{
    public BasketCheckoutProfile()
    {
        CreateMap<BasketCheckout, BasketCheckoutMessage>().ReverseMap();
    }
}

