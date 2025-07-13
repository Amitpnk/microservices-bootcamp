namespace EvenTicket.Services.ShoppingBasket.Models;

public record Basket
{
    public Guid BasketId { get; set; }
    public Guid UserId { get; set; }
    public int NumberOfItems { get; set; }
}