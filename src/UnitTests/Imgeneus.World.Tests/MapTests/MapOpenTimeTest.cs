using Imgeneus.World.Game.Zone.MapConfig;
using System;
using System.ComponentModel;
using Xunit;

namespace Imgeneus.World.Tests.MapTests
{
    public class MapOpenTimeTest : BaseTest
    {
        [Theory]
        [Description("Map should be open at provided time range.")]
        [InlineData("0 17 * * Sunday", "0 18 * * Sunday", 2021, 3, 28, 17, 30, 00, true)] // Every sunday at 17:00-18:00, now is 28 of March 2021, sunday 17:30
        [InlineData("0 17 * * Sunday", "0 18 * * Sunday", 2021, 3, 27, 17, 30, 00, false)] // Every sunday at 17:00-18:00, now is 27 of March 2021, saturday 17:30
        [InlineData("0 17 * * Sunday", "0 18 * * Sunday", 2021, 3, 28, 18, 00, 00, false)] // Every sunday at 17:00-18:00, now is 28 of March 2021, sunday 18:00
        [InlineData("0 17 * * Sunday", "0 18 * * Sunday", 2021, 3, 28, 17, 59, 00, true)] // Every sunday at 17:00-18:00, now is 28 of March 2021, sunday 17:59
        public void MapIsOpen(string openTime, string closeTime, int year, int month, int day, int hour, int minute, int second, bool isOpen)
        {
            var def = new MapDefinition()
            {
                OpenTime = openTime,
                CloseTime = closeTime
            };

            var now = new DateTime(year, month, day, hour, minute, second);
            Assert.Equal(isOpen, def.IsOpen(now));
        }
    }
}
