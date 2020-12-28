using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Imgeneus.World.Game.Monster;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Sends to client character start-up information.
        /// </summary>
        private void SendCharacterInfo()
        {
            SendDetails();
            SendAdditionalStats();
            SendCurrentHitpoints();
            SendInventoryItems(); // TODO: game.exe crashes, when number of items >= 80. Investigate why?
            SendLearnedSkills();
            SendOpenQuests();
            SendFinishedQuests();
            SendActiveBuffs();
            SendMoveAndAttackSpeed();
            SendFriends();
            SendBlessAmount();
        }

        private void HandleGMGetItemPacket(GMGetItemPacket gMGetItemPacket)
        {
            if (!IsAdmin)
                return;

            var itemCount = gMGetItemPacket.Count;

            while (itemCount > 0)
            {
                var newItem = new Item(_databasePreloader, gMGetItemPacket.Type, gMGetItemPacket.TypeId,
                    itemCount);

                var item = AddItemToInventory(newItem);
                if (item != null)
                {
                    _packetsHelper.SendAddItem(Client, item);
                    _packetsHelper.SendGmCommandSuccess(Client);
                }
                else
                    _packetsHelper.SendGmCommandError(Client, PacketType.GM_COMMAND_GET_ITEM);

                itemCount -= newItem.Count;
            }
        }

        private void HandlePlayerInTarget(PlayerInTargetPacket packet)
        {
            Target = Map.GetPlayer(packet.TargetId);
        }

        private void HandleMobInTarget(MobInTargetPacket packet)
        {
            Target = Map.GetMob(CellId, packet.TargetId);
        }

        private void HandleMotion(MotionPacket packet)
        {
            if (packet.Motion == Motion.None || packet.Motion == Motion.Sit)
            {
                Motion = packet.Motion;
            }

            _logger.LogDebug($"Character {Id} sends motion {packet.Motion}");
            OnMotion?.Invoke(this, packet.Motion);
        }

        private void HandleMove(MoveCharacterPacket packet)
        {
            UpdatePosition(packet.X, packet.Y, packet.Z, packet.Angle, packet.MovementType == MovementType.Stopped);
        }

        private void HandleMoveItem(MoveItemInInventoryPacket moveItemPacket)
        {
            var items = MoveItem(moveItemPacket.CurrentBag, moveItemPacket.CurrentSlot, moveItemPacket.DestinationBag, moveItemPacket.DestinationSlot);
            _packetsHelper.SendMoveItemInInventory(Client, items.sourceItem, items.destinationItem);
        }

        private void HandleLearnNewSkill(LearnNewSkillPacket learnNewSkillsPacket)
        {
            LearnNewSkill(learnNewSkillsPacket.SkillId, learnNewSkillsPacket.SkillLevel);
        }

        private void HandleSkillBarPacket(SkillBarPacket skillBarPacket)
        {
            _taskQueue.Enqueue(ActionType.SAVE_QUICK_BAR, Id, skillBarPacket.QuickItems);
        }

        private void HandleAutoAttackOnMob(int targetId)
        {
            var target = Map.GetMob(CellId, targetId);
            Attack(255, target);
        }

        private void HandleAutoAttackOnPlayer(int targetId)
        {
            var target = Map.GetPlayer(targetId);
            Attack(255, target);
        }

        private void HandleUseSkillOnMob(byte number, int targetId)
        {
            var target = Map.GetMob(CellId, targetId);
            Attack(number, target);
        }

        private void HandleUseSkillOnPlayer(byte number, int targetId)
        {
            IKillable target = Map.GetPlayer(targetId);
            Attack(number, target);
        }

        private void HandleGetCharacterBuffs(int targetId)
        {
            var target = Map.GetPlayer(targetId);
            if (target != null)
                _packetsHelper.SendCurrentBuffs(Client, target);
        }

        private void HandleGetMobBuffs(int targetId)
        {
            var target = Map.GetMob(CellId, targetId);
            if (target != null)
                _packetsHelper.SendCurrentBuffs(Client, target);
        }

        private void HandleGetMobState(int targetId)
        {
            var target = Map.GetMob(CellId, targetId);
            if (target != null)
            {
                _packetsHelper.SendMobPosition(Client, target);
                _packetsHelper.SendMobState(Client, target);
            }
            else
                _logger.LogWarning($"Coudn't find mob {targetId} state.");
        }

        private void HandleCharacterShape(int characterId)
        {
            var character = _gameWorld.Players[characterId];
            if (character is null)
            {
                _logger.LogWarning($"Trying to get player {characterId}, that is not presented in game world.");
                return;
            }

            _packetsHelper.SendCharacterShape(Client, character);
        }

        private void HandleChangeAppearance(ChangeAppearancePacket changeAppearancePacket)
        {
            InventoryItems.TryGetValue((changeAppearancePacket.Bag, changeAppearancePacket.Slot), out var item);
            if (item is null || (item.Special != SpecialEffect.AppearanceChange && item.Special != SpecialEffect.SexChange))
                return;

            UseItem(changeAppearancePacket.Bag, changeAppearancePacket.Slot);
            ChangeAppearance(changeAppearancePacket.Face, changeAppearancePacket.Hair, changeAppearancePacket.Size, changeAppearancePacket.Sex);
        }

        private void HandleFriendRequest(string characterName)
        {
            var character = _gameWorld.Players.FirstOrDefault(p => p.Value.Name == characterName).Value;
            if (character is null || character.Country != this.Country)
                return;

            character.RequestFriendship(this);
        }

        private void HandleSearchParty()
        {
            if (Party != null)
                return;

            Map.RegisterSearchForParty(this);
            _packetsHelper.SendRegisteredInPartySearch(Client, true);

            var searchers = Map.PartySearchers.Where(s => s.Country == Country && s != this);
            if (searchers.Any())
                _packetsHelper.SendPartySearchList(Client, searchers.Take(30));
        }

        private void HandleSummonPlayer(string playerName)
        {
            if (!IsAdmin)
                return;

            var player = _gameWorld.Players.Values.FirstOrDefault(p => p.Name == playerName);

            if (player is null)
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_SUMMON_PLAYER);
            else
            {
                player.Teleport(MapId, PosX, PosY, PosZ);

                _packetsHelper.SendGmCommandSuccess(Client);
                _packetsHelper.SendGmSummon(player.Client, player);
            }
        }

        private void HandleFindPlayerPacket(string playerName)
        {
            if (!IsAdmin)
                return;

            var player = _gameWorld.Players.Values.FirstOrDefault(p => p.Name == playerName);
            if (player is null)
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_FIND_PLAYER);
            else
            {
                _packetsHelper.SendGmCommandSuccess(Client);
                _packetsHelper.SendCharacterPosition(Client, player);
            }
        }

        private void HandleTeleportToPlayer(string playerName)
        {
            if (!IsAdmin)
                return;

            var player = _gameWorld.Players.Values.FirstOrDefault(p => p.Name == playerName);
            if (player is null)
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_TO_PLAYER);
            else
            {
                Teleport(player.MapId, player.PosX, player.PosY, player.PosZ);

                _packetsHelper.SendGmCommandSuccess(Client);
                _packetsHelper.SendGmTeleportToPlayer(Client, player);
            }
        }

        private void HandleDyeSelectItem(byte dyeItemBag, byte dyeItemSlot, byte targetItemBag, byte targetItemSlot)
        {
            InventoryItems.TryGetValue((dyeItemBag, dyeItemSlot), out var dyeItem);
            if (dyeItem is null || dyeItem.Special != SpecialEffect.Dye)
            {
                _packetsHelper.SendSelectDyeItem(Client, false);
                return;
            }

            InventoryItems.TryGetValue((targetItemBag, targetItemSlot), out var item);
            if (item is null)
            {
                _packetsHelper.SendSelectDyeItem(Client, false);
                return;
            }

            if (dyeItem.TypeId == 55 && item.IsWeapon)
            {
                _dyeingManager.DyeingItem = item;
                _packetsHelper.SendSelectDyeItem(Client, true);
            }
            else if (dyeItem.TypeId == 56 && item.IsArmor)
            {
                _dyeingManager.DyeingItem = item;
                _packetsHelper.SendSelectDyeItem(Client, true);
            }
            else if (dyeItem.TypeId == 57 && item.IsMount)
            {
                _dyeingManager.DyeingItem = item;
                _packetsHelper.SendSelectDyeItem(Client, true);
            }
            else if (dyeItem.TypeId == 58 && item.IsPet)
            {
                _dyeingManager.DyeingItem = item;
                _packetsHelper.SendSelectDyeItem(Client, true);
            }
            else if (dyeItem.TypeId == 59 && item.IsCostume)
            {
                _dyeingManager.DyeingItem = item;
                _packetsHelper.SendSelectDyeItem(Client, true);
            }
            else
            {
                _packetsHelper.SendSelectDyeItem(Client, false);
                return;
            }
        }

        private void HandleDyeReroll()
        {
            _dyeingManager.Reroll();
            _packetsHelper.SendDyeColors(Client, _dyeingManager.AvailableColors);
        }

        private void HandleDyeConfirm(byte dyeItemBag, byte dyeItemSlot, byte targetItemBag, byte targetItemSlot)
        {
            if (_dyeingManager.AvailableColors.Count == 0)
                _dyeingManager.Reroll();

            var color = _dyeingManager.AvailableColors[new Random().Next(0, 5)];

            InventoryItems.TryGetValue((dyeItemBag, dyeItemSlot), out var dyeItem);
            if (dyeItem is null || dyeItem.Special != SpecialEffect.Dye || _dyeingManager.DyeingItem is null)
            {
                _packetsHelper.SendDyeConfirm(Client, false, color);
                return;
            }

            InventoryItems.TryGetValue((targetItemBag, targetItemSlot), out var item);
            if (item is null)
            {
                _packetsHelper.SendDyeConfirm(Client, false, color);
                return;
            }

            bool success = (dyeItem.TypeId == 55 && item.IsWeapon) ||
                           (dyeItem.TypeId == 56 && item.IsArmor) ||
                           (dyeItem.TypeId == 57 && item.IsMount) ||
                           (dyeItem.TypeId == 58 && item.IsPet) ||
                           (dyeItem.TypeId == 59 && item.IsCostume);

            if (success)
            {
                _dyeingManager.DyeingItem.DyeColor = color;
                _dyeingManager.DyeingItem = null;
                _packetsHelper.SendDyeConfirm(Client, success, color);
            }
            else
            {
                _packetsHelper.SendDyeConfirm(Client, false, color);
                return;
            }

            if (success)
            {
                _taskQueue.Enqueue(ActionType.CREATE_DYE_COLOR, Id, item.Bag, item.Slot, color.Alpha, color.Saturation, color.R, color.G, color.B);

                if (item.Bag == 0)
                    OnEquipmentChanged?.Invoke(this, item, item.Slot);

                UseItem(dyeItem.Bag, dyeItem.Slot);
            }
        }

        private void HandleAbsoluteCompose(byte runeBag, byte runeSlot, byte itemBag, byte itemSlot)
        {
            InventoryItems.TryGetValue((runeBag, runeSlot), out var rune);
            InventoryItems.TryGetValue((itemBag, itemSlot), out var item);

            if (rune is null || item is null || rune.Special != SpecialEffect.AbsoluteRecreationRune || !item.IsComposable)
            {
                _packetsHelper.SendComposition(Client, true, item);
                return;
            }

            var itemClone = item.Clone();
            _linkingManager.Item = itemClone;
            _linkingManager.Compose(rune);

            _packetsHelper.SendAbsoluteComposition(Client, false, itemClone.GetCraftName());

            // TODO: I'm not sure how absolute composite works and what to do next.

            _linkingManager.Item = null;
        }

        private void HandleItemComposePacket(byte runeBag, byte runeSlot, byte itemBag, byte itemSlot)
        {
            InventoryItems.TryGetValue((runeBag, runeSlot), out var rune);
            InventoryItems.TryGetValue((itemBag, itemSlot), out var item);

            if (rune is null || item is null ||
                   (rune.Special != SpecialEffect.RecreationRune &&
                    rune.Special != SpecialEffect.RecreationRune_STR &&
                    rune.Special != SpecialEffect.RecreationRune_DEX &&
                    rune.Special != SpecialEffect.RecreationRune_REC &&
                    rune.Special != SpecialEffect.RecreationRune_INT &&
                    rune.Special != SpecialEffect.RecreationRune_WIS &&
                    rune.Special != SpecialEffect.RecreationRune_LUC) ||
                !item.IsComposable)
            {
                _packetsHelper.SendComposition(Client, true, item);
                return;
            }

            if (item.Bag == 0)
            {
                ExtraStr -= item.ComposedStr;
                ExtraDex -= item.ComposedDex;
                ExtraRec -= item.ComposedRec;
                ExtraInt -= item.ComposedInt;
                ExtraWis -= item.ComposedWis;
                ExtraLuc -= item.ComposedLuc;
                ExtraHP -= item.ComposedHP;
                ExtraMP -= item.ComposedMP;
                ExtraSP -= item.ComposedSP;
            }

            _linkingManager.Item = item;
            _linkingManager.Compose(rune);

            _packetsHelper.SendComposition(Client, false, item);

            if (item.Bag == 0)
            {
                ExtraStr += item.ComposedStr;
                ExtraDex += item.ComposedDex;
                ExtraRec += item.ComposedRec;
                ExtraInt += item.ComposedInt;
                ExtraWis += item.ComposedWis;
                ExtraLuc += item.ComposedLuc;
                ExtraHP += item.ComposedHP;
                ExtraMP += item.ComposedMP;
                ExtraSP += item.ComposedSP;

                SendAdditionalStats();
            }

            _taskQueue.Enqueue(ActionType.UPDATE_CRAFT_NAME, Id, item.Bag, item.Slot, item.GetCraftName());
            UseItem(rune.Bag, rune.Slot);

            _linkingManager.Item = null;
        }

        private void HandleUpdateStats(ushort str, ushort dex, ushort rec, ushort intl, ushort wis, ushort luc)
        {
            var fullStat = str + dex + rec + intl + wis + luc;
            if (fullStat > StatPoint || fullStat > ushort.MaxValue)
                return;

            Strength += str;
            Dexterity += dex;
            Reaction += rec;
            Intelligence += intl;
            Wisdom += wis;
            Luck += luc;
            StatPoint -= (ushort)fullStat;

            _taskQueue.Enqueue(ActionType.UPDATE_STATS, Id, Strength, Dexterity, Reaction, Intelligence, Wisdom, Luck, StatPoint);

            _packetsHelper.SendStatsUpdate(Client, str, dex, rec, intl, wis, luc);
            SendAdditionalStats();
        }

        private void HandleGMSetAttributePacket(GMSetAttributePacket gmSetAttributePacket)
        {
            var (attribute, attributeValue, player) = gmSetAttributePacket;

            void SendCommandError()
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.CHARACTER_ATTRIBUTE_SET);
            }

            void SetAttributeAndSendCommandSuccess()
            {
                SendAttribute(attribute);
                _packetsHelper.SendGmCommandSuccess(Client);
            }

            // TODO: This should get player from player dictionary when implemented
            var targetPlayer = _gameWorld.Players.Values.FirstOrDefault(p => p.Name == player);

            if (targetPlayer == null)
            {
                SendCommandError();
                return;
            }

            switch (attribute)
            {
                case CharacterAttributeEnum.Grow:
                    if (targetPlayer.TrySetMode((Mode)attributeValue))
                        SetAttributeAndSendCommandSuccess();
                    else
                        SendCommandError();
                    break;

                case CharacterAttributeEnum.Level:
                    if (targetPlayer.TryChangeLevel((ushort)attributeValue, true))
                        SetAttributeAndSendCommandSuccess();
                    else
                        SendCommandError();
                    break;

                case CharacterAttributeEnum.Money:
                    targetPlayer.ChangeGold(attributeValue);
                    SetAttributeAndSendCommandSuccess();
                    break;

                case CharacterAttributeEnum.StatPoint:
                    targetPlayer.SetStatPoint((ushort)attributeValue);
                    SetAttributeAndSendCommandSuccess();
                    break;

                case CharacterAttributeEnum.SkillPoint:
                    targetPlayer.SetSkillPoint((ushort)attributeValue);
                    SetAttributeAndSendCommandSuccess();
                    break;

                case CharacterAttributeEnum.Strength:
                case CharacterAttributeEnum.Dexterity:
                case CharacterAttributeEnum.Reaction:
                case CharacterAttributeEnum.Intelligence:
                case CharacterAttributeEnum.Luck:
                case CharacterAttributeEnum.Wisdom:
                    targetPlayer.SetStat(attribute, (ushort)attributeValue);
                    SetAttributeAndSendCommandSuccess();
                    break;

                case CharacterAttributeEnum.Hg:
                case CharacterAttributeEnum.Vg:
                case CharacterAttributeEnum.Cg:
                case CharacterAttributeEnum.Og:
                case CharacterAttributeEnum.Ig:
                    SendCommandError();
                    return;

                case CharacterAttributeEnum.Exp:
                    if (targetPlayer.TryChangeExperience((ushort)attributeValue, true))
                        SetAttributeAndSendCommandSuccess();
                    else
                        SendCommandError();
                    break;

                case CharacterAttributeEnum.Kills:
                    targetPlayer.SetKills((ushort)attributeValue);
                    SetAttributeAndSendCommandSuccess();
                    break;

                case CharacterAttributeEnum.Deaths:
                    targetPlayer.SetDeaths((ushort)attributeValue);
                    SetAttributeAndSendCommandSuccess();
                    break;

                default:
                    _packetsHelper.SendGmCommandError(Client, PacketType.CHARACTER_ATTRIBUTE_SET);
                    return;
            }
        }

        private void HandleEnterPortalPacket(CharacterEnteredPortalPacket enterPortalPacket)
        {
            var success = TryTeleport(enterPortalPacket.PortalId, out var reason);

            if (!success)
                SendPortalTeleportNotAllowed(reason);
        }

        private void HandleGMCreateMob(GMCreateMobPacket gmCreateMobPacket)
        {
            if (!_databasePreloader.Mobs.ContainsKey(gmCreateMobPacket.MobId))
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_CREATE_MOB);
                return;
            }

            for (int i = 0; i < gmCreateMobPacket.NumberOfMobs; i++)
            {
                // TODO: calculate move area.
                var moveArea = new MoveArea(PosX > 10 ? PosX - 10 : 1, PosX + 10, PosY > 10 ? PosY - 10 : PosY, PosY + 10, PosZ > 10 ? PosZ - 10 : 1, PosZ + 10);

                var mob = _mobFactory.CreateMob(gmCreateMobPacket.MobId, false, moveArea, Map);

                Map.AddMob(mob);
            }

            _packetsHelper.SendGmCommandSuccess(Client);
        }

        private void HandleGMCurePlayerPacket(GMCurePlayerPacket gmCurePlayerPacket)
        {
            var target = _gameWorld.Players.FirstOrDefault(p => p.Value.Name == gmCurePlayerPacket.Name).Value;

            if (target == null)
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_CURE_PLAYER);
                return;
            }

            target?.FullRecover();

            _packetsHelper.SendGmCommandSuccess(Client);
        }

        private void HandleGMWarningPlayer(GMWarningPacket gmWarningPacket)
        {
            var target = _gameWorld.Players.FirstOrDefault(p => p.Value.Name == gmWarningPacket.Name).Value;

            if (target == null)
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_WARNING_PLAYER);
                return;
            }

            target?.SendWarning(gmWarningPacket.Message);

            _packetsHelper.SendGmCommandSuccess(Client);
        }

        private void HandleGMTeleportToMap(GMTeleportMapPacket gmTeleportMapPacket)
        {
            if (!_gameWorld.Maps.ContainsKey(gmTeleportMapPacket.MapId))
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_MAP);
                return;
            }

            _packetsHelper.SendGmCommandSuccess(Client);

            // TODO: Use fixed xz coordinates per map?
            Teleport(gmTeleportMapPacket.MapId, 100, PosY, 100, true);
        }

        private void HandleGMTeleportToMapCoordinates(GMTeleportMapCoordinatesPacket gmTeleportMapCoordinatesPacket)
        {
            var (newPosX, newPosZ, mapId) = gmTeleportMapCoordinatesPacket;

            if (!_gameWorld.Maps.ContainsKey(mapId))
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_MAP_COORDINATES);
                return;
            }

            _packetsHelper.SendGmCommandSuccess(Client);

            Teleport(mapId, newPosX, PosY, newPosZ, true);
        }

        private void HandleGMTeleportPlayer(GMTeleportPlayerPacket gmTeleportPlayerPacket)
        {
            var (name, newPosX, newPosZ, mapId) = gmTeleportPlayerPacket;

            var target = _gameWorld.Players.FirstOrDefault(p => p.Value.Name == name).Value;

            if (target == null)
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_PLAYER);
                return;
            }

            if (!_gameWorld.Maps.ContainsKey(mapId))
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_MAP_COORDINATES);
                return;
            }

            _packetsHelper.SendGmCommandSuccess(Client);

            target?.Teleport(mapId, newPosX, PosY, newPosZ, true);
        }
    }
}
