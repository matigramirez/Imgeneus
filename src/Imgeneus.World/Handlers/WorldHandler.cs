using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Packets;

namespace Imgeneus.World.Handlers
{
    internal static class WorldHandler
    {
        [PacketHandler(PacketType.GAME_HANDSHAKE)]
        public static void OnGameHandshake(WorldClient client, IPacketStream packet)
        {
            var handshake = new HandshakePacket(packet);

            client.SetClientUserID(handshake.UserId);

            using var sendPacket = new Packet(PacketType.GAME_HANDSHAKE);

            sendPacket.Write<byte>(0);
            client.SendPacket(sendPacket);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            DbUser user = database.Users.Get(handshake.UserId);

            WorldPacketFactory.SendAccountFaction(client, 2, 0);
        }

        [PacketHandler(PacketType.CHECK_CHARACTER_AVAILABLE_NAME)]
        public static void OnCheckAvailableName(WorldClient client, IPacketStream packet)
        {
            var checkNamePacket = new CheckCharacterAvailableNamePacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbCharacter character = database.Charaters.Get(c => c.Name == checkNamePacket.CharacterName);

            WorldPacketFactory.SendCharacterAvailability(client, character is null);

        }
    }
}
