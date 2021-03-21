using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterEnteredMap : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharId { get; }

        [FieldOrder(1)]
        public byte IsAdmin { get; }

        [FieldOrder(2)]
        public ushort Angle { get; }

        [FieldOrder(3)]
        public float PosX { get; }

        [FieldOrder(4)]
        public float PosY { get; }

        [FieldOrder(5)]
        public float PosZ { get; }

        [FieldOrder(6)]
        public int GuildId { get; }

        [FieldOrder(7)]
        public int Vehicle { get; }

        public CharacterEnteredMap(Character character)
        {
            CharId = character.Id;
            IsAdmin = character.IsAdmin ? (byte)2 : (byte)3; // Looks like it's bool value +2. Don't ask me, it just works.
            Angle = character.Angle;
            PosX = character.PosX;
            PosY = character.PosY;
            PosZ = character.PosZ;
            GuildId = character.GuildId ?? 0;
            Vehicle = 0; // TODO: implement vehicle.
        }
    }
}
