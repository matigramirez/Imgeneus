using Imgeneus.Core.DependencyInjection;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Game;
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

            var gameWorld = DependencyContainer.Instance.Resolve<IGameWorld>();

            var player = gameWorld.Players.FirstOrDefault(p => p.Id == client.CharID);
            await player.AddItemToInventory(getItemPacket.Type, getItemPacket.TypeId, getItemPacket.Count);

            WorldPacketFactory.SendCharacterItems(client, player.InventoryItems);
        }
    }
}
