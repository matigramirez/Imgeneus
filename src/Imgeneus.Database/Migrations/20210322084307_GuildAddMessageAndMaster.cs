using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class GuildAddMessageAndMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Guilds",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Rank",
                table: "Guilds",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_MasterId",
                table: "Guilds",
                column: "MasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guilds_Characters_MasterId",
                table: "Guilds",
                column: "MasterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guilds_Characters_MasterId",
                table: "Guilds");

            migrationBuilder.DropIndex(
                name: "IX_Guilds_MasterId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Guilds");
        }
    }
}
