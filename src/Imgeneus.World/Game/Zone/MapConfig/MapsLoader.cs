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

        public IEnumerable<MapConfiguration> LoadMaps()
        {
            if (!Directory.Exists(ConfigsFolder))
            {
                _logger.LogError("No map configuration is found.");
                return new List<MapConfiguration>();
            }

            var configs = new List<MapConfiguration>();
            var files = Directory.GetFiles(ConfigsFolder).Where(f => f.EndsWith(".json"));
            foreach (var f in files)
            {
                try
                {
                    var config = ConfigurationHelper.Load<MapConfiguration>(f);
                    config.Id = ushort.Parse(Path.GetFileNameWithoutExtension(f));
                    configs.Add(config);
                }
                catch
                {
                    _logger.LogError($"Failed to parse {f} map configuration.");
                }
            }

            return configs;
        }
    }
}
