using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.Network.Packets.Game;
using System;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        #region Character info

        public string Name { get; set; } = "";
        public Fraction Country { get; set; }
        public ushort MapId { get; private set; }
        public Race Race { get; set; }
        public CharacterProfession Class { get; set; }
        public Mode Mode { get; set; }
        public byte Hair { get; set; }
        public byte Face { get; set; }
        public byte Height { get; set; }
        public Gender Gender { get; set; }
        public ushort StatPoint { get; set; }
        public ushort SkillPoint { get; set; }
        public ushort Strength { get; set; }
        public ushort Dexterity { get; set; }
        public ushort Reaction { get; set; }
        public ushort Intelligence { get; set; }
        public ushort Luck { get; set; }
        public ushort Wisdom { get; set; }
        public uint Exp { get; set; }
        public ushort Kills { get; set; }
        public ushort Deaths { get; set; }
        public ushort Victories { get; set; }
        public ushort Defeats { get; set; }
        public bool IsAdmin { get; set; }

        private byte[] _nameAsByteArray;
        public byte[] NameAsByteArray
        {
            get
            {
                if (_nameAsByteArray is null)
                {
                    _nameAsByteArray = new byte[21];

                    var chars = Name.ToCharArray(0, Name.Length);
                    for (var i = 0; i < chars.Length; i++)
                    {
                        _nameAsByteArray[i] = (byte)chars[i];
                    }
                }
                return _nameAsByteArray;
            }
        }

        #endregion

        #region Total stats

        public int TotalStr => Strength + ExtraStr;
        public override int TotalDex => Dexterity + ExtraDex;
        public int TotalRec => Reaction + ExtraRec;
        public int TotalInt => Intelligence + ExtraInt;
        public override int TotalWis => Wisdom + ExtraWis;
        public override int TotalLuc => Luck + ExtraLuc;

        #endregion

        #region Max HP & SP & MP

        private void Character_OnMaxHPChanged(IKillable sender, int maxHP)
        {
            if (Client != null)
                SendMaxHP();
        }

        private void Character_OnMaxMPChanged(IKillable sender, int maxMP)
        {
            if (Client != null)
                SendMaxMP();
        }

        private void Character_OnMaxSPChanged(IKillable sender, int maxSP)
        {
            if (Client != null)
                SendMaxSP();
        }

        /// <summary>
        /// HP based on level.
        /// </summary>
        private int ConstHP
        {
            get
            {
                var level = Level <= 60 ? Level : 60;
                var index = (level - 1) * 6 + (byte)Class;
                var constHP = _characterConfig.GetConfig(index).HP;

                return constHP;
            }
        }

        public override int MaxHP
        {
            get
            {
                return ConstHP + ExtraHP;
            }
        }

        /// <summary>
        /// MP based on level.
        /// </summary>
        private int ConstMP
        {
            get
            {
                var level = Level <= 60 ? Level : 60;
                var index = (level - 1) * 6 + (byte)Class;
                var constMP = _characterConfig.GetConfig(index).MP;

                return constMP;
            }
        }

        public override int MaxMP
        {
            get
            {
                return ConstMP + ExtraMP;
            }
        }

        /// <summary>
        /// SP based on level.
        /// </summary>
        private int ConstSP
        {
            get
            {
                var level = Level <= 60 ? Level : 60;
                var index = (level - 1) * 6 + (byte)Class;
                var constSP = _characterConfig.GetConfig(index).SP;

                return constSP;
            }
        }

        public override int MaxSP
        {
            get
            {
                return ConstSP + ExtraSP;
            }
        }

        #endregion

        #region Defense & Resistance

        /// <summary>
        /// Physical defense.
        /// </summary>
        public override int Defense
        {
            get
            {
                return TotalRec + ExtraDefense;
            }
        }

        /// <summary>
        /// Magic resistance.
        /// </summary>
        public override int Resistance
        {
            get
            {
                return TotalWis + ExtraResistance;
            }
        }

        #endregion

        #region Attack & Move speed

        /// <summary>
        /// Pure weapon speed without any gems or buffs.
        /// </summary>
        private byte _weaponSpeed;

        /// <summary>
        /// Sets weapon speed.
        /// </summary>
        private void SetWeaponSpeed(byte speed)
        {
            _weaponSpeed = speed;
            InvokeAttackOrMoveChanged();
        }

        private int NextAttackTime
        {
            get
            {
                switch (AttackSpeed)
                {
                    case AttackSpeed.ExteremelySlow:
                        return 4000;

                    case AttackSpeed.VerySlow:
                        return 3750;

                    case AttackSpeed.Slow:
                        return 3500;

                    case AttackSpeed.ABitSlow:
                        return 3250;

                    case AttackSpeed.Normal:
                        return 3000;

                    case AttackSpeed.ABitFast:
                        return 2750;

                    case AttackSpeed.Fast:
                        return 2500;

                    case AttackSpeed.VeryFast:
                        return 2250;

                    case AttackSpeed.ExteremelyFast:
                        return 2000;

                    default:
                        return 2000;
                }
            }
        }

        /// <summary>
        /// How fast character can make new hit.
        /// </summary>
        public override AttackSpeed AttackSpeed
        {
            get
            {
                if (_weaponSpeed == 0)
                    return AttackSpeed.None;

                var weaponType = Weapon.ToPassiveSkillType();
                _weaponSpeedPassiveSkillModificator.TryGetValue(weaponType, out var passiveSkillModifier);

                var finalSpeed = _weaponSpeed + _attackSpeedModifier + passiveSkillModifier;

                if (finalSpeed < 0)
                    return AttackSpeed.ExteremelySlow;

                if (finalSpeed > 9)
                    return AttackSpeed.ExteremelyFast;

                return (AttackSpeed)finalSpeed;
            }
        }

        private int _moveSpeed = 2; // 2 == normal by default.
        /// <summary>
        /// How fast character moves.
        /// </summary>
        public override int MoveSpeed
        {
            protected set
            {
                if (_moveSpeed == value)
                    return;

                if (value < 0)
                    value = 0;

                _moveSpeed = value;
                InvokeAttackOrMoveChanged();
            }
            get
            {
                if (ActiveBuffs.Any(b => b.StateType == StateType.Sleep || b.StateType == StateType.Stun || b.StateType == StateType.Immobilize))
                    return (int)MoveSpeedEnum.CanNotMove;

                if (IsStealth)
                    return (int)MoveSpeedEnum.Normal;

                if (IsOnVehicle)
                    return (int)MoveSpeedEnum.VeryFast;

                return _moveSpeed;
            }
        }

        #endregion

        #region Min/Max Attack & Magic attack

        /// <summary>
        /// Calculates character attack, based on character profession.
        /// </summary>
        private int GetCharacterAttack()
        {
            int characterAttack;
            switch (Class)
            {
                case CharacterProfession.Fighter:
                case CharacterProfession.Defender:
                case CharacterProfession.Ranger:
                    characterAttack = (int)(Math.Floor(1.3 * TotalStr) + Math.Floor(0.25 * TotalDex));
                    break;

                case CharacterProfession.Mage:
                case CharacterProfession.Priest:
                    characterAttack = (int)(Math.Floor(1.3 * TotalInt) + Math.Floor(0.2 * TotalWis));
                    break;

                case CharacterProfession.Archer:
                    characterAttack = (int)(TotalStr + Math.Floor(0.3 * TotalLuc) + Math.Floor(0.2 * TotalDex));
                    break;

                default:
                    throw new NotImplementedException("Not implemented job.");
            }

            return characterAttack;
        }

        /// <summary>
        /// Min physical attack.
        /// </summary>
        public int MinAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MinAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Fighter ||
                    Class == CharacterProfession.Defender ||
                    Class == CharacterProfession.Ranger ||
                    Class == CharacterProfession.Archer)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillPhysicalAttackPower;
            }
        }

        /// <summary>
        /// Max physical attack.
        /// </summary>
        public int MaxAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MaxAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Fighter ||
                    Class == CharacterProfession.Defender ||
                    Class == CharacterProfession.Ranger ||
                    Class == CharacterProfession.Archer)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillPhysicalAttackPower;
            }
        }

        /// <summary>
        /// Min magic attack.
        /// </summary>
        public int MinMagicAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MinAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Mage ||
                    Class == CharacterProfession.Priest)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillMagicAttackPower;
            }
        }

        /// <summary>
        /// Max magic attack.
        /// </summary>
        public int MaxMagicAttack
        {
            get
            {
                var weaponAttack = Weapon != null ? Weapon.MaxAttack : 0;
                int characterAttack = 0;

                if (Class == CharacterProfession.Mage ||
                    Class == CharacterProfession.Priest)
                {
                    characterAttack = GetCharacterAttack();
                }

                return weaponAttack + characterAttack + _skillMagicAttackPower;
            }
        }

        #endregion

        #region Elements

        /// <inheritdoc />
        public override Element DefenceElement
        {
            get
            {
                if (RemoveElement)
                    return Element.None;

                if (Armor is null)
                    return Element.None;

                if (DefenceSkillElement != Element.None)
                    return DefenceSkillElement;

                return Armor.Element;
            }
        }

        /// <inheritdoc />
        public override Element AttackElement
        {
            get
            {
                if (Weapon is null)
                    return Element.None;

                if (AttackSkillElement != Element.None)
                    return AttackSkillElement;

                return Weapon.Element;
            }
        }

        #endregion

        #region Reset stats

        public void ResetStats()
        {
            var defaultStat = _characterConfig.DefaultStats.First(s => s.Job == Class);
            Strength = defaultStat.Str;
            Dexterity = defaultStat.Dex;
            Reaction = defaultStat.Rec;
            Intelligence = defaultStat.Int;
            Wisdom = defaultStat.Wis;
            Luck = defaultStat.Luc;

            ushort statPerLevel;
            switch (Mode)
            {
                case Mode.Beginner:
                    statPerLevel = 3;
                    break;

                case Mode.Normal:
                    statPerLevel = 5;
                    break;

                case Mode.Hard:
                    statPerLevel = 7;
                    break;

                case Mode.Ultimate:
                    statPerLevel = 9;
                    break;

                default:
                    statPerLevel = 0;
                    break;
            }

            StatPoint = (ushort)((Level - 1) * statPerLevel); // Level - 1, because we are starting with 1 level.

            switch (defaultStat.MainStat)
            {
                case 0:
                    Strength += (ushort)(Level - 1);
                    break;

                case 1:
                    Dexterity += (ushort)(Level - 1);
                    break;

                case 2:
                    Reaction += (ushort)(Level - 1);
                    break;

                case 3:
                    Intelligence += (ushort)(Level - 1);
                    break;

                case 4:
                    Wisdom += (ushort)(Level - 1);
                    break;

                case 5:
                    Luck += (ushort)(Level - 1);
                    break;

                default:
                    break;
            }

            _taskQueue.Enqueue(ActionType.UPDATE_STATS, Id, Strength, Dexterity, Reaction, Intelligence, Wisdom, Luck, StatPoint);
            _packetsHelper.SendResetStats(Client, this);
            SendAdditionalStats();
        }

        #endregion

        #region Attributes

        public uint GetAttributeValue(CharacterAttributeEnum attribute)
        {
            switch (attribute)
            {
                case CharacterAttributeEnum.Grow:
                    return (uint)Mode;

                case CharacterAttributeEnum.Level:
                    return Level;

                case CharacterAttributeEnum.Money:
                    return Gold;

                case CharacterAttributeEnum.StatPoint:
                    return StatPoint;

                case CharacterAttributeEnum.SkillPoint:
                    return SkillPoint;

                case CharacterAttributeEnum.Strength:
                    return Strength;

                case CharacterAttributeEnum.Dexterity:
                    return Dexterity;

                case CharacterAttributeEnum.Reaction:
                    return Reaction;

                case CharacterAttributeEnum.Intelligence:
                    return Intelligence;

                case CharacterAttributeEnum.Luck:
                    return Luck;

                case CharacterAttributeEnum.Wisdom:
                    return Wisdom;

                // TODO: Investigate what these attributes represent
                case CharacterAttributeEnum.Hg:
                case CharacterAttributeEnum.Vg:
                case CharacterAttributeEnum.Cg:
                case CharacterAttributeEnum.Og:
                case CharacterAttributeEnum.Ig:
                    return 0;

                case CharacterAttributeEnum.Exp:
                    return Exp;

                case CharacterAttributeEnum.Kills:
                    return Kills;

                case CharacterAttributeEnum.Deaths:
                    return Deaths;

                default:
                    return 0;
            }
        }

        public void SetStat(CharacterAttributeEnum statAttribute, ushort newStatValue)
        {
            switch (statAttribute)
            {
                case CharacterAttributeEnum.Strength:
                    Strength = newStatValue;
                    break;
                case CharacterAttributeEnum.Dexterity:
                    Dexterity = newStatValue;
                    break;
                case CharacterAttributeEnum.Reaction:
                    Reaction = newStatValue;
                    break;
                case CharacterAttributeEnum.Intelligence:
                    Intelligence = newStatValue;
                    break;
                case CharacterAttributeEnum.Luck:
                    Luck = newStatValue;
                    break;
                case CharacterAttributeEnum.Wisdom:
                    Wisdom = newStatValue;
                    break;
                default:
                    return;
            }

            SendAttribute(statAttribute);
        }

        #endregion

        #region Mode

        /// <summary>
        /// Set the mode (Grow)
        /// </summary>
        public void SetMode(Mode mode)
        {
            Mode = mode;
            SendAttribute(CharacterAttributeEnum.Grow);
        }

        #endregion

        #region Stat and Skill Points

        /// <summary>
        /// Set the stat points amount
        /// </summary>
        public void SetStatPoint(ushort statPoint)
        {
            StatPoint = statPoint;
            SendAttribute(CharacterAttributeEnum.StatPoint);
        }

        /// <summary>
        /// Set the skill points amount
        /// </summary>
        public void SetSkillPoint(ushort skillPoint)
        {
            SkillPoint = skillPoint;
            SendAttribute(CharacterAttributeEnum.SkillPoint);
        }

        #endregion

        #region Kills and Deaths

        /// <summary>
        /// Sets the kill count
        /// </summary>
        public void SetKills(ushort kills)
        {
            Kills = kills;
            SendAttribute(CharacterAttributeEnum.Kills);
        }

        /// <summary>
        /// Sets the death count
        /// </summary>
        public void SetDeaths(ushort deaths)
        {
            Deaths = deaths;
            SendAttribute(CharacterAttributeEnum.Deaths);
        }

        #endregion
    }
}
