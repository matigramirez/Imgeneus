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
            client.CharID = character.Id;

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
    }
}
