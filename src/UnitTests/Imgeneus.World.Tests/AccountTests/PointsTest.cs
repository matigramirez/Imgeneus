using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.AccountTests
{
    public class PointsTest : BaseTest
    {
        [Fact]
        [Description("Account points should be updated.")]
        public void Points_UpdateTest()
        {
            var character = CreateCharacter();

            character.SetPoints(200);
            Assert.Equal((uint)200, character.Points);

            character.SetPoints(50);
            Assert.Equal((uint)50, character.Points);
        }
    }
}
