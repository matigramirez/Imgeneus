using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class SerializedBankItem : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Slot { get; set; }

        [FieldOrder(1)]
        public byte Type { get; set; }

        [FieldOrder(2)]
        public byte TypeId { get; set; }

        [FieldOrder(3)]
        public byte Count { get; set; }

        public SerializedBankItem(BankItem bankItem)
        {
            Slot = bankItem.Slot;
            Type = bankItem.Type;
            TypeId = bankItem.TypeId;
            Count = bankItem.Count;
        }
    }
}
