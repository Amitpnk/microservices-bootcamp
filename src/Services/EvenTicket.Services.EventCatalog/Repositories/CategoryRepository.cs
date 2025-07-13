using EvenTicket.Services.EventCatalog.DbContexts;
using EvenTicket.Services.EventCatalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvenTicket.Services.EventCatalog.Repositories;

public class CategoryRepository(EventCatalogDbContext eventCatalogDbContext) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await eventCatalogDbContext.Categories.ToListAsync();
    }

    public async Task<Category> GetCategoryById(string categoryId)
    {
        return await eventCatalogDbContext.Categories.Where(x => x.CategoryId.ToString() == categoryId)
            .FirstOrDefaultAsync();
    }
}