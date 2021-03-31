using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class PartyMapTest : BaseTest
    {
        [Fact]
        [Description("The Party map should notify as soon as all party members left the map and the party was destroyed.")]
        public void PartyMapDestroyWhenAllPlayersLeft()
        {
            var usualMap = testMap;
            var character1 = CreateCharacter(usualMap);
            var character2 = CreateCharacter(usualMap);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var partyMap = new PartyMap(party,
                                        Map.TEST_MAP_ID,
                                        new MapDefinition() { CreateType = CreateType.Party },
                                        new MapConfiguration() { Size = 100, CellSize = 100 },
                                        mapLoggerMock.Object,
                                        databasePreloader.Object,
                                        mobFactoryMock.Object,
                                        npcFactoryMock.Object,
                                        obeliskFactoryMock.Object,
                                        timeMock.Object);
            var allLeftWasCalled = false;
            partyMap.OnAllMembersLeft += (sender) =>
            {
                allLeftWasCalled = true;
            };

            partyMap.LoadPlayer(character1);
            partyMap.LoadPlayer(character2);

            character1.SetParty(null);

            Assert.False(allLeftWasCalled); // Should be called only after all members left.

            partyMap.UnloadPlayer(character1);
            partyMap.UnloadPlayer(character2);

            Assert.True(allLeftWasCalled);
        }

        [Fact]
        [Description("The Party map should notify as soon as all party members left the party.")]
        public void PartyMapDestroyWhenPartyDestroyed()
        {
            var usualMap = testMap;
            var character1 = CreateCharacter(usualMap);
            var character2 = CreateCharacter(usualMap);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            var partyMap = new PartyMap(party,
                                        Map.TEST_MAP_ID,
                                        new MapDefinition() { CreateType = CreateType.Party },
                                        new MapConfiguration() { Size = 100, CellSize = 100 },
                                        mapLoggerMock.Object,
                                        databasePreloader.Object,
                                        mobFactoryMock.Object,
                                        npcFactoryMock.Object,
                                        obeliskFactoryMock.Object,
                                        timeMock.Object);
            var allLeftWasCalled = false;
            partyMap.OnAllMembersLeft += (sender) =>
            {
                allLeftWasCalled = true;
            };

            character1.SetParty(null);

            Assert.Null(character1.Party);
            Assert.Null(character2.Party);

            Assert.True(allLeftWasCalled); // No party member visited map, we can delete it.
        }
    }
}
