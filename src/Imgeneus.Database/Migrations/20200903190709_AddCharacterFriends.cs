using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddCharacterFriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterFriends",
                columns: table => new
                {
                    CharacterId = table.Column<int>(nullable: false),
                    FriendId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterFriends", x => new { x.CharacterId, x.FriendId });
                    table.ForeignKey(
                        name: "FK_CharacterFriends_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterFriends_Characters_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterFriends_FriendId",
                table: "CharacterFriends",
                column: "FriendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterFriends");
        }
    }
}
