using Imgeneus.DatabaseBackgroundService.Handlers;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    /// <summary>
    /// Handles only changes in inventory.
    /// </summary>
    public partial class Character
    {
        #region Equipment

        /// <summary>
        /// Event, that is fired, when some equipment of character changes.
        /// </summary>
        public event Action<Character, Item, byte> OnEquipmentChanged;

        /// <summary>
        /// Worm helmet. Set it through <see cref="Helmet"/>.
        /// </summary>
        private Item _helmet;
        public Item Helmet
        {
            get => _helmet;
            set
            {

                TakeOffItem(_helmet);
                _helmet = value;
                TakeOnItem(_helmet);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _helmet, 0);
                _logger.LogDebug($"Character {Id} changed equipment on slot 0");
            }
        }

        /// <summary>
        /// Worm armor. Set it through <see cref="Armor"/>.
        /// </summary>
        private Item _armor;
        public Item Armor
        {
            get => _armor;
            set
            {
                TakeOffItem(_armor);
                _armor = value;
                TakeOnItem(_armor);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _armor, 1);
                _logger.LogDebug($"Character {Id} changed equipment on slot 1");
            }
        }

        /// <summary>
        /// Worm pants. Set it through <see cref="Pants"/>.
        /// </summary>
        private Item _pants;
        public Item Pants
        {
            get => _pants;
            set
            {
                TakeOffItem(_pants);
                _pants = value;
                TakeOnItem(_pants);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _pants, 2);
                _logger.LogDebug($"Character {Id} changed equipment on slot 2");
            }
        }

        /// <summary>
        /// Worm gauntlet. Set it through <see cref="Gauntlet"/>.
        /// </summary>
        private Item _gauntlet;
        public Item Gauntlet
        {
            get => _gauntlet;
            set
            {
                TakeOffItem(_gauntlet);
                _gauntlet = value;
                TakeOnItem(_gauntlet);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _gauntlet, 3);
                _logger.LogDebug($"Character {Id} changed equipment on slot 3");
            }
        }

        /// <summary>
        /// Worm boots. Set it through <see cref="Boots"/>.
        /// </summary>
        private Item _boots;
        public Item Boots
        {
            get => _boots;
            set
            {
                TakeOffItem(_boots);
                _boots = value;
                TakeOnItem(_boots);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _boots, 4);
                _logger.LogDebug($"Character {Id} changed equipment on slot 4");
            }
        }

        /// <summary>
        /// Worm weapon. Set it through <see cref="Weapon"/>.
        /// </summary>
        private Item _weapon;
        public Item Weapon
        {
            get => _weapon;
            set
            {
                TakeOffItem(_weapon);
                _weapon = value;

                if (_weapon != null)
                    SetWeaponSpeed(_weapon.AttackSpeed);
                else
                    SetWeaponSpeed(0);

                TakeOnItem(_weapon);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _weapon, 5);
                _logger.LogDebug($"Character {Id} changed equipment on slot 5");
            }
        }

        /// <summary>
        /// Worm shield. Set it through <see cref="Shield"/>.
        /// </summary>
        private Item _shield;
        public Item Shield
        {
            get => _shield;
            set
            {
                TakeOffItem(_shield);
                _shield = value;
                TakeOnItem(_shield);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _shield, 6);
                _logger.LogDebug($"Character {Id} changed equipment on slot 6");
            }
        }

        /// <summary>
        /// Worm cape. Set it through <see cref="Cape"/>.
        /// </summary>
        private Item _cape;
        public Item Cape
        {
            get => _cape;
            set
            {
                TakeOffItem(_cape);
                _cape = value;
                TakeOnItem(_cape);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _cape, 7);
                _logger.LogDebug($"Character {Id} changed equipment on slot 7");
            }
        }

        /// <summary>
        /// Worm amulet. Set it through <see cref="Amulet"/>.
        /// </summary>
        private Item _amulet;
        public Item Amulet
        {
            get => _amulet;
            set
            {
                TakeOffItem(_amulet);
                _amulet = value;
                TakeOnItem(_amulet);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _amulet, 8);
                _logger.LogDebug($"Character {Id} changed equipment on slot 8");
            }
        }

        /// <summary>
        /// Worm ring1. Set it through <see cref="Ring1"/>.
        /// </summary>
        private Item _ring1;
        public Item Ring1
        {
            get => _ring1;
            set
            {
                TakeOffItem(_ring1);
                _ring1 = value;
                TakeOnItem(_ring1);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _ring1, 9);
                _logger.LogDebug($"Character {Id} changed equipment on slot 9");
            }
        }

        /// <summary>
        /// Worm ring2. Set it through <see cref="Ring2"/>.
        /// </summary>
        private Item _ring2;
        public Item Ring2
        {
            get => _ring2;
            set
            {
                TakeOffItem(_ring2);
                _ring2 = value;
                TakeOnItem(_ring2);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _ring2, 10);
                _logger.LogDebug($"Character {Id} changed equipment on slot 10");
            }
        }

        /// <summary>
        /// Worm bracelet1. Set it through <see cref="Bracelet1"/>.
        /// </summary>
        private Item _bracelet1;
        public Item Bracelet1
        {
            get => _bracelet1;
            set
            {
                TakeOffItem(_bracelet1);
                _bracelet1 = value;
                TakeOnItem(_bracelet1);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _bracelet1, 11);
                _logger.LogDebug($"Character {Id} changed equipment on slot 11");
            }
        }

        /// <summary>
        /// Worm bracelet2. Set it through <see cref="Bracelet2"/>.
        /// </summary>
        private Item _bracelet2;
        public Item Bracelet2
        {
            get => _bracelet2;
            set
            {
                TakeOffItem(_bracelet2);
                _bracelet2 = value;
                TakeOnItem(_bracelet2);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _bracelet2, 12);
                _logger.LogDebug($"Character {Id} changed equipment on slot 12");
            }
        }

        /// <summary>
        /// Worm mount. Set it through <see cref="Mount"/>.
        /// </summary>
        private Item _mount;
        public Item Mount
        {
            get => _mount;
            set
            {
                TakeOffItem(_mount);
                _mount = value;
                TakeOnItem(_mount);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _mount, 13);
                _logger.LogDebug($"Character {Id} changed equipment on slot 13");
            }
        }

        /// <summary>
        /// Worm pet. Set it through <see cref="Pet"/>.
        /// </summary>
        private Item _pet;
        public Item Pet
        {
            get => _pet;
            set
            {
                TakeOffItem(_pet);
                _pet = value;
                TakeOnItem(_pet);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _pet, 14);
                _logger.LogDebug($"Character {Id} changed equipment on slot 14");
            }
        }

        /// <summary>
        /// Worm costume. Set it through <see cref="Costume"/>.
        /// </summary>
        private Item _costume;
        public Item Costume
        {
            get => _costume;
            set
            {
                TakeOffItem(_costume);
                _costume = value;
                TakeOnItem(_costume);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _costume, 15);
                _logger.LogDebug($"Character {Id} changed equipment on slot 15");
            }
        }

        /// <summary>
        /// Worm wings. Set it through <see cref="Wings"/>.
        /// </summary>
        private Item _wings;
        public Item Wings
        {
            get => _wings;
            set
            {
                TakeOffItem(_wings);
                _wings = value;
                TakeOnItem(_wings);

                if (Client != null)
                    SendAdditionalStats();

                OnEquipmentChanged?.Invoke(this, _wings, 16);
                _logger.LogDebug($"Character {Id} changed equipment on slot 16");
            }
        }

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

        /// <summary>
        /// Method, that is called, when character takes off some equipped item.
        /// </summary>
        private void TakeOffItem(Item item)
        {
            if (item is null)
                return;

            ExtraStr -= item.Str;
            ExtraDex -= item.Dex;
            ExtraRec -= item.Rec;
            ExtraInt -= item.Int;
            ExtraLuc -= item.Luc;
            ExtraWis -= item.Wis;
            ExtraHP -= item.HP;
            ExtraSP -= item.SP;
            ExtraMP -= item.MP;
            ExtraDefense -= item.Defense;
            ExtraResistance -= item.Resistance;

            if (item != Weapon && item != Mount)
                SetAttackSpeedModifier(-1 * item.AttackSpeed);
            MoveSpeed -= item.MoveSpeed;
        }

        /// <summary>
        /// Method, that is called, when character takes on some item.
        /// </summary>
        private void TakeOnItem(Item item)
        {
            if (item is null)
                return;

            ExtraStr += item.Str;
            ExtraDex += item.Dex;
            ExtraRec += item.Rec;
            ExtraInt += item.Int;
            ExtraLuc += item.Luc;
            ExtraWis += item.Wis;
            ExtraHP += item.HP;
            ExtraSP += item.SP;
            ExtraMP += item.MP;
            ExtraDefense += item.Defense;
            ExtraResistance += item.Resistance;

            if (item != Weapon && item != Mount)
                SetAttackSpeedModifier(item.AttackSpeed);
            MoveSpeed += item.MoveSpeed;
        }

        #endregion

        #region Inventory

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

                sourceItem = new Item(_databasePreloader, 0, 0) { Bag = currentBag, Slot = currentSlot }; // empty item.
            }
            else
            {
                // There is some item at destination place.
                if (sourceItem.Type == destinationItem.Type &&
                    sourceItem.TypeId == destinationItem.TypeId &&
                    destinationItem.IsJoinable &&
                    destinationItem.Count + sourceItem.Count <= destinationItem.MaxCount)
                {
                    // Increase destination item count, if they are joinable.
                    destinationItem.Count += sourceItem.Count;
                    shouldDeleteSourceItemFromDB = true;
                    InventoryItems.Remove(sourceItem);

                    sourceItem = new Item(_databasePreloader, 0, 0) { Bag = currentBag, Slot = currentSlot }; // empty item.
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

            // Update equipment if needed.
            if (currentBag == 0 && destinationBag != 0)
            {
                switch (currentSlot)
                {
                    case 0:
                        Helmet = null;
                        break;
                    case 1:
                        Armor = null;
                        break;
                    case 2:
                        Pants = null;
                        break;
                    case 3:
                        Gauntlet = null;
                        break;
                    case 4:
                        Boots = null;
                        break;
                    case 5:
                        Weapon = null;
                        break;
                    case 6:
                        Shield = null;
                        break;
                    case 7:
                        Cape = null;
                        break;
                    case 8:
                        Amulet = null;
                        break;
                    case 9:
                        Ring1 = null;
                        break;
                    case 10:
                        Ring2 = null;
                        break;
                    case 11:
                        Bracelet1 = null;
                        break;
                    case 12:
                        Bracelet2 = null;
                        break;
                    case 13:
                        Mount = null;
                        break;
                    case 14:
                        Pet = null;
                        break;
                    case 15:
                        Costume = null;
                        break;
                    case 16:
                        Wings = null;
                        break;
                }
            }

            if (destinationBag == 0)
            {
                var item = sourceItem.Bag == destinationBag && sourceItem.Slot == destinationSlot ? sourceItem : destinationItem;
                switch (item.Slot)
                {
                    case 0:
                        Helmet = item;
                        break;
                    case 1:
                        Armor = item;
                        break;
                    case 2:
                        Pants = item;
                        break;
                    case 3:
                        Gauntlet = item;
                        break;
                    case 4:
                        Boots = item;
                        break;
                    case 5:
                        Weapon = item;
                        break;
                    case 6:
                        Shield = item;
                        break;
                    case 7:
                        Cape = item;
                        break;
                    case 8:
                        Amulet = item;
                        break;
                    case 9:
                        Ring1 = item;
                        break;
                    case 10:
                        Ring2 = item;
                        break;
                    case 11:
                        Bracelet1 = item;
                        break;
                    case 12:
                        Bracelet2 = item;
                        break;
                    case 13:
                        Mount = item;
                        break;
                    case 14:
                        Pet = item;
                        break;
                    case 15:
                        Costume = item;
                        break;
                    case 16:
                        Wings = item;
                        break;
                }
            }

            return (sourceItem, destinationItem);
        }

        #endregion

        #region Use Item

        /// <summary>
        /// Event, that is fired, when player uses any item from inventory.
        /// </summary>
        public event Action<Character, Item> OnUsedItem;

        /// <summary>
        /// Use item from inventory.
        /// </summary>
        /// <param name="bag">bag, where item is situated</param>
        /// <param name="slot">slot, where item is situated</param>
        private void UseItem(byte bag, byte slot)
        {
            var item = InventoryItems.FirstOrDefault(itm => itm.Bag == bag && itm.Slot == slot);
            if (item is null)
                return;

            if (!CanUseItem(item))
                return;

            item.Count--;

            // TODO: implement all useable items.

            CurrentHP += item.HP;
            CurrentMP += item.MP;
            CurrentSP += item.SP;

            OnUsedItem?.Invoke(this, item);

            if (item.Count > 0)
            {
                _taskQueue.Enqueue(ActionType.UPDATE_ITEM_COUNT_IN_INVENTORY,
                                   Id, item.Bag, item.Slot, item.Count);
            }
            else
            {
                InventoryItems.Remove(item);
                _taskQueue.Enqueue(ActionType.REMOVE_ITEM_FROM_INVENTORY,
                                   Id, item.Bag, item.Slot);
            }
        }

        /// <summary>
        /// Checks if item can be used. E.g. cooldown is over, required level is right etc.
        /// </summary>
        private bool CanUseItem(Item item)
        {
            // TODO: implement checks.

            return true;
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
