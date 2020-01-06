using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
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

        [PacketHandler(PacketType.CREATE_CHARACTER)]
        public static async void OnCreateCharacter(WorldClient client, IPacketStream packet)
        {
            var createCharacterPacket = new CreateCharacterPacket(packet);
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            // Get number of user characters.
            var count = (byte)database.Charaters.Count(x => x.UserId == client.UserID);

            if (count == Constants.MaxCharacters)
            {
                // Max number is reached.
                WorldPacketFactory.SendCreatedCharacter(client, false);
            }

            DbCharacter character = new DbCharacter()
            {
                Name = createCharacterPacket.CharacterName,
                Race = createCharacterPacket.Race,
                Mode = createCharacterPacket.Mode,
                Hair = createCharacterPacket.Hair,
                Face = createCharacterPacket.Face,
                Height = createCharacterPacket.Height,
                Class = createCharacterPacket.Class,
                Gender = createCharacterPacket.Gender,
                Level = 1,
                Slot = count,
                UserId = client.UserID
            };

            await database.Charaters.CreateAsync(character);
            await database.CompleteAsync();
            WorldPacketFactory.SendCreatedCharacter(client, true);
        }
    }
}
