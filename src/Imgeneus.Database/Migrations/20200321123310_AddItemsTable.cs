using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddItemsTable : Migration
    {
        private const string MIGRATION_SQL_SCRIPT_FILE_NAME = @"Migrations\sql\InitItemData.sql";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemName = table.Column<string>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    TypeID = table.Column<byte>(nullable: false),
                    Reqlevel = table.Column<ushort>(nullable: false),
                    Country = table.Column<byte>(nullable: false),
                    Attackfighter = table.Column<byte>(nullable: false),
                    Defensefighter = table.Column<byte>(nullable: false),
                    Patrolrogue = table.Column<byte>(nullable: false),
                    Shootrogue = table.Column<byte>(nullable: false),
                    Attackmage = table.Column<byte>(nullable: false),
                    Defensemage = table.Column<byte>(nullable: false),
                    Grow = table.Column<byte>(nullable: false),
                    ReqStr = table.Column<ushort>(nullable: false),
                    ReqDex = table.Column<ushort>(nullable: false),
                    ReqRec = table.Column<ushort>(nullable: false),
                    ReqInt = table.Column<ushort>(nullable: false),
                    ReqWis = table.Column<ushort>(nullable: false),
                    Reqluc = table.Column<short>(nullable: false),
                    ReqVg = table.Column<ushort>(nullable: false),
                    ReqOg = table.Column<byte>(nullable: false),
                    ReqIg = table.Column<byte>(nullable: false),
                    Range = table.Column<ushort>(nullable: false),
                    AttackTime = table.Column<byte>(nullable: false),
                    Attrib = table.Column<byte>(nullable: false),
                    Special = table.Column<byte>(nullable: false),
                    Slot = table.Column<byte>(nullable: false),
                    Quality = table.Column<ushort>(nullable: false),
                    Effect1 = table.Column<ushort>(nullable: false),
                    Effect2 = table.Column<ushort>(nullable: false),
                    Effect3 = table.Column<ushort>(nullable: false),
                    Effect4 = table.Column<ushort>(nullable: false),
                    ConstHP = table.Column<ushort>(nullable: false),
                    ConstSP = table.Column<ushort>(nullable: false),
                    ConstMP = table.Column<ushort>(nullable: false),
                    ConstStr = table.Column<ushort>(nullable: false),
                    ConstDex = table.Column<ushort>(nullable: false),
                    ConstRec = table.Column<ushort>(nullable: false),
                    ConstInt = table.Column<ushort>(nullable: false),
                    ConstWis = table.Column<ushort>(nullable: false),
                    ConstLuc = table.Column<ushort>(nullable: false),
                    Speed = table.Column<byte>(nullable: false),
                    Exp = table.Column<byte>(nullable: false),
                    Buy = table.Column<int>(nullable: false),
                    Sell = table.Column<int>(nullable: false),
                    Grade = table.Column<ushort>(nullable: false),
                    Drop = table.Column<ushort>(nullable: false),
                    Server = table.Column<byte>(nullable: false),
                    Count = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
