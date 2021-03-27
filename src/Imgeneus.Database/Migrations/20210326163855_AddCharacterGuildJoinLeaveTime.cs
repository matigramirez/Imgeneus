using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddCharacterGuildJoinLeaveTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GuildJoinTime",
                table: "Characters",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GuildLeaveTime",
                table: "Characters",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildJoinTime",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "GuildLeaveTime",
                table: "Characters");
        }
    }
}
