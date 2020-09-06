using Imgeneus.Core.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class MapsLoader : IMapsLoader
    {
        private readonly ILogger<MapsLoader> _logger;

        public MapsLoader(ILogger<MapsLoader> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Relative path to configs folder.
        /// </summary>
        private const string ConfigsFolder = "config/maps/";

        /// <summary>
        /// File, that contains definitions for all maps, that must be loaded.
        /// </summary>
        private const string InitFile = "map_init.json";

        public MapDefinitions LoadMapDefinitions()
        {
            var initFilePath = Path.Combine(ConfigsFolder, InitFile);
            if (!File.Exists(initFilePath))
            {
                _logger.LogError("No map definition is found.");
                return new MapDefinitions();
            }

            return ConfigurationHelper.Load<MapDefinitions>(initFilePath); ;
        }

        private readonly Dictionary<ushort, MapConfiguration> _loadedConfigs = new Dictionary<ushort, MapConfiguration>();

        public MapConfiguration LoadMapConfiguration(ushort mapId)
        {
            if (_loadedConfigs.ContainsKey(mapId))
            {
                return _loadedConfigs[mapId];
            }
            else
            {
                var mapFile = Path.Combine(ConfigsFolder, $"{mapId}.json");
                if (!File.Exists(mapFile))
                {
                    _logger.LogError($"Configuration for map {mapId} is not found.");
                    return new MapConfiguration();
                }

                var config = ConfigurationHelper.Load<MapConfiguration>(mapFile);
                _loadedConfigs.Add(mapId, config);
                return config;
            }
        }
    }
}
