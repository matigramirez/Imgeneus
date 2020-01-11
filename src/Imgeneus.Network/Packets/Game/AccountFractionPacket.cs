using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct AccountFractionPacket
    {
        public Fraction Fraction { get; }

        public AccountFractionPacket(IPacketStream packet)
        {
            Fraction = (Fraction)packet.Read<byte>();
        }
    }
}
