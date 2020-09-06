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
    }
}
