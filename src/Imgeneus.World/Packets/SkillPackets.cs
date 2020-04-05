using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Player;

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
    }
}
