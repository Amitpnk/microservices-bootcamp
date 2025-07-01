using EvenTicket.Services.Discount.Entities;
using Microsoft.EntityFrameworkCore;

namespace EvenTicket.Services.Discount.DbContexts;

public class DiscountDbContext : DbContext
{
    public DiscountDbContext(DbContextOptions<DiscountDbContext> options) : base(options)
    {
    }
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            CouponId = Guid.Parse("140b49de-a914-435b-980f-ca0187d1733f"),
            Code = "BeNice",
            Amount = 10,
            AlreadyUsed = false
        });

        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            CouponId = Guid.Parse("c1e882f4-6379-4cf3-a617-106729662c27"),
            Code = "Awesome",
            Amount = 20,
            AlreadyUsed = false
        });

        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            CouponId = Guid.Parse("a42b824b-f719-4f3e-a894-1026fa0560d7"),
            Code = "AlmostFree",
            Amount = 100,
            AlreadyUsed = false
        });

    }
}