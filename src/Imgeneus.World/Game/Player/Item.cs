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
        public int GemTypeId1;
        public int GemTypeId2;
        public int GemTypeId3;
        public int GemTypeId4;
        public int GemTypeId5;
        public int GemTypeId6;
        public byte Count;

        public Item(IDatabasePreloader databasePreloader, DbCharacterItems dbItem) : this(databasePreloader)
        {
            Bag = dbItem.Bag;
            Slot = dbItem.Slot;
            Type = dbItem.Type;
            TypeId = dbItem.TypeId;
            Quality = dbItem.Quality;
            GemTypeId1 = dbItem.GemTypeId1;
            GemTypeId2 = dbItem.GemTypeId2;
            GemTypeId3 = dbItem.GemTypeId3;
            GemTypeId4 = dbItem.GemTypeId4;
            GemTypeId5 = dbItem.GemTypeId5;
            GemTypeId6 = dbItem.GemTypeId6;
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
        public ushort Str
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstStr; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// Dex contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public ushort Dex
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstDex; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// Rec contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public ushort Rec
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstRec; // + TODO: orange stats from craft name.
            }
        }


        /// <summary>
        /// Int contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public ushort Int
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstInt; // + TODO: orange stats from craft name.
            }
        }


        /// <summary>
        /// Luc contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public ushort Luc
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstLuc; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// Wis contains yellow(default) stat + orange stat (take it from craft name later).
        /// </summary>
        public ushort Wis
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstWis; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// HP stats.
        /// </summary>
        public ushort HP
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstHP; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// MP stats.
        /// </summary>
        public ushort MP
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstMP; // + TODO: orange stats from craft name.
            }
        }

        /// <summary>
        /// SP stats.
        /// </summary>
        public ushort SP
        {
            get
            {
                return _databasePreloader.Items[(Type, TypeId)].ConstSP; // + TODO: orange stats from craft name.
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

        public DbCharacterItems ToDbItem(int characterId)
        {
            return new DbCharacterItems()
            {
                Bag = Bag,
                Slot = Slot,
                Type = Type,
                TypeId = TypeId,
                Quality = Quality,
                GemTypeId1 = GemTypeId1,
                GemTypeId2 = GemTypeId2,
                GemTypeId3 = GemTypeId3,
                GemTypeId4 = GemTypeId4,
                GemTypeId5 = GemTypeId5,
                GemTypeId6 = GemTypeId6,
                Count = Count,
                CharacterId = characterId
            };
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
                GemTypeId1 = GemTypeId1,
                GemTypeId2 = GemTypeId2,
                GemTypeId3 = GemTypeId3,
                GemTypeId4 = GemTypeId4,
                GemTypeId5 = GemTypeId5,
                GemTypeId6 = GemTypeId6,
                Count = Count,
            };
        }
    }
}
