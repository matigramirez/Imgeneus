using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddMobItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MobItems",
                columns: table => new
                {
                    MobId = table.Column<ushort>(nullable: false),
                    ItemOrder = table.Column<byte>(nullable: false),
                    Grade = table.Column<ushort>(nullable: false),
                    DropRate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobItems", x => new { x.MobId, x.ItemOrder });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobItems");
        }
    }
}
