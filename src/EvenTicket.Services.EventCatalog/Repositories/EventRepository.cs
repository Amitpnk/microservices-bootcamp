using EvenTicket.Services.EventCatalog.DbContexts;
using EvenTicket.Services.EventCatalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvenTicket.Services.EventCatalog.Repositories;

public class EventRepository(EventCatalogDbContext eventCatalogDbContext) : IEventRepository
{
    public async Task<IEnumerable<Event>> GetEvents(Guid categoryId)
    {
        return await eventCatalogDbContext.Events
            .Include(x => x.Category)
            .Where(x => x.CategoryId == categoryId || categoryId == Guid.Empty).ToListAsync();
    }

    public async Task<Event> GetEventById(Guid eventId)
    {
        return await eventCatalogDbContext.Events.Include(x => x.Category).Where(x => x.EventId == eventId).FirstOrDefaultAsync();
    }
}