using Imgeneus.Core.Helpers;
using Imgeneus.World.Game.Zone.Obelisks;
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

        /// <summary>
        /// File, that contains definitions for all obelisks, that must be loaded.
        /// </summary>
        private const string ObeliskInitFile = "obelisk_init.json";

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

        #region Map configs

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

        #endregion

        #region Obelisks

        private MapObeliskConfigurations _obelisksConfig;

        public IEnumerable<ObeliskConfiguration> GetObelisks(ushort mapId)
        {
            if (_obelisksConfig == null)
            {
                var obelisksFile = Path.Combine(ConfigsFolder, ObeliskInitFile);
                if (!File.Exists(obelisksFile))
                {
                    _logger.LogError($"Obelisks init file is not found.");
                    return new List<ObeliskConfiguration>();
                }

                _obelisksConfig = ConfigurationHelper.Load<MapObeliskConfigurations>(obelisksFile);
            }

            var mapObelisks = _obelisksConfig.Maps.FirstOrDefault(m => m.MapId == mapId);
            if (mapObelisks == null)
                return new List<ObeliskConfiguration>();
            else
                return mapObelisks.Obelisks;
        }

        #endregion
    }
}
