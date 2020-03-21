using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddItemToCharItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "Items",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                columns: new[] { "Type", "TypeID" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterItems_Type_TypeId",
                table: "CharacterItems",
                columns: new[] { "Type", "TypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterItems_Items_Type_TypeId",
                table: "CharacterItems",
                columns: new[] { "Type", "TypeId" },
                principalTable: "Items",
                principalColumns: new[] { "Type", "TypeID" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterItems_Items_Type_TypeId",
                table: "CharacterItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_CharacterItems_Type_TypeId",
                table: "CharacterItems");

            migrationBuilder.AlterColumn<int>(
                name: "ItemID",
                table: "Items",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "ItemID");
        }
    }
}
