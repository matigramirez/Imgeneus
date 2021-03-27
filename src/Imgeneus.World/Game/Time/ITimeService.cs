using System;

namespace Imgeneus.World.Game.Time
{
    /// <summary>
    /// UTC time provider.
    /// </summary>
    public interface ITimeService
    {
        public DateTime UtcNow { get; }
    }
}
