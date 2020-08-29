using System.Collections.Generic;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    /// <summary>
    /// Searches /config/maps folder, parse map json to map configurations.
    /// </summary>
    public interface IMapsLoader
    {
        IEnumerable<MapConfiguration> LoadMaps();
    }
}
