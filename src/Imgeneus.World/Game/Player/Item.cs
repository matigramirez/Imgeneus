using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;

namespace Imgeneus.World.Game.Player
{
    public class Item
    {
        private readonly IDatabasePreloader _databasePreloader;

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

        public Item(IDatabasePreloader databasePreloader, DbCharacterItems dbItem) : this(databasePreloader, dbItem.Type, dbItem.TypeId)
        {
            Bag = dbItem.Bag;
            Slot = dbItem.Slot;
            Quality = dbItem.Quality;

            if (dbItem.GemTypeId1 != 0)
                Gem1 = new Gem(databasePreloader, dbItem.GemTypeId1);
            if (dbItem.GemTypeId2 != 0)
                Gem2 = new Gem(databasePreloader, dbItem.GemTypeId2);
            if (dbItem.GemTypeId3 != 0)
                Gem3 = new Gem(databasePreloader, dbItem.GemTypeId3);
            if (dbItem.GemTypeId4 != 0)
                Gem4 = new Gem(databasePreloader, dbItem.GemTypeId4);
            if (dbItem.GemTypeId5 != 0)
                Gem5 = new Gem(databasePreloader, dbItem.GemTypeId5);
            if (dbItem.GemTypeId6 != 0)
                Gem6 = new Gem(databasePreloader, dbItem.GemTypeId6);
            Count = dbItem.Count;
        }

        public Item(IDatabasePreloader databasePreloader, byte type, byte typeId)
        {
            _databasePreloader = databasePreloader;
            Type = type;
            TypeId = typeId;

            if (Type != 0 && TypeId != 0)
            {
                var item = _databasePreloader.Items[(Type, TypeId)];
                ConstStr = item.ConstStr;
                ConstDex = item.ConstDex;
                ConstRec = item.ConstRec;
                ConstInt = item.ConstInt;
                ConstLuc = item.ConstLuc;
                ConstWis = item.ConstWis;
                ConstHP = item.ConstHP;
                ConstMP = item.ConstMP;
                ConstSP = item.ConstSP;
                ConstAttackSpeed = item.AttackTime;
                ConstMoveSpeed = item.Speed;
                MaxCount = item.Count;
            }
        }

        #region Trade

        public byte TradeQuantity;

        #endregion

        #region Extra stats

        private readonly int ConstStr;
        private readonly int ConstDex;
        private readonly int ConstRec;
        private readonly int ConstInt;
        private readonly int ConstLuc;
        private readonly int ConstWis;
        private readonly int ConstHP;
        private readonly int ConstMP;
        private readonly int ConstSP;
        private readonly byte ConstAttackSpeed;
        private readonly byte ConstMoveSpeed;

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

                return ConstStr + gemsStr; // + TODO: orange stats from craft name.
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

                return ConstDex + gemsDex; // + TODO: orange stats from craft name.
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

                return ConstRec + gemsRec; // + TODO: orange stats from craft name.
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

                return ConstInt + gemsInt; // + TODO: orange stats from craft name.
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

                return ConstLuc + gemsLuc; // + TODO: orange stats from craft name.
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

                return ConstWis + gemsWis; // + TODO: orange stats from craft name.
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

                return ConstHP + gemsHP; // + TODO: orange stats from craft name.
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

                return ConstMP + gemsMP; // + TODO: orange stats from craft name.
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

                return ConstSP + gemsSP; // + TODO: orange stats from craft name.
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

        #endregion

        #region Max count

        public byte MaxCount { get; }

        /// <summary>
        /// Consumables and lapis are joinable objects. I.e. count can be > 1.
        /// </summary>
        public bool IsJoinable
        {
            get => MaxCount > 1;
        }

        #endregion

        public bool IsCloakSlot
        {
            get => Slot == 7;
        }

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
            };
        }
    }
}
