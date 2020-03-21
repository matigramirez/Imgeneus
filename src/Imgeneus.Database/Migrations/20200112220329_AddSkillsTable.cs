using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imgeneus.Database.Migrations
{
    public partial class AddSkillsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SkillId = table.Column<ushort>(nullable: false),
                    SkillLevel = table.Column<byte>(nullable: false),
                    SkillName = table.Column<string>(nullable: true),
                    SkillUtilizer = table.Column<byte>(nullable: false),
                    UsedByFighter = table.Column<byte>(nullable: false),
                    UsedByDefender = table.Column<byte>(nullable: false),
                    UsedByRanger = table.Column<byte>(nullable: false),
                    UsedByArcher = table.Column<byte>(nullable: false),
                    UsedByMage = table.Column<byte>(nullable: false),
                    UsedByPriest = table.Column<byte>(nullable: false),
                    PreviousSkillId = table.Column<ushort>(nullable: false),
                    ReqLevel = table.Column<ushort>(nullable: false),
                    Grow = table.Column<byte>(nullable: false),
                    SkillPoint = table.Column<byte>(nullable: false),
                    TypeShow = table.Column<byte>(nullable: false),
                    TypeAttack = table.Column<byte>(nullable: false),
                    TypeEffect = table.Column<byte>(nullable: false),
                    TypeDetail = table.Column<ushort>(nullable: false),
                    NeedWeapon1 = table.Column<byte>(nullable: false),
                    NeedWeapon2 = table.Column<byte>(nullable: false),
                    NeedWeapon3 = table.Column<byte>(nullable: false),
                    NeedWeapon4 = table.Column<byte>(nullable: false),
                    NeedWeapon5 = table.Column<byte>(nullable: false),
                    NeedWeapon6 = table.Column<byte>(nullable: false),
                    NeedWeapon7 = table.Column<byte>(nullable: false),
                    NeedWeapon8 = table.Column<byte>(nullable: false),
                    NeedWeapon9 = table.Column<byte>(nullable: false),
                    NeedWeapon10 = table.Column<byte>(nullable: false),
                    NeedWeapon11 = table.Column<byte>(nullable: false),
                    NeedWeapon12 = table.Column<byte>(nullable: false),
                    NeedWeapon13 = table.Column<byte>(nullable: false),
                    NeedWeapon14 = table.Column<byte>(nullable: false),
                    NeedWeapon15 = table.Column<byte>(nullable: false),
                    NeedShield = table.Column<byte>(nullable: false),
                    SP = table.Column<ushort>(nullable: false),
                    MP = table.Column<ushort>(nullable: false),
                    ReadyTime = table.Column<byte>(nullable: false),
                    ResetTime = table.Column<ushort>(nullable: false),
                    AttackRange = table.Column<byte>(nullable: false),
                    StateType = table.Column<byte>(nullable: false),
                    AttrType = table.Column<byte>(nullable: false),
                    DisabledSkill = table.Column<ushort>(nullable: false),
                    SuccessType = table.Column<byte>(nullable: false),
                    SuccessValue = table.Column<byte>(nullable: false),
                    TargetType = table.Column<byte>(nullable: false),
                    ApplyRange = table.Column<byte>(nullable: false),
                    MultiAttack = table.Column<byte>(nullable: false),
                    KeepTime = table.Column<int>(nullable: false),
                    Weapon1 = table.Column<byte>(nullable: false),
                    Weapon2 = table.Column<byte>(nullable: false),
                    Weaponvalue = table.Column<byte>(nullable: false),
                    Bag = table.Column<byte>(nullable: false),
                    Arrow = table.Column<ushort>(nullable: false),
                    DamageType = table.Column<byte>(nullable: false),
                    DamageHP = table.Column<ushort>(nullable: false),
                    DamageSP = table.Column<ushort>(nullable: false),
                    DamageMP = table.Column<ushort>(nullable: false),
                    TimeDamageType = table.Column<byte>(nullable: false),
                    TimeDamageHP = table.Column<ushort>(nullable: false),
                    TimeDamageSP = table.Column<ushort>(nullable: false),
                    TimeDamageMP = table.Column<ushort>(nullable: false),
                    AddDamageHP = table.Column<ushort>(nullable: false),
                    AddDamageSP = table.Column<ushort>(nullable: false),
                    AddDamageMP = table.Column<ushort>(nullable: false),
                    AbilityType1 = table.Column<byte>(nullable: false),
                    AbilityValue1 = table.Column<ushort>(nullable: false),
                    AbilityType2 = table.Column<byte>(nullable: false),
                    AbilityValue2 = table.Column<ushort>(nullable: false),
                    AbilityType3 = table.Column<byte>(nullable: false),
                    AbilityValue3 = table.Column<ushort>(nullable: false),
                    AbilityType4 = table.Column<byte>(nullable: false),
                    AbilityValue4 = table.Column<ushort>(nullable: false),
                    AbilityType5 = table.Column<byte>(nullable: false),
                    AbilityValue5 = table.Column<ushort>(nullable: false),
                    AbilityType6 = table.Column<byte>(nullable: false),
                    AbilityValue6 = table.Column<ushort>(nullable: false),
                    AbilityType7 = table.Column<byte>(nullable: false),
                    AbilityValue7 = table.Column<ushort>(nullable: false),
                    AbilityType8 = table.Column<byte>(nullable: false),
                    AbilityValue8 = table.Column<ushort>(nullable: false),
                    AbilityType9 = table.Column<byte>(nullable: false),
                    AbilityValue9 = table.Column<ushort>(nullable: false),
                    AbilityType10 = table.Column<byte>(nullable: false),
                    AbilityValue10 = table.Column<ushort>(nullable: false),
                    HealHP = table.Column<ushort>(nullable: false),
                    HealMP = table.Column<ushort>(nullable: false),
                    HealSP = table.Column<ushort>(nullable: false),
                    TimeHealHP = table.Column<ushort>(nullable: false),
                    TimeHealMP = table.Column<ushort>(nullable: false),
                    TimeHealSP = table.Column<ushort>(nullable: false),
                    DefenceType = table.Column<byte>(nullable: false),
                    DefenceValue = table.Column<byte>(nullable: false),
                    LimitHP = table.Column<byte>(nullable: false),
                    FixRange = table.Column<byte>(nullable: false),
                    ChangeType = table.Column<ushort>(nullable: false),
                    ChangeLevel = table.Column<ushort>(nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
