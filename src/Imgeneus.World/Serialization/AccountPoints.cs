using BinarySerialization;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class AccountPoints : BaseSerializable
    {
        [FieldOrder(0)]
        public uint Points { get; }

        [FieldOrder(1)]
        public byte Unknown { get; } = 0;

        public AccountPoints(uint points)
        {
            Points = points;
        }
    }
}
