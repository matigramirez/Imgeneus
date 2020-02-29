using System;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendSelectedCharacter(WorldClient client, DbCharacter character)
        {
            using var packet = new Packet(PacketType.SELECT_CHARACTER);
            packet.WriteByte(0); // ok response
            packet.Write(character.Id);
            client.SendPacket(packet);

            SendCharacterDetails(client, character);
            SendLearnedSkills(client, character);
        }

        private static void SendCharacterDetails(WorldClient client, DbCharacter character)
        {
            using var packet = new Packet(PacketType.CHARACTER_DETAILS);
            var bytes = new CharacterDetails(character).Serialize();
            packet.Write(bytes);
            client.SendPacket(packet);
        }

        private static void SendLearnedSkills(WorldClient client, DbCharacter character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SKILLS);
            packet.Write((ushort)5); // Skill points
            packet.WriteByte(2); // Length of skills array.
            packet.Write((ushort)625); // Skill Id
            packet.WriteByte(4); // Skill Level
            packet.WriteByte(0); // ?
            packet.Write(130); // Cooldown, in seconds

            packet.Write((ushort)636); // Skill Id
            packet.WriteByte(1); // Skill Level
            packet.WriteByte(0); // ?
            packet.Write(0);
            client.SendPacket(packet);
        }

    }
}
