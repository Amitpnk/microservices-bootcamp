using EvenTicket.Services.ShoppingBasket.Entities;

namespace EvenTicket.Services.ShoppingBasket.Repositories;

public interface IBasketLinesRepository
{
    Task<IEnumerable<BasketLine>> GetBasketLines(Guid basketId);

    Task<BasketLine> GetBasketLineById(Guid basketLineId);

    Task<BasketLine> AddOrUpdateBasketLine(Guid basketId, BasketLine basketLine);

    void UpdateBasketLine(BasketLine basketLine);

    void RemoveBasketLine(BasketLine basketLine);

    Task<bool> SaveChanges();
}