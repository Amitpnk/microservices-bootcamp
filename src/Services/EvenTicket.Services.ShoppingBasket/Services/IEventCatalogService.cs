using EvenTicket.Services.ShoppingBasket.Entities;

namespace EvenTicket.Services.ShoppingBasket.Services;

public interface IEventCatalogService
{
    Task<Event> GetEvent(Guid id);
}