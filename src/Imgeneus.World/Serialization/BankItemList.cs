using System.Collections.Generic;
using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class BankItemList : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<SerializedBankItem> Items { get; } = new List<SerializedBankItem>();

        public BankItemList(IEnumerable<BankItem> bankItems)
        {
            foreach (var bankItem in bankItems)
                Items.Add(new SerializedBankItem(bankItem));
        }
    }
}
