using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Zone.Portals;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class PortalTest : BaseTest
    {
        [Fact]
        [Description("Character must have right level to enter portal.")]
        public void Portal_RightLevel()
        {
            var portal = new Portal(new PortalConfiguration() { MinLvl = 20, MaxLvl = 30 });

            Assert.False(portal.IsRightLevel(9));
            Assert.False(portal.IsRightLevel(31));

            Assert.True(portal.IsRightLevel(20));
            Assert.True(portal.IsRightLevel(25));
            Assert.True(portal.IsRightLevel(30));
        }

        [Fact]
        [Description("Character must have right faction to enter portal.")]
        public void Portal_RightFaction()
        {
            Portal portal;

            portal = new Portal(new PortalConfiguration() { Faction = 0 });
            Assert.True(portal.IsSameFaction(Fraction.Light));
            Assert.True(portal.IsSameFaction(Fraction.Dark));

            portal = new Portal(new PortalConfiguration() { Faction = 1 });
            Assert.True(portal.IsSameFaction(Fraction.Light));
            Assert.False(portal.IsSameFaction(Fraction.Dark));

            portal = new Portal(new PortalConfiguration() { Faction = 2 });
            Assert.False(portal.IsSameFaction(Fraction.Light));
            Assert.True(portal.IsSameFaction(Fraction.Dark));
        }
    }
}
