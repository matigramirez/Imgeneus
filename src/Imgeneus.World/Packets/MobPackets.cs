using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Serialization;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void MobEntered(WorldClient client, Mob mob)
        {
            using var packet = new Packet(PacketType.MOB_ENTER);
            packet.Write(new MobEnter(mob).Serialize());
            client.SendPacket(packet);
        }

        public static void MobInTarget(WorldClient client, Mob mob)
        {
            using var packet = new Packet(PacketType.TARGET_SELECT_MOB);
            packet.Write(new MobInTarget(mob).Serialize());
            client.SendPacket(packet);
        }
    }
}
