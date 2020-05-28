using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Player
{
    public class Skill
    {
        public Skill(DbSkill dbSkill, byte skillNumber, int cooldown)
        {
            Number = skillNumber;
            CooldownInSeconds = cooldown;

            // DB values willl be stored.
            SkillId = dbSkill.SkillId;
            SkillLevel = dbSkill.SkillLevel;
            Type = dbSkill.TypeDetail;
            TargetType = dbSkill.TargetType;
            ResetTime = dbSkill.ResetTime;
            KeepTime = dbSkill.KeepTime;
            CastTime = dbSkill.ReadyTime * 250;
            TypeAttack = dbSkill.TypeAttack;
            StateType = dbSkill.StateType;
            DamageType = dbSkill.DamageType;
            DamageHP = dbSkill.DamageHP;
            DamageSP = dbSkill.DamageSP;
            DamageMP = dbSkill.DamageMP;
            NeedSP = dbSkill.SP;
            NeedMP = dbSkill.MP;
            NeedWeapon1 = dbSkill.NeedWeapon1 == 1;
            NeedWeapon2 = dbSkill.NeedWeapon2 == 1;
            NeedWeapon3 = dbSkill.NeedWeapon3 == 1;
            NeedWeapon4 = dbSkill.NeedWeapon4 == 1;
            NeedWeapon5 = dbSkill.NeedWeapon5 == 1;
            NeedWeapon6 = dbSkill.NeedWeapon6 == 1;
            NeedWeapon7 = dbSkill.NeedWeapon7 == 1;
            NeedWeapon8 = dbSkill.NeedWeapon8 == 1;
            NeedWeapon9 = dbSkill.NeedWeapon9 == 1;
            NeedWeapon10 = dbSkill.NeedWeapon10 == 1;
            NeedWeapon11 = dbSkill.NeedWeapon11 == 1;
            NeedWeapon12 = dbSkill.NeedWeapon12 == 1;
            NeedWeapon13 = dbSkill.NeedWeapon13 == 1;
            NeedWeapon14 = dbSkill.NeedWeapon14 == 1;
            NeedWeapon15 = dbSkill.NeedWeapon15 == 1;
            NeedShield = dbSkill.NeedShield == 1;
            UseSuccessValue = dbSkill.SuccessType == SuccessType.SuccessBasedOnValue;
            SuccessValue = dbSkill.SuccessValue;
        }

        /// <summary>
        /// Skill id.
        /// </summary>
        public ushort SkillId;

        /// <summary>
        /// Skill level.
        /// </summary>
        public byte SkillLevel;

        /// <summary>
        /// Number. This value client sends, when player used any skill.
        /// </summary>
        public byte Number;

        /// <summary>
        /// Countdown in seconds.
        /// </summary>
        public int CooldownInSeconds;

        /// <summary>
        /// Skill type.
        /// </summary>
        public TypeDetail Type;

        /// <summary>
        /// To what target this skill can be applied.
        /// </summary>
        public TargetType TargetType;

        /// <summary>
        /// Passive, physical, magic or shooting attack.
        /// </summary>
        public TypeAttack TypeAttack;

        /// <summary>
        /// Bool indicator, that shows if we need to use success value in calculations.
        /// </summary>
        public bool UseSuccessValue;

        /// <summary>
        /// Success rate in %.
        /// </summary>
        public byte SuccessValue;

        /// <summary>
        /// State type contains information about what bad influence debuff has on target.
        /// </summary>
        public StateType StateType;

        /// <summary>
        /// Time after which skill can be used again.
        /// </summary>
        public ushort ResetTime;

        /// <summary>
        /// Time for example for buffs. This time shows how long the skill will be applied.
        /// </summary>
        public int KeepTime;

        /// <summary>
        /// How long character should wait until skill is casted. In milliseconds.
        /// </summary>
        public int CastTime;

        /// <summary>
        /// Damage type;
        /// </summary>
        public DamageType DamageType;

        /// <summary>
        /// Const damage used, when skill makes fixed damage.
        /// </summary>
        public ushort DamageHP;

        /// <summary>
        /// Const damage used, when skill makes fixed damage.
        /// </summary>
        public ushort DamageSP;

        /// <summary>
        /// Const damage used, when skill makes fixed damage.
        /// </summary>
        public ushort DamageMP;

        /// <summary>
        /// How much stamina is needed in order to use this skill.
        /// </summary>
        public ushort NeedSP;

        /// <summary>
        /// How much mana is needed in order to use this skill.
        /// </summary>
        public ushort NeedMP;

        /// <summary>
        /// Needs 1 Handed Sword.
        /// </summary>
        public bool NeedWeapon1;

        /// <summary>
        /// Needs 2 Handed Sword.
        /// </summary>
        public bool NeedWeapon2;

        /// <summary>
        /// Needs 1 Handed Axe.
        /// </summary>
        public bool NeedWeapon3;

        /// <summary>
        /// Needs 2 Handed Axe.
        /// </summary>
        public bool NeedWeapon4;

        /// <summary>
        /// Needs Double Sword.
        /// </summary>
        public bool NeedWeapon5;

        /// <summary>
        /// Needs 1 Spear.
        /// </summary>
        public bool NeedWeapon6;

        /// <summary>
        /// Needs 1 Handed Blunt.
        /// </summary>
        public bool NeedWeapon7;

        /// <summary>
        /// Needs 2 Handed Blunt.
        /// </summary>
        public bool NeedWeapon8;

        /// <summary>
        /// Needs Reverse sword.
        /// </summary>
        public bool NeedWeapon9;

        /// <summary>
        /// Needs Dagger.
        /// </summary>
        public bool NeedWeapon10;

        /// <summary>
        /// Needs Javelin.
        /// </summary>
        public bool NeedWeapon11;

        /// <summary>
        /// Needs 1 Staff.
        /// </summary>
        public bool NeedWeapon12;

        /// <summary>
        /// Needs Bow.
        /// </summary>
        public bool NeedWeapon13;

        /// <summary>
        /// Needs Crossbow.
        /// </summary>
        public bool NeedWeapon14;

        /// <summary>
        /// Needs Knuckle.
        /// </summary>
        public bool NeedWeapon15;

        private List<byte> _requiredWeapons;
        public List<byte> RequiredWeapons
        {
            get
            {
                // Maybe it's not the best way to check required weapon, but i don't care.
                // I just added item type, that is right compering to NeedWeaponN in original db.
                // E.g. NeedWeapon1 means we need 1 hand sword, which is Type 1 and Type 45.
                if (_requiredWeapons is null)
                {
                    _requiredWeapons = new List<byte>();

                    if (NeedWeapon1)
                    {
                        _requiredWeapons.Add(1);
                        _requiredWeapons.Add(45);
                    }
                    if (NeedWeapon2)
                    {
                        _requiredWeapons.Add(2);
                        _requiredWeapons.Add(46);
                    }
                    if (NeedWeapon3)
                    {
                        _requiredWeapons.Add(3);
                        _requiredWeapons.Add(47);
                    }
                    if (NeedWeapon4)
                    {
                        _requiredWeapons.Add(4);
                        _requiredWeapons.Add(48);
                    }
                    if (NeedWeapon5)
                    {
                        _requiredWeapons.Add(5);
                        _requiredWeapons.Add(49);
                        _requiredWeapons.Add(50);
                    }
                    if (NeedWeapon6)
                    {
                        _requiredWeapons.Add(6);
                        _requiredWeapons.Add(51);
                        _requiredWeapons.Add(52);
                    }
                    if (NeedWeapon7)
                    {
                        _requiredWeapons.Add(7);
                        _requiredWeapons.Add(53);
                        _requiredWeapons.Add(54);
                    }
                    if (NeedWeapon8)
                    {
                        _requiredWeapons.Add(8);
                        _requiredWeapons.Add(55);
                        _requiredWeapons.Add(56);
                    }
                    if (NeedWeapon9)
                    {
                        _requiredWeapons.Add(9);
                        _requiredWeapons.Add(57);
                    }
                    if (NeedWeapon10)
                    {
                        _requiredWeapons.Add(10);
                        _requiredWeapons.Add(58);
                    }
                    if (NeedWeapon11)
                    {
                        _requiredWeapons.Add(11);
                        _requiredWeapons.Add(59);
                    }
                    if (NeedWeapon12)
                    {
                        _requiredWeapons.Add(12);
                        _requiredWeapons.Add(60);
                        _requiredWeapons.Add(61);
                    }
                    if (NeedWeapon13)
                    {
                        _requiredWeapons.Add(13);
                        _requiredWeapons.Add(62);
                        _requiredWeapons.Add(63);
                    }
                    if (NeedWeapon14)
                    {
                        _requiredWeapons.Add(14);
                        _requiredWeapons.Add(64);
                    }
                    if (NeedWeapon15)
                    {
                        _requiredWeapons.Add(15);
                        _requiredWeapons.Add(65);
                    }
                }

                return _requiredWeapons;
            }
        }

        /// <summary>
        /// Needs Shield.
        /// </summary>
        public bool NeedShield;
    }
}
