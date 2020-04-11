using Imgeneus.Database.Entities;
using Microsoft.Extensions.Logging;

namespace Imgeneus.World.Game.Monster
{
    public class Mob : ITargetable
    {
        private readonly ILogger<Mob> _logger;

        public Mob(uint globalId, ILogger<Mob> logger)
        {
            _logger = logger;
            GlobalId = globalId;
        }

        /// <inheritdoc />
        public uint GlobalId { get; private set; }

        /// <inheritdoc />
        public int CurrentHP { get; set; }

        /// <summary>
        /// Mob id from database.
        /// </summary>
        public ushort MobId;

        /// <summary>
        /// Current x position.
        /// </summary>
        public float PosX;

        /// <summary>
        /// Current y position.
        /// </summary>
        public float PosY;

        /// <summary>
        /// Current z position.
        /// </summary>
        public float PosZ;

        public static Mob FromDbMob(uint globalId, DbMob mob, ILogger<Mob> logger)
        {
            return new Mob(globalId, logger)
            {
                MobId = mob.Id,
                CurrentHP = mob.HP
            };
        }
    }
}
