using EvenTicket.Services.ShoppingBasket.Entities;
using EvenTicket.Services.ShoppingBasket.Extensions;
using System.Net.Http;

namespace EvenTicket.Services.ShoppingBasket.Services;

public class EventCatalogService : IEventCatalogService
{
    private readonly IHttpClientFactory httpClientFactory;

    public EventCatalogService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<Event> GetEvent(Guid id)
    {
        var client = httpClientFactory.CreateClient("EventCatalog");
        var response = await client.GetAsync($"/api/event/{id}");
        return await response.ReadContentAs<Event>();
    }
}