using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddDyeColorToCharacterItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "DyeColorAlpha",
                table: "CharacterItems",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DyeColorB",
                table: "CharacterItems",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DyeColorG",
                table: "CharacterItems",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DyeColorR",
                table: "CharacterItems",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DyeColorSaturation",
                table: "CharacterItems",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<bool>(
                name: "HasDyeColor",
                table: "CharacterItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DyeColorAlpha",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "DyeColorB",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "DyeColorG",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "DyeColorR",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "DyeColorSaturation",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "HasDyeColor",
                table: "CharacterItems");
        }
    }
}
