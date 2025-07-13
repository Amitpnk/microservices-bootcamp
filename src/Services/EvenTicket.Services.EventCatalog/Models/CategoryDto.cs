namespace EvenTicket.Services.EventCatalog.Models;

public record CategoryDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
}