using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Zone.Portals
{
    public class Portal
    {
        private readonly PortalConfiguration _config;
        private readonly float X1;
        private readonly float X2;
        private readonly float Y1;
        private readonly float Y2;
        private readonly float Z1;
        private readonly float Z2;

        public Portal(PortalConfiguration config)
        {
            _config = config;
            X1 = _config.X - 5;
            X2 = _config.X + 5;
            Y1 = _config.Y - 5;
            Y2 = _config.Y + 5;
            Z1 = _config.Z - 5;
            Z2 = _config.Z + 5;
        }

        /// <summary>
        /// Checks if character of some fraction can use this portal.
        /// </summary>
        public bool IsSameFraction(Fraction fraction)
        {
            if (fraction == Fraction.Light && _config.Faction == 1)
                return true;

            if (fraction == Fraction.Dark && _config.Faction == 2)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if character can use this portal by level.
        /// </summary>
        public bool IsRightLevel(ushort level)
        {
            return level >= _config.MinLvl && level <= _config.MaxLvl;
        }

        /// <summary>
        /// Checks if character is in portal zone.
        /// </summary>
        /// <param name="x">player x coordinate</param>
        /// <param name="y">player y coordinate</param>
        /// <param name="z">player z coordinate</param>
        public bool IsInPortalZone(float x, float y, float z)
        {
            return x >= X1 && x <= X2 &&
                   y >= Y1 && y <= Y2 &&
                   z >= Z1 && z <= Z2;
        }

        public ushort MapId => _config.Destination.MapId;

        public float Destination_X => _config.Destination.X;

        public float Destination_Y => _config.Destination.Y;

        public float Destination_Z => _config.Destination.Z;
    }

}
