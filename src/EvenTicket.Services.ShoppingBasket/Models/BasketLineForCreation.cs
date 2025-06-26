using System;
using System.ComponentModel.DataAnnotations;

namespace EvenTicket.Services.ShoppingBasket.Models
{
    public record BasketLineForCreation
    { 
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int TicketAmount { get; set; }
    }
}
