using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fantasy_Web_API.Migrations
{
    public partial class AdditionalItemProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Rarity",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Rarity",
                table: "Items");
        }
    }
}
