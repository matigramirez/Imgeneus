using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Zone.Obelisks;

namespace Imgeneus.World.Serialization
{
    public class ObeliskUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public int Id;

        [FieldOrder(1)]
        public ObeliskCountry ObeliskCountry;

        [FieldOrder(2)]
        public float X;

        [FieldOrder(3)]
        public float Z;

        public ObeliskUnit(Obelisk obelisk)
        {
            Id = obelisk.Id;
            ObeliskCountry = obelisk.ObeliskCountry;
            X = obelisk.PosX;
            Z = obelisk.PosZ;
        }
    }
}