using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.World.Game.Duel;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Teleports character inside one map or to another map.
        /// </summary>
        /// <param name="mapId">map id, where to teleport</param>
        /// <param name="X">x coordinate, where to teleport</param>
        /// <param name="Y">y coordinate, where to teleport</param>
        /// <param name="Z">z coordinate, where to teleport</param>
        public void Teleport(ushort mapId, float x, float y, float z)
        {
            var prevMapId = MapId;
            MapId = mapId;
            PosX = x;
            PosY = y;
            PosZ = z;
            _taskQueue.Enqueue(ActionType.SAVE_MAP_ID, Id, MapId);
            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_MOVE, Id, x, y, z, Angle);

            Map.TeleportPlayer(Id);
            if (prevMapId != MapId)
            {
                if (IsDuelApproved)
                    FinishDuel(DuelCancelReason.TooFarAway);
                Map.UnloadPlayer(this);
            }
        }
    }
}
