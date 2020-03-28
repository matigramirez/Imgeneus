using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendAccountFaction(WorldClient client, DbUser user)
        {
            using var packet = new Packet(PacketType.ACCOUNT_FACTION);
            packet.Write((byte)user.Faction);
            packet.Write(user.MaxMode);

            client.SendPacket(packet);
        }

        public static void SendCharacterList(WorldClient client, ICollection<DbCharacter> characters)
        {
            for (byte i = 0; i < Constants.MaxCharacters; i++)
            {
                using var packet = new Packet(PacketType.CHARACTER_LIST);
                packet.Write(i);
                var character = characters.FirstOrDefault(c => c.Slot == i);
                if (character is null)
                {
                    // No char at this slot.
                    packet.Write(0);
                }
                else
                {
                    var bytes = new CharacterSelectionScreen(character).Serialize();
                    packet.Write(bytes);
                }

                client.SendPacket(packet);
            }
        }

        public static void SendCharacterAvailability(WorldClient client, bool isAvailable)
        {
            using var packet = new Packet(PacketType.CHECK_CHARACTER_AVAILABLE_NAME);
            packet.Write(isAvailable);

            client.SendPacket(packet);
        }

        public static void SendCreatedCharacter(WorldClient client, bool isCreated)
        {
            using var packet = new Packet(PacketType.CREATE_CHARACTER);

            if (isCreated)
            {
                packet.Write(0); // 0 means character was created.
            }
            else
            {
                // Send nothing.
            }


            client.SendPacket(packet);
        }

        private static void SendLearnedSkills(WorldClient client, DbCharacter character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SKILLS);
            var bytes = new CharacterSkills(character).Serialize();
            packet.Write(bytes);
            client.SendPacket(packet);
        }
    }
}
