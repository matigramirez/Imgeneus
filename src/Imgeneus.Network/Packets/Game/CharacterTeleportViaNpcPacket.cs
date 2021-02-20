using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CharacterTeleportViaNpcPacket : IDeserializedPacket
    {
        public int NpcId;

        public byte GateId;

        public CharacterTeleportViaNpcPacket(IPacketStream packet)
        {
            NpcId = packet.Read<int>();
            GateId = packet.Read<byte>();
        }
    }
}
