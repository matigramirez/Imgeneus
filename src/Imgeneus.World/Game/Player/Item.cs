using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Dyeing;
using System.Collections.Generic;
using System.Text;

namespace Imgeneus.World.Game.Player
{
    public class Item
    {
        private readonly IDatabasePreloader _databasePreloader;
        private readonly DbItem _dbItem;

        /// <summary>
        /// Unique type, used only for drop money on map.
        /// </summary>
        public const byte MONEY_ITEM_TYPE = 26;

        public byte Bag;
        public byte Slot;
        public byte Type;
        public byte TypeId;
        public ushort Quality;

        public Gem Gem1;
        public Gem Gem2;
        public Gem Gem3;
        public Gem Gem4;
        public Gem Gem5;
        public Gem Gem6;

        public byte Count;
        
        public Item(IDatabasePreloader databasePreloader, DbCharacterItems dbItem) : this(databasePreloader, dbItem.Type, dbItem.TypeId, dbItem.Count)
        {
            Bag = dbItem.Bag;
            Slot = dbItem.Slot;
            Quality = dbItem.Quality;

            if (!string.IsNullOrWhiteSpace(dbItem.Craftname))
                ParseCraftname(dbItem.Craftname);

            if (dbItem.HasDyeColor)
                DyeColor = new DyeColor(dbItem.DyeColorAlpha, dbItem.DyeColorSaturation, dbItem.DyeColorR, dbItem.DyeColorG, dbItem.DyeColorB);

            if (dbItem.GemTypeId1 != 0)
                Gem1 = new Gem(databasePreloader, dbItem.GemTypeId1, 0);
            if (dbItem.GemTypeId2 != 0)
                Gem2 = new Gem(databasePreloader, dbItem.GemTypeId2, 1);
            if (dbItem.GemTypeId3 != 0)
                Gem3 = new Gem(databasePreloader, dbItem.GemTypeId3, 2);
            if (dbItem.GemTypeId4 != 0)
                Gem4 = new Gem(databasePreloader, dbItem.GemTypeId4, 3);
            if (dbItem.GemTypeId5 != 0)
                Gem5 = new Gem(databasePreloader, dbItem.GemTypeId5, 4);
            if (dbItem.GemTypeId6 != 0)
                Gem6 = new Gem(databasePreloader, dbItem.GemTypeId6, 5);
        }

        public Item(IDatabasePreloader databasePreloader, byte type, byte typeId, byte count = 1)
        {
            _databasePreloader = databasePreloader;
            Type = type;
            TypeId = typeId;
            Count = count;

            if (Type != 0 && TypeId != 0 && Type != MONEY_ITEM_TYPE)
            {
                _dbItem = _databasePreloader.Items[(Type, TypeId)];
                // Prevent Count from exceeding MaxCount and from being 0 (zero)
                var newCount = count > MaxCount ? MaxCount : count;
                Count = newCount < 1 ? (byte)1 : newCount;
            }
        }

        #region Trade

        public byte TradeQuantity;

        #endregion

        #region Extra stats

        private int ConstStr => _dbItem.ConstStr;
        private int ConstDex => _dbItem.ConstDex;
        private int ConstRec => _dbItem.ConstRec;
        private int ConstInt => _dbItem.ConstInt;
        private int ConstLuc => _dbItem.ConstLuc;
        private int ConstWis => _dbItem.ConstWis;
        private int ConstHP => _dbItem.ConstHP;
        private int ConstMP => _dbItem.ConstMP;
        private int ConstSP => _dbItem.ConstSP;
        private byte ConstAttackSpeed => _dbItem.AttackTime;
        private byte ConstMoveSpeed => _dbItem.Speed;
        private ushort ConstDefense => _dbItem.Defense;
        private ushort ConstResistance => _dbItem.Resistance;
        private ushort ConstMinAttack => _dbItem.MinAttack;
        private ushort ConstPlusAttack => _dbItem.PlusAttack;
        private Element ConstElement => _dbItem.Element;
        public int Sell => _dbItem.Sell;
        public byte ReqIg => _dbItem.ReqIg;

