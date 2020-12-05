using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMSetAttributePacket : IDeserializedPacket
    {
        public CharacterAttributeEnum Attribute;
        public uint Value;
        public string Name;

        public GMSetAttributePacket(IPacketStream packet)
        {
            Attribute = (CharacterAttributeEnum)packet.Read<byte>();
            Value = packet.Read<uint>();
            Name = packet.ReadString(21);
        }

        public void Deconstruct(out CharacterAttributeEnum attribute, out uint value, out string charname)
        {
            attribute = Attribute;
            value = Value;
            charname = Name;
        }
    }
}