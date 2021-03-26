using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Zone;
using System.Collections.Generic;
using Imgeneus.World.Game.Zone.Portals;
using Imgeneus.World.Game.Guild;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
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
                    SendAddItemToInventory(item);
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
                // Teleport to party instance map.
                if (player.Map is IPartyMap)
                {
                    SetParty(null);
                    var mapId = _gameWorld.PartyMaps.FirstOrDefault(m => m.Value == player.Map).Key;
                    PreviousPartyId = mapId;
                }

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

        private void HandleAutoStatsSettings(byte str, byte dex, byte rec, byte @int, byte wis, byte luc)
        {
            if (str + dex + rec + @int + wis + luc > _characterConfig.GetLevelStatSkillPoints(Mode).StatPoint)
            {
                return;
            }

            AutoStr = str;
            AutoDex = dex;
            AutoRec = rec;
            AutoInt = @int;
            AutoWis = wis;
            AutoLuc = luc;

            _taskQueue.Enqueue(ActionType.UPDATE_AUTO_STATS, Id, AutoStr, AutoDex, AutoRec, AutoInt, AutoWis, AutoLuc);

            SendAutoStats();
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


        private void HandleTeleportViaNpc(CharacterTeleportViaNpcPacket teleportViaNpcPacket)
        {
            var npc = Map.GetNPC(CellId, teleportViaNpcPacket.NpcId);
            if (npc is null)
            {
                _logger.LogWarning($"Character {Id} is trying to get non-existing npc via teleport packet.");
                return;
            }

            if (!npc.ContainsGate(teleportViaNpcPacket.GateId))
                return;

            var gate = npc.Gates[teleportViaNpcPacket.GateId];

            if (Gold < gate.Cost)
            {
                SendTeleportViaNpc(NpcTeleportNotAllowedReason.NotEnoughMoney);
                return;
            }

            var mapConfig = _mapLoader.LoadMapConfiguration(gate.MapId);
            if (mapConfig is null)
            {
                SendTeleportViaNpc(NpcTeleportNotAllowedReason.MapCapacityIsFull);
                return;
            }

            // TODO: there should be somewhere player's level check. But I can not find it in gate config.

            ChangeGold((uint)(Gold - gate.Cost));
            SendTeleportViaNpc(NpcTeleportNotAllowedReason.Success);
            Teleport(gate.MapId, gate.X, gate.Y, gate.Z);
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
            var mapId = gmTeleportMapPacket.MapId;

            if (!_gameWorld.AvailableMapIds.Contains(mapId))
            {
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_MAP);
                return;
            }

            float x = 100;
            float z = 100;
            var spawn = _mapLoader.LoadMapConfiguration(mapId).Spawns.FirstOrDefault(s => (s.Faction == 1 && Country == Fraction.Light) || (s.Faction == 2 && Country == Fraction.Dark));
            if (spawn != null)
            {
                x = spawn.X1;
                z = spawn.Z1;
            }

            _packetsHelper.SendGmCommandSuccess(Client);

            Teleport(mapId, x, PosY, z, true);
        }

        private void HandleGMTeleportToMapCoordinates(GMTeleportMapCoordinatesPacket gmTeleportMapCoordinatesPacket)
        {
            var (newPosX, newPosZ, mapId) = gmTeleportMapCoordinatesPacket;

            if (!_gameWorld.AvailableMapIds.Contains(mapId))
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

        private void HandleCreateGuild(string name, string message)
        {
            var result = _guildManager.CanCreateGuild(this, name);
            if (result != GuildCreateFailedReason.Success)
            {
                SendGuildCreateFailed(result);
                return;
            }

            _guildManager.SendGuildRequest(this, name, message);
        }


        private void HandleGuildAgree(bool ok)
        {
            _guildManager.SetAgreeRequest(this, ok);
        }

        private async void HandleGuildJoinRequest(int guildId)
        {
            if (HasGuild)
            {
                _packetsHelper.SendGuildJoinRequest(Client, false);
                return;
            }

            var success = await _guildManager.RequestJoin(guildId, Id);
            _packetsHelper.SendGuildJoinRequest(Client, success);
        }

        private async void HandleJoinResult(bool ok, int characterId)
        {
            if (!HasGuild || GuildRank > 3)
                return;

            var guild = await _guildManager.GetGuild((int)GuildId);
            if (guild is null)
                return;

            await _guildManager.RemoveRequestJoin(characterId);

            var onlinePlayer = _gameWorld.Players[characterId];
            if (!ok)
            {
                if (onlinePlayer != null)
                    onlinePlayer.SendGuildJoinResult(false, guild);

                return;
            }

            var dbCharacter = await _guildManager.TryAddMember((int)GuildId, characterId);
            if (dbCharacter is null)
            {
                if (onlinePlayer != null)
                    onlinePlayer.SendGuildJoinResult(false, guild);

                return;
            }

            // Update guild members.
            foreach (var member in GuildMembers.ToList())
            {
                if (!_gameWorld.Players.ContainsKey(member.Id))
                    continue;

                var guildPlayer = _gameWorld.Players[member.Id];
                guildPlayer.GuildMembers.Add(dbCharacter);
                guildPlayer.SendGuildUserListAdd(dbCharacter, onlinePlayer != null);
            }

            // Send additional info to new member, if he is online.
            if (onlinePlayer != null)
            {
                onlinePlayer.GuildId = guild.Id;
                onlinePlayer.GuildName = guild.Name;
                onlinePlayer.GuildRank = 9;
                onlinePlayer.GuildMembers.AddRange(GuildMembers);

                onlinePlayer.SendGuildJoinResult(true, guild);
                onlinePlayer.SendGuildMembersOnline();
            }
        }

        private async void HandleGuildKick(int removeId)
        {
            if (!HasGuild || GuildRank > 3)
            {
                SendGuildKickMember(false, removeId);
                return;
            }

            var dbCharacter = await _guildManager.TryRemoveMember((int)GuildId, removeId);
            if (dbCharacter is null)
            {
                SendGuildKickMember(false, removeId);
                return;
            }

            // Update guild members.
            foreach (var member in GuildMembers.ToList())
            {
                if (!_gameWorld.Players.ContainsKey(member.Id))
                    continue;

                var guildPlayer = _gameWorld.Players[member.Id];

                if (guildPlayer.Id == removeId)
                    guildPlayer.ClearGuild();
                else
                {
                    var temp = guildPlayer.GuildMembers.FirstOrDefault(x => x.Id == removeId);

                    if (temp != null)
                        guildPlayer.GuildMembers.Remove(temp);

                    guildPlayer.SendGuildMemberRemove(removeId);
                }

                guildPlayer.SendGuildKickMember(true, removeId);
            }
        }

        private async void HandleChangeRank(bool demote, int characterId)
        {
            if (!HasGuild || GuildRank > 3)
                return;

            var dbCharacter = await _guildManager.TryChangeRank((int)GuildId, characterId, demote);
            if (dbCharacter is null)
                return;

            foreach (var member in GuildMembers.ToList())
            {
                if (!_gameWorld.Players.ContainsKey(member.Id))
                    continue;

                var guildPlayer = _gameWorld.Players[member.Id];
                var changed = guildPlayer.GuildMembers.FirstOrDefault(x => x.Id == characterId);
                if (changed is null)
                    continue;

                changed.GuildRank = dbCharacter.GuildRank;
                guildPlayer.SendGuildUserChangeRank(changed.Id, changed.GuildRank);
            }
        }

        private void HandleLeaveGuild()
        {
            if (!HasGuild)
                return;

            var dbCharacter = _guildManager.TryRemoveMember((int)GuildId, Id);
            if (dbCharacter == null)
            {
                SendGuildMemberLeaveResult(false);
                return;
            }

            foreach (var member in GuildMembers.ToList())
            {
                if (!_gameWorld.Players.ContainsKey(member.Id))
                    continue;

                var guildPlayer = _gameWorld.Players[member.Id];

                if (guildPlayer.Id == Id)
                {
                    guildPlayer.ClearGuild();
                }
                else
                {
                    var temp = guildPlayer.GuildMembers.FirstOrDefault(x => x.Id == Id);

                    if (temp != null)
                        guildPlayer.GuildMembers.Remove(temp);

                    guildPlayer.SendGuildMemberLeave(Id);
                }
            }

            SendGuildMemberLeaveResult(true);
        }

        private async void HandleGuildDismantle()
        {
            if (!HasGuild || GuildRank != 1)
                return;

            var result = await _guildManager.TryDeleteGuild((int)GuildId);
            if (!result)
                return;

            foreach (var member in GuildMembers.ToList())
            {
                if (!_gameWorld.Players.ContainsKey(member.Id))
                    continue;

                var guildPlayer = _gameWorld.Players[member.Id];
                guildPlayer.ClearGuild();
                guildPlayer.SendGuildDismantle();
            }

            // TODO: send guild remove from list
        }
    }
}
