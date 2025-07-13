using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvenTicket.Services.ShoppingBasket.Migrations.ShoppingBasketDb
{
    /// <inheritdoc />
    public partial class addingcoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CouponId",
                table: "Baskets",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Baskets");
        }
    }
}
