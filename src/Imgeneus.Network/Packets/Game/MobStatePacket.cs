using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MobStatePacket
    {
        public uint MobId { get; }

        public MobStatePacket(IPacketStream packet)
        {
            MobId = packet.Read<uint>();
        }
    }
}
