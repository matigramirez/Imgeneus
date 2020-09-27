
using Imgeneus.Database.Constants;
using Imgeneus.Database.Preload;

namespace Imgeneus.World.Game.Player
{
    public class Gem
    {
        public const byte GEM_TYPE = 30;

        private readonly IDatabasePreloader _databasePreloader;

        public int TypeId { get; private set; }

        public Gem(IDatabasePreloader databasePreloader, int typeId)
        {
            _databasePreloader = databasePreloader;
            TypeId = typeId;

            // 30 type is always lapis.
            var item = _databasePreloader.Items[(GEM_TYPE, (byte)TypeId)];
            Str = item.ConstStr;
            Dex = item.ConstDex;
            Rec = item.ConstRec;
            Int = item.ConstInt;
            Luc = item.ConstLuc;
            Wis = item.ConstWis;
            HP = item.ConstHP;
            MP = item.ConstMP;
            SP = item.ConstSP;
            AttackSpeed = item.AttackTime;
            MoveSpeed = item.Speed;
            Defense = item.Defense;
            Resistance = item.Resistance;
            MinAttack = item.MinAttack;
            PlusAttack = item.PlusAttack;
            Element = item.Element;
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

        public ushort Str { get; }

        public ushort Dex { get; }

        public ushort Rec { get; }

        public ushort Int { get; }

        public ushort Luc { get; }

        public ushort Wis { get; }

        public ushort HP { get; }

        public ushort MP { get; }

        public ushort SP { get; }

        public byte AttackSpeed { get; }

        public byte MoveSpeed { get; }

        public ushort Defense { get; }

        public ushort Resistance { get; }

        public ushort MinAttack { get; }

        public ushort PlusAttack { get; }

        public Element Element { get; }
    }
}
