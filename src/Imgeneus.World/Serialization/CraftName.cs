using BinarySerialization;
using Imgeneus.Network.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Imgeneus.World.Serialization
{
    public class CraftName : BaseSerializable
    {
        [FieldOrder(0), MinLength(20)]
        public byte[] Name;

        [FieldOrder(1)]
        public bool IsDisabled;

        public CraftName(string craftname)
        {
            Name = new byte[20];

            var chars = craftname.ToCharArray(0, Name.Length);
            for (var i = 0; i < chars.Length; i++)
                Name[i] = (byte)chars[i];

            IsDisabled = string.IsNullOrWhiteSpace(craftname);
        }
    }
}
