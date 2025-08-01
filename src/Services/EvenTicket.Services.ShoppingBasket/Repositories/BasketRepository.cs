using EvenTicket.Services.ShoppingBasket.DbContexts;
using EvenTicket.Services.ShoppingBasket.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvenTicket.Services.ShoppingBasket.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly ShoppingBasketDbContext _shoppingBasketDbContext;

    public BasketRepository(ShoppingBasketDbContext shoppingBasketDbContext)
    {
        _shoppingBasketDbContext = shoppingBasketDbContext;
    }

    public async Task<Basket> GetBasketById(Guid basketId)
    {
        return await _shoppingBasketDbContext.Baskets.Include(sb => sb.BasketLines)
            .Where(b => b.BasketId == basketId).FirstOrDefaultAsync();
    }

    public async Task<bool> BasketExists(Guid basketId)
    {
        return await _shoppingBasketDbContext.Baskets
            .AnyAsync(b => b.BasketId == basketId);
    }

    public async Task ClearBasket(Guid basketId)
    {
        var basketLinesToClear = _shoppingBasketDbContext.BasketLines.Where(b => b.BasketId == basketId);
        _shoppingBasketDbContext.BasketLines.RemoveRange(basketLinesToClear);

        var basket = _shoppingBasketDbContext.Baskets.FirstOrDefault(b => b.BasketId == basketId);
        if (basket != null) basket.CouponId = null;

        await SaveChanges();
    }

    public void AddBasket(Basket basket)
    {
        _shoppingBasketDbContext.Baskets.Add(basket);
    }

    public async Task<bool> SaveChanges()
    {
        return (await _shoppingBasketDbContext.SaveChangesAsync() > 0);
    }
}