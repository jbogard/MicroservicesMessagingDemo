using Microsoft.EntityFrameworkCore.Migrations;

namespace ClientUI.Migrations
{
    public partial class AddColorChoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorSelected",
                table: "PlacedOrderRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorSelected",
                table: "PlacedOrderRequests");
        }
    }
}
