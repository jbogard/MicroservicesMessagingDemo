using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClientUI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlacedOrderRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Color = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    OrderId = table.Column<string>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    OrderStatus = table.Column<int>(nullable: false),
                    ColorRequired = table.Column<bool>(nullable: true),
                    ColorOptions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacedOrderRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlacedOrderRequests");
        }
    }
}
