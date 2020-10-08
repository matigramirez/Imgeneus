using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.ItemTests
{
    public class CraftNameTest : BaseTest
    {
        [Fact]
        [Description("If item has composed stats, they are added to total stats calculation.")]
        public void ComposedStatsAreAdded()
        {
            var item = new Item(databasePreloader.Object, JustiaArmor.Type, JustiaArmor.TypeId);
            Assert.Equal(30, item.Str);
            Assert.Equal(30, item.Dex);
            Assert.Equal(30, item.Rec);
            Assert.Equal(0, item.Int);
            Assert.Equal(0, item.Luc);
            Assert.Equal(0, item.Wis);
            Assert.Equal(1800, item.HP);
            Assert.Equal(0, item.MP);
            Assert.Equal(600, item.SP);

            item.ComposedStr = 1;
            item.ComposedDex = 2;
            item.ComposedRec = 3;
            item.ComposedInt = 4;
            item.ComposedLuc = 5;
            item.ComposedWis = 6;
            item.ComposedHP = 100;
            item.ComposedMP = 200;
            item.ComposedSP = 300;

            Assert.Equal(31, item.Str);
            Assert.Equal(32, item.Dex);
            Assert.Equal(33, item.Rec);
            Assert.Equal(4, item.Int);
            Assert.Equal(5, item.Luc);
            Assert.Equal(6, item.Wis);
            Assert.Equal(1900, item.HP);
            Assert.Equal(200, item.MP);
            Assert.Equal(900, item.SP);
        }

        [Theory]
        [Description("If item has composed stats, we can create craft name.")]
        [InlineData(0, 0, 0, 0, 0, 0, 100, 200, 300, "00000000000001020300")]
        [InlineData(1, 2, 3, 4, 5, 6, 100, 200, 300, "01020304050601020300")]
        [InlineData(10, 22, 33, 44, 55, 66, 1000, 2000, 3000, "10223344556610203000")]
        public void ComposedStatsCanBeTranslatedToCraftName(int str, int dex, int rec, int intl, int wis, int luc, int hp, int mp, int sp, string expected)
        {
            var item = new Item(databasePreloader.Object, JustiaArmor.Type, JustiaArmor.TypeId);
            item.ComposedStr = str;
            item.ComposedDex = dex;
            item.ComposedRec = rec;
            item.ComposedInt = intl;
            item.ComposedWis = wis;
            item.ComposedLuc = luc;
            item.ComposedHP = hp;
            item.ComposedMP = mp;
            item.ComposedSP = sp;

            Assert.Equal(expected, item.GetCraftName());
        }

        [Theory]
        [Description("It should be possible to recreate item compose stats from craft name.")]
        [InlineData(null, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [InlineData("", 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [InlineData("bad_craftname_length", 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [InlineData("111111111111111111111", 0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [InlineData("00000000000001020300", 0, 0, 0, 0, 0, 0, 100, 200, 300)]
        [InlineData("01020304050601020300", 1, 2, 3, 4, 5, 6, 100, 200, 300)]
        [InlineData("10223344556610203000", 10, 22, 33, 44, 55, 66, 1000, 2000, 3000)]
        public void ComposedStatsCanBeReadFromCraftName(string craftName, int str, int dex, int rec, int intl, int wis, int luc, int hp, int mp, int sp)
        {
            var item = new Item(databasePreloader.Object, new DbCharacterItems()
            {
                Craftname = craftName
            });

            Assert.Equal(str, item.ComposedStr);
            Assert.Equal(dex, item.ComposedDex);
            Assert.Equal(rec, item.ComposedRec);
            Assert.Equal(intl, item.ComposedInt);
            Assert.Equal(wis, item.ComposedWis);
            Assert.Equal(luc, item.ComposedLuc);
            Assert.Equal(hp, item.ComposedHP);
            Assert.Equal(mp, item.ComposedMP);
            Assert.Equal(sp, item.ComposedSP);
        }
    }
}
