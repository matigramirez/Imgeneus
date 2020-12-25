using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;

namespace Imgeneus.World.Game.Zone
{
    /// <summary>
    /// Instance map, only for parties.
    /// </summary>
    public class PartyMap : Map
    {
        public PartyMap(ushort id, MapDefinition definition, MapConfiguration config, ILogger<Map> logger, IDatabasePreloader databasePreloader, IMobFactory mobFactory, INpcFactory npcFactory, IObeliskFactory obeliskFactory)
            : base(id, definition, config, logger, databasePreloader, mobFactory, npcFactory, obeliskFactory)
        {
        }
    }
}
