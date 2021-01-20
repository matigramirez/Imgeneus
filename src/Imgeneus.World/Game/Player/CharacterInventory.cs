using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.World.Game.Blessing;
using Imgeneus.World.Game.NPCs;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
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

                // Remove mount if user was mounted while switching mount
                RemoveVehicle();

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
            Item item;

            InventoryItems.TryGetValue((0, 0), out item);
            Helmet = item;

            InventoryItems.TryGetValue((0, 1), out item);
            Armor = item;

            InventoryItems.TryGetValue((0, 2), out item);
            Pants = item;

            InventoryItems.TryGetValue((0, 3), out item);
            Gauntlet = item;

            InventoryItems.TryGetValue((0, 4), out item);
            Boots = item;

            InventoryItems.TryGetValue((0, 5), out item);
            Weapon = item;

            InventoryItems.TryGetValue((0, 6), out item);
            Shield = item;

            InventoryItems.TryGetValue((0, 7), out item);
            Cape = item;

            InventoryItems.TryGetValue((0, 8), out item);
            Amulet = item;

            InventoryItems.TryGetValue((0, 9), out item);
            Ring1 = item;

            InventoryItems.TryGetValue((0, 10), out item);
            Ring2 = item;

            InventoryItems.TryGetValue((0, 11), out item);
            Bracelet1 = item;

            InventoryItems.TryGetValue((0, 12), out item);
            Bracelet2 = item;

            InventoryItems.TryGetValue((0, 13), out item);
            Mount = item;

            InventoryItems.TryGetValue((0, 14), out item);
            Pet = item;

            InventoryItems.TryGetValue((0, 15), out item);
            Costume = item;

            InventoryItems.TryGetValue((0, 16), out item);
            Wings = item;
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

            if(item != Mount)
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

            if(item != Mount)
                MoveSpeed += item.MoveSpeed;
        }

        #endregion

        #region Inventory

        /// <summary>
        /// Collection of inventory items.
        /// </summary>
        public readonly ConcurrentDictionary<(byte Bag, byte Slot), Item> InventoryItems = new ConcurrentDictionary<(byte Bag, byte Slot), Item>();

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
                               Id, item.Type, item.TypeId, item.Count, item.Quality, item.Bag, item.Slot,
                               item.Gem1 is null ? 0 : item.Gem1.TypeId,
                               item.Gem2 is null ? 0 : item.Gem2.TypeId,
                               item.Gem3 is null ? 0 : item.Gem3.TypeId,
                               item.Gem4 is null ? 0 : item.Gem4.TypeId,
                               item.Gem5 is null ? 0 : item.Gem5.TypeId,
                               item.Gem6 is null ? 0 : item.Gem6.TypeId,
                               item.DyeColor.IsEnabled, item.DyeColor.Alpha, item.DyeColor.Saturation, item.DyeColor.R, item.DyeColor.G, item.DyeColor.B, item.CreationTime, item.ExpirationTime);

            InventoryItems.TryAdd((item.Bag, item.Slot), item);

            if (item.ExpirationTime != null)
            {
                item.OnExpiration += CharacterItem_OnExpiration;
            }

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

                return givenItem;
            }

            _taskQueue.Enqueue(ActionType.REMOVE_ITEM_FROM_INVENTORY,
                               Id, item.Bag, item.Slot);

            InventoryItems.TryRemove((item.Bag, item.Slot), out var removedItem);

            if (item.ExpirationTime != null)
            {
                item.StopExpirationTimer();
                item.OnExpiration -= CharacterItem_OnExpiration;
            }

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
            InventoryItems.TryRemove((currentBag, currentSlot), out var sourceItem);

            // Source item is always to remove.
            // TODO: rethink if this can be done without removing item from db.
            _taskQueue.Enqueue(ActionType.REMOVE_ITEM_FROM_INVENTORY,
                               Id, currentBag, currentSlot);

            // Check, if any other item is at destination slot.
            InventoryItems.TryRemove((destinationBag, destinationSlot), out var destinationItem);
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
                                   Id, destinationItem.Type, destinationItem.TypeId, destinationItem.Count, destinationItem.Quality, destinationItem.Bag, destinationItem.Slot,
                                   destinationItem.Gem1 is null ? 0 : destinationItem.Gem1.TypeId,
                                   destinationItem.Gem2 is null ? 0 : destinationItem.Gem2.TypeId,
                                   destinationItem.Gem3 is null ? 0 : destinationItem.Gem3.TypeId,
                                   destinationItem.Gem4 is null ? 0 : destinationItem.Gem4.TypeId,
                                   destinationItem.Gem5 is null ? 0 : destinationItem.Gem5.TypeId,
                                   destinationItem.Gem6 is null ? 0 : destinationItem.Gem6.TypeId,
                                   destinationItem.DyeColor.IsEnabled, destinationItem.DyeColor.Alpha, destinationItem.DyeColor.Saturation, destinationItem.DyeColor.R, destinationItem.DyeColor.G, destinationItem.DyeColor.B, destinationItem.CreationTime, destinationItem.ExpirationTime);
            }
            else
            {
                _taskQueue.Enqueue(ActionType.SAVE_ITEM_IN_INVENTORY,
                                   Id, sourceItem.Type, sourceItem.TypeId, sourceItem.Count, sourceItem.Quality, sourceItem.Bag, sourceItem.Slot,
                                   sourceItem.Gem1 is null ? 0 : sourceItem.Gem1.TypeId,
                                   sourceItem.Gem2 is null ? 0 : sourceItem.Gem2.TypeId,
                                   sourceItem.Gem3 is null ? 0 : sourceItem.Gem3.TypeId,
                                   sourceItem.Gem4 is null ? 0 : sourceItem.Gem4.TypeId,
                                   sourceItem.Gem5 is null ? 0 : sourceItem.Gem5.TypeId,
                                   sourceItem.Gem6 is null ? 0 : sourceItem.Gem6.TypeId,
                                   sourceItem.DyeColor.IsEnabled, sourceItem.DyeColor.Alpha, sourceItem.DyeColor.Saturation, sourceItem.DyeColor.R, sourceItem.DyeColor.G, sourceItem.DyeColor.B, sourceItem.CreationTime, sourceItem.ExpirationTime);
                _taskQueue.Enqueue(ActionType.SAVE_ITEM_IN_INVENTORY,
                                   Id, destinationItem.Type, destinationItem.TypeId, destinationItem.Count, destinationItem.Quality, destinationItem.Bag, destinationItem.Slot,
                                   destinationItem.Gem1 is null ? 0 : destinationItem.Gem1.TypeId,
                                   destinationItem.Gem2 is null ? 0 : destinationItem.Gem2.TypeId,
                                   destinationItem.Gem3 is null ? 0 : destinationItem.Gem3.TypeId,
                                   destinationItem.Gem4 is null ? 0 : destinationItem.Gem4.TypeId,
                                   destinationItem.Gem5 is null ? 0 : destinationItem.Gem5.TypeId,
                                   destinationItem.Gem6 is null ? 0 : destinationItem.Gem6.TypeId,
                                   destinationItem.DyeColor.IsEnabled, destinationItem.DyeColor.Alpha, destinationItem.DyeColor.Saturation, destinationItem.DyeColor.R, destinationItem.DyeColor.G, destinationItem.DyeColor.B, destinationItem.CreationTime, destinationItem.ExpirationTime);
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

            if (sourceItem.Type != 0 && sourceItem.TypeId != 0)
                InventoryItems.TryAdd((sourceItem.Bag, sourceItem.Slot), sourceItem);

            if (destinationItem.Type != 0 && destinationItem.TypeId != 0)
                InventoryItems.TryAdd((destinationItem.Bag, destinationItem.Slot), destinationItem);

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
        public void UseItem(byte bag, byte slot)
        {
            InventoryItems.TryGetValue((bag, slot), out var item);
            if (item is null)
            {
                _logger.LogWarning($"Character {Id} is trying to use item, that does not exist. Possible hack?");
                return;
            }

            if (!CanUseItem(item))
            {
                _packetsHelper.SendCanNotUseItem(Client, Id);
                return;
            }

            item.Count--;
            ApplyItemEffect(item);
            OnUsedItem?.Invoke(this, item);

            if (item.Count > 0)
            {
                _taskQueue.Enqueue(ActionType.UPDATE_ITEM_COUNT_IN_INVENTORY,
                                   Id, item.Bag, item.Slot, item.Count);
            }
            else
            {
                InventoryItems.TryRemove((item.Bag, item.Slot), out var removedItem);
                _taskQueue.Enqueue(ActionType.REMOVE_ITEM_FROM_INVENTORY,
                                   Id, item.Bag, item.Slot);
            }
        }

        /// <summary>
        /// Adds the effect of the item to the character.
        /// </summary>
        private void ApplyItemEffect(Item item)
        {
            switch (item.Special)
            {
                case SpecialEffect.Multipurpose:
                    HandleMultipurposeEffect(item);
                    break;

                case SpecialEffect.PercentHealingPotion:
                    UsePercentHealingPotion(item);
                    break;

                case SpecialEffect.HypnosisCure:
                    UseCureDebuffPotion(StateType.Sleep);
                    break;

                case SpecialEffect.StunCure:
                    UseCureDebuffPotion(StateType.Stun);
                    break;

                case SpecialEffect.SilenceCure:
                    UseCureDebuffPotion(StateType.Silence);
                    break;

                case SpecialEffect.DarknessCure:
                    UseCureDebuffPotion(StateType.Darkness);
                    break;

                case SpecialEffect.StopCure:
                    UseCureDebuffPotion(StateType.Immobilize);
                    break;

                case SpecialEffect.SlowCure:
                    UseCureDebuffPotion(StateType.Slow);
                    break;

                case SpecialEffect.VenomCure:
                    UseCureDebuffPotion(StateType.HPDamageOverTime);
                    break;

                case SpecialEffect.DiseaseCure:
                    UseCureDebuffPotion(StateType.SPDamageOverTime);
                    UseCureDebuffPotion(StateType.MPDamageOverTime);
                    break;

                case SpecialEffect.IllnessDelusionCure:
                    UseCureDebuffPotion(StateType.HPDamageOverTime);
                    UseCureDebuffPotion(StateType.SPDamageOverTime);
                    UseCureDebuffPotion(StateType.MPDamageOverTime);
                    break;

                case SpecialEffect.SleepStunStopSlowCure:
                    UseCureDebuffPotion(StateType.Sleep);
                    UseCureDebuffPotion(StateType.Stun);
                    UseCureDebuffPotion(StateType.Immobilize);
                    UseCureDebuffPotion(StateType.Slow);
                    break;

                case SpecialEffect.SilenceDarknessCure:
                    UseCureDebuffPotion(StateType.Silence);
                    UseCureDebuffPotion(StateType.Darkness);
                    break;

                case SpecialEffect.DullBadLuckCure:
                    UseCureDebuffPotion(StateType.DexDecrease);
                    UseCureDebuffPotion(StateType.Misfortunate);
                    break;

                case SpecialEffect.DoomFearCure:
                    UseCureDebuffPotion(StateType.MentalSmasher);
                    UseCureDebuffPotion(StateType.LowerAttackOrDefence);
                    break;

                case SpecialEffect.FullCure:
                    UseCureDebuffPotion(StateType.Sleep);
                    UseCureDebuffPotion(StateType.Stun);
                    UseCureDebuffPotion(StateType.Silence);
                    UseCureDebuffPotion(StateType.Darkness);
                    UseCureDebuffPotion(StateType.Immobilize);
                    UseCureDebuffPotion(StateType.Slow);
                    UseCureDebuffPotion(StateType.HPDamageOverTime);
                    UseCureDebuffPotion(StateType.SPDamageOverTime);
                    UseCureDebuffPotion(StateType.MPDamageOverTime);
                    UseCureDebuffPotion(StateType.DexDecrease);
                    UseCureDebuffPotion(StateType.Misfortunate);
                    UseCureDebuffPotion(StateType.MentalSmasher);
                    UseCureDebuffPotion(StateType.LowerAttackOrDefence);
                    break;

                case SpecialEffect.DisorderCure:
                    // ?
                    break;

                case SpecialEffect.StatResetStone:
                    ResetStats();
                    break;

                case SpecialEffect.GoddessBlessing:
                    UseBlessItem();
                    break;

                case SpecialEffect.AppearanceChange:
                case SpecialEffect.SexChange:
                    // Used in ChangeAppearance call.
                    break;

                case SpecialEffect.LinkingHammer:
                case SpecialEffect.PerfectLinkingHammer:
                case SpecialEffect.RecreationRune:
                case SpecialEffect.AbsoluteRecreationRune:
                    // Effect is added in linking manager.
                    break;

                case SpecialEffect.Dye:
                    // Effect is handled in dyeing manager.
                    break;

                case SpecialEffect.NameChange:
                    UseNameChangeStone();
                    break;

                default:
                    _logger.LogError($"Uninplemented item effect {item.Special}.");
                    break;
            }
        }

        /// <summary>
        /// Event, that is fired, when player changes appearance.
        /// </summary>
        public event Action<Character> OnAppearanceChanged;

        /// <summary>
        /// Changes player's appearance.
        /// </summary>
        /// <param name="face">new face</param>
        /// <param name="hair">new hair</param>
        /// <param name="size">new size</param>
        /// <param name="sex">new sex</param>
        private void ChangeAppearance(byte face, byte hair, byte size, byte sex)
        {
            Face = face;
            Hair = hair;
            Height = size;
            Gender = (Gender)sex;

            _taskQueue.Enqueue(ActionType.SAVE_APPEARANCE, Id, Face, Hair, Height, Gender);

            OnAppearanceChanged?.Invoke(this);
        }

        /// <summary>
        /// Initiates name change process
        /// </summary>
        public void UseNameChangeStone()
        {
            IsRename = true;

            _taskQueue.Enqueue(ActionType.SAVE_IS_RENAME, Id, true);
        }

        /// <summary>
        /// Uses potion, that restores hp,sp,mp.
        /// </summary>
        private void UseHealingPotion(Item potion)
        {
            if (potion.HP > 0 || potion.MP > 0 || potion.SP > 0)
                Recover(potion.HP, potion.MP, potion.SP);
        }

        /// <summary>
        /// Uses potion, that restores % of hp,sp,mp.
        /// </summary>
        private void UsePercentHealingPotion(Item potion)
        {
            var hp = Convert.ToInt32(MaxHP * potion.HP / 100);
            var mp = Convert.ToInt32(MaxMP * potion.MP / 100);
            var sp = Convert.ToInt32(MaxSP * potion.SP / 100);

            Recover(hp, mp, sp);
        }

        /// <summary>
        /// Cures characterfrom some debuff.
        /// </summary>
        private void UseCureDebuffPotion(StateType debuffType)
        {
            var debuffs = ActiveBuffs.Where(b => b.StateType == debuffType).ToList();
            foreach (var d in debuffs)
            {
                d.CancelBuff();
            }
        }

        /// <summary>
        /// GM item ,that increases bless amount of player's fraction.
        /// </summary>
        private void UseBlessItem()
        {
            if (Country == Fraction.Light)
                Bless.Instance.LightAmount += 500;
            else
                Bless.Instance.DarkAmount += 500;
        }

        /// <summary>
        /// Checks if item can be used. E.g. cooldown is over, required level is right etc.
        /// </summary>
        private bool CanUseItem(Item item)
        {
            // TODO: implement checks.

            switch (item.Special)
            {
                case SpecialEffect.RecreationRune:
                case SpecialEffect.AbsoluteRecreationRune:
                case SpecialEffect.RecreationRune_STR:
                case SpecialEffect.RecreationRune_DEX:
                case SpecialEffect.RecreationRune_REC:
                case SpecialEffect.RecreationRune_INT:
                case SpecialEffect.RecreationRune_WIS:
                case SpecialEffect.RecreationRune_LUC:
                    return _linkingManager.Item != null && _linkingManager.Item.IsComposable;
            }

            return true;
        }

        #endregion

        #region Buy/sell Item

        /// <summary>
        /// Buys item from npc store.
        /// </summary>
        /// <param name="product">product to buy</param>
        /// <param name="count">how many items player want to buy</param>
        public Item BuyItem(NpcProduct product, byte count)
        {
            _databasePreloader.Items.TryGetValue((product.Type, product.TypeId), out var dbItem);
            if (dbItem is null)
            {
                _logger.LogError($"Trying to buy not presented item(type={product.Type},typeId={product.TypeId}).");
                return null;
            }

            if (dbItem.Buy * count > Gold) // Not enough money.
            {
                _packetsHelper.SendBuyItemIssue(Client, 1);
                return null;
            }

            var freeSlot = FindFreeSlotInInventory();
            if (freeSlot.Slot == -1) // No free slot.
            {
                _packetsHelper.SendBuyItemIssue(Client, 2);
                return null;
            }

            ChangeGold((uint)(Gold - dbItem.Buy * count));
            var item = new Item(_databasePreloader, dbItem.Type, dbItem.TypeId);
            item.Count = count;

            return AddItemToInventory(item);
        }

        /// <summary>
        /// Sells item.
        /// </summary>
        /// <param name="item">item to sell</param>
        /// <param name="count">how many item player want to sell</param>
        public Item SellItem(Item item, byte count)
        {
            if (!InventoryItems.ContainsKey((item.Bag, item.Slot)))
            {
                return null;
            }

            item.TradeQuantity = count > item.Count ? item.Count : count;
            ChangeGold((uint)(Gold + item.Sell * item.TradeQuantity));
            return RemoveItemFromInventory(item);
        }

        #endregion

        #region Item Expiration

        public void CharacterItem_OnExpiration(Item item)
        {
            RemoveItemFromInventory(item);
            SendRemoveItemFromInventory(item, true);
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
                    var bagItems = InventoryItems.Where(itm => itm.Value.Bag == i).OrderBy(b => b.Value.Slot);
                    for (var j = 0; j < maxSlots; j++)
                    {
                        if (!bagItems.Any(b => b.Value.Slot == j))
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

        /// <summary>
        /// Defines the behavior of items with multipurpose effect
        /// </summary>
        private void HandleMultipurposeEffect(Item item)
        {
            // Spell group
            if (item.ReqIg > 0)
            {
                // Healing potion
                if (item.HP > 0 || item.MP > 0 || item.SP > 0)
                {
                    UseHealingPotion(item);
                }
            }

            // TODO: Handle items with skills, etc.
        }

        #endregion
    }
}
