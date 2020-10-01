
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;

namespace Imgeneus.World.Game.Player
{
    public class Gem
    {
        /// <summary>
        /// 30 type is always lapis.
        /// </summary>
        public const byte GEM_TYPE = 30;

        private readonly IDatabasePreloader _databasePreloader;
        private readonly DbItem _item;

        public int TypeId { get; private set; }

        public Gem(IDatabasePreloader databasePreloader, int typeId, byte position)
        {
            _databasePreloader = databasePreloader;
            TypeId = typeId;
            Position = position;

            _item = _databasePreloader.Items[(GEM_TYPE, (byte)TypeId)];
        }

        /// <summary>
        /// ONLY USED FOR MONEY GENERATION! DON'T USE IT UNLESS YOU GENERATE MONEY DROP.
        /// </summary>
        public Gem(int moneyAmount)
        {
            TypeId = moneyAmount;
        }

        /// <summary>
        /// ONLY USED FOR MONEY GENERATION! DON'T USE IT UNLESS YOU GENERATE MONEY DROP.
        /// </summary>
        public void SetTypeId(int typeId)
        {
            TypeId = typeId;
        }

        public byte Position { get; private set; }

        public ushort Str => _item.ConstStr;

        public ushort Dex => _item.ConstDex;

        public ushort Rec => _item.ConstRec;

        public ushort Int => _item.ConstInt;

        public ushort Luc => _item.ConstLuc;

        public ushort Wis => _item.ConstWis;

        public ushort HP => _item.ConstHP;

        public ushort MP => _item.ConstMP;

        public ushort SP => _item.ConstSP;

        public byte AttackSpeed => _item.AttackTime;

        public byte MoveSpeed => _item.Speed;

        public ushort Defense => _item.Defense;

        public ushort Resistance => _item.Resistance;

        public ushort MinAttack => _item.MinAttack;

        public ushort PlusAttack => _item.PlusAttack;

        public Element Element => _item.Element;

        public byte ReqIg => _item.ReqIg;

        public ushort ReqVg => _item.ReqVg;
    }
}
