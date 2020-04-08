using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void LearnedNewSkill(WorldClient client, Character character, bool success)
        {
            using var packet = new Packet(PacketType.LEARN_NEW_SKILL);
            if (success)
            {
                packet.Write(0);
                SendLearnedSkills(client, character);
            }
            else
            {
                packet.Write(1);
            }

            client.SendPacket(packet);
        }

        private static void SendLearnedSkills(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SKILLS);
            var bytes = new CharacterSkills(character).Serialize();
            packet.Write(bytes);
            client.SendPacket(packet);
        }

        private static void SendActiveBuffs(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_ACTIVE_BUFFS);
            packet.Write(new CharacterActiveBuffs(character.ActiveBuffs).Serialize());
            client.SendPacket(packet);
        }
    }
}
