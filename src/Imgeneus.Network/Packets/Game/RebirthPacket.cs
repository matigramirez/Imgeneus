using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RebirthPacket : IDeserializedPacket
    {
        // 2 - KillSoul
        // 3 - To party leader?
        // 4 - KillSoulByItem
        // 5 - KillSoulByItemNoMove
        public byte RebirthType;

        public RebirthPacket(IPacketStream packet)
        {
            RebirthType = packet.Read<byte>();
        }
    }
}
