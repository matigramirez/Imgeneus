using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CharacterAutoAttackPacket : IDeserializedPacket
    {
        public int TargetId { get; }

        public CharacterAutoAttackPacket(IPacketStream packet)
        {
            TargetId = packet.Read<int>();
        }
    }
}
