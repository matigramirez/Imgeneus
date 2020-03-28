using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Packets;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Imgeneus.World.Handlers
{
    internal static partial class WorldHandler
    {
        [PacketHandler(PacketType.GAME_HANDSHAKE)]
        public static void OnGameHandshake(WorldClient client, IPacketStream packet)
        {
            var handshake = new HandshakePacket(packet);

            client.SetClientUserID(handshake.UserId);

            using var sendPacket = new Packet(PacketType.GAME_HANDSHAKE);

            sendPacket.Write(0);
            client.SendPacket(sendPacket);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            DbUser user = database.Users.Include(u => u.Characters)
                                        .ThenInclude(c => c.Items)
                                        .Where(u => u.Id == handshake.UserId)
                                        .FirstOrDefault();

            WorldPacketFactory.SendCharacterList(client, user.Characters);
            WorldPacketFactory.SendAccountFaction(client, user);
        }

        [PacketHandler(PacketType.ACCOUNT_FACTION)]
        public static async void OnAccountFraction(WorldClient client, IPacketStream packet)
        {
            var accountFractionPacket = new AccountFractionPacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbUser user = database.Users.Find(client.UserID);
            user.Faction = accountFractionPacket.Fraction;

            await database.SaveChangesAsync();
        }

        [PacketHandler(PacketType.CHECK_CHARACTER_AVAILABLE_NAME)]
        public static void OnCheckAvailableName(WorldClient client, IPacketStream packet)
        {
            var checkNamePacket = new CheckCharacterAvailableNamePacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbCharacter character = database.Characters.FirstOrDefault(c => c.Name == checkNamePacket.CharacterName);

            WorldPacketFactory.SendCharacterAvailability(client, character is null);
        }

        [PacketHandler(PacketType.CREATE_CHARACTER)]
        public static async void OnCreateCharacter(WorldClient client, IPacketStream packet)
        {
            var createCharacterPacket = new CreateCharacterPacket(packet);
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            // Get number of user characters.
            var characters = database.Characters.Where(x => x.UserId == client.UserID).ToList();

            if (characters.Count == Constants.MaxCharacters - 1)
            {
                // Max number is reached.
                WorldPacketFactory.SendCreatedCharacter(client, false);
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
                UserId = client.UserID
            };

            await database.Characters.AddAsync(character);
            if (await database.SaveChangesAsync() > 0)
            {
                characters.Add(character);
                WorldPacketFactory.SendCreatedCharacter(client, true);
                WorldPacketFactory.SendCharacterList(client, characters);
            }
        }

        [PacketHandler(PacketType.SELECT_CHARACTER)]
        public static void OnSelectCharacter(WorldClient client, IPacketStream packet)
        {
            var selectCharacterPacket = new SelectCharacterPacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var character = database.Characters.Include(c => c.Skills).ThenInclude(cs => cs.Skill)
                                               .Include(c => c.Items).ThenInclude(ci => ci.Item)
                                               .FirstOrDefault(c => c.Id == selectCharacterPacket.CharacterId);

            WorldPacketFactory.SendSelectedCharacter(client, character);
        }

        [PacketHandler(PacketType.LEARN_NEW_SKILL)]
        public static async void OnNewSkillLearn(WorldClient client, IPacketStream packet)
        {
            var learnNewSkillsPacket = new LearnNewSkillPacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            // Find learned skill.
            var skill = database.Skills.First(s => s.SkillId == learnNewSkillsPacket.SkillId && s.SkillLevel == learnNewSkillsPacket.SkillLevel);

            // Find character.
            var character = database.Characters.Include(c => c.Skills)
                                               .ThenInclude(s => s.Skill)
                                               .Where(c => c.Id == client.CharID).First();
            if (character.Skills.Any(s => s.SkillId == skill.Id))
            {
                // Character has already learned this skill.
                // TODO: log it or throw exception?
                return;
            }

            // Check if the character has enough skill points.
            if (character.SkillPoint >= skill.SkillPoint)
            {
                // Save char and learned skill.
                var charSkill = new DbCharacterSkill()
                {
                    CharacterId = client.CharID,
                    SkillId = skill.Id
                };
                character.Skills.Add(charSkill);
                character.SkillPoint -= skill.SkillPoint;
                await database.SaveChangesAsync();

                // Send response.
                WorldPacketFactory.LearnedNewSkill(client, character, true);
            }
            else // Not enough skill points.
            {
                WorldPacketFactory.LearnedNewSkill(client, character, false);
            }
        }

        [PacketHandler(PacketType.INVENTORY_MOVE_ITEM)]
        public static async void OnMoveItem(WorldClient client, IPacketStream packet)
        {
            var moveItemPacket = new MoveItemInInventoryPacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            var charItems = database.CharacterItems.Where(ci => ci.CharacterId == client.CharID);

            // Find source item.
            var sourceItem = charItems.FirstOrDefault(ci => ci.Bag == moveItemPacket.CurrentBag && ci.Slot == moveItemPacket.CurrentSlot);

            // Check, if any other item is at destination slot.
            var destinationItem = charItems.FirstOrDefault(ci => ci.Bag == moveItemPacket.DestinationBag && ci.Slot == moveItemPacket.DestinationSlot);
            if (destinationItem is null)
            {
                // No item at destination place.
                // Since there is no destination item in database we will use source item as destination.
                // The only change, that we need to do is to set new bag and slot.
                destinationItem = sourceItem;
                destinationItem.Bag = moveItemPacket.DestinationBag;
                destinationItem.Slot = moveItemPacket.DestinationSlot;

                // Clear old place. For this we need to create "empty" item, i.e. item with type and type_id and count == 0.
                sourceItem = new DbCharacterItems();
                sourceItem.Bag = moveItemPacket.CurrentBag;
                sourceItem.Slot = moveItemPacket.CurrentSlot;
            }
            else
            {
                // There is some item at destination place.
                if (sourceItem.Type == destinationItem.Type && sourceItem.TypeId == destinationItem.TypeId && destinationItem.IsJoinable)
                {
                    // Increase destination item count, if they are joinable.
                    destinationItem.Count += sourceItem.Count;

                    // This time unlike when destination was null, we have to remove source item from database.
                    database.CharacterItems.Remove(sourceItem);

                    // Clear old item. Again fake "empty" item with 0 type, type_id, count.
                    sourceItem = new DbCharacterItems();
                    sourceItem.Bag = moveItemPacket.CurrentBag;
                    sourceItem.Slot = moveItemPacket.CurrentSlot;
                }
                else
                {
                    // Swap them.
                    destinationItem.Bag = moveItemPacket.CurrentBag;
                    destinationItem.Slot = moveItemPacket.CurrentSlot;

                    sourceItem.Bag = moveItemPacket.DestinationBag;
                    sourceItem.Slot = moveItemPacket.DestinationSlot;
                }
            }

            await database.SaveChangesAsync();
            WorldPacketFactory.SendMoveItem(client, sourceItem, destinationItem);

            if (sourceItem.Bag == 0 || destinationItem.Bag == 0)
            {
                // Send equipment update to character.
                WorldPacketFactory.SendEquipment(client, client.CharID, sourceItem.Bag == 0 ? sourceItem : destinationItem);

                // TODO: send equipment update to all characters nearby.
            }
        }
    }
}
