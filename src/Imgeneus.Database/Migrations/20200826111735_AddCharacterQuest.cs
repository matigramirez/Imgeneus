using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddCharacterQuest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterQuest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(nullable: false),
                    QuestId = table.Column<ushort>(nullable: false),
                    Delay = table.Column<ushort>(nullable: false),
                    Count1 = table.Column<byte>(nullable: false),
                    Count2 = table.Column<byte>(nullable: false),
                    Count3 = table.Column<byte>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    Finish = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterQuest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterQuest_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterQuest_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterQuest_CharacterId",
                table: "CharacterQuest",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterQuest_QuestId",
                table: "CharacterQuest",
                column: "QuestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterQuest");
        }
    }
}
