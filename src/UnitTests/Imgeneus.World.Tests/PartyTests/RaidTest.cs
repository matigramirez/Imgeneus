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
        public void Raid_Leader()
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

        [Fact]
        [Description("If drop type is RaidDropType.Leader, then leader should get all items in drop.")]
        public void Raid_DropToLeader()
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

            var raid = new Raid(true, RaidDropType.Leader);
            character1.SetParty(raid);
            character2.SetParty(raid);

            raid.DistributeDrop(new List<Item>()
            {
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId)
            }, character2);

            Assert.Equal(3, character1.InventoryItems.Count);
        }

        [Fact]
        [Description("If drop type is RaidDropType.Leader, but leader is too far away, then he doesn't get drop.")]
        public void Raid_DropToLeader_LeaderIsFarAway()
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

            // Set leader far away from character2.
            character1.PosX = 1000;
            character1.PosY = 1000;
            character1.PosZ = 1000;

            var raid = new Raid(true, RaidDropType.Leader);
            character1.SetParty(raid);
            character2.SetParty(raid);

            raid.DistributeDrop(new List<Item>()
            {
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId)
            }, character2);

            Assert.Empty(character1.InventoryItems);
        }

        [Fact]
        [Description("If drop type is RaidDropType.Group, items are distributed one by one to each raid member.")]
        public void Raid_DropToGroup()
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

            var raid = new Raid(true, RaidDropType.Group);
            character1.SetParty(raid);
            character2.SetParty(raid);

            raid.DistributeDrop(new List<Item>()
            {
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId)
            }, character2);

            Assert.Equal(2, character1.InventoryItems.Count);
            Assert.Single(character2.InventoryItems);

            Assert.Equal(WaterArmor.Type, character1.InventoryItems[0].Type);
            Assert.Equal(FireSword.Type, character1.InventoryItems[1].Type);

            Assert.Equal(WaterArmor.Type, character2.InventoryItems[0].Type);
        }

        [Fact]
        [Description("If drop type is RaidDropType.Group, but some member is far away he doesn't get drop.")]
        public void Raid_DropToGroup_FarAway()
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
            var character3 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object)
            {
                Name = "Character3"
            };
            character3.Client = worldClientMock.Object;

            // Set character3 far away from character1 and character2.
            character3.PosX = 1000;
            character3.PosY = 1000;
            character3.PosZ = 1000;

            var raid = new Raid(true, RaidDropType.Group);
            character1.SetParty(raid);
            character2.SetParty(raid);
            character3.SetParty(raid);

            raid.DistributeDrop(new List<Item>()
            {
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId)
            }, character2);

            Assert.Equal(2, character1.InventoryItems.Count);
            Assert.Single(character2.InventoryItems);
            Assert.Empty(character3.InventoryItems);
        }

        [Fact]
        [Description("If drop type is RaidDropType.Group, money should be distributed equally.")]
        public void Raid_DropToGroup_GoldDistribution()
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
            var character3 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object)
            {
                Name = "Character3"
            };
            character3.Client = worldClientMock.Object;

            var raid = new Raid(true, RaidDropType.Group);
            character1.SetParty(raid);
            character2.SetParty(raid);
            character3.SetParty(raid);

            var money = new Item(databasePreloader.Object, Item.MONEY_ITEM_TYPE, 0);
            money.Gem1 = new Gem(100);
            raid.DistributeDrop(new List<Item>() { money }, character2);

            Assert.Equal((uint)33, character1.Gold);
            Assert.Equal((uint)33, character2.Gold);
            Assert.Equal((uint)33, character3.Gold);
        }

        [Fact]
        [Description("If drop type is RaidDropType.Group, but members do not have place in inventory, items are not distributed.")]
        public void Raid_DropToGroup_NoPlaceInInventory()
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

            for (int i = 0; i < 5 * 25; i++) // 5 bags, 24 slots per 1 bag.
            {
                character1.AddItemToInventory(new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId));
                character2.AddItemToInventory(new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId));
            }

            var raid = new Raid(true, RaidDropType.Group);
            character1.SetParty(raid);
            character2.SetParty(raid);

            var notDistributedItems = raid.DistributeDrop(new List<Item>()
            {
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId)
            }, character2);

            Assert.Equal(3, notDistributedItems.Count);
        }

        [Fact]
        [Description("If drop type is RaidDropType.Random, drop items should be assign to random users.")]
        public void Raid_DropToRandom()
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
            var character3 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object, databasePreloader.Object, chatMock.Object)
            {
                Name = "Character3"
            };
            character3.Client = worldClientMock.Object;

            var raid = new Raid(true, RaidDropType.Random);
            character1.SetParty(raid);
            character2.SetParty(raid);
            character3.SetParty(raid);

            raid.DistributeDrop(new List<Item>()
            {
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, WaterArmor.Type, WaterArmor.TypeId),
                new Item(databasePreloader.Object, FireSword.Type, FireSword.TypeId)
            }, character2);

            Assert.True(character1.InventoryItems.Count >= 0);
            Assert.True(character2.InventoryItems.Count >= 0);
            Assert.True(character3.InventoryItems.Count >= 0);

            Assert.Equal(3, character1.InventoryItems.Count + character2.InventoryItems.Count + character3.InventoryItems.Count);
        }
    }
}
