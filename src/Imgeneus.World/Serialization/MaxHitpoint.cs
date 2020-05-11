using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class MaxHitpoint : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId;

        [FieldOrder(1)]
        public HitpointType HitpointType;

        [FieldOrder(2)]
        public int Value;

        public MaxHitpoint(Character character, HitpointType hitpointType)
        {
            CharacterId = character.Id;
            HitpointType = hitpointType;

            switch (hitpointType)
            {
                case HitpointType.HP:
                    Value = character.MaxHP;
                    break;

                case HitpointType.MP:
                    Value = character.MaxMP;
                    break;

                case HitpointType.SP:
                    Value = character.MaxSP;
                    break;
            }
        }
    }
}
