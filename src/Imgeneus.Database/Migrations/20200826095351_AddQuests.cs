using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddQuests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<ushort>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    MinLevel = table.Column<ushort>(nullable: false),
                    MaxLevel = table.Column<ushort>(nullable: false),
                    Grow = table.Column<byte>(nullable: false),
                    AttackFighter = table.Column<byte>(nullable: false),
                    DefenseDefender = table.Column<byte>(nullable: false),
                    PatrolRogue = table.Column<byte>(nullable: false),
                    ShooterRogue = table.Column<byte>(nullable: false),
                    AttackMage = table.Column<byte>(nullable: false),
                    DefenseMage = table.Column<byte>(nullable: false),
                    PrevQuestId_1 = table.Column<ushort>(nullable: false),
                    PrevQuestId_2 = table.Column<ushort>(nullable: false),
                    PrevQuestId_3 = table.Column<ushort>(nullable: false),
                    QuestTimer = table.Column<ushort>(nullable: false),
                    InitItemType = table.Column<byte>(nullable: false),
                    InitItemTypeId = table.Column<byte>(nullable: false),
                    QuestTypeGiver = table.Column<byte>(nullable: false),
                    GiverType = table.Column<byte>(nullable: false),
                    GiverTypeId = table.Column<ushort>(nullable: false),
                    ReceivedItemType_1 = table.Column<byte>(nullable: false),
                    ReceivedItemTypeId_1 = table.Column<byte>(nullable: false),
                    ReceivedItemCount_1 = table.Column<byte>(nullable: false),
                    ReceivedItemType_2 = table.Column<byte>(nullable: false),
                    ReceivedItemTypeId_2 = table.Column<byte>(nullable: false),
                    ReceivedItemCount_2 = table.Column<byte>(nullable: false),
                    ReceivedItemType_3 = table.Column<byte>(nullable: false),
                    ReceivedItemTypeId_3 = table.Column<byte>(nullable: false),
                    ReceivedItemCount_3 = table.Column<byte>(nullable: false),
                    QuestTypeReceiver = table.Column<byte>(nullable: false),
                    ReceiverType = table.Column<byte>(nullable: false),
                    ReceiverTypeId = table.Column<ushort>(nullable: false),
                    FarmItemType_1 = table.Column<byte>(nullable: false),
                    FarmItemTypeId_1 = table.Column<byte>(nullable: false),
                    FarmItemCount_1 = table.Column<byte>(nullable: false),
                    FarmItemType_2 = table.Column<byte>(nullable: false),
                    FarmItemTypeId_2 = table.Column<byte>(nullable: false),
                    FarmItemCount_2 = table.Column<byte>(nullable: false),
                    FarmItemType_3 = table.Column<byte>(nullable: false),
                    FarmItemTypeId_3 = table.Column<byte>(nullable: false),
                    FarmItemCount_3 = table.Column<byte>(nullable: false),
                    XP = table.Column<uint>(nullable: false),
                    Gold = table.Column<uint>(nullable: false),
                    RevardItemType_1 = table.Column<byte>(nullable: false),
                    RevardItemTypeId_1 = table.Column<byte>(nullable: false),
                    RevardItemCount_1 = table.Column<byte>(nullable: false),
                    RevardItemType_2 = table.Column<byte>(nullable: false),
                    RevardItemTypeId_2 = table.Column<byte>(nullable: false),
                    RevardItemCount_2 = table.Column<byte>(nullable: false),
                    RevardItemType_3 = table.Column<byte>(nullable: false),
                    RevardItemTypeId_3 = table.Column<byte>(nullable: false),
                    RevardItemCount_3 = table.Column<byte>(nullable: false),
                    QuestUnlock_1 = table.Column<ushort>(nullable: false),
                    QuestUnlock_2 = table.Column<ushort>(nullable: false),
                    MsgSummary = table.Column<string>(nullable: true),
                    MsgComplete = table.Column<string>(nullable: true),
                    MsgResponse_1 = table.Column<string>(nullable: true),
                    MsgResponse_2 = table.Column<string>(nullable: true),
                    MsgResponse_3 = table.Column<string>(nullable: true),
                    MsgTake = table.Column<string>(nullable: true),
                    MsgWelcome = table.Column<string>(nullable: true),
                    MsgIncomplete = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quests");
        }
    }
}
