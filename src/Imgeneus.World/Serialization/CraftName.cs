using BinarySerialization;
using System.ComponentModel.DataAnnotations;

namespace Imgeneus.World.Serialization
{
    public struct CraftName
    {
        [FieldOrder(0), MinLength(21)]
        public string Name;

        [FieldOrder(1)]
        public bool IsEnabled;

        public CraftName(string craftname)
        {
            Name = craftname;
            IsEnabled = string.IsNullOrWhiteSpace(craftname);
        }
    }
}
