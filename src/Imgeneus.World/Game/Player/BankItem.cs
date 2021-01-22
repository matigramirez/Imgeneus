using System.Transactions;
using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Player
{
    public class BankItem
    {
        public byte Slot { get; set; }

        public byte Type { get; set; }

        public byte TypeId { get; set; }

        public byte Count { get; set; }

        public BankItem(byte slot, byte type, byte typeId, byte count) : this(type, typeId, count)
        {
            Slot = slot;
        }

        public BankItem(byte type, byte typeId, byte count)
        {
            Type = type;
            TypeId = typeId;
            Count = count;
        }

        public BankItem(DbBankItem dbBankItem) : this (dbBankItem.Slot, dbBankItem.Type, dbBankItem.TypeId, dbBankItem.Count)
        {
        }
    }
}
