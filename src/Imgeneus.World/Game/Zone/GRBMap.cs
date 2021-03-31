using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Guild;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Time;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Imgeneus.World.Game.Zone
{
    public class GRBMap : GuildMap, IGRBMap
    {
        public GRBMap(int guildId, IGuildRankingManager guildRankingManager, ushort id, MapDefinition definition, MapConfiguration config, ILogger<Map> logger, IDatabasePreloader databasePreloader, IMobFactory mobFactory, INpcFactory npcFactory, IObeliskFactory obeliskFactory, ITimeService timeService)
            : base(guildId, guildRankingManager, id, definition, config, logger, databasePreloader, mobFactory, npcFactory, obeliskFactory, timeService)
        {
            _guildRankingManager.OnPointsChanged += GuildRankingManager_OnPointsChanged;
        }

        private void GuildRankingManager_OnPointsChanged(int guildId, int points)
        {
            var topGuild = _guildRankingManager.GetTopGuilds(1).FirstOrDefault();
            var myGuild = _guildRankingManager.GetGuild(GuildId);

            foreach (var player in Players.Values.ToList())
                player.SendGBRPoints(myGuild.Points, topGuild.Points, topGuild.Id);
        }

        protected override async void Mob_OnDead(IKillable sender, IKiller killer)
        {
            var mob = sender as Mob;
            await _guildRankingManager.AddPoints(GuildId, mob.GuildPoints);

            base.Mob_OnDead(sender, killer);
        }
    }
}
