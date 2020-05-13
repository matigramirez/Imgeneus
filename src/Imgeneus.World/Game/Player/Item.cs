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

        public Item(IDatabasePreloader databasePreloader, DbCharacterItems dbItem) : this(databasePreloader)
        {
            Bag = dbItem.Bag;
            Slot = dbItem.Slot;
            Type = dbItem.Type;
            TypeId = dbItem.TypeId;
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

        public Item(IDatabasePreloader databasePreloader)
        {
            _databasePreloader = databasePreloader;
        }

        #region Trade

        public byte TradeQuantity;

        #endregion

        #region Extra stats

        /// <summary>
        /// Str contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Str
        {
            get
            {
                var constStr = _databasePreloader.Items[(Type, TypeId)].ConstStr;
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

                return constStr + gemsStr; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// Dex contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Dex
        {
            get
            {
                var constDex = _databasePreloader.Items[(Type, TypeId)].ConstDex;
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

                return constDex + gemsDex; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// Rec contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Rec
        {
            get
            {
                var constRec = _databasePreloader.Items[(Type, TypeId)].ConstRec;
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

                return constRec + gemsRec; // + TODO: orange stats from craft name.
            }
        }


        /// <summary>
        /// Int contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Int
        {
            get
            {
                var constInt = _databasePreloader.Items[(Type, TypeId)].ConstInt;
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

                return constInt + gemsInt; // + TODO: orange stats from craft name.
            }
        }


        /// <summary>
        /// Luc contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Luc
        {
            get
            {
                var constLuc = _databasePreloader.Items[(Type, TypeId)].ConstLuc;
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

                return constLuc + gemsLuc; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// Wis contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public int Wis
        {
            get
            {
                var constWis = _databasePreloader.Items[(Type, TypeId)].ConstWis;
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

                return constWis + gemsWis; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// HP stats.
        /// </summary>
        public int HP
        {
            get
            {
                var constHP = _databasePreloader.Items[(Type, TypeId)].ConstHP;
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

                return constHP + gemsHP; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// MP stats.
        /// </summary>
        public int MP
        {
            get
            {
                var constMP = _databasePreloader.Items[(Type, TypeId)].ConstMP;
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

                return constMP + gemsMP; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// SP stats.
        /// </summary>
        public int SP
        {
            get
            {
                var constSP = _databasePreloader.Items[(Type, TypeId)].ConstSP;
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

                return constSP + gemsSP; // + TODO: orange stats from craft name.
            }
        }

        public byte AttackSpeed
        {
            get
            {
                var constAttackSpeed = _databasePreloader.Items[(Type, TypeId)].AttackTime;
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

                return (byte)(constAttackSpeed + gemsSpeed);
            }
        }

        public byte MoveSpeed
        {
            get
            {
                var constMoveSpeed = _databasePreloader.Items[(Type, TypeId)].Speed;
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

                return (byte)(constMoveSpeed + gemsSpeed);
            }
        }

        #endregion

        /// <summary>
        /// Consumables and lapis are joinable objects. I.e. count can be > 1.
        /// </summary>
        public bool IsJoinable
        {
            get
            {
                return
                   // All consumables in game, checked via shaiya studio v0.8
                   Type == 25 ||
                   Type == 27 ||
                   Type == 28 ||
                   Type == 29 ||
                   Type == 38 ||
                   Type == 41 ||
                   Type == 43 ||
                   Type == 44 ||
                   Type == 78 ||
                   Type == 79 ||
                   Type == 80 ||
                   Type == 94 ||
                   Type == 99 ||
                   Type == 100 ||
                   Type == 101 ||
                   Type == 102 ||
                   // All lapis in game, checked via shaiya studio v0.8
                   Type == 30 ||
                   Type == 95 ||
                   Type == 98;
            }
        }

        public bool IsCloakSlot
        {
            get => Slot == 7;
        }

        public Item Clone()
        {
            return new Item(_databasePreloader)
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
