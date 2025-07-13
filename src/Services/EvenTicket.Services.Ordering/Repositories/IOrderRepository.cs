using EvenTicket.Services.Ordering.Entities;

namespace EvenTicket.Services.Ordering.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetOrdersForUser(Guid userId);
    Task AddOrder(Order order);
    Task<Order> GetOrderById(Guid orderId);
    Task UpdateOrderPaymentStatus(Guid orderId, bool paid);

}