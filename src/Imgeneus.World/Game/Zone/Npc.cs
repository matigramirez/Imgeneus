using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Zone
{
    public class Npc : IMapMember
    {
        private readonly DbNpc _dbNpc;

        public Npc(DbNpc dbNpc, float x, float y, float z)
        {
            _dbNpc = dbNpc;
            PosX = x;
            PosY = y;
            PosZ = z;
        }

        public int Id { get; set; }

        /// <inheritdoc />
        public float PosX { get; set; }

        /// <inheritdoc />
        public float PosY { get; set; }

        /// <inheritdoc />
        public float PosZ { get; set; }

        /// <inheritdoc />
        public ushort Angle { get; set; }

        /// <summary>
        /// Type of NPC.
        /// </summary>
        public byte Type { get => _dbNpc.Type; }

        /// <summary>
        /// Type id of NPC.
        /// </summary>
        public ushort TypeId { get => _dbNpc.TypeId; }
    }
}
