using BinarySerialization;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class UseSPMP : BaseSerializable
    {
        [FieldOrder(0)]
        public uint SP;

        [FieldOrder(1)]
        public uint MP;

        public UseSPMP(ushort sp, ushort mp)
        {
            // Pay attention, that in previous episodes character was using ushort for sp and mp.
            // For ep 8 it's already int.
            // If someone would like to use this emulator for earlier episodes, he should keep it in mind.
            SP = sp;
            MP = mp;
        }
    }
}
