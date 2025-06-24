using EvenTicket.Services.EventCatalog.Entities;
using EvenTicket.Services.EventCatalog.Models;
using Mapster;

namespace EvenTicket.Services.EventCatalog.Profiles;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Event, EventDto>.NewConfig();

        TypeAdapterConfig<Category, CategoryDto>.NewConfig();

    }
}

