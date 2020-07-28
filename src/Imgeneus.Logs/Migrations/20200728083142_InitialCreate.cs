using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Logs.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CharacterId = table.Column<int>(nullable: false),
                    CharacterName = table.Column<string>(nullable: true),
                    MessageType = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    TargetId = table.Column<int>(nullable: false),
                    TargetName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatLogs");
        }
    }
}
