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
                    packet.Write(character.Id);
                    packet.Write(0); // Unknown byte
                    packet.Write(character.Level);
                    packet.Write((byte)character.Race);
                    packet.Write((byte)character.Mode);
                    packet.Write(character.Hair);
                    packet.Write(character.Face);
                    packet.Write(character.Height);
                    packet.Write((byte)character.Class);
                    packet.Write((byte)character.Gender);

                    packet.Write((ushort)character.Map); // Map

                    packet.Write(character.Strength);
                    packet.Write(character.Dexterity);
                    packet.Write(character.Rec);
                    packet.Write(character.Intelligence);
                    packet.Write(character.Wisdom);
                    packet.Write(character.Luck);
                    packet.Write(character.HealthPoints);
                    packet.Write(character.ManaPoints);
                    packet.Write(character.StaminaPoints);

                    // TODO: right now I'm investigating where which byte is used.
                    // In the end, all these values should come from the database.
                    // Now, if you want to test these hardcoded values, you should create a light fighter.
                    // Unknown bytes have assigned number.

                    packet.WriteByte(16); // Helmet type
                    packet.WriteByte(17); // Armor type
                    packet.WriteByte(18); // Pants type
                    packet.WriteByte(20); // Gauntlets type
                    packet.WriteByte(21); // Boots type

                    packet.WriteByte(2); // Weapon type 
                    packet.WriteByte(69); // Shield type
                    packet.WriteByte(24); // Cape type
                    packet.WriteByte(0); // 3
                    packet.WriteByte(0); // 4
                    packet.WriteByte(0); // 5
                    packet.WriteByte(0); // 6
                    packet.WriteByte(0); // 7
                    packet.WriteByte(0); // 8
                    packet.WriteByte(120); // Pet type
                    packet.WriteByte(0); // Costume type
                    packet.WriteByte(121); // Wings type
                    packet.WriteByte(56); // Helmet type id
                    packet.WriteByte(46); // Armor type id
                    packet.WriteByte(32); // Pants type id
                    packet.WriteByte(53); // Gauntlets type id
                    packet.WriteByte(54); // Boots type id

                    packet.WriteByte(5); // Weapon type id
                    packet.WriteByte(245); // Shield type id
                    packet.WriteByte(94); // Cape type id
                    packet.WriteByte(0); // 3
                    packet.WriteByte(0); // 4
                    packet.WriteByte(0); // 5
                    packet.WriteByte(0); // 6
                    packet.WriteByte(0); // 7
                    packet.WriteByte(0); // 8
                    packet.WriteByte(254); // Pet type id
                    packet.WriteByte(0); // Costume type id
                    packet.WriteByte(10); // Wings type id
                    packet.WriteByte(0); // 12
                    packet.WriteByte(0); // 13
                    packet.WriteByte(0); // 14
                    packet.WriteByte(0); // 15
                    packet.WriteByte(0); // 16

                    for (int j = 0; j < 535; j++)
                        packet.WriteByte(0);

                    packet.Write(character.Name);
                    packet.Write(character.IsDelete);
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
