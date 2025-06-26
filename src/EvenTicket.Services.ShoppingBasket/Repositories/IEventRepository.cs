using System;
using System.Threading.Tasks;
using EvenTicket.Services.ShoppingBasket.Entities;

namespace EvenTicket.Services.ShoppingBasket.Repositories
{
    public interface IEventRepository
    {
        void AddEvent(Event theEvent);
        Task<bool> EventExists(Guid eventId);
        Task<bool> SaveChanges();
    }
}