using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddMob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mobs",
                columns: table => new
                {
                    MobID = table.Column<ushort>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MobName = table.Column<string>(maxLength: 40, nullable: true),
                    Level = table.Column<ushort>(nullable: false),
                    Exp = table.Column<short>(nullable: false),
                    AI = table.Column<byte>(nullable: false),
                    Money1 = table.Column<short>(nullable: false),
                    Money2 = table.Column<short>(nullable: false),
                    QuestItemId = table.Column<int>(nullable: false),
                    HP = table.Column<int>(nullable: false),
                    SP = table.Column<short>(nullable: false),
                    MP = table.Column<short>(nullable: false),
                    Dex = table.Column<ushort>(nullable: false),
                    Wis = table.Column<ushort>(nullable: false),
                    Luc = table.Column<ushort>(nullable: false),
                    Day = table.Column<byte>(nullable: false),
                    Size = table.Column<byte>(nullable: false),
                    Element = table.Column<byte>(nullable: false),
                    Defense = table.Column<ushort>(nullable: false),
                    Magic = table.Column<ushort>(nullable: false),
                    ResistState1 = table.Column<byte>(nullable: false),
                    ResistState2 = table.Column<byte>(nullable: false),
                    ResistState3 = table.Column<byte>(nullable: false),
                    ResistState4 = table.Column<byte>(nullable: false),
                    ResistState5 = table.Column<byte>(nullable: false),
                    ResistState6 = table.Column<byte>(nullable: false),
                    ResistState7 = table.Column<byte>(nullable: false),
                    ResistState8 = table.Column<byte>(nullable: false),
                    ResistState9 = table.Column<byte>(nullable: false),
                    ResistState10 = table.Column<byte>(nullable: false),
                    ResistState11 = table.Column<byte>(nullable: false),
                    ResistState12 = table.Column<byte>(nullable: false),
                    ResistState13 = table.Column<byte>(nullable: false),
                    ResistState14 = table.Column<byte>(nullable: false),
                    ResistState15 = table.Column<byte>(nullable: false),
                    ResistSkill1 = table.Column<byte>(nullable: false),
                    ResistSkill2 = table.Column<byte>(nullable: false),
                    ResistSkill3 = table.Column<byte>(nullable: false),
                    ResistSkill4 = table.Column<byte>(nullable: false),
                    ResistSkill5 = table.Column<byte>(nullable: false),
                    ResistSkill6 = table.Column<byte>(nullable: false),
                    NormalTime = table.Column<int>(nullable: false),
                    NormalStep = table.Column<byte>(nullable: false),
                    ChaseTime = table.Column<int>(nullable: false),
                    ChaseStep = table.Column<byte>(nullable: false),
                    ChaseRange = table.Column<byte>(nullable: false),
                    AttackType1 = table.Column<ushort>(nullable: false),
                    AttackTime1 = table.Column<int>(nullable: false),
                    AttackRange1 = table.Column<byte>(nullable: false),
                    Attack1 = table.Column<short>(nullable: false),
                    AttackPlus1 = table.Column<ushort>(nullable: false),
                    AttackAttrib1 = table.Column<byte>(nullable: false),
                    AttackSpecial1 = table.Column<byte>(nullable: false),
                    AttackOk1 = table.Column<byte>(nullable: false),
                    AttackType2 = table.Column<ushort>(nullable: false),
                    AttackTime2 = table.Column<int>(nullable: false),
                    AttackRange2 = table.Column<byte>(nullable: false),
                    Attack2 = table.Column<short>(nullable: false),
                    AttackPlus2 = table.Column<ushort>(nullable: false),
                    AttackAttrib2 = table.Column<byte>(nullable: false),
                    AttackSpecial2 = table.Column<byte>(nullable: false),
                    AttackOk2 = table.Column<byte>(nullable: false),
                    AttackType3 = table.Column<ushort>(nullable: false),
                    AttackTime3 = table.Column<int>(nullable: false),
                    AttackRange3 = table.Column<byte>(nullable: false),
                    Attack3 = table.Column<short>(nullable: false),
                    AttackPlus3 = table.Column<ushort>(nullable: false),
                    AttackAttrib3 = table.Column<byte>(nullable: false),
                    AttackSpecial3 = table.Column<byte>(nullable: false),
                    AttackOk3 = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mobs", x => x.MobID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mobs");
        }
    }
}
