using System;
using System.ComponentModel.DataAnnotations.Schema;
using Imgeneus.Database.Constants;

namespace Imgeneus.Database.Entities
{
    [Table("Skills")]
    public class DbSkill : DbEntity
    {
        /// <summary>
        /// Id of skill.
        /// </summary>
        public ushort SkillId { get; set; }

        /// <summary>
        /// Level of skill.
        /// </summary>
        public byte SkillLevel { get; set; }

        /// <summary>
        /// Title of skill.
        /// </summary>
        public string SkillName { get; set; }

        /// <summary>
        /// Which fraction and profession can use this skill.
        /// </summary>
        public SkillUtilizer SkillUtilizer { get; set; }

        /// <summary>
        /// Indicates if skill can be used by fighter. Maybe this can be migrated to bool...
        /// </summary>
        public byte UsedByFighter { get; set; }

        /// <summary>
        /// Indicates if skill can be used by defender. Maybe this can be migrated to bool...
        /// </summary>
        public byte UsedByDefender { get; set; }

        /// <summary>
        /// Indicates if skill can be used by ranger. Maybe this can be migrated to bool...
        /// </summary>
        public byte UsedByRanger { get; set; }

        /// <summary>
        /// Indicates if skill can be used by archer. Maybe this can be migrated to bool...
        /// </summary>
        public byte UsedByArcher { get; set; }

        /// <summary>
        /// Indicates if skill can be used by mage. Maybe this can be migrated to bool...
        /// </summary>
        public byte UsedByMage { get; set; }

        /// <summary>
        /// Indicates if skill can be used by priest. Maybe this can be migrated to bool...
        /// </summary>
        public byte UsedByPriest { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public ushort PreviousSkillId { get; set; }

        /// <summary>
        /// Required level of skill.
        /// </summary>
        public ushort ReqLevel { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte Grow { get; set; }

        /// <summary>
        /// How many skill points are needed in order to learn this skill.
        /// </summary>
        public byte SkillPoint { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte TypeShow { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte TypeAttack { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte TypeEffect { get; set; }

        /// <summary>
        /// Type detail describes what skill does.
        /// </summary>
        public TypeDetail TypeDetail { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon1 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon2 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon3 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon4 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon5 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon6 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon7 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon8 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon9 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon10 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon11 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon12 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon13 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon14 { get; set; }

        /// <summary>
        /// Skill requires TODO: ?. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedWeapon15 { get; set; }

        /// <summary>
        /// Skill requires shield. Maybe this can migrate to bool value...
        /// </summary>
        public byte NeedShield { get; set; }

        /// <summary>
        /// How many stamina points requires this skill.
        /// </summary>
        public ushort SP { get; set; }

        /// <summary>
        /// How many mana points requires this skill.
        /// </summary>
        public ushort MP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte ReadyTime { get; set; }

        /// <summary>
        /// Time after which skill can be used again.
        /// </summary>
        public ushort ResetTime { get; set; }

        /// <summary>
        /// How many meters are needed in order to use skill.
        /// </summary>
        public byte AttackRange { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte StateType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AttrType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort DisabledSkill { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte SuccessType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte SuccessValue { get; set; }

        /// <summary>
        /// What target is required for this skill.
        /// </summary>
        public TargetType TargetType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte ApplyRange { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte MultiAttack { get; set; }

        /// <summary>
        /// Time for example for buffs. This time shows how long the skill will be applied.
        /// NB! It was ushort in original db. But I could not migrate it! Changed to int.
        /// </summary>
        public int KeepTime { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte Weapon1 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte Weapon2 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte Weaponvalue { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte Bag { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort Arrow { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte DamageType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort DamageHP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort DamageSP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort DamageMP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte TimeDamageType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort TimeDamageHP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort TimeDamageSP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort TimeDamageMP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AddDamageHP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AddDamageSP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AddDamageMP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType1 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue1 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType2 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue2 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType3 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue3 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType4 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue4 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType5 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue5 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType6 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue6 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType7 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue7 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType8 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue8 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType9 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue9 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte AbilityType10 { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort AbilityValue10 { get; set; }

        /// <summary>
        /// How many health points can be healed.
        /// </summary>
        public ushort HealHP { get; set; }

        /// <summary>
        /// How many mana points can be healed.
        /// </summary>
        public ushort HealMP { get; set; }

        /// <summary>
        /// How many stamina points can be healed.
        /// </summary>
        public ushort HealSP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort TimeHealHP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort TimeHealMP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort TimeHealSP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte DefenceType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte DefenceValue { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte LimitHP { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public byte FixRange { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort ChangeType { get; set; }

        /// <summary>
        /// TODO: ?
        /// </summary>
        public ushort ChangeLevel { get; set; }

        /// <summary>
        /// Gets the update time.
        /// </summary>
        [Column(TypeName = "DATETIME")]
        public DateTime UpdateDate { get; set; }
    }

    public enum SkillUtilizer : byte
    {
        /// <summary>
        /// Skill can be used only by humans.
        /// </summary>
        Human,

        /// <summary>
        /// Skill can be used only by elfs.
        /// </summary>
        Elf,

        /// <summary>
        /// Skill can be used only by both humans and elfs.
        /// </summary>
        Light,

        /// <summary>
        /// Skill can be used only by deatheaters.
        /// </summary>
        Deatheater,

        /// <summary>
        /// Skill can be used only by both humans and vails.
        /// </summary>
        Vail,

        /// <summary>
        /// Skill can be used only by both deatheaters and vails.
        /// </summary>
        Fury,

        /// <summary>
        /// Skill can be used by any profession in any fraction.
        /// </summary>
        AllFractions
    }
}
