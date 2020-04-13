using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CreateCharacterPacket : IDeserializedPacket
    {
        public Race Race { get; }

        public Mode Mode { get; }

        public byte Hair { get; }

        public byte Face { get; }

        public byte Height { get; }

        public CharacterProfession Class { get; }

        public Gender Gender { get; }

        public string CharacterName { get; }

        public CreateCharacterPacket(IPacketStream packet)
        {
            packet.Skip(1); // Length of packet.
            Race = (Race)packet.Read<byte>();
            Mode = (Mode)packet.Read<byte>();
            Hair = packet.Read<byte>();
            Face = packet.Read<byte>();
            Height = packet.Read<byte>();
            Class = (CharacterProfession)packet.Read<byte>();
            Gender = (Gender)packet.Read<byte>();
            CharacterName = packet.ReadString((int)packet.Length - 1);
        }
    }
}