        /// <summary>
        /// Str contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Str
        {
            get
            {
                ushort gemsStr = 0;

                if (Gem1 != null)
                    gemsStr += Gem1.Str;
                if (Gem2 != null)
                    gemsStr += Gem2.Str;
                if (Gem3 != null)
                    gemsStr += Gem3.Str;
                if (Gem4 != null)
                    gemsStr += Gem4.Str;
                if (Gem5 != null)
                    gemsStr += Gem5.Str;
                if (Gem6 != null)
                    gemsStr += Gem6.Str;

                return ConstStr + gemsStr + ComposedStr;
            }
        }

        /// <summary>
        /// Dex contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Dex
        {
            get
            {
                ushort gemsDex = 0;

                if (Gem1 != null)
                    gemsDex += Gem1.Dex;
                if (Gem2 != null)
                    gemsDex += Gem2.Dex;
                if (Gem3 != null)
                    gemsDex += Gem3.Dex;
                if (Gem4 != null)
                    gemsDex += Gem4.Dex;
                if (Gem5 != null)
                    gemsDex += Gem5.Dex;
                if (Gem6 != null)
                    gemsDex += Gem6.Dex;

                return ConstDex + gemsDex + ComposedDex;
            }
        }

        /// <summary>
        /// Rec contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Rec
        {
            get
            {
                ushort gemsRec = 0;

                if (Gem1 != null)
                    gemsRec += Gem1.Rec;
                if (Gem2 != null)
                    gemsRec += Gem2.Rec;
                if (Gem3 != null)
                    gemsRec += Gem3.Rec;
                if (Gem4 != null)
                    gemsRec += Gem4.Rec;
                if (Gem5 != null)
                    gemsRec += Gem5.Rec;
                if (Gem6 != null)
                    gemsRec += Gem6.Rec;

                return ConstRec + gemsRec + ComposedRec;
            }
        }

        /// <summary>
        /// Int contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Int
        {
            get
            {
                ushort gemsInt = 0;

                if (Gem1 != null)
                    gemsInt += Gem1.Int;
                if (Gem2 != null)
                    gemsInt += Gem2.Int;
                if (Gem3 != null)
                    gemsInt += Gem3.Int;
                if (Gem4 != null)
                    gemsInt += Gem4.Int;
                if (Gem5 != null)
                    gemsInt += Gem5.Int;
                if (Gem6 != null)
                    gemsInt += Gem6.Int;

                return ConstInt + gemsInt + ComposedInt;
            }
        }

        /// <summary>
        /// Luc contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Luc
        {
            get
            {
                ushort gemsLuc = 0;

                if (Gem1 != null)
                    gemsLuc += Gem1.Luc;
                if (Gem2 != null)
                    gemsLuc += Gem2.Luc;
                if (Gem3 != null)
                    gemsLuc += Gem3.Luc;
                if (Gem4 != null)
                    gemsLuc += Gem4.Luc;
                if (Gem5 != null)
                    gemsLuc += Gem5.Luc;
                if (Gem6 != null)
                    gemsLuc += Gem6.Luc;

                return ConstLuc + gemsLuc + ComposedLuc;
            }
        }

        /// <summary>
        /// Wis contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Wis
        {
            get
            {
                ushort gemsWis = 0;

                if (Gem1 != null)
                    gemsWis += Gem1.Wis;
                if (Gem2 != null)
                    gemsWis += Gem2.Wis;
                if (Gem3 != null)
                    gemsWis += Gem3.Wis;
                if (Gem4 != null)
                    gemsWis += Gem4.Wis;
                if (Gem5 != null)
                    gemsWis += Gem5.Wis;
                if (Gem6 != null)
                    gemsWis += Gem6.Wis;

                return ConstWis + gemsWis + ComposedWis;
            }
        }

        /// <summary>
        /// HP stats.
        /// </summary>
        public int HP
        {
            get
            {
                ushort gemsHP = 0;

                if (Gem1 != null)
                    gemsHP += Gem1.HP;
                if (Gem2 != null)
                    gemsHP += Gem2.HP;
                if (Gem3 != null)
                    gemsHP += Gem3.HP;
                if (Gem4 != null)
                    gemsHP += Gem4.HP;
                if (Gem5 != null)
                    gemsHP += Gem5.HP;
                if (Gem6 != null)
                    gemsHP += Gem6.HP;

                return ConstHP + gemsHP + ComposedHP;
            }
        }

