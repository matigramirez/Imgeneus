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
            character.Points = 5;

            character.TryAddPoints(195);
            Assert.Equal((uint)200, character.Points);

            character.TrySubtractPoints(50);
            Assert.Equal((uint)150, character.Points);
        }

        [Fact]
        [Description("Points shouldn't be modified if an update would surpass the boundaries.")]
        public void Points_BoundaryTest()
        {
            var character = CreateCharacter();
            character.Points = uint.MaxValue;

            var addResult = character.TryAddPoints(100);
            Assert.False(addResult);
            Assert.Equal(uint.MaxValue, character.Points);

            character.Points = 0;

            var subtractResult = character.TrySubtractPoints(100);
            Assert.False(subtractResult);
            Assert.Equal((uint)0, character.Points);
        }
    }
}
