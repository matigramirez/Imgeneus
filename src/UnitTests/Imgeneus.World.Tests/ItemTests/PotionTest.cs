using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.ItemTests
{
    public class PotionTest : BaseTest
    {
        [Fact]
        [Description("Etain Potion should recover 75% of hp, mp, sp.")]
        public void ComposedStatsAreAdded()
        {
            var character = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object,
                databasePreloader.Object, chatMock.Object, linkingMock.Object, dyeingMock.Object)
            {
                Class = CharacterProfession.Fighter,
            };
            var character2 = new Character(loggerMock.Object, gameWorldMock.Object, config.Object, taskQueuMock.Object,
                databasePreloader.Object, chatMock.Object, linkingMock.Object, dyeingMock.Object);

            Assert.Equal(100, character.MaxHP);
            Assert.Equal(200, character.MaxMP);
            Assert.Equal(300, character.MaxSP);

            character.IncreaseHP(100);
            Assert.Equal(100, character.CurrentHP);
            Assert.Equal(0, character.CurrentMP);
            Assert.Equal(0, character.CurrentSP);

            character.DecreaseHP(90, character2);
            Assert.Equal(10, character.CurrentHP);

            character.AddItemToInventory(new Item(databasePreloader.Object, EtainPotion.Type, EtainPotion.TypeId));
            character.UseItem(1, 0);

            Assert.Equal(85, character.CurrentHP);
            Assert.Equal(150, character.CurrentMP);
            Assert.Equal(225, character.CurrentSP);
        }
    }
}
