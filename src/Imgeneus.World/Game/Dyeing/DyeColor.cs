namespace Imgeneus.World.Game.Dyeing
{
    public struct DyeColor
    {
        public bool IsEnabled;
        public byte Alpha;
        public byte Saturation;
        public byte R;
        public byte G;
        public byte B;

        public DyeColor(byte alpha, byte saturation, byte r, byte g, byte b, bool isEnabled = true)
        {
            Alpha = alpha;
            Saturation = saturation;
            R = r;
            G = g;
            B = b;
            IsEnabled = isEnabled;
        }
    }
}
