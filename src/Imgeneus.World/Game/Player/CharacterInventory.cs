using Imgeneus.DatabaseBackgroundService.Handlers;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    /// <summary>
    /// Handles only changes in inventory.
    /// </summary>
    public partial class Character
    {
        #region Equipment

        private Item Helmet;
        private Item Armor;
        private Item Pants;
        private Item Gauntlet;
        private Item Boots;
        private Item Weapon;
        private Item Shield;
        private Item Cape;
        private Item Amulet;
        private Item Ring1;
        private Item Ring2;
        private Item Bracelet1;
        private Item Bracelet2;
        private Item Mount;
        private Item Pet;
        private Item Costume;
        private Item Wings;

        /// <summary>
        /// Initializes equipped items.
        /// </summary>
        private void InitEquipment()
        {
            Helmet = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 0);
            Armor = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 1);
            Pants = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 2);
            Gauntlet = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 3);
            Boots = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 4);
            Weapon = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 5);
            Shield = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 6);
            Cape = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 7);
            Amulet = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 8);
            Ring1 = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 9);
            Ring2 = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 10);
            Bracelet1 = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 11);
            Bracelet2 = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 12);
            Mount = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 13);
            Pet = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 14);
            Costume = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 15);
            Wings = InventoryItems.FirstOrDefault(itm => itm.Bag == 0 && itm.Slot == 16);
        }

        #endregion

        #region Inventory

        /// <summary>
        /// Event, that is fired, when some equipment of character changes.
        /// </summary>
        public event Action<Character, Item> OnEquipmentChanged;

        /// <summary>
        /// Collection of inventory items.
        /// </summary>
        public ObservableRangeCollection<Item> InventoryItems { get; private set; } = new ObservableRangeCollection<Item>();

        /// <summary>
        /// Adds item to player's inventory.
        /// </summary>
        /// <param name="itemType">item type</param>
        /// <param name="itemTypeId">item type id</param>
        /// <param name="count">how many items</param>
        public Item AddItemToInventory(Item item)
        {
            // Find free space.
            var free = FindFreeSlotInInventory();

            // Calculated bag slot can not be 0, because 0 means worn item. Newerly created item can not be worn.
            if (free.Bag == 0 || free.Slot == -1)
            {
                return null;
            }

            item.Bag = free.Bag;
            item.Slot = (byte)free.Slot;

            _taskQueue.Enqueue(ActionType.SAVE_ITEM_IN_INVENTORY,
                               Id, item.Type, item.TypeId, item.Count, item.Bag, item.Slot);

            InventoryItems.Add(item);
            _logger.LogDebug($"Character {Id} got item {item.Type} {item.TypeId}");
            return item;
        }

        /// <summary>
        /// Removes item from inventory
        /// </summary>
        /// <param name="item">item, that we want to remove</param>
        public Item RemoveItemFromInventory(Item item)
        {
            // If we are giving consumable item.
            if (item.TradeQuantity < item.Count && item.TradeQuantity != 0)
            {
                var givenItem = item.Clone();
                givenItem.Count = item.TradeQuantity;

                item.Count -= item.TradeQuantity;
                item.TradeQuantity = 0;

                _taskQueue.Enqueue(ActionType.UPDATE_ITEM_COUNT_IN_INVENTORY,
                                   Id, item.Bag, item.Slot, item.Count);

                _packetsHelper.SendRemoveItem(Client, item, false);

                return givenItem;
            }

            _taskQueue.Enqueue(ActionType.REMOVE_ITEM_FROM_INVENTORY,
                               Id, item.Bag, item.Slot);

            InventoryItems.Remove(item);
            _logger.LogDebug($"Character {Id} lost item {item.Type} {item.TypeId}");
            return item;
        }

        /// <summary>
        /// Moves item inside inventory.
        /// </summary>
        /// <param name="currentBag">current bag id</param>
        /// <param name="currentSlot">current slot id</param>
        /// <param name="destinationBag">bag id, where item should be moved</param>
        /// <param name="destinationSlot">slot id, where item should be moved</param>
        /// <returns></returns>
        public (Item sourceItem, Item destinationItem) MoveItem(byte currentBag, byte currentSlot, byte destinationBag, byte destinationSlot)
        {
            bool shouldDeleteSourceItemFromDB = false;

            // Find source item.
            var sourceItem = InventoryItems.First(ci => ci.Bag == currentBag && ci.Slot == currentSlot);

            // Source item is always to remove.
            _taskQueue.Enqueue(ActionType.REMOVE_ITEM_FROM_INVENTORY,
                               Id, currentBag, currentSlot);

            // Check, if any other item is at destination slot.
            var destinationItem = InventoryItems.FirstOrDefault(ci => ci.Bag == destinationBag && ci.Slot == destinationSlot);
            if (destinationItem is null)
            {
                // No item at destination place.
                // Since there is no destination item we will use source item as destination.
                // The only change, that we need to do is to set new bag and slot.
                destinationItem = sourceItem;
                destinationItem.Bag = destinationBag;
                destinationItem.Slot = destinationSlot;
                shouldDeleteSourceItemFromDB = true;
                sourceItem = new Item() { Bag = currentBag, Slot = currentSlot }; // empty item.
            }
            else
            {
                // There is some item at destination place.
                if (sourceItem.Type == destinationItem.Type && sourceItem.TypeId == destinationItem.TypeId && destinationItem.IsJoinable)
                {
                    // Increase destination item count, if they are joinable.
                    destinationItem.Count += sourceItem.Count;
                    shouldDeleteSourceItemFromDB = true;
                    InventoryItems.Remove(sourceItem);
                    sourceItem = new Item() { Bag = currentBag, Slot = currentSlot }; // empty item.
                }
                else
                {
                    // Swap them.
                    destinationItem.Bag = currentBag;
                    destinationItem.Slot = currentSlot;

                    sourceItem.Bag = destinationBag;
                    sourceItem.Slot = destinationSlot;
                    shouldDeleteSourceItemFromDB = false;
                }

                _taskQueue.Enqueue(ActionType.REMOVE_ITEM_FROM_INVENTORY,
                                   Id, destinationBag, destinationSlot);
            }

            // Add new items to database.
            if (shouldDeleteSourceItemFromDB)
            {
                _taskQueue.Enqueue(ActionType.SAVE_ITEM_IN_INVENTORY,
                                   Id, destinationItem.Type, destinationItem.TypeId, destinationItem.Count, destinationItem.Bag, destinationItem.Slot);
            }
            else
            {
                _taskQueue.Enqueue(ActionType.SAVE_ITEM_IN_INVENTORY,
                                   Id, sourceItem.Type, sourceItem.TypeId, sourceItem.Count, sourceItem.Bag, sourceItem.Slot);
                _taskQueue.Enqueue(ActionType.SAVE_ITEM_IN_INVENTORY,
                                   Id, destinationItem.Type, destinationItem.TypeId, destinationItem.Count, destinationItem.Bag, destinationItem.Slot);
            }

            if (sourceItem.Bag == 0 || destinationItem.Bag == 0)
            {
                var equipmentItem = sourceItem.Bag == 0 ? sourceItem : destinationItem;
                OnEquipmentChanged?.Invoke(this, equipmentItem);

                _logger.LogDebug($"Character {Id} changed equipment on slot {equipmentItem.Slot}");
            }

            return (sourceItem, destinationItem);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Tries to find free slot in inventory.
        /// </summary>
        /// <returns>tuple of bag and slot; slot is -1 if there is no free slot</returns>
        private (byte Bag, int Slot) FindFreeSlotInInventory()
        {
            byte bagSlot = 0;
            int freeSlot = -1;

            if (InventoryItems.Count > 0)
            {
                var maxBag = 5;
                var maxSlots = 24;

                // Go though all bags and try to find any free slot.
                // Start with 1, because 0 is worn items.
                for (byte i = 1; i <= maxBag; i++)
                {
                    var bagItems = InventoryItems.Where(itm => itm.Bag == i).OrderBy(b => b.Slot);
                    for (var j = 0; j < maxSlots; j++)
                    {
                        if (!bagItems.Any(b => b.Slot == j))
                        {
                            freeSlot = j;
                            break;
                        }
                    }

                    if (freeSlot != -1)
                    {
                        bagSlot = i;
                        break;
                    }
                }
            }
            else
            {
                bagSlot = 1; // Start with 1, because 0 is worn items.
                freeSlot = 0;
            }

            return (bagSlot, freeSlot);
        }


        #endregion
    }
}
