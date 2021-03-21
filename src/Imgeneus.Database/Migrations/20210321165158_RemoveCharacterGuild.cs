using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class RemoveCharacterGuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Guilds_GuildId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterGuild");

            migrationBuilder.DropIndex(
                name: "IX_Characters_GuildId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Characters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuildId",
                table: "Characters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CharacterGuild",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    GuildLevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LeaveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGuild", x => new { x.CharacterId, x.GuildId });
                    table.ForeignKey(
                        name: "FK_CharacterGuild_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterGuild_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_GuildId",
                table: "Characters",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGuild_GuildId",
                table: "CharacterGuild",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Guilds_GuildId",
                table: "Characters",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
