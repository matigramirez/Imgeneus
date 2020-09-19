using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMTeleportToPlayerPacket : IDeserializedPacket
    {
        public string Name;

        public GMTeleportToPlayerPacket(IPacketStream packet)
        {
            Name = packet.ReadString(21);
        }
    }
}
