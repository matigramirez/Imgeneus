using System;

namespace Imgeneus.World.Game.Time
{
    /// <inheritdoc/>
    public class TimeService : ITimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
