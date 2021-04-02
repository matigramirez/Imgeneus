using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Guild;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Time;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;

namespace Imgeneus.World.Tests.GuildTests
{
    public class GuildTest : BaseTest
    {
        [Fact]
        [Description("It should not be possible to create a guild, if not enough money.")]
        public async Task CanCreateGuild_MinGoldTest()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinGold = 10
            });

            var character = CreateCharacter();
            var guildManager = new GuildManager(guildLoggerMock.Object, config, databaseMock.Object, gameWorldMock.Object, timeMock.Object);
            var result = await guildManager.CanCreateGuild(character, "guild_name");

            Assert.Equal((uint)0, character.Gold);
            Assert.Equal(GuildCreateFailedReason.NotEnoughGold, result);
        }

        [Fact]
        [Description("It should not be possible to create a guild, if not enough party members.")]
        public async Task CanCreateGuild_MinMembersTest()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinMembers = 2
            });

            var character = CreateCharacter();
            var guildManager = new GuildManager(guildLoggerMock.Object, config, databaseMock.Object, gameWorldMock.Object, timeMock.Object);
            var result = await guildManager.CanCreateGuild(character, "guild_name");

            Assert.Equal(GuildCreateFailedReason.NotEnoughMembers, result);
        }

        [Fact]
        [Description("It should not be possible to create a guild, if not enough party members level.")]
        public async Task CanCreateGuild_MinLevelTest()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinMembers = 2,
                MinLevel = 2
            });

            var character1 = CreateCharacter(testMap);
            var character2 = CreateCharacter(testMap);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var guildManager = new GuildManager(guildLoggerMock.Object, config, databaseMock.Object, gameWorldMock.Object, timeMock.Object);
            var result = await guildManager.CanCreateGuild(character1, "guild_name");

            Assert.Equal(1, character1.Level);
            Assert.Equal(1, character2.Level);

            Assert.Equal(GuildCreateFailedReason.LevelLimit, result);
        }

        [Fact]
        [Description("It should not be possible to create a guild, if one party meber belongs to another guild.")]
        public async Task CanCreateGuild_AnotherGuildTest()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinMembers = 2
            });

            var character1 = CreateCharacter(testMap);
            var character2 = CreateCharacter(testMap);
            character2.GuildId = 1;

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var guildManager = new GuildManager(guildLoggerMock.Object, config, databaseMock.Object, gameWorldMock.Object, timeMock.Object);
            var result = await guildManager.CanCreateGuild(character1, "guild_name");

            Assert.Equal(GuildCreateFailedReason.PartyMemberInAnotherGuild, result);
        }

        [Fact]
        [Description("It should not be possible to create a guild with wrong name.")]
        public async Task CanCreateGuild_GuildNameTest()
        {
            var character = CreateCharacter(testMap);

            var guildManager = new GuildManager(guildLoggerMock.Object, Options.Create(new GuildConfiguration()), databaseMock.Object, gameWorldMock.Object, timeMock.Object);
            var result = await guildManager.CanCreateGuild(character, "");

            Assert.Equal(GuildCreateFailedReason.WrongName, result);
        }

        [Fact]
        [Description("It should not be possible to create a guild if at least one party member has penalty.")]
        public async Task CanCreateGuild_PenaltyTest()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinMembers = 2,
                MinPenalty = 2 // 2 hours
            });

            var character1 = CreateCharacter(testMap);
            var character2 = CreateCharacter(testMap);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var time = new Mock<ITimeService>();
            time.Setup(x => x.UtcNow)
                .Returns(new DateTime(2021, 1, 1, 2, 0, 0));  // 1 Jan 2021 02:00

            var database = new Mock<IDatabase>();
            database.Setup(x => x.Characters.FindAsync(character1.Id))
                .ReturnsAsync(new DbCharacter()
                {
                    Id = character1.Id,
                    GuildLeaveTime = new DateTime(2021, 1, 1, 1, 0, 0) // 1 Jan 2021 01:00
                });
            database.Setup(x => x.Characters.FindAsync(character2.Id))
                .ReturnsAsync(new DbCharacter());

            var guildManager = new GuildManager(guildLoggerMock.Object, config, database.Object, gameWorldMock.Object, time.Object);
            var result = await guildManager.CanCreateGuild(character1, "guild_name");

            Assert.Equal(GuildCreateFailedReason.PartyMemberGuildPenalty, result);
        }

        [Fact]
        [Description("It should be possible to create a guild.")]
        public async Task CanCreateGuild_SuccessTest()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinMembers = 2
            });

            var character1 = CreateCharacter(testMap);
            var character2 = CreateCharacter(testMap);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var database = new Mock<IDatabase>();
            database.Setup(x => x.Characters.FindAsync(character1.Id)).ReturnsAsync(new DbCharacter());
            database.Setup(x => x.Characters.FindAsync(character2.Id)).ReturnsAsync(new DbCharacter());

            var guildManager = new GuildManager(guildLoggerMock.Object, config, database.Object, gameWorldMock.Object, timeMock.Object);
            var result = await guildManager.CanCreateGuild(character1, "guild_name");

            Assert.Equal(GuildCreateFailedReason.Success, result);
        }

        [Fact]
        [Description("It should send guild create request to all party members. As soon as party changes, request is not valid.")]
        public void SendGuildRequest_PartyChangeTest()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinMembers = 2
            });

            var character1 = CreateCharacter(testMap);
            var character2 = CreateCharacter(testMap);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var guildManager = new GuildManager(guildLoggerMock.Object, config, databaseMock.Object, gameWorldMock.Object, timeMock.Object);
            guildManager.SendGuildRequest(character1, "guild_name", "guild_message");

            Assert.NotEmpty(GuildManager.CreationRequests);
            Assert.True(GuildManager.CreationRequests.ContainsKey(party));

            character2.SetParty(null);

            Assert.Empty(GuildManager.CreationRequests);
            Assert.False(GuildManager.CreationRequests.ContainsKey(party));
        }

        [Fact]
        [Description("Guild should be created as soon as all party members agree to create it.")]
        public void SetAgreeRequest_Test()
        {
            var config = Options.Create(new GuildConfiguration()
            {
                MinMembers = 2
            });

            var character1 = CreateCharacter(testMap);
            var character2 = CreateCharacter(testMap);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var database = new Mock<IDatabase>();
            database.Setup(x => x.Guilds.Add(It.IsAny<DbGuild>()));
            database.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var guildManager = new GuildManager(guildLoggerMock.Object, config, database.Object, gameWorldMock.Object, timeMock.Object);
            guildManager.SendGuildRequest(character1, "guild_name", "guild_message");

            guildManager.SetAgreeRequest(character1, true);
            guildManager.SetAgreeRequest(character2, true);

            Assert.Equal("guild_name", character1.GuildName);
            Assert.Equal("guild_name", character2.GuildName);

            Assert.Equal(1, character1.GuildRank);
            Assert.Equal(9, character2.GuildRank);
        }

        [Fact]
        [Description("Player can request to join only 1 guild at a time.")]
        public async void RequestJoin_Test()
        {
            var character = CreateCharacter(testMap);

            var database = new Mock<IDatabase>();
            database.Setup(x => x.Guilds.FindAsync(It.IsAny<int>())).ReturnsAsync(new DbGuild("test_guild", "test_message", 99, Fraction.Light));
            database.Setup(x => x.Characters.FindAsync(It.IsAny<int>())).ReturnsAsync(new DbCharacter());

            var guildManager = new GuildManager(guildLoggerMock.Object, Options.Create(new GuildConfiguration()), database.Object, gameWorldMock.Object, timeMock.Object);
            await guildManager.RequestJoin(1, character.Id);

            Assert.Single(GuildManager.JoinRequests);
            Assert.True(GuildManager.JoinRequests.ContainsKey(character.Id));
            Assert.Equal(1, GuildManager.JoinRequests[character.Id]);

            await guildManager.RequestJoin(2, character.Id);

            Assert.Single(GuildManager.JoinRequests);
            Assert.True(GuildManager.JoinRequests.ContainsKey(character.Id));
            Assert.Equal(2, GuildManager.JoinRequests[character.Id]);
        }

        [Fact]
        [Description("Player can buy guild house only if he is guild owner (i.e. rank is 1).")]
        public async void TryBuyHouse_OnlyRank1Test()
        {
            var character = CreateCharacter(testMap);

            var guildManager = new GuildManager(guildLoggerMock.Object, Options.Create(new GuildConfiguration()), databaseMock.Object, gameWorldMock.Object, timeMock.Object);

            var result = await guildManager.TryBuyHouse(character);

            Assert.Equal(GuildHouseBuyReason.NotAuthorized, result);
        }

        [Fact]
        [Description("Player can buy guild house if he has enough gold.")]
        public async void TryBuyHouse_NotEnoughtGoldTest()
        {
            var character = CreateCharacter(testMap);
            character.GuildRank = 1;

            var guildManager = new GuildManager(guildLoggerMock.Object, Options.Create(new GuildConfiguration() { HouseBuyMoney = 100 }), databaseMock.Object, gameWorldMock.Object, timeMock.Object);

            var result = await guildManager.TryBuyHouse(character);

            Assert.Equal(GuildHouseBuyReason.NoGold, result);
        }

        [Fact]
        [Description("Player can buy guild house if his guild is in top 30 rank.")]
        public async void TryBuyHouse_Top30Test()
        {
            var character = CreateCharacter(testMap);
            character.GuildId = 1;
            character.GuildRank = 1;

            var database = new Mock<IDatabase>();
            database.Setup(x => x.Guilds.FindAsync(It.IsAny<int>())).ReturnsAsync(new DbGuild("test_guild", "test_message", 99, Fraction.Light) { Rank = 31 });

            var guildManager = new GuildManager(guildLoggerMock.Object, Options.Create(new GuildConfiguration()), database.Object, gameWorldMock.Object, timeMock.Object);

            var result = await guildManager.TryBuyHouse(character);

            Assert.Equal(GuildHouseBuyReason.LowRank, result);
        }

        [Fact]
        [Description("Player can buy guild house only once.")]
        public async void TryBuyHouse_HasHouseTest()
        {
            var character = CreateCharacter(testMap);
            character.GuildId = 1;
            character.GuildRank = 1;

            var database = new Mock<IDatabase>();
            database.Setup(x => x.Guilds.FindAsync(It.IsAny<int>())).ReturnsAsync(new DbGuild("test_guild", "test_message", 99, Fraction.Light) { Rank = 1, HasHouse = true });

            var guildManager = new GuildManager(guildLoggerMock.Object, Options.Create(new GuildConfiguration()), database.Object, gameWorldMock.Object, timeMock.Object);

            var result = await guildManager.TryBuyHouse(character);

            Assert.Equal(GuildHouseBuyReason.AlreadyBought, result);
        }

        [Fact]
        [Description("Player can buy guild house.")]
        public async void TryBuyHouse_SuccessTest()
        {
            var character = CreateCharacter(testMap);
            character.GuildId = 1;
            character.GuildRank = 1;

            var database = new Mock<IDatabase>();
            database.Setup(x => x.Guilds.FindAsync(It.IsAny<int>())).ReturnsAsync(new DbGuild("test_guild", "test_message", 99, Fraction.Light) { Rank = 1, HasHouse = false });

            var guildManager = new GuildManager(guildLoggerMock.Object, Options.Create(new GuildConfiguration()), database.Object, gameWorldMock.Object, timeMock.Object);

            var result = await guildManager.TryBuyHouse(character);

            Assert.Equal(GuildHouseBuyReason.Ok, result);
        }
    }
}
