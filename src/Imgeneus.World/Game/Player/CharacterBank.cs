using System;
using System.Collections.Concurrent;
using Imgeneus.DatabaseBackgroundService.Handlers;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Collection of bank items.
        /// </summary>
        public readonly ConcurrentDictionary<byte, BankItem> BankItems = new ConcurrentDictionary<byte, BankItem>();

        /// <summary>
        /// Adds an item to a player's bank
        /// </summary>
        public BankItem AddBankItem(byte type, byte typeId, byte count)
        {
            var freeSlot = FindFreeBankSlot();

            // No available slots
            if (freeSlot == -1)
            {
                return null;
            }

            var bankItem = new BankItem((byte)freeSlot, type, typeId, count);

            BankItems.TryAdd(bankItem.Slot, bankItem);

            _taskQueue.Enqueue(ActionType.SAVE_BANK_ITEM, Client.UserID, DateTime.UtcNow, null, false, bankItem.Type, bankItem.TypeId, bankItem.Count, bankItem.Slot, false);

            return bankItem;
        }

        /// <summary>
        /// Attempts to take an item from the bank and put it into the player's inventory.
        /// </summary>
        /// <param name="slot">Bank slot where the item is.</param>
        /// <param name="claimedItem">Generated item.</param>
        public bool TryClaimBankItem(byte slot, out Item claimedItem)
        {
            claimedItem = null;

            if (!BankItems.TryGetValue(slot, out var bankItem))
                return false;

            var item = AddItemToInventory(new Item(_databasePreloader, bankItem));

            if (item == null)
                return false;

            claimedItem = item;

            BankItems.TryRemove(slot, out _);

            SendBankItemClaim(bankItem.Slot, item);

            if (item.IsExpirable)
                SendItemExpiration(item);

            _taskQueue.Enqueue(ActionType.CLAIM_BANK_ITEM, Client.UserID, bankItem.Slot, DateTime.UtcNow, true);

            return true;
        }

        #region Helpers

        private int FindFreeBankSlot()
        {
            var maxSlot = 239;
            int freeSlot = -1;

            if (BankItems.Count > 0)
            {
                // Try to find any free slot.
                for (byte i = 0; i <= maxSlot; i++)
                {
                    if (!BankItems.TryGetValue(i, out _))
                    {
                        freeSlot = i;
                        break;
                    }
                }
            }
            else
            {
                freeSlot = 0;
            }

            return freeSlot;
        }

        #endregion
    }
}
