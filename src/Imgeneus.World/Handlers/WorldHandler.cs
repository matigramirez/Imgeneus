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

            DbUser user = database.Users.Include(u => u.Characters).Where(u => u.Id == handshake.UserId).FirstOrDefault();

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
            var count = (byte)database.Characters.Count(x => x.UserId == client.UserID);

            if (count == Constants.MaxCharacters - 1)
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

            await database.Characters.AddAsync(character);
            await database.SaveChangesAsync();
            WorldPacketFactory.SendCreatedCharacter(client, true);
        }

        [PacketHandler(PacketType.SELECT_CHARACTER)]
        public static void OnSelectCharacter(WorldClient client, IPacketStream packet)
        {
            var selectCharacterPacket = new SelectCharacterPacket(packet);

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var character = database.Characters.Include(c => c.Skills).ThenInclude(cs => cs.Skill)
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
    }
}
