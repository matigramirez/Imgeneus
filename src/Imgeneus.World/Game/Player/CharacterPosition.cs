using Imgeneus.Core.Extensions;
using Imgeneus.Database.Constants;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.World.Game.Duel;
using System;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {

        /// <summary>
        /// Event, that is fired, when character changes his/her position.
        /// </summary>
        public event Action<Character> OnPositionChanged;

        /// <summary>
        /// Updates player position. Saves change to database if needed.
        /// </summary>
        /// <param name="x">new x</param>
        /// <param name="y">new y</param>
        /// <param name="z">new z</param>
        /// <param name="saveChangesToDB">set it to true, if this change should be saved to database</param>
        private void UpdatePosition(float x, float y, float z, ushort angle, bool saveChangesToDB)
        {
            if (IsTeleporting)
            {
                return;
            }

            if (ActiveBuffs.Any(b => b.StateType == StateType.Immobilize || b.StateType == StateType.Sleep || b.StateType == StateType.Stun))
            {
                OnPositionChanged?.Invoke(this);
                return;
            }

            PosX = x;
            PosY = y;
            PosZ = z;
            Angle = angle;

            if (IsDuelApproved && MathExtensions.Distance(PosX, DuelX, PosZ, DuelZ) >= 45)
            {
                FinishDuel(DuelCancelReason.TooFarAway);
            }

            //_logger.LogDebug($"Character {Id} moved to x={PosX} y={PosY} z={PosZ} angle={Angle}");

            if (saveChangesToDB)
            {
                _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_MOVE,
                                   Id, x, y, z, angle);
            }

            OnPositionChanged?.Invoke(this);
        }
    }
}
