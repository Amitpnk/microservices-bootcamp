using System.ComponentModel.DataAnnotations;

namespace EvenTicket.Services.ShoppingBasket.Models
{
    public record BasketLineForUpdate
    {
        [Required]
        public int TicketAmount { get; set; }
    }
}
