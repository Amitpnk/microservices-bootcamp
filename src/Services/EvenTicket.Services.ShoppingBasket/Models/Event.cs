namespace EvenTicket.Services.ShoppingBasket.Models;

public record Event
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
}