        /// <summary>
        /// MP stats.
        /// </summary>
        public int MP
        {
            get
            {
                ushort gemsMP = 0;

                if (Gem1 != null)
                    gemsMP += Gem1.MP;
                if (Gem2 != null)
                    gemsMP += Gem2.MP;
                if (Gem3 != null)
                    gemsMP += Gem3.MP;
                if (Gem4 != null)
                    gemsMP += Gem4.MP;
                if (Gem5 != null)
                    gemsMP += Gem5.MP;
                if (Gem6 != null)
                    gemsMP += Gem6.MP;

                return ConstMP + gemsMP + ComposedMP;
            }
        }

        /// <summary>
        /// SP stats.
        /// </summary>
        public int SP
        {
            get
            {
                ushort gemsSP = 0;

                if (Gem1 != null)
                    gemsSP += Gem1.SP;
                if (Gem2 != null)
                    gemsSP += Gem2.SP;
                if (Gem3 != null)
                    gemsSP += Gem3.SP;
                if (Gem4 != null)
                    gemsSP += Gem4.SP;
                if (Gem5 != null)
                    gemsSP += Gem5.SP;
                if (Gem6 != null)
                    gemsSP += Gem6.SP;

                return ConstSP + gemsSP + ComposedSP;
            }
        }

        public byte AttackSpeed
        {
            get
            {
                byte gemsSpeed = 0;

                if (Gem1 != null)
                    gemsSpeed += Gem1.AttackSpeed;
                if (Gem2 != null)
                    gemsSpeed += Gem2.AttackSpeed;
                if (Gem3 != null)
                    gemsSpeed += Gem3.AttackSpeed;
                if (Gem4 != null)
                    gemsSpeed += Gem4.AttackSpeed;
                if (Gem5 != null)
                    gemsSpeed += Gem5.AttackSpeed;
                if (Gem6 != null)
                    gemsSpeed += Gem6.AttackSpeed;

                return (byte)(ConstAttackSpeed + gemsSpeed);
            }
        }

        public byte MoveSpeed
        {
            get
            {
                byte gemsSpeed = 0;

                if (Gem1 != null)
                    gemsSpeed += Gem1.MoveSpeed;
                if (Gem2 != null)
                    gemsSpeed += Gem2.MoveSpeed;
                if (Gem3 != null)
                    gemsSpeed += Gem3.MoveSpeed;
                if (Gem4 != null)
                    gemsSpeed += Gem4.MoveSpeed;
                if (Gem5 != null)
                    gemsSpeed += Gem5.MoveSpeed;
                if (Gem6 != null)
                    gemsSpeed += Gem6.MoveSpeed;

                return (byte)(ConstMoveSpeed + gemsSpeed);
            }
        }

        public int Defense
        {
            get
            {
                int gemDefense = 0;
                if (Gem1 != null)
                    gemDefense += Gem1.Defense;
                if (Gem2 != null)
                    gemDefense += Gem2.Defense;
                if (Gem3 != null)
                    gemDefense += Gem3.Defense;
                if (Gem4 != null)
                    gemDefense += Gem4.Defense;
                if (Gem5 != null)
                    gemDefense += Gem5.Defense;
                if (Gem6 != null)
                    gemDefense += Gem6.Defense;

                return ConstDefense + gemDefense;
            }
        }

        public int Resistance
        {
            get
            {
                int gemResistance = 0;
                if (Gem1 != null)
                    gemResistance += Gem1.Resistance;
                if (Gem2 != null)
                    gemResistance += Gem2.Resistance;
                if (Gem3 != null)
                    gemResistance += Gem3.Resistance;
                if (Gem4 != null)
                    gemResistance += Gem4.Resistance;
                if (Gem5 != null)
                    gemResistance += Gem5.Resistance;
                if (Gem6 != null)
                    gemResistance += Gem6.Resistance;

                return ConstResistance + gemResistance;
            }
        }

