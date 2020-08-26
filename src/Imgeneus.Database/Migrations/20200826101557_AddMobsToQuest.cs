using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddMobsToQuest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "MobCount_1",
                table: "Quests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "MobCount_2",
                table: "Quests",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ushort>(
                name: "MobId_1",
                table: "Quests",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<ushort>(
                name: "MobId_2",
                table: "Quests",
                nullable: false,
                defaultValue: (ushort)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobCount_1",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "MobCount_2",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "MobId_1",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "MobId_2",
                table: "Quests");
        }
    }
}
