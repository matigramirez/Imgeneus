using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Guild;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Time;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;

namespace Imgeneus.World.Game.Zone
{
    public class MapFactory : IMapFactory
    {
        private readonly ILogger<Map> _logger;
        private readonly IDatabasePreloader _databasePreloader;
        private readonly IMobFactory _mobFactory;
        private readonly INpcFactory _npcFactory;
        private readonly IObeliskFactory _obeliskFactory;
        private readonly ITimeService _timeService;
        private readonly IGuildRankingManager _guildRankingManager;

        public MapFactory(ILogger<Map> logger, IDatabasePreloader databasePreloader, IMobFactory mobFactory, INpcFactory npcFactory, IObeliskFactory obeliskFactory, ITimeService timeService, IGuildRankingManager guildRankingManger)
        {
            _logger = logger;
            _databasePreloader = databasePreloader;
            _mobFactory = mobFactory;
            _npcFactory = npcFactory;
            _obeliskFactory = obeliskFactory;
            _timeService = timeService;
            _guildRankingManager = guildRankingManger;
        }

        /// <inheritdoc/>
        public IMap CreateMap(ushort id, MapDefinition definition, MapConfiguration config)
        {
            return new Map(id, definition, config, _logger, _databasePreloader, _mobFactory, _npcFactory, _obeliskFactory, _timeService);
        }

        /// <inheritdoc/>
        public IPartyMap CreatePartyMap(ushort id, MapDefinition definition, MapConfiguration config, IParty party)
        {
            return new PartyMap(party, id, definition, config, _logger, _databasePreloader, _mobFactory, _npcFactory, _obeliskFactory, _timeService);
        }

        /// <inheritdoc/>
        public IGuildMap CreateGuildMap(ushort id, MapDefinition definition, MapConfiguration config, int guildId)
        {
            if (definition.CreateType == CreateType.GRB)
                return new GRBMap(guildId, _guildRankingManager, id, definition, config, _logger, _databasePreloader, _mobFactory, _npcFactory, _obeliskFactory, _timeService);

            return new GuildMap(guildId, _guildRankingManager, id, definition, config, _logger, _databasePreloader, _mobFactory, _npcFactory, _obeliskFactory, _timeService);
        }
    }
}
