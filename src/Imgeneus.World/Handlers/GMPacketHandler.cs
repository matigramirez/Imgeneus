using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Packets;
using System.Linq;

namespace Imgeneus.World.Handlers
{
    internal static partial class WorldHandler
    {
        [PacketHandler(PacketType.GM_COMMAND_GET_ITEM)]
        public static async void OnGMGetItem(WorldClient client, IPacketStream packet)
        {
            var getItemPacket = new GMGetItemPacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            // Get local copy of char items. ToList() will create copy.
            var charItems = database.CharacterItems.Where(c => c.CharacterId == client.CharID).ToList();

            // Find free space.
            byte bagSlot = 0;
            int freeSlot = -1;

            if (charItems.Count > 0)
            {
                var maxBag = 5;
                var maxSlots = 24;

                // Go though all bags and try to find any free slot.
                // Start with 1, because 0 is worn items.
                for (byte i = 1; i <= maxBag; i++)
                {
                    var bagItems = charItems.Where(itm => itm.Bag == i).OrderBy(b => b.Slot);
                    for (var j = 0; j < maxSlots; j++)
                    {
                        if (!bagItems.Any(b => b.Slot == j))
                        {
                            freeSlot = j;
                            break;
                        }
                    }

                    if (freeSlot != -1)
                    {
                        bagSlot = i;
                        break;
                    }
                }
            }
            // No items yet.
            else
            {
                bagSlot = 1; // Start with 1, because 0 is worn items.
                freeSlot = 0;
            }

            // Calculated bag slot can not be 0, because 0 means worn item. Newerly created item can not be worn.
            if (bagSlot != 0)
            {
                var charItem = new DbCharacterItems()
                {
                    Type = getItemPacket.Type,
                    TypeId = getItemPacket.TypeId,
                    Count = getItemPacket.Count,
                    Bag = bagSlot,
                    Slot = (byte)freeSlot,
                    CharacterId = client.CharID
                };

                database.CharacterItems.Add(charItem);
                await database.SaveChangesAsync();

                // Since charItem is localcopy, if we don't want to create another call to database, we can use it.
                // Just add to local copy the last item, that was added to database.
                charItems.Add(charItem);
                WorldPacketFactory.SendCharacterItems(client, charItems);
            }
        }
    }
}
