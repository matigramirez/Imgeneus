using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Zone;
using Microsoft.Extensions.Logging;

namespace Imgeneus.World.Game.Monster
{
    public class MobFactory : IMobFactory
    {
        private readonly ILogger<Mob> _logger;
        private readonly IDatabasePreloader _preloader;

        public MobFactory(ILogger<Mob> logger, IDatabasePreloader preloader)
        {
            _logger = logger;
            _preloader = preloader;
        }

        /// <inheritdoc/>
        public Mob CreateMob(ushort mobId, bool shouldRebirth, MoveArea moveArea, Map map)
        {
            return new Mob(mobId, shouldRebirth, moveArea, map, _logger, _preloader);
        }
    }
}
