using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterMove : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharId { get; }

        [FieldOrder(1)]
        public byte Motion { get; }

        [FieldOrder(2)]
        public ushort Angle { get; }

        [FieldOrder(3)]
        public float PosX { get; }

        [FieldOrder(4)]
        public float PosY { get; }

        [FieldOrder(5)]
        public float PosZ { get; }

        public CharacterMove(Character character)
        {
            CharId = character.Id;
            Motion = character.MoveMotion;
            Angle = character.Angle;
            PosX = character.PosX;
            PosY = character.PosY;
            PosZ = character.PosZ;
        }
    }
}
