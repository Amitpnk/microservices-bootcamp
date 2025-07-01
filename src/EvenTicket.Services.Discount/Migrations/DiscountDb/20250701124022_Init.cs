using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EvenTicket.Services.Discount.Migrations.DiscountDb;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Coupons",
            columns: table => new
            {
                CouponId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Amount = table.Column<int>(type: "int", nullable: false),
                AlreadyUsed = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Coupons", x => x.CouponId);
            });

        migrationBuilder.InsertData(
            table: "Coupons",
            columns: new[] { "CouponId", "AlreadyUsed", "Amount", "Code" },
            values: new object[,]
            {
                    { new Guid("140b49de-a914-435b-980f-ca0187d1733f"), false, 10, "BeNice" },
                    { new Guid("a42b824b-f719-4f3e-a894-1026fa0560d7"), false, 100, "AlmostFree" },
                    { new Guid("c1e882f4-6379-4cf3-a617-106729662c27"), false, 20, "Awesome" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Coupons");
    }
}