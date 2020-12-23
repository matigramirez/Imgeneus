using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.PartyTests
{
    public class PartyTest : BaseTest
    {
        private Map _map;

        public PartyTest()
        {
            _map = testMap;
        }

        [Fact]
        [Description("First player, that connected party is its' leader.")]
        public void Party_Leader()
        {
            var character1 = CreateCharacter(_map);
            var character2 = CreateCharacter(_map);

            Assert.False(character1.IsPartyLead);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            Assert.True(character1.IsPartyLead);
            Assert.Equal(character1, party.Leader);
        }

        [Fact]
        [Description("Party drop should be for each player, 1 by 1.")]
        public void Party_DropCalculation()
        {
            var character1 = CreateCharacter(_map);
            var character2 = CreateCharacter(_map);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);

            party.DistributeDrop(new List<Item>()
            {
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId)
            }, character2);

            Assert.Equal(2, character1.InventoryItems.Count);
            Assert.Single(character2.InventoryItems);

            Assert.Equal(WaterArmor.Type, character1.InventoryItems[(1, 0)].Type);
            Assert.Equal(FireSword.Type, character1.InventoryItems[(1, 1)].Type);

            Assert.Equal(WaterArmor.Type, character2.InventoryItems[(1, 0)].Type);
        }
    }
}
