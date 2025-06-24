using AutoMapper;
using EvenTicket.Services.EventCatalog.Entities;
using EvenTicket.Services.EventCatalog.Models;

namespace EvenTicket.Services.EventCatalog.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
    }
}