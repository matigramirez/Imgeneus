using Imgeneus.Database.Constants;
using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MotionPacket
    {
        public Motion Motion { get; }

        public MotionPacket(IPacketStream packet)
        {
            Motion = (Motion)packet.Read<byte>();
        }
    }
}
