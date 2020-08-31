using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public class TargetGetMobStatePacket : IDeserializedPacket
    {
        public int MobId { get; }

        public TargetGetMobStatePacket(IPacketStream packet)
        {
            MobId = packet.Read<int>();
        }
    }
}
