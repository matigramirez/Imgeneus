using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using System.Collections.Generic;
using System.ComponentModel;
using Imgeneus.Database.Entities;
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
        [Description("Experience should be split among party members who are within range of mob killer player.")]
        public void Party_Experience()
        {
            var killerCharacter = CreateCharacter(_map);
            var nearbyCharacter = CreateCharacter(_map);
            var farAwayCharacter = CreateCharacter(_map);

            killerCharacter.TrySetMode(Mode.Ultimate);
            nearbyCharacter.TrySetMode(Mode.Ultimate);
            farAwayCharacter.TrySetMode(Mode.Ultimate);

            killerCharacter.PosX = 0;
            killerCharacter.PosZ = 0;

            nearbyCharacter.PosX = 0;
            nearbyCharacter.PosZ = 0;

            farAwayCharacter.PosX = 1000;
            farAwayCharacter.PosZ = 1000;

            var party = new Party();
            killerCharacter.SetParty(party);
            nearbyCharacter.SetParty(party);
            farAwayCharacter.SetParty(party);

            var mob = new Mob(Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), _map, mobLoggerMock.Object,
                              databasePreloader.Object);

            killerCharacter.TryChangeLevel(mob.Level, true);
            nearbyCharacter.TryChangeLevel(mob.Level, true);
            farAwayCharacter.TryChangeLevel(mob.Level, true);

            _map.LoadPlayer(killerCharacter);
            _map.LoadPlayer(nearbyCharacter);
            _map.LoadPlayer(farAwayCharacter);
            _map.AddMob(mob);

            Assert.Equal((uint)3022800, killerCharacter.Exp);
            Assert.Equal((uint)3022800, nearbyCharacter.Exp);
            Assert.Equal((uint)3022800, farAwayCharacter.Exp);

            mob.DecreaseHP(20000000, killerCharacter);

            Assert.True(mob.IsDead);

            var expectedNewExp = (uint)3022800 + 120 / 3;

            Assert.Equal(expectedNewExp, killerCharacter.Exp);
            Assert.Equal(expectedNewExp, nearbyCharacter.Exp);
            Assert.Equal((uint)3022800, farAwayCharacter.Exp);
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

            var party = new Party();
            character1.SetParty(party);
            character2.SetParty(party);
            character3.SetParty(party);
            character4.SetParty(party);
            character5.SetParty(party);
            character6.SetParty(party);
            character7.SetParty(party);

            var mob = new Mob(Wolf.Id, true, new MoveArea(0, 0, 0, 0, 0, 0), _map, mobLoggerMock.Object,
                              databasePreloader.Object);

            character1.TrySetMode(Mode.Ultimate);
            character2.TrySetMode(Mode.Ultimate);
            character3.TrySetMode(Mode.Ultimate);
            character4.TrySetMode(Mode.Ultimate);
            character5.TrySetMode(Mode.Ultimate);
            character6.TrySetMode(Mode.Ultimate);
            character7.TrySetMode(Mode.Ultimate);

            character1.TryChangeLevel(mob.Level, true);
            character2.TryChangeLevel(mob.Level, true);
            character3.TryChangeLevel(mob.Level, true);
            character4.TryChangeLevel(mob.Level, true);
            character5.TryChangeLevel(mob.Level, true);
            character6.TryChangeLevel(mob.Level, true);
            character7.TryChangeLevel(mob.Level, true);

            _map.LoadPlayer(character1);
            _map.LoadPlayer(character2);
            _map.LoadPlayer(character3);
            _map.LoadPlayer(character4);
            _map.LoadPlayer(character5);
            _map.LoadPlayer(character6);
            _map.LoadPlayer(character7);
            _map.AddMob(mob);

            Assert.Equal((uint)3022800, character1.Exp);
            Assert.Equal((uint)3022800, character2.Exp);
            Assert.Equal((uint)3022800, character3.Exp);
            Assert.Equal((uint)3022800, character4.Exp);
            Assert.Equal((uint)3022800, character5.Exp);
            Assert.Equal((uint)3022800, character6.Exp);
            Assert.Equal((uint)3022800, character7.Exp);

            mob.DecreaseHP(20000000, character1);

            Assert.True(mob.IsDead);

            var expectedExp = (uint)3022800 + 120 / 2;

            Assert.Equal(expectedExp, character1.Exp);
            Assert.Equal(expectedExp, character2.Exp);
            Assert.Equal(expectedExp, character3.Exp);
            Assert.Equal(expectedExp, character4.Exp);
            Assert.Equal(expectedExp, character5.Exp);
            Assert.Equal(expectedExp, character6.Exp);
            Assert.Equal(expectedExp, character7.Exp);
        }
    }
}
