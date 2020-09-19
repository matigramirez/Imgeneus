using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMFindPlayerPacket : IDeserializedPacket
    {
        public string Name;

        public GMFindPlayerPacket(IPacketStream packet)
        {
            Name = packet.ReadString(21);
        }
    }
}
