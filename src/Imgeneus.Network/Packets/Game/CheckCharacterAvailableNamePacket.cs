using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CheckCharacterAvailableNamePacket : IDeserializedPacket
    {
        /// <summary>
        /// Character name which the client sends.
        /// </summary>
        public string CharacterName { get; }

        public CheckCharacterAvailableNamePacket(IPacketStream packet)
        {
            CharacterName = packet.ReadString((int)packet.Length - 1);
        }
    }
}
