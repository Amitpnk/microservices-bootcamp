using EvenTicket.Services.ShoppingBasket.Entities;
using EvenTicket.Services.ShoppingBasket.Extensions;

namespace EvenTicket.Services.ShoppingBasket.Services;

public class EventCatalogService : IEventCatalogService
{
    private readonly HttpClient client;

    public EventCatalogService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<Event> GetEvent(Guid id)
    {
        var response = await client.GetAsync($"/api/event/{id}");
        return await response.ReadContentAs<Event>();
    }
}