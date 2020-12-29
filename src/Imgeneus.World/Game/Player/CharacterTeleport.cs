using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.World.Game.Duel;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.Portals;

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
        /// <param name="teleportedByAdmin">Indicates whether the teleport was issued by an admin or not</param>
        public void Teleport(ushort mapId, float x, float y, float z, bool teleportedByAdmin = false)
        {
            var prevMapId = MapId;
            MapId = mapId;
            PosX = x;
            PosY = y;
            PosZ = z;
            _taskQueue.Enqueue(ActionType.SAVE_MAP_ID, Id, MapId);
            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_MOVE, Id, x, y, z, Angle);

            Map.TeleportPlayer(Id, teleportedByAdmin);
            if (prevMapId != MapId)
            {
                if (IsDuelApproved)
                    FinishDuel(DuelCancelReason.TooFarAway);
                Map.UnloadPlayer(this);
            }
        }

        /// <summary>
        /// Teleports character with the help of the portal, if it's possible.
        /// </summary>
        public bool TryTeleport(byte portalIndex, out PortalTeleportNotAllowedReason reason)
        {
            if (_gameWorld.CanTeleport(this, portalIndex, out reason))
            {
                var portal = Map.Portals[portalIndex];
                Teleport(portal.MapId, portal.Destination_X, portal.Destination_Y, portal.Destination_Z);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
