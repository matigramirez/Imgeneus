using Imgeneus.Database.Entities;
using System.Collections.Generic;

namespace Imgeneus.Database.Preload
{
    /// <summary>
    /// Database preloader loads game definitions from database, that not gonna change during game server lifetime.
    /// E.g. item definitions, mob definitions, buff/dbuff definitions etc.
    /// </summary>
    public interface IDatabasePreloader
    {
        /// <summary>
        /// Preloaded items.
        /// </summary>
        Dictionary<(byte Type, byte TypeId), DbItem> Items { get; }

        /// <summary>
        /// Preloads all needed info.
        /// </summary>
        void Preload();
    }
}
