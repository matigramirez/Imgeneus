using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using System;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class PartyMapTest : BaseTest
    {
        [Fact]
        [Description("The Party map should be disposed as soon as all party members left the map and the party was destroyed.")]
        public void PartyMapDestroy()
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
                                        obeliskFactoryMock.Object);
            var allLeftWasCalled = false;
            partyMap.OnAllMembersLeft += (sender) =>
            {
                allLeftWasCalled = true;
            };

            partyMap.LoadPlayer(character1);
            partyMap.LoadPlayer(character2);

            character1.SetParty(null);

            partyMap.UnloadPlayer(character1);
            partyMap.UnloadPlayer(character2);

            Assert.True(allLeftWasCalled);
            Assert.Throws<ObjectDisposedException>(() => partyMap.LoadPlayer(character1));
        }
    }
}
