using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DuelOkPacket : IDeserializedPacket
    {
        public byte Result;

        public DuelOkPacket(IPacketStream packet)
        {
            Result = packet.Read<byte>();
        }
    }
}
