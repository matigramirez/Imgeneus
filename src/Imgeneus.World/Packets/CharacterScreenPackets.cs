using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendAccountFaction(WorldClient client, byte faction, byte maxMode)
        {
            using var packet = new Packet(PacketType.ACCOUNT_FACTION);
            packet.Write<byte>(faction);
            packet.Write<byte>(maxMode);

            client.SendPacket(packet);
        }

        public static void SendCharacterAvailability(WorldClient client, bool isAvailable)
        {
            using var packet = new Packet(PacketType.CHECK_CHARACTER_AVAILABLE_NAME);
            packet.Write(isAvailable);

            client.SendPacket(packet);
        }
    }
}
