using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Player
{
    public class BankItem
    {
        public byte Slot { get; set; }

        public byte Type { get; set; }

        public byte TypeId { get; set; }

        public byte Count { get; set; }

        public BankItem(DbBankItem dbBankItem)
        {
            Slot = dbBankItem.Slot;
            Type = dbBankItem.Type;
            TypeId = dbBankItem.TypeId;
            Count = dbBankItem.Count;
        }
    }
}
