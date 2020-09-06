using Imgeneus.World.Game.Zone.Obelisks;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    /// <summary>
    /// Searches /config/maps folder, parse map json to map configurations.
    /// </summary>
    public interface IMapsLoader
    {
        /// <summary>
        /// Loads map.init.json, parses it.
        /// </summary>
        MapDefinitions LoadMapDefinitions();

        /// <summary>
        /// Loads map configuration.
        /// </summary>
        MapConfiguration LoadMapConfiguration(ushort mapId);

        /// <summary>
        /// Loads obelisks configuration, based on map id.
        /// </summary>
        IEnumerable<ObeliskConfiguration> GetObelisks(ushort mapId);
    }
}
