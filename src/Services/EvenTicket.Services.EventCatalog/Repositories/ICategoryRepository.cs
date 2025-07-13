using EvenTicket.Services.EventCatalog.Entities;

namespace EvenTicket.Services.EventCatalog.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category> GetCategoryById(string categoryId);
}