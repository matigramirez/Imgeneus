using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddNumberToDbCharacterSkill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Number",
                table: "CharacterSkill",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "CharacterSkill");
        }
    }
}
