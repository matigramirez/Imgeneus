using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddGuildNpcLvl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildNpcLvl",
                columns: table => new
                {
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    NpcType = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Group = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    NpcLevel = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildNpcLvl", x => new { x.GuildId, x.NpcType, x.Group, x.NpcLevel });
                    table.ForeignKey(
                        name: "FK_GuildNpcLvl_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildNpcLvl");
        }
    }
}
