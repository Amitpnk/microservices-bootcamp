using EvenTicket.Services.EventCatalog.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EvenTicket.Services.EventCatalog.DbContexts;

public class EventCatalogDbContext(DbContextOptions<EventCatalogDbContext> options, ILogger<EventCatalogDbContext> logger) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Load categories from JSON
        var categories = LoadSeedData<Category>("categories.json");
        if (categories != null)
        {
            modelBuilder.Entity<Category>().HasData(categories);
        }

        // Load events from JSON
        var events = LoadSeedData<Event>("events.json");
        if (events != null)
        {
            modelBuilder.Entity<Event>().HasData(events);
        }
    }

    //todo : move this to cross-cutting layer
    private List<T> LoadSeedData<T>(string fileName)
    {
        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "DbContexts/SeedData", fileName);
            if (!File.Exists(filePath))
            {
                logger.LogWarning("Seed data file not found: {FilePath}", filePath);
                return null;
            }

            var jsonData = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(jsonData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading seed data from {FileName}", fileName);
            return null;
        }
    }
}