using EvenTicket.Services.EventCatalog.Entities;

namespace EvenTicket.Services.EventCatalog.Repositories;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetEvents(Guid categoryId);
    Task<Event> GetEventById(Guid eventId);
}