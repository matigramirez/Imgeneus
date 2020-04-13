using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct PingPacket : IDeserializedPacket
    {
        public PingPacket(IPacketStream packet)
        {
            // This is empty packet. Needed for server.
        }
    }
}
