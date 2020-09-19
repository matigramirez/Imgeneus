using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMSummonPlayerPacket : IDeserializedPacket
    {
        public string Name;

        public GMSummonPlayerPacket(IPacketStream packet)
        {
            Name = packet.ReadString(21);
        }
    }
}
