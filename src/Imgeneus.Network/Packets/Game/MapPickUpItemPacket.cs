using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MapPickUpItemPacket : IDeserializedPacket
    {
        public int ItemId;

        public MapPickUpItemPacket(IPacketStream packet)
        {
            ItemId = packet.Read<int>();
        }
    }
}
