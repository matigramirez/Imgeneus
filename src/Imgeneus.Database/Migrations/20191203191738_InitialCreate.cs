using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(maxLength: 19, nullable: false),
                    Password = table.Column<string>(maxLength: 16, nullable: false),
                    Email = table.Column<string>(maxLength: 30, nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Authority = table.Column<byte>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    Faction = table.Column<byte>(nullable: false),
                    MaxMode = table.Column<byte>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastConnectionTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: false),
                    Level = table.Column<ushort>(nullable: false),
                    Slot = table.Column<byte>(nullable: false),
                    Race = table.Column<byte>(nullable: false),
                    Class = table.Column<byte>(nullable: false),
                    Mode = table.Column<byte>(nullable: false),
                    Hair = table.Column<byte>(nullable: false),
                    Face = table.Column<byte>(nullable: false),
                    Height = table.Column<byte>(nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    Map = table.Column<ushort>(nullable: false),
                    PosX = table.Column<float>(nullable: false),
                    PosY = table.Column<float>(nullable: false),
                    PosZ = table.Column<float>(nullable: false),
                    Angle = table.Column<ushort>(nullable: false),
                    StatPoint = table.Column<ushort>(nullable: false),
                    SkillPoint = table.Column<ushort>(nullable: false),
                    Strength = table.Column<ushort>(nullable: false),
                    Dexterity = table.Column<ushort>(nullable: false),
                    Rec = table.Column<ushort>(nullable: false),
                    Intelligence = table.Column<ushort>(nullable: false),
                    Luck = table.Column<ushort>(nullable: false),
                    Wisdom = table.Column<ushort>(nullable: false),
                    HealthPoints = table.Column<ushort>(nullable: false),
                    ManaPoints = table.Column<ushort>(nullable: false),
                    StaminaPoints = table.Column<ushort>(nullable: false),
                    Exp = table.Column<uint>(nullable: false),
                    Gold = table.Column<uint>(nullable: false),
                    Kills = table.Column<ushort>(nullable: false),
                    Deaths = table.Column<ushort>(nullable: false),
                    Victories = table.Column<ushort>(nullable: false),
                    Defeats = table.Column<ushort>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    IsRename = table.Column<bool>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DeleteTime = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<byte>(nullable: false),
                    TypeId = table.Column<byte>(nullable: false),
                    Bag = table.Column<byte>(nullable: false),
                    Slot = table.Column<byte>(nullable: false),
                    Count = table.Column<byte>(nullable: false),
                    Quality = table.Column<ushort>(nullable: false),
                    Gems = table.Column<byte[]>(maxLength: 6, nullable: false),
                    Craftname = table.Column<string>(maxLength: 20, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    MakeType = table.Column<string>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CharacterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterItems_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterItems_CharacterId",
                table: "CharacterItems",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username_Email",
                table: "Users",
                columns: new[] { "Username", "Email" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterItems");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
