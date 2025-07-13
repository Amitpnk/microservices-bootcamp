using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EvenTicket.Services.EventCatalog.Migrations.EventCatalogDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Name" },
                values: new object[,]
                {
                    { new Guid("1a58c498-f6fe-4a41-b1ec-bd2f33b1d2b2"), "Plays" },
                    { new Guid("373e11ef-4711-4f07-950b-b3aec7b39565"), "Conference" },
                    { new Guid("3f2373fa-525b-4118-9cde-c99f8d5a1d38"), "Musicals" },
                    { new Guid("68ffabe5-d4bb-408d-88d8-39f4120bc9e4"), "Concerts" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "EventId", "Artist", "CategoryId", "Date", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("730f67eb-7ef9-4d90-b16d-1cfc381cf20d"), "Pt. Birju Maharaj Troupe", new Guid("3f2373fa-525b-4118-9cde-c99f8d5a1d38"), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kathak adaptation of the great epic featuring 100+ artists. Directed by Kathak legend Pt. Birju Maharaj.", "/img/mahabharata-musical.jpg", "Mahabharata: The Eternal Epic", 8500 },
                    { new Guid("7ee7bfee-bdb3-4c46-90d3-57293f08f366"), "Shankar Mahadevan", new Guid("3f2373fa-525b-4118-9cde-c99f8d5a1d38"), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Experience this spectacular musical journey through India's history, composed by Shankar Mahadevan. The show has received rave reviews from critics and audiences alike.", "/img/desi-musical.jpg", "Bharat: The Great Indian Musical", 10000 },
                    { new Guid("e34a9568-52a2-4b6c-85ea-a5463124cd30"), "Arijit Singh", new Guid("68ffabe5-d4bb-408d-88d8-39f4120bc9e4"), new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Join India's most beloved playback singer Arijit Singh for his farewell tour across 15 Indian cities. Experience the magic of his soulful voice live.", "/img/arijit.jpg", "Arijit Singh Live in Concert", 5400 },
                    { new Guid("f14b62ec-376c-43c8-bb53-a3cb167f16a2"), "Kapil Sharma", new Guid("68ffabe5-d4bb-408d-88d8-39f4120bc9e4"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "India's comedy king Kapil Sharma brings his hilarious team for a live show that will leave you in splits. Featuring Sunil Grover, Krushna Abhishek and other special guests!", "/img/kapil.jpg", "Comedy Night with Kapil Sharma", 5500 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_CategoryId",
                table: "Events",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
