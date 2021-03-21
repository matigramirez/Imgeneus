using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class CharacterToGuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuildId",
                table: "Characters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "GuildRank",
                table: "Characters",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_GuildId",
                table: "Characters",
                column: "GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Guilds_GuildId",
                table: "Characters",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Guilds_GuildId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_GuildId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "GuildRank",
                table: "Characters");
        }
    }
}
