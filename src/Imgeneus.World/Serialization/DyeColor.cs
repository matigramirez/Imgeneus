using BinarySerialization;

namespace Imgeneus.World.Serialization
{
    public struct DyeColor
    {
        [FieldOrder(0)]
        public byte Color1;

        [FieldOrder(1)]
        public byte Color2;

        [FieldOrder(2)]
        public byte Color3;

        [FieldOrder(3)]
        public byte Color4;
    }
}
