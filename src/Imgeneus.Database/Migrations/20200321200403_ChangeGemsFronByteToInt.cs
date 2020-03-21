using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class ChangeGemsFronByteToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gems",
                table: "CharacterItems");

            migrationBuilder.AddColumn<int>(
                name: "GemTypeId1",
                table: "CharacterItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GemTypeId2",
                table: "CharacterItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GemTypeId3",
                table: "CharacterItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GemTypeId4",
                table: "CharacterItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GemTypeId5",
                table: "CharacterItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GemTypeId6",
                table: "CharacterItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GemTypeId1",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "GemTypeId2",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "GemTypeId3",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "GemTypeId4",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "GemTypeId5",
                table: "CharacterItems");

            migrationBuilder.DropColumn(
                name: "GemTypeId6",
                table: "CharacterItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "Gems",
                table: "CharacterItems",
                type: "varbinary(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: new byte[] {  });
        }
    }
}
