using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using System.Collections.Generic;
using System.ComponentModel;
using Imgeneus.World.Game.Monster;
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

        [Fact]
        [Description("Experience should be split among party members.")]
        public void Party_Experience()
        {
            var character1 = CreateCharacter(_map);
            var character2 = CreateCharacter(_map);
            var character3 = CreateCharacter(_map);

            character1.TryChangeExperience(0);
            character2.TryChangeExperience(0);
            character3.TryChangeExperience(0);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);
            character3.SetParty(party);

            var mob = new Mob(Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), _map, mobLoggerMock.Object, databasePreloader.Object);

            _map.LoadPlayer(character1);
            _map.LoadPlayer(character2);
            _map.LoadPlayer(character3);
            _map.AddMob(mob);

            Assert.Equal((uint)0, character1.Exp);
            Assert.Equal((uint)0, character2.Exp);
            Assert.Equal((uint)0, character3.Exp);

            mob.DecreaseHP(20000000, character1);

            Assert.True(mob.IsDead);
            Assert.Equal((uint)(mob.Exp / 3), character1.Exp);
            Assert.Equal((uint)(mob.Exp / 3), character2.Exp);
            Assert.Equal((uint)(mob.Exp / 3), character3.Exp);
        }

        [Fact]
        [Description("Experience in perfect parties should be gained as if there were only 2 party members.")]
        public void Party_PerfectPartyExperience()
        {
            var character1 = CreateCharacter(_map);
            var character2 = CreateCharacter(_map);
            var character3 = CreateCharacter(_map);
            var character4 = CreateCharacter(_map);
            var character5 = CreateCharacter(_map);
            var character6 = CreateCharacter(_map);
            var character7 = CreateCharacter(_map);

            character1.TryChangeExperience(0);
            character2.TryChangeExperience(0);
            character3.TryChangeExperience(0);
            character4.TryChangeExperience(0);
            character5.TryChangeExperience(0);
            character6.TryChangeExperience(0);
            character7.TryChangeExperience(0);

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);
            character3.SetParty(party);
            character4.SetParty(party);
            character5.SetParty(party);
            character6.SetParty(party);
            character7.SetParty(party);

            var mob = new Mob(Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), _map, mobLoggerMock.Object, databasePreloader.Object);

            _map.LoadPlayer(character1);
            _map.LoadPlayer(character2);
            _map.LoadPlayer(character3);
            _map.LoadPlayer(character4);
            _map.LoadPlayer(character5);
            _map.LoadPlayer(character6);
            _map.LoadPlayer(character7);
            _map.AddMob(mob);

            Assert.Equal((uint)0, character1.Exp);
            Assert.Equal((uint)0, character2.Exp);
            Assert.Equal((uint)0, character3.Exp);
            Assert.Equal((uint)0, character4.Exp);
            Assert.Equal((uint)0, character5.Exp);
            Assert.Equal((uint)0, character6.Exp);
            Assert.Equal((uint)0, character7.Exp);

            mob.DecreaseHP(20000000, character1);

            Assert.True(mob.IsDead);
            Assert.Equal((uint)(mob.Exp / 2), character1.Exp);
            Assert.Equal((uint)(mob.Exp / 2), character2.Exp);
            Assert.Equal((uint)(mob.Exp / 2), character3.Exp);
            Assert.Equal((uint)(mob.Exp / 2), character4.Exp);
            Assert.Equal((uint)(mob.Exp / 2), character5.Exp);
            Assert.Equal((uint)(mob.Exp / 2), character6.Exp);
            Assert.Equal((uint)(mob.Exp / 2), character7.Exp);
        }
    }
}
