using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Zone
{
    public class Npc
    {
        private readonly DbNpc _dbNpc;

        public Npc(DbNpc dbNpc)
        {
            _dbNpc = dbNpc;
        }
    }
}
