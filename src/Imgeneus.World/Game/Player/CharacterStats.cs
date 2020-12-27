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
        public Mode Mode { get; private set; } = Mode.Beginner;
        public byte Hair { get; set; }
        public byte Face { get; set; }
        public byte Height { get; set; }
        public Gender Gender { get; set; }
        public ushort StatPoint { get; private set; }
        public ushort SkillPoint { get; private set; }
        public ushort Strength { get; private set; }
        public ushort Dexterity { get; private set; }
        public ushort Reaction { get; private set; }
        public ushort Intelligence { get; private set; }
        public ushort Luck { get; private set; }
        public ushort Wisdom { get; private set; }
        public uint Exp { get; private set; }
        public ushort Kills { get; private set; }
        public ushort Deaths { get; private set; }
        public ushort Victories { get; set; }
        public ushort Defeats { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsRename { get; set; }

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

        /// <summary>
        /// Event that's fired when max hp, mp and sp change.
        /// </summary>
        public event Action<Character> OnMax_HP_MP_SP_Changed;

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
                return ConstHP + ExtraHP + ReactionExtraHP;
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
                return ConstMP + ExtraMP + WisdomExtraMP;
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
                return ConstSP + ExtraSP + DexterityExtraSP;
            }
        }

        /// <summary>
        /// Gets the character's primary stat
        /// </summary>
        public CharacterStatEnum GetPrimaryStat()
        {
            var defaultStat = _characterConfig.DefaultStats.First(s => s.Job == Class);

            switch (defaultStat.MainStat)
            {
                case 0:
                    return CharacterStatEnum.Strength;

                case 1:
                    return CharacterStatEnum.Dexterity;

                case 2:
                    return CharacterStatEnum.Reaction;

                case 3:
                    return CharacterStatEnum.Intelligence;

                case 4:
                    return CharacterStatEnum.Wisdom;

                case 5:
                    return CharacterStatEnum.Luck;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the character's primary stat
        /// </summary>
        public CharacterAttributeEnum GetAttributeByStat(CharacterStatEnum stat)
        {
            switch (stat)
            {
                case CharacterStatEnum.Strength:
                    return CharacterAttributeEnum.Strength;

                case CharacterStatEnum.Dexterity:
                    return CharacterAttributeEnum.Dexterity;

                case CharacterStatEnum.Reaction:
                    return CharacterAttributeEnum.Reaction;

                case CharacterStatEnum.Intelligence:
                    return CharacterAttributeEnum.Intelligence;

                case CharacterStatEnum.Wisdom:
                    return CharacterAttributeEnum.Wisdom;

                case CharacterStatEnum.Luck:
                    return CharacterAttributeEnum.Luck;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Increases a character's main stat by a certain amount
        /// </summary>
        /// <param name="amount">Decrease amount</param>
        public void IncreasePrimaryStat(ushort amount = 1)
        {
            var primaryAttribute = GetPrimaryStat();

            switch (primaryAttribute)
            {
                case CharacterStatEnum.Strength:
                    Strength += amount;
                    break;

                case CharacterStatEnum.Dexterity:
                    Dexterity += amount;
                    break;

                case CharacterStatEnum.Reaction:
                    Reaction += amount;
                    break;

                case CharacterStatEnum.Intelligence:
                    Intelligence += amount;
                    break;

                case CharacterStatEnum.Wisdom:
                    Wisdom += amount;
                    break;

                case CharacterStatEnum.Luck:
                    Luck += amount;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Decreases a character's main stat by a certain amount
        /// </summary>
        /// <param name="amount">Decrease amount</param>
        public void DecreasePrimaryStat(ushort amount = 1)
        {
            var primaryAttribute = GetPrimaryStat();

            switch (primaryAttribute)
            {
                case CharacterStatEnum.Strength:
                    Strength -= amount;
                    break;

                case CharacterStatEnum.Dexterity:
                    Dexterity -= amount;
                    break;

                case CharacterStatEnum.Reaction:
                    Reaction -= amount;
                    break;

                case CharacterStatEnum.Intelligence:
                    Intelligence -= amount;
                    break;

                case CharacterStatEnum.Wisdom:
                    Wisdom -= amount;
                    break;

                case CharacterStatEnum.Luck:
                    Luck -= amount;
                    break;

                default:
                    break;
            }
        }
        /// Extra HP given by Reaction formula
        /// </summary>
        public int ReactionExtraHP => Reaction * 5;

        /// <summary>
        /// Extra MP given by Wisdom formula
        /// </summary>
        public int WisdomExtraMP => Wisdom * 5;

        /// <summary>
        /// Extra SP given by Dexterity formula
        /// </summary>
        public int DexterityExtraSP => Dexterity * 5;

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

                if (DefenceSkillElement != Element.None)
                    return DefenceSkillElement;

                if (Armor is null)
                    return Element.None;

                return Armor.Element;
            }
        }

        /// <inheritdoc />
        public override Element AttackElement
        {
            get
            {
                if (AttackSkillElement != Element.None)
                    return AttackSkillElement;

                if (Weapon is null)
                    return Element.None;

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

            var statPerLevel = _characterConfig.GetLevelStatSkillPoints(Mode).StatPoint;

            SetStatPoint((ushort)((Level - 1) * statPerLevel)); // Level - 1, because we are starting with 1 level.

            IncreasePrimaryStat((ushort)(Level - 1));

            _taskQueue.Enqueue(ActionType.UPDATE_STATS, Id, Strength, Dexterity, Reaction, Intelligence, Wisdom, Luck, StatPoint);
            _packetsHelper.SendResetStats(Client, this);
            SendAdditionalStats();
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Gets a character's attribute.
        /// </summary>
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

        /// <summary>
        /// Change a character's stat value.
        /// </summary>
        /// <param name="statAttribute">Stat to change</param>
        /// <param name="newStatValue">New stat value</param>
        public void SetStat(CharacterAttributeEnum statAttribute, ushort newStatValue)
        {
            switch (statAttribute)
            {
                case CharacterAttributeEnum.Strength:
                    Strength = newStatValue;
                    break;

                case CharacterAttributeEnum.Dexterity:
                    Dexterity = newStatValue;
                    SendMaxSP();
                    break;

                case CharacterAttributeEnum.Reaction:
                    Reaction = newStatValue;
                    SendMaxHP();
                    break;

                case CharacterAttributeEnum.Intelligence:
                    Intelligence = newStatValue;
                    break;

                case CharacterAttributeEnum.Luck:
                    Luck = newStatValue;
                    break;

                case CharacterAttributeEnum.Wisdom:
                    Wisdom = newStatValue;
                    SendMaxMP();
                    break;

                default:
                    return;
            }

            SendAdditionalStats();

            _taskQueue.Enqueue(ActionType.UPDATE_STATS, Id, Strength, Dexterity, Reaction, Intelligence, Wisdom, Luck, StatPoint);
        }

        #endregion

        #region Mode

        /// <summary>
        /// Set the mode (Grow)
        /// </summary>
        private void SetMode(Mode mode)
        {
            if (mode > Mode.Ultimate) return;

            Mode = mode;

            _taskQueue.Enqueue(ActionType.UPDATE_CHARACTER_MODE, Id, mode);
        }

        /// <summary>
        /// Attempts to set a new mode for a character
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool TrySetMode(Mode mode)
        {
            if (mode > Mode.Ultimate)
                return false;

            SetMode(mode);
            return true;
        }

        #endregion

        #region Stat and Skill Points

        /// <summary>
        /// Set the stat points amount
        /// </summary>
        public void SetStatPoint(ushort statPoint)
        {
            StatPoint = statPoint;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_STATPOINT, Id, StatPoint);
        }

        /// <summary>
        /// Increases the player's stat points by a certain amount
        /// </summary>
        /// <param name="amount"></param>
        public void IncreaseStatPoint(ushort amount) => SetStatPoint(StatPoint += amount);

        /// <summary>
        /// Set the skill points amount
        /// </summary>
        public void SetSkillPoint(ushort skillPoint)
        {
            SkillPoint = skillPoint;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_SKILLPOINT, Id, SkillPoint);
        }

        /// <summary>
        /// Increases the player's skill points by a certain amount
        /// </summary>
        /// <param name="amount"></param>
        public void IncreaseSkillPoint(ushort amount) => SetSkillPoint(StatPoint += amount);

        #endregion

        #region Kills and Deaths

        /// <summary>
        /// Sets the kill count
        /// </summary>
        public void SetKills(ushort kills)
        {
            Kills = kills;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_KILLS, Id, Kills);
        }

        /// <summary>
        /// Sets the death count
        /// </summary>
        public void SetDeaths(ushort deaths)
        {
            Deaths = deaths;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_DEATHS, Id, Deaths);
        }

        #endregion
    }
}
