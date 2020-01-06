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
    }
}
