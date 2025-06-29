﻿using EvenTicket.Services.ShoppingBasket.Entities;
using System;
using System.Threading.Tasks;
using EvenTicket.Services.ShoppingBasket.Entities;

namespace EvenTicket.Services.ShoppingBasket.Repositories
{
    public interface IBasketRepository
    {
        Task<bool> BasketExists(Guid basketId);

        Task<Basket> GetBasketById(Guid basketId);

        void AddBasket(Basket basket);

        Task<bool> SaveChanges();
    }
}