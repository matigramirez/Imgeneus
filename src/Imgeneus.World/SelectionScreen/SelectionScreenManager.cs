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

                case DeleteCharacterPacket characterDeletePacket:
                    HandleDeleteCharacter(characterDeletePacket);
                    break;

                case RestoreCharacterPacket restoreCharacterPacket:
                    HandleRestoreCharacter(restoreCharacterPacket);
                    break;
            }
        }

        /// <summary>
        /// Call this right after gameshake to get user characters.
        /// </summary>
        public async void SendSelectionScrenInformation(int userId)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbUser user = await database.Users.Include(u => u.Characters)
                                        .ThenInclude(c => c.Items)
                                        .Where(u => u.Id == userId)
                                        .FirstOrDefaultAsync();

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

            byte freeSlot = createCharacterPacket.Slot;
            if (characters.Any(c => c.Slot == freeSlot && !c.IsDelete))
            {
                // Wrong slot.
                SendCreatedCharacter(false);
                return;
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
            for (byte i = 0; i < 5; i++)
            {
                using var packet = new Packet(PacketType.CHARACTER_LIST);
                packet.Write(i);
                var character = characters.FirstOrDefault(c => c.Slot == i && (!c.IsDelete || c.IsDelete && c.DeleteTime != null && DateTime.UtcNow.Subtract((DateTime)c.DeleteTime) < TimeSpan.FromHours(2)));
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
        private async void HandleSelectCharacter(SelectCharacterPacket selectCharacterPacket)
        {
            var gameWorld = DependencyContainer.Instance.Resolve<IGameWorld>();
            var character = await gameWorld.LoadPlayer(selectCharacterPacket.CharacterId, _client);

            if (character != null)
            {
                _client.CharID = character.Id;

                using var packet = new Packet(PacketType.SELECT_CHARACTER);
                packet.WriteByte(0); // ok response
                packet.Write(character.Id);
                _client.SendPacket(packet);
            }
        }


        /// <summary>
        /// Marks character as deleted.
        /// </summary>
        private async void HandleDeleteCharacter(DeleteCharacterPacket characterDeletePacket)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var character = await database.Characters.FirstOrDefaultAsync(c => c.UserId == _client.UserID && c.Id == characterDeletePacket.CharacterId);
            if (character is null)
                return;

            character.IsDelete = true;
            character.DeleteTime = DateTime.UtcNow;

            await database.SaveChangesAsync();

            using var packet = new Packet(PacketType.DELETE_CHARACTER);
            packet.WriteByte(0); // ok response
            packet.Write(character.Id);
            _client.SendPacket(packet);
        }

        /// <summary>
        /// Restores dead character.
        /// </summary>
        private async void HandleRestoreCharacter(RestoreCharacterPacket restoreCharacterPacket)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var character = await database.Characters.FirstOrDefaultAsync(c => c.UserId == _client.UserID && c.Id == restoreCharacterPacket.CharacterId);
            if (character is null)
                return;

            character.IsDelete = false;
            character.DeleteTime = null;

            await database.SaveChangesAsync();

            using var packet = new Packet(PacketType.RESTORE_CHARACTER);
            packet.WriteByte(0); // ok response
            packet.Write(character.Id);
            _client.SendPacket(packet);
        }
    }
}
