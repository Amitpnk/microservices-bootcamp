using AutoMapper;
using EvenTicket.Services.EventCatalog.Entities;
using EvenTicket.Services.EventCatalog.Models;

namespace EvenTicket.Services.EventCatalog.Profiles;

public class EventProfile : Profile
{
    public EventProfile()
    {
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.CategoryName, opts => opts.MapFrom(src => src.Category.Name));
    }
}