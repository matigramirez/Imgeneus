using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.PartyTests
{
    public class RaidTest : BaseTest
    {
        [Fact]
        [Description("First player, that connected raid is its' leader. Second - subleader.")]
        public void Party_Leader()
        {
            var character1 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object)
            {
                Name = "Character1"
            };
            character1.Client = worldClientMock.Object;
            var character2 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object)
            {
                Name = "Character2"
            };
            character2.Client = worldClientMock.Object;
            Assert.False(character1.IsPartyLead);

            var raid = new Raid(true, RaidDropType.Group);
            character1.SetParty(raid);
            character2.SetParty(raid);

            Assert.True(character1.IsPartyLead);
            Assert.Equal(character1, raid.Leader);

            Assert.True(character2.IsPartySubLeader);
            Assert.Equal(character2, raid.SubLeader);
        }
    }
}
