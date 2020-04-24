using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct LogOutPacket : IDeserializedPacket
    {
        public LogOutPacket(IPacketStream packet)
        {
            // Logout packet is called, when user  leaves game or goes back to selection screen.
        }
    }
}
