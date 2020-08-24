using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddNPCs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Npcs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<byte>(nullable: false),
                    TypeId = table.Column<ushort>(nullable: false),
                    MerchantType = table.Column<byte>(nullable: false),
                    Model = table.Column<byte>(nullable: false),
                    Country = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    WelcomeMessage = table.Column<string>(nullable: true),
                    QuestStart = table.Column<string>(nullable: true),
                    QuestEnd = table.Column<string>(nullable: true),
                    Maps = table.Column<string>(nullable: true),
                    Products = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Npcs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Npcs");
        }
    }
}
