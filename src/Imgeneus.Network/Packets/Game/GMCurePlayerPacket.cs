using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMCurePlayerPacket : IDeserializedPacket
    {
        public string Name;

        public GMCurePlayerPacket(IPacketStream packet)
        {
            Name = packet.ReadString(21);
        }
    }
}