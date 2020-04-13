using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MobInTargetPacket : IDeserializedPacket
    {
        public int TargetId { get; }

        public MobInTargetPacket(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }

    public struct PlayerInTargetPacket : IDeserializedPacket
    {
        public int TargetId { get; }

        public PlayerInTargetPacket(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }
}
