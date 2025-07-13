using System.ComponentModel.DataAnnotations;

namespace EvenTicket.Services.ShoppingBasket.Models;

public record BasketForCreation
{
    [Required]
    public Guid UserId { get; set; }
}