using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddAutoStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "AutoDex",
                table: "Characters",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "AutoInt",
                table: "Characters",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "AutoLuc",
                table: "Characters",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "AutoRec",
                table: "Characters",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "AutoStr",
                table: "Characters",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "AutoWis",
                table: "Characters",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoDex",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AutoInt",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AutoLuc",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AutoRec",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AutoStr",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AutoWis",
                table: "Characters");
        }
    }
}
