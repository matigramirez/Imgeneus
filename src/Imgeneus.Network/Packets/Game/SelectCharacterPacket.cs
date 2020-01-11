using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct SelectCharacterPacket
    {
        /// <summary>
        /// Id of character, that should be loaded.
        /// </summary>
        public int CharacterId { get; }

        public SelectCharacterPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