        public int MinAttack
        {
            get
            {
                int gemMinAttack = 0;
                if (Gem1 != null)
                    gemMinAttack += Gem1.MinAttack;
                if (Gem2 != null)
                    gemMinAttack += Gem2.MinAttack;
                if (Gem3 != null)
                    gemMinAttack += Gem3.MinAttack;
                if (Gem4 != null)
                    gemMinAttack += Gem4.MinAttack;
                if (Gem5 != null)
                    gemMinAttack += Gem5.MinAttack;
                if (Gem6 != null)
                    gemMinAttack += Gem6.MinAttack;

                return ConstMinAttack + gemMinAttack;
            }
        }

        public int MaxAttack
        {
            get
            {
                int gemPlusAttack = 0;
                if (Gem1 != null)
                    gemPlusAttack += Gem1.PlusAttack;
                if (Gem2 != null)
                    gemPlusAttack += Gem2.PlusAttack;
                if (Gem3 != null)
                    gemPlusAttack += Gem3.PlusAttack;
                if (Gem4 != null)
                    gemPlusAttack += Gem4.PlusAttack;
                if (Gem5 != null)
                    gemPlusAttack += Gem5.PlusAttack;
                if (Gem6 != null)
                    gemPlusAttack += Gem6.PlusAttack;

                return ConstMinAttack + gemPlusAttack + ConstPlusAttack;
            }
        }

        public Element Element
        {
            get
            {
                if (ConstElement != Element.None)
                    return ConstElement;

                if (Gem1 != null && Gem1.Element != Element.None)
                    return Gem1.Element;

                if (Gem2 != null && Gem2.Element != Element.None)
                    return Gem2.Element;

                if (Gem3 != null && Gem3.Element != Element.None)
                    return Gem3.Element;

                if (Gem4 != null && Gem4.Element != Element.None)
                    return Gem4.Element;

                if (Gem5 != null && Gem5.Element != Element.None)
                    return Gem5.Element;

                if (Gem6 != null && Gem6.Element != Element.None)
                    return Gem6.Element;

                return Element.None;
            }
        }

        /// <summary>
        /// Special effect like teleport/cure/summon etc.
        /// </summary>
        public SpecialEffect Special => _dbItem.Special;

        /// <summary>
        /// If item can be given away.
        /// </summary>
        public ItemAccountRestrictionType AccountRestriction => _dbItem.ReqOg;

        /// <summary>
        /// For mounts, its value specifies which character shape we should use.
        /// </summary>
        public ushort Range => _dbItem.Range;

        /// <summary>
        /// Can be used in basic, ultimate etc. mode.
        /// </summary>
        public byte Grow => _dbItem.Grow;

        /// <summary>
        /// Defines "color" of item.
        /// </summary>
        public ushort ReqDex => _dbItem.ReqDex;

        /// <summary>
        /// For linking hammer, it's how many times it increases the success linking rate.
        /// For lapis, if it's set to 1, lapis can break equipment while unsuccessful linking.
        /// </summary>
        public ushort ReqVg => _dbItem.ReqVg;

        #endregion

        #region Craft name stats (orange stats)

        public const string DEFAULT_CRAFT_NAME = "00000000000000000000";

        /// <summary>
        /// Orange str stat.
        /// </summary>
        public int ComposedStr { get; set; }

        /// <summary>
        /// Orange dex stat.
        /// </summary>
        public int ComposedDex { get; set; }

        /// <summary>
        /// Orange rec stat.
        /// </summary>
        public int ComposedRec { get; set; }

        /// <summary>
        /// Orange str stat.
        /// </summary>
        public int ComposedInt { get; set; }

        /// <summary>
        /// Orange str stat.
        /// </summary>
        public int ComposedLuc { get; set; }

        /// <summary>
        /// Orange str stat.
        /// </summary>
        public int ComposedWis { get; set; }

        /// <summary>
        /// Orange str stat.
        /// </summary>
        public int ComposedHP { get; set; }

        /// <summary>
        /// Orange str stat.
        /// </summary>
        public int ComposedMP { get; set; }

