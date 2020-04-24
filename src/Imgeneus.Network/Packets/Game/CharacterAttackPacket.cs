using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CharacterAttackPacket : IDeserializedPacket
    {
        public int TargetId { get; }

        public CharacterAttackPacket(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }
}
