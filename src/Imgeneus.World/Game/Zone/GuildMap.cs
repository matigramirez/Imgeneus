using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Guild;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Time;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;

namespace Imgeneus.World.Game.Zone
{
    public class GuildMap : Map, IGuildMap
    {
        protected readonly int _guildId;
        protected readonly IGuildRankingManager _guildRankingManager;

        public int GuildId
        {
            get
            {
                return _guildId;
            }
        }

        public GuildMap(int guildId, IGuildRankingManager guildRankingManager, ushort id, MapDefinition definition, MapConfiguration config, ILogger<Map> logger, IDatabasePreloader databasePreloader, IMobFactory mobFactory, INpcFactory npcFactory, IObeliskFactory obeliskFactory, ITimeService timeService)
            : base(id, definition, config, logger, databasePreloader, mobFactory, npcFactory, obeliskFactory, timeService)
        {
            _guildId = guildId;
            _guildRankingManager = guildRankingManager;
        }

    }
}