        /// <summary>
        /// Orange str stat.
        /// </summary>
        public int ComposedSP { get; set; }

        /// <summary>
        /// Max number of composed stats.
        /// </summary>
        public ushort ReqWis { get => _dbItem.ReqWis; }

        /// <summary>
        /// Bool indicator, that shows if item can be rec-runed.
        /// </summary>
        public bool IsComposable { get => _dbItem != null && ReqWis > 0; }

        /// <summary>
        /// Generates craft name, that is stored in db.
        /// </summary>
        public string GetCraftName()
        {
            if (!IsComposable)
                return DEFAULT_CRAFT_NAME;

            var strBuilder = new StringBuilder();

            if (ComposedStr > 9)
                strBuilder.Append($"{ComposedStr}");
            else
                strBuilder.Append($"0{ComposedStr}");

            if (ComposedDex > 9)
                strBuilder.Append($"{ComposedDex}");
            else
                strBuilder.Append($"0{ComposedDex}");

            if (ComposedRec > 9)
                strBuilder.Append($"{ComposedRec}");
            else
                strBuilder.Append($"0{ComposedRec}");

            if (ComposedInt > 9)
                strBuilder.Append($"{ComposedInt}");
            else
                strBuilder.Append($"0{ComposedInt}");

            if (ComposedWis > 9)
                strBuilder.Append($"{ComposedWis}");
            else
                strBuilder.Append($"0{ComposedWis}");

            if (ComposedLuc > 9)
                strBuilder.Append($"{ComposedLuc}");
            else
                strBuilder.Append($"0{ComposedLuc}");

            var hp = ComposedHP / 100;
            if (hp > 9)
                strBuilder.Append($"{hp}");
            else
                strBuilder.Append($"0{hp}");

            var mp = ComposedMP / 100;
            if (mp > 9)
                strBuilder.Append($"{mp}");
            else
                strBuilder.Append($"0{mp}");

            var sp = ComposedSP / 100;
            if (sp > 9)
                strBuilder.Append($"{sp}");
            else
                strBuilder.Append($"0{sp}");

            strBuilder.Append("00"); // TODO: support enchanted level

            return strBuilder.ToString();
        }

        /// <summary>
        /// Parses db craft name into numbers.
        /// </summary>
        private void ParseCraftname(string craftname)
        {
            if (craftname.Length != 20)
                return;

            for (var i = 0; i <= 18; i += 2)
            {
                var strBuilder = new StringBuilder();
                strBuilder.Append(craftname[i]);
                strBuilder.Append(craftname[i + 1]);
                if (int.TryParse(strBuilder.ToString(), out var number))
                {
                    switch (i)
                    {
                        case 0:
                            ComposedStr = number;
                            break;

                        case 2:
                            ComposedDex = number;
                            break;

                        case 4:
                            ComposedRec = number;
                            break;

                        case 6:
                            ComposedInt = number;
                            break;

                        case 8:
                            ComposedWis = number;
                            break;

                        case 10:
                            ComposedLuc = number;
                            break;

                        case 12:
                            ComposedHP = number * 100;
                            break;

                        case 14:
                            ComposedMP = number * 100;
                            break;

                        case 16:
                            ComposedSP = number * 100;
                            break;

                        case 18:
                            // TODO: support enchanted level
                            break;
                    }
                }
            }
        }

        #endregion

        #region Max count

        public byte MaxCount => _dbItem.Count;

        /// <summary>
        /// Consumables and lapis are joinable objects. I.e. count can be > 1.
        /// </summary>
        public bool IsJoinable
        {
            get => MaxCount > 1;
        }

        #endregion

        #region Dye color

        /// <summary>
        /// Unique color.
        /// </summary>
        public DyeColor DyeColor { get; set; }

        #endregion

        #region Helpers

