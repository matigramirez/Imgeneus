using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Player
{
    public class DefaultStat
    {
        public CharacterProfession Job { get; set; }

        public ushort Str { get; set; }

        public ushort Dex { get; set; }

        public ushort Rec { get; set; }

        public ushort Int { get; set; }

        public ushort Wis { get; set; }

        public ushort Luc { get; set; }

        public byte MainStat { get; set; }
    }
}