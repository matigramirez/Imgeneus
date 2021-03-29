using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;

namespace Imgeneus.World.Game.Zone
{
    public class GuildMap : Map, IGuildMap
    {
        private readonly int _guildId;

        public int GuildId
        {
            get
            {
                return _guildId;
            }
        }

        public GuildMap(int guildId, ushort id, MapDefinition definition, MapConfiguration config, ILogger<Map> logger, IDatabasePreloader databasePreloader, IMobFactory mobFactory, INpcFactory npcFactory, IObeliskFactory obeliskFactory)
            : base(id, definition, config, logger, databasePreloader, mobFactory, npcFactory, obeliskFactory)
        {
            _guildId = guildId;
        }

    }
}