        /// <summary>
        /// Number of still free slots.
        /// </summary>
        public byte FreeSlots
        {
            get
            {
                byte count = 0;
                switch (_dbItem.Slot)
                {
                    case 0:
                        break;

                    case 1:
                        if (Gem1 is null)
                            count++;
                        break;

                    case 2:
                        if (Gem1 is null)
                            count++;
                        if (Gem2 is null)
                            count++;
                        break;

                    case 3:
                        if (Gem1 is null)
                            count++;
                        if (Gem2 is null)
                            count++;
                        if (Gem3 is null)
                            count++;
                        break;

                    case 4:
                        if (Gem1 is null)
                            count++;
                        if (Gem2 is null)
                            count++;
                        if (Gem3 is null)
                            count++;
                        if (Gem4 is null)
                            count++;
                        break;

                    case 5:
                        if (Gem1 is null)
                            count++;
                        if (Gem2 is null)
                            count++;
                        if (Gem3 is null)
                            count++;
                        if (Gem4 is null)
                            count++;
                        if (Gem5 is null)
                            count++;
                        break;

                    case 6:
                        if (Gem1 is null)
                            count++;
                        if (Gem2 is null)
                            count++;
                        if (Gem3 is null)
                            count++;
                        if (Gem4 is null)
                            count++;
                        if (Gem5 is null)
                            count++;
                        if (Gem6 is null)
                            count++;
                        break;


                    default:
                        return 0;
                }

                return count;
            }
        }

        /// <summary>
        /// Checks if item already has such gem linked.
        /// </summary>
        /// <param name="typeId">gem type id</param>
        /// <returns>true, if item already has such gem</returns>
        public bool ContainsGem(byte typeId)
        {
            return (Gem1 != null && Gem1.TypeId == typeId) ||
                   (Gem2 != null && Gem2.TypeId == typeId) ||
                   (Gem3 != null && Gem3.TypeId == typeId) ||
                   (Gem4 != null && Gem4.TypeId == typeId) ||
                   (Gem5 != null && Gem5.TypeId == typeId) ||
                   (Gem6 != null && Gem6.TypeId == typeId);
        }

        public bool IsCloakSlot
        {
            get => Slot == 7;
        }

        private static readonly List<byte> AllWeaponIds = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 59, 60, 61, 62, 63, 64, 65 };

        public bool IsWeapon
        {
            get => AllWeaponIds.Contains(Type);
        }

        private static readonly List<byte> AddArmorIds = new List<byte>() { 16, 17, 18, 20, 21, 31, 32, 33, 35, 36, 67, 68, 70, 71, 72, 73, 74, 76, 77, 82, 83, 85, 86, 87, 88, 89, 91, 92 };

        public bool IsArmor
        {
            get => AddArmorIds.Contains(Type);
        }

        public bool IsMount
        {
            get => Type == 42;
        }

        public bool IsPet
        {
            get => Type == 120;
        }

        public bool IsCostume
        {
            get => Type == 150;
        }

        /// <summary>
        /// Transforms weapon type to passive skill type.
        /// </summary>
        public byte ToPassiveSkillType()
        {
            switch (Type)
            {
                case 1:
                case 45:
                    return 1; // 1 Handed Sword

                case 2:
                case 46:
                    return 2; // 2 Handed Sword.

                case 3:
                case 47:
                    return 3; // 1 Handed Axe.

                case 4:
                case 48:
                    return 4; // 2 Handed Axe.

                case 5:
                case 49:
                case 50:
                    return 5; // Double sword.

                case 6:
                case 51:
                case 52:
                    return 6; // Spear.

                case 7:
                case 53:
                case 54:
                    return 7; // 1 Handed blunt.

                case 8:
                case 55:
                case 56:
                    return 8; // 2 Handed blunt.

                case 9:
                case 57:
                    return 9; // Reverse sword.

                case 11:
                case 59:
                    return 11; // Javelin.

                case 12:
                case 60:
                case 61:
                    return 12; // Staff.

                case 13:
                case 62:
                case 63:
                    return 13; // Bow.

                case 14:
                case 64:
                    return 14; // Crossbow.

                case 15:
                case 65:
                    return 15; // Knuckle.

                default:
                    return 0;
            }
        }

        #endregion

        public Item Clone()
        {
            return new Item(_databasePreloader, Type, TypeId)
            {
                Bag = Bag,
                Slot = Slot,
                Type = Type,
                TypeId = TypeId,
                Quality = Quality,
                Count = Count,
                DyeColor = DyeColor
            };
        }
    }
}
