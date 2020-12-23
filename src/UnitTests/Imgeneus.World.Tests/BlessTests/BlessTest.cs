using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Blessing;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.BlessTests
{
    public class BlessTest : BaseTest
    {
        [Fact]
        [Description("Amount of bless can change max HP, SP and MP.")]
        public void Bless_Max_HP_SP_MP()
        {
            var character = CreateCharacter();

            Assert.Equal(100, character.MaxHP);
            Assert.Equal(200, character.MaxMP);
            Assert.Equal(300, character.MaxSP);
            Assert.Equal(Fraction.Light, character.Country);

            Bless.Instance.LightAmount = Bless.MAX_HP_SP_MP;
            Assert.Equal(120, character.MaxHP);
            Assert.Equal(240, character.MaxMP);
            Assert.Equal(360, character.MaxSP);

            Bless.Instance.LightAmount = Bless.MAX_HP_SP_MP - 100;
            Assert.Equal(100, character.MaxHP);
            Assert.Equal(200, character.MaxMP);
            Assert.Equal(300, character.MaxSP);
        }
    }
}
