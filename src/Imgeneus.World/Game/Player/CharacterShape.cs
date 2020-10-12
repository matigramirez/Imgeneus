using System;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Event, that is fired, when character changes shape.
        /// </summary>
        public event Action<Character> OnShapeChange;

        public CharacterShapeEnum Shape
        {
            get
            {
                if (IsStealth)
                    return CharacterShapeEnum.Stealth;

                if (IsOnVehicle)
                {
                    var value1 = Mount.Grow >= 2 ? 15 : 14;
                    var value2 = Mount.Range < 2 ? Mount.Range * 2 : Mount.Range + 7;
                    var mountType = value1 + value2;
                    return (CharacterShapeEnum)mountType;
                }

                return CharacterShapeEnum.None;
            }
        }
    }
}
