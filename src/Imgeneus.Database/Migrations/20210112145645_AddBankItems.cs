using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddBankItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Slot = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Count = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ObtainmentTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ClaimTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsClaimed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankItems_Items_Type_TypeId",
                        columns: x => new { x.Type, x.TypeId },
                        principalTable: "Items",
                        principalColumns: new[] { "Type", "TypeID" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankItems_Type_TypeId",
                table: "BankItems",
                columns: new[] { "Type", "TypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_BankItems_UserId",
                table: "BankItems",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankItems");
        }
    }
}
