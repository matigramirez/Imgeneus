using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Serialization;
using Imgeneus.Network.Server;
using Imgeneus.World.Game;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.SelectionScreen
{
    /// <summary>
    /// Manager, that handles selection screen packets.
    /// </summary>
    public class SelectionScreenManager : IDisposable
    {
        private readonly WorldClient _client;

        public SelectionScreenManager(WorldClient client)
        {
            _client = client;
            _client.OnPacketArrived += Client_OnPacketArrived;
        }

        public void Dispose()
        {
            _client.OnPacketArrived -= Client_OnPacketArrived;
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case AccountFractionPacket accountFractionPacket:
                    HandleChangeFraction(accountFractionPacket);
                    break;

                case CheckCharacterAvailableNamePacket checkNamePacket:
                    HandleCheckName(checkNamePacket);
                    break;

                case CreateCharacterPacket createCharacterPacket:
                    HandleCreateCharacter(createCharacterPacket);
                    break;

                case SelectCharacterPacket selectCharacterPacket:
                    HandleSelectCharacter(selectCharacterPacket);
                    break;
            }
        }

        /// <summary>
        /// Call this right after gameshake to get user characters.
        /// </summary>
        public void SendSelectionScrenInformation(int userId)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbUser user = database.Users.Include(u => u.Characters)
                                        .ThenInclude(c => c.Items)
                                        .Where(u => u.Id == userId)
                                        .FirstOrDefault();

            SendCharacterList(user.Characters);

            using var packet = new Packet(PacketType.ACCOUNT_FACTION);
            packet.Write((byte)user.Faction);
            packet.Write(user.MaxMode);
            _client.SendPacket(packet);
        }

        /// <summary>
        /// Handles fraction change. Saves change to databse.
        /// </summary>
        private async void HandleChangeFraction(AccountFractionPacket accountFractionPacket)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbUser user = database.Users.Find(_client.UserID);
            user.Faction = accountFractionPacket.Fraction;

            await database.SaveChangesAsync();
        }

        /// <summary>
        /// Handles event, when user clicks "check name button".
        /// </summary>
        private void HandleCheckName(CheckCharacterAvailableNamePacket checkNamePacket)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbCharacter character = database.Characters.FirstOrDefault(c => c.Name == checkNamePacket.CharacterName);

            using var packet = new Packet(PacketType.CHECK_CHARACTER_AVAILABLE_NAME);
            packet.Write(character is null);

            _client.SendPacket(packet);
        }

        /// <summary>
        /// Handles creation of character.
        /// </summary>
        private async void HandleCreateCharacter(CreateCharacterPacket createCharacterPacket)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            // Get number of user characters.
            var characters = database.Characters.Where(x => x.UserId == _client.UserID).ToList();

            if (characters.Count == Constants.MaxCharacters)
            {
                // Max number is reached.
                SendCreatedCharacter(false);
                return;
            }

            byte freeSlot = 0;
            for (byte i = 0; i < Constants.MaxCharacters; i++)
            {
                if (!characters.Any(c => c.Slot == i))
                {
                    freeSlot = i;
                    break;
                }
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
                Slot = freeSlot,
                UserId = _client.UserID
            };

            await database.Characters.AddAsync(character);
            if (await database.SaveChangesAsync() > 0)
            {
                characters.Add(character);
                SendCreatedCharacter(true);
                SendCharacterList(characters);
            }
        }

        /// <summary>
        /// Sends to client list of available characters.
        /// </summary>
        private void SendCharacterList(ICollection<DbCharacter> characters)
        {
            for (byte i = 0; i < Constants.MaxCharacters; i++)
            {
                using var packet = new Packet(PacketType.CHARACTER_LIST);
                packet.Write(i);
                var character = characters.FirstOrDefault(c => c.Slot == i);
                if (character is null)
                {
                    // No char at this slot.
                    packet.Write(0);
                }
                else
                {
                    packet.Write(new CharacterSelectionScreen(character).Serialize());
                }

                _client.SendPacket(packet);
            }
        }

        /// <summary>
        /// Sends response to client if character was created or not.
        /// </summary>
        private void SendCreatedCharacter(bool isCreated)
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


            _client.SendPacket(packet);
        }

        /// <summary>
        /// Selects character and loads it into game world.
        /// </summary>
        /// <param name="packet"></param>
        private void HandleSelectCharacter(SelectCharacterPacket selectCharacterPacket)
        {
            var gameWorld = DependencyContainer.Instance.Resolve<IGameWorld>();
            var character = gameWorld.LoadPlayer(selectCharacterPacket.CharacterId, _client);

            if (character != null)
            {
                _client.CharID = character.Id;

                using var packet = new Packet(PacketType.SELECT_CHARACTER);
                packet.WriteByte(0); // ok response
                packet.Write(character.Id);
                _client.SendPacket(packet);
            }

        }
    }
}
