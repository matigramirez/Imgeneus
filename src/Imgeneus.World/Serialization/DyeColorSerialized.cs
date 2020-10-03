using BinarySerialization;

namespace Imgeneus.World.Serialization
{
    public struct DyeColorSerialized
    {
        [FieldOrder(0)]
        public byte Saturation;

        [FieldOrder(1)]
        public byte R;

        [FieldOrder(2)]
        public byte G;

        [FieldOrder(3)]
        public byte B;

        public DyeColorSerialized(byte s, byte r, byte g, byte b)
        {
            Saturation = s;
            R = r;
            G = g;
            B = b;
        }
    }
}
