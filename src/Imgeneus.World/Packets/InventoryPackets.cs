using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Serialization;
using System.Collections.Generic;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendCharacterItems(WorldClient client, IEnumerable<DbCharacterItems> items)
        {
            using var packet = new Packet(PacketType.CHARACTER_ITEMS);
            var bytes = new InventoryItems(items).Serialize();
            packet.Write(bytes);
            client.SendPacket(packet);
        }

        public static void SendMoveItem(WorldClient client, DbCharacterItems sourceItem, DbCharacterItems destinationItem)
        {
            using var packet = new Packet(PacketType.INVENTORY_MOVE_ITEM);

            var bytes = new SerializedMovedItem(sourceItem).Serialize();
            packet.Write(bytes);

            bytes = new SerializedMovedItem(destinationItem).Serialize();
            packet.Write(bytes);

            client.SendPacket(packet);
        }

        public static void SendEquipment(WorldClient client, int charId, DbCharacterItems item)
        {
            using var packet = new Packet(PacketType.SEND_EQUIPMENT);
            packet.Write(charId);

            packet.WriteByte(item.Slot);
            packet.WriteByte(item.Type);
            packet.WriteByte(item.TypeId);
            packet.WriteByte(20); // TODO: implement enchant here.

            if (item.IsCloakSlot)
            {
                for (var i = 0; i < 6; i++) // Something about cloak.
                {
                    packet.WriteByte(0);
                }
            }

            client.SendPacket(packet);
        }
    }
}
