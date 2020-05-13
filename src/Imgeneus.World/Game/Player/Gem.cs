
using Imgeneus.Database.Preload;

namespace Imgeneus.World.Game.Player
{
    public class Gem
    {
        private readonly IDatabasePreloader _databasePreloader;

        public int TypeId { get; private set; }

        public Gem(IDatabasePreloader databasePreloader, int typeId)
        {
            _databasePreloader = databasePreloader;
            TypeId = typeId;
        }

        public ushort Str
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstStr;
            }
        }

        public ushort Dex
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstDex;
            }
        }

        public ushort Rec
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstRec;
            }
        }

        public ushort Int
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstInt;
            }
        }

        public ushort Luc
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstLuc;
            }
        }

        public ushort Wis
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstWis;
            }
        }

        public ushort HP
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstHP;
            }
        }

        public ushort MP
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstMP;
            }
        }

        public ushort SP
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].ConstSP;
            }
        }

        public byte AttackSpeed
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].AttackTime;
            }
        }

        public byte MoveSpeed
        {
            get
            {
                // 30 type is always lapis.
                return _databasePreloader.Items[(30, (byte)TypeId)].Speed;
            }
        }
    }
}
