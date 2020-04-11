using Imgeneus.Core.DependencyInjection;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Game;
using Imgeneus.World.Packets;

namespace Imgeneus.World.Handlers
{
    internal static partial class WorldHandler
    {
        [PacketHandler(PacketType.GM_COMMAND_GET_ITEM)]
        public static async void OnGMGetItem(WorldClient client, IPacketStream packet)
        {
            var getItemPacket = new GMGetItemPacket(packet);

            var gameWorld = DependencyContainer.Instance.Resolve<IGameWorld>();

            var player = gameWorld.Players[client.CharID];
            await player.AddItemToInventory(getItemPacket.Type, getItemPacket.TypeId, getItemPacket.Count);

            WorldPacketFactory.SendCharacterItems(client, player.InventoryItems);
        }

        [PacketHandler(PacketType.GM_CREATE_MOB)]
        public static void OnGMCreateMob(WorldClient client, IPacketStream packet)
        {
            var getItemPacket = new GMCreateMobPacket(packet);

            var gameWorld = DependencyContainer.Instance.Resolve<IGameWorld>();

            for (var i = 0; i < getItemPacket.NumberOfMobs; i++)
                gameWorld.GMCreateMob(client.CharID, getItemPacket.MobId);
        }
    }
}

