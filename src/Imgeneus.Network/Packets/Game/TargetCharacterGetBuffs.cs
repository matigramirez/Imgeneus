using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TargetCharacterGetBuffs : IDeserializedPacket
    {
        public int TargetId { get; }

        public TargetCharacterGetBuffs(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }
}
