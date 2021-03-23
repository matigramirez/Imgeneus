using Imgeneus.Database.Constants;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Zone;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        private IWorldClient _client;

        /// <summary>
        /// TCP connection with client.
        /// </summary>
        public IWorldClient Client
        {
            get => _client;

            set
            {
                if (_client is null)
                {
                    _client = value;
                    _client.OnPacketArrived += Client_OnPacketArrived;
                }
                else
                {
                    throw new ArgumentException("TCP connection can not be set twice");
                }
            }
        }

        /// <summary>
        /// Removes TCP connection.
        /// </summary>
        public void ClearConnection()
        {
            _client = null;
        }

        /// <summary>
        /// Tries to handle all packets, that client sends.
        /// </summary>
        /// <param name="sender">TCP connection with client</param>
        /// <param name="packet">packet, that clients sends</param>
        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case ChangeEncryptionPacket changeEcryptionPacket:
                    Client.CryptoManager.UseExpandedKey = true;
                    break;

                case LearnNewSkillPacket learnNewSkillPacket:
                    HandleLearnNewSkill(learnNewSkillPacket);
                    break;

                case MoveItemInInventoryPacket itemInInventoryPacket:
                    HandleMoveItem(itemInInventoryPacket);
                    break;

                case MoveCharacterPacket moveCharacterPacket:
                    HandleMove(moveCharacterPacket);
                    break;

                case MotionPacket motionPacket:
                    HandleMotion(motionPacket);
                    break;

                case MobInTargetPacket mobInTargetPacket:
                    HandleMobInTarget(mobInTargetPacket);
                    break;

                case PlayerInTargetPacket playerInTargetPacket:
                    HandlePlayerInTarget(playerInTargetPacket);
                    break;

                case GMGetItemPacket gMGetItemPacket:
                    HandleGMGetItemPacket(gMGetItemPacket);
                    break;

                case SkillBarPacket skillBarPacket:
                    HandleSkillBarPacket(skillBarPacket);
                    break;

                case AttackStart attackStartPacket:
                    // Not sure, but maybe I should not permit any attack start?
                    sender.SendPacket(new Packet(PacketType.ATTACK_START));
                    break;

                case MobAutoAttackPacket attackPacket:
                    HandleAutoAttackOnMob(attackPacket.TargetId);
                    break;

                case CharacterAutoAttackPacket attackPacket:
                    HandleAutoAttackOnPlayer(attackPacket.TargetId);
                    break;

                case MobSkillAttackPacket usedSkillMobAttackPacket:
                    HandleUseSkillOnMob(usedSkillMobAttackPacket.Number, usedSkillMobAttackPacket.TargetId);
                    break;

                case CharacterSkillAttackPacket usedSkillPlayerAttackPacket:
                    HandleUseSkillOnPlayer(usedSkillPlayerAttackPacket.Number, usedSkillPlayerAttackPacket.TargetId);
                    break;

                case TargetCharacterGetBuffs targetCharacterGetBuffsPacket:
                    HandleGetCharacterBuffs(targetCharacterGetBuffsPacket.TargetId);
                    break;

                case TargetMobGetBuffs targetMobGetBuffsPacket:
                    HandleGetMobBuffs(targetMobGetBuffsPacket.TargetId);
                    break;

                case TargetGetMobStatePacket targetGetMobStatePacket:
                    HandleGetMobState(targetGetMobStatePacket.MobId);
                    break;

                case CharacterShapePacket characterShapePacket:
                    HandleCharacterShape(characterShapePacket.CharacterId);
                    break;

                case CharacterEnteredPortalPacket enterPortalPacket:
                    HandleEnterPortalPacket(enterPortalPacket);
                    break;

                case CharacterTeleportViaNpcPacket teleportViaNpcPacket:
                    HandleTeleportViaNpc(teleportViaNpcPacket);
                    break;

                case UseItemPacket useItemPacket:
                    UseItem(useItemPacket.Bag, useItemPacket.Slot);
                    break;

                case ChatNormalPacket chatNormalPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Normal, chatNormalPacket.Message);
                    break;

                case ChatWhisperPacket chatWisperPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Whisper, chatWisperPacket.Message, chatWisperPacket.TargetName);
                    break;

                case ChatPartyPacket chatPartyPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Party, chatPartyPacket.Message);
                    break;

                case ChatMapPacket chatMapPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Map, chatMapPacket.Message);
                    break;

                case ChatWorldPacket chatWorldPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.World, chatWorldPacket.Message);
                    break;

                case DuelDefeatPacket duelDefeatPacket:
                    FinishDuel(Duel.DuelCancelReason.AdmitDefeat);
                    break;

                case ChangeAppearancePacket changeAppearancePacket:
                    HandleChangeAppearance(changeAppearancePacket);
                    break;

                case FriendRequestPacket friendRequestPacket:
                    HandleFriendRequest(friendRequestPacket.CharacterName);
                    break;

                case FriendResponsePacket friendResponsePacket:
                    ClearFriend(friendResponsePacket.Accepted);
                    break;

                case FriendDeletePacket friendDeletePacket:
                    DeleteFriend(friendDeletePacket.CharacterId);
                    break;

                case RemoveItemPacket removeItemPacket:
                    InventoryItems.TryGetValue((removeItemPacket.Bag, removeItemPacket.Slot), out var item);
                    if (item is null || item.AccountRestriction == ItemAccountRestrictionType.AccountRestricted || item.AccountRestriction == ItemAccountRestrictionType.CharacterRestricted)
                        return;
                    item.TradeQuantity = removeItemPacket.Count <= item.Count ? removeItemPacket.Count : item.Count;
                    var removedItem = RemoveItemFromInventory(item);
                    _packetsHelper.SendRemoveItem(Client, item, removedItem.Count == item.Count);
                    Map.AddItem(new MapItem(removedItem, this, PosX, PosY, PosZ));
                    break;

                case MapPickUpItemPacket mapPickUpItemPacket:
                    var mapItem = Map.GetItem(mapPickUpItemPacket.ItemId, this);
                    if (mapItem is null)
                    {
                        _packetsHelper.SendItemDoesNotBelong(Client);
                        return;
                    }
                    if (mapItem.Item.Type == Item.MONEY_ITEM_TYPE)
                    {
                        Map.RemoveItem(CellId, mapItem.Id);
                        mapItem.Item.Bag = 1;
                        ChangeGold(Gold + (uint)mapItem.Item.Gem1.TypeId);
                        SendAddItemToInventory(mapItem.Item);
                    }
                    else
                    {
                        var inventoryItem = AddItemToInventory(mapItem.Item);
                        if (inventoryItem is null)
                        {
                            _packetsHelper.SendFullInventory(Client);
                        }
                        else
                        {
                            Map.RemoveItem(CellId, mapItem.Id);
                            SendAddItemToInventory(inventoryItem);
                            if (Party != null)
                                Party.MemberGetItem(this, inventoryItem);
                        }
                    }
                    break;

                case NpcBuyItemPacket npcBuyItemPacket:
                    var npc = Map.GetNPC(CellId, npcBuyItemPacket.NpcId);
                    if (npc is null || !npc.ContainsProduct(npcBuyItemPacket.ItemIndex))
                        return;
                    var buyItem = npc.Products[npcBuyItemPacket.ItemIndex];
                    var boughtItem = BuyItem(buyItem, npcBuyItemPacket.Count);
                    if (boughtItem != null)
                        _packetsHelper.SendBoughtItem(Client, boughtItem, Gold);
                    break;

                case NpcSellItemPacket npcSellItemPacket:
                    if (npcSellItemPacket.Bag == 0) // Worn item can not be sold, player should take it off first.
                        return;

                    InventoryItems.TryGetValue((npcSellItemPacket.Bag, npcSellItemPacket.Slot), out var itemToSell);
                    if (itemToSell is null) // Item for sale not found.
                        return;

                    var fullSold = itemToSell.Count <= npcSellItemPacket.Count;

                    var soldItem = SellItem(itemToSell, npcSellItemPacket.Count);
                    if (soldItem != null)
                    {
                        if (fullSold)
                            itemToSell.Count = 0;
                        _packetsHelper.SendSoldItem(Client, itemToSell, Gold);
                    }
                    break;

                case QuestStartPacket questStartPacket:
                    var npcQuestGiver = Map.GetNPC(CellId, questStartPacket.NpcId);
                    if (npcQuestGiver is null || !npcQuestGiver.StartQuestIds.Contains(questStartPacket.QuestId))
                    {
                        _logger.LogWarning($"Trying to start unknown quest {questStartPacket.QuestId} at npc {questStartPacket.NpcId}");
                        return;
                    }

                    var quest = new Quest(_databasePreloader, questStartPacket.QuestId);
                    StartQuest(quest, npcQuestGiver.Id);
                    break;

                case QuestEndPacket questEndPacket:
                    var npcQuestReceiver = Map.GetNPC(CellId, questEndPacket.NpcId);
                    if (npcQuestReceiver is null || !npcQuestReceiver.EndQuestIds.Contains(questEndPacket.QuestId))
                    {
                        _logger.LogWarning($"Trying to finish unknown quest {questEndPacket.QuestId} at npc {questEndPacket.NpcId}");
                        return;
                    }
                    FinishQuest(questEndPacket.QuestId, npcQuestReceiver.Id);
                    break;

                case QuestQuitPacket questQuitPacket:
                    QuitQuest(questQuitPacket.QuestId);
                    break;

                case RebirthPacket rebirthPacket:
                    var rebirthCoordinate = Map.GetRebirthMap(this);
                    Rebirth(rebirthCoordinate.MapId, rebirthCoordinate.X, rebirthCoordinate.Y, rebirthCoordinate.Z);
                    break;

                case PartySearchRegistrationPacket searchPartyPacket:
                    HandleSearchParty();
                    break;

                case UseVehiclePacket useVehiclePacket:
                    if (IsOnVehicle)
                        RemoveVehicle();
                    else
                        CallVehicle();
                    break;

                case GemAddPacket gemAddPacket:
                    AddGem(gemAddPacket.Bag, gemAddPacket.Slot, gemAddPacket.DestinationBag, gemAddPacket.DestinationSlot, gemAddPacket.HammerBag, gemAddPacket.HammerSlot);
                    break;

                case GemAddPossibilityPacket gemPossibilityPacket:
                    AddGemPossibility(gemPossibilityPacket.GemBag, gemPossibilityPacket.GemSlot, gemPossibilityPacket.DestinationBag, gemPossibilityPacket.DestinationSlot, gemPossibilityPacket.HammerBag, gemPossibilityPacket.HammerSlot);
                    break;

                case GemRemovePacket gemRemovePacket:
                    RemoveGem(gemRemovePacket.Bag, gemRemovePacket.Slot, gemRemovePacket.ShouldRemoveSpecificGem, gemRemovePacket.GemPosition, gemRemovePacket.HammerBag, gemRemovePacket.HammerSlot);
                    break;

                case GemRemovePossibilityPacket gemRemovePossibilityPacket:
                    GemRemovePossibility(gemRemovePossibilityPacket.Bag, gemRemovePossibilityPacket.Slot, gemRemovePossibilityPacket.ShouldRemoveSpecificGem, gemRemovePossibilityPacket.GemPosition, gemRemovePossibilityPacket.HammerBag, gemRemovePossibilityPacket.HammerSlot);
                    break;

                case DyeSelectItemPacket dyeSelectItemPacket:
                    HandleDyeSelectItem(dyeSelectItemPacket.DyeItemBag, dyeSelectItemPacket.DyeItemSlot, dyeSelectItemPacket.TargetItemBag, dyeSelectItemPacket.TargetItemSlot);
                    break;

                case DyeRerollPacket dyeRerollPacket:
                    HandleDyeReroll();
                    break;

                case DyeConfirmPacket dyeConfirmPacket:
                    HandleDyeConfirm(dyeConfirmPacket.DyeItemBag, dyeConfirmPacket.DyeItemSlot, dyeConfirmPacket.TargetItemBag, dyeConfirmPacket.TargetItemSlot);
                    break;

                case ItemComposeAbsolutePacket itemComposeAbsolutePacket:
                    HandleAbsoluteCompose(itemComposeAbsolutePacket.RuneBag, itemComposeAbsolutePacket.RuneSlot, itemComposeAbsolutePacket.ItemBag, itemComposeAbsolutePacket.ItemSlot);
                    break;

                case ItemComposePacket itemComposePacket:
                    HandleItemComposePacket(itemComposePacket.RuneBag, itemComposePacket.RuneSlot, itemComposePacket.ItemBag, itemComposePacket.ItemSlot);
                    break;

                case UpdateStatsPacket updateStatsPacket:
                    HandleUpdateStats(updateStatsPacket.Str, updateStatsPacket.Dex, updateStatsPacket.Rec, updateStatsPacket.Int, updateStatsPacket.Wis, updateStatsPacket.Luc);
                    break;

                case AutoStatsSettingsPacket autoStatsSettingsPacket:
                    HandleAutoStatsSettings(autoStatsSettingsPacket.Str, autoStatsSettingsPacket.Dex, autoStatsSettingsPacket.Rec, autoStatsSettingsPacket.Int, autoStatsSettingsPacket.Wis, autoStatsSettingsPacket.Luc);
                    break;

                case GuildCreatePacket guildCreatePacket:
                    HandleCreateGuild(guildCreatePacket.Name, guildCreatePacket.Message);
                    break;

                case GuildAgreePacket guildAgreePacket:
                    HandleGuildAgree(guildAgreePacket.Ok);
                    break;

                case GuildJoinRequestPacket guildJoinRequestPacket:
                    HandleGuildJoinRequest(guildJoinRequestPacket.GuildId);
                    break;

                case GMCreateMobPacket gMCreateMobPacket:
                    if (!IsAdmin)
                        return;

                    HandleGMCreateMob(gMCreateMobPacket);
                    break;

                case GMTeleportMapCoordinatesPacket gmTeleportMapCoordinatesPacket:
                    if (!IsAdmin)
                        return;

                    HandleGMTeleportToMapCoordinates(gmTeleportMapCoordinatesPacket);
                    break;

                case GMTeleportMapPacket gmTeleportMapPacket:
                    if (!IsAdmin)
                        return;

                    HandleGMTeleportToMap(gmTeleportMapPacket);
                    break;

                case GMCreateNpcPacket gMCreateNpcPacket:
                    if (!IsAdmin)
                        return;

                    if (_databasePreloader.NPCs.TryGetValue((gMCreateNpcPacket.Type, gMCreateNpcPacket.TypeId), out var dbNpc))
                    {
                        var moveCoordinates = new List<(float, float, float, ushort)>()
                        {
                            (PosX, PosY, PosZ, Angle)
                        };
                        Map.AddNPC(CellId, _npcFactory.CreateNpc((gMCreateNpcPacket.Type, gMCreateNpcPacket.TypeId), moveCoordinates, Map));
                        _packetsHelper.SendGmCommandSuccess(Client);
                    }
                    else
                    {
                        _packetsHelper.SendGmCommandError(Client, PacketType.GM_CREATE_NPC);
                    }
                    break;

                case GMRemoveNpcPacket gMRemoveNpcPacket:
                    if (!IsAdmin)
                        return;
                    Map.RemoveNPC(CellId, gMRemoveNpcPacket.Type, gMRemoveNpcPacket.TypeId, gMRemoveNpcPacket.Count);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    break;

                case GMFindPlayerPacket gMFindPlayerPacket:
                    HandleFindPlayerPacket(gMFindPlayerPacket.Name);
                    break;

                case GMSummonPlayerPacket gMSummonPlayerPacket:
                    HandleSummonPlayer(gMSummonPlayerPacket.Name);
                    break;

                case GMTeleportToPlayerPacket gMTeleportToPlayerPacket:
                    HandleTeleportToPlayer(gMTeleportToPlayerPacket.Name);
                    break;

                case GMSetAttributePacket gmSetAttributePacket:
                    if (!IsAdmin)
                        return;
                    HandleGMSetAttributePacket(gmSetAttributePacket);
                    break;

                case GMNoticeWorldPacket gmNoticeWorldPacket:
                    if (!IsAdmin)
                        return;

                    _noticeManager.SendWorldNotice(gmNoticeWorldPacket.Message, gmNoticeWorldPacket.TimeInterval);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    break;

                case GMNoticePlayerPacket gmNoticePlayerPacket:
                    if (!IsAdmin)
                        return;

                    if (_noticeManager.TrySendPlayerNotice(gmNoticePlayerPacket.Message, gmNoticePlayerPacket.TargetName,
                        gmNoticePlayerPacket.TimeInterval))
                        _packetsHelper.SendGmCommandSuccess(Client);
                    else
                        _packetsHelper.SendGmCommandError(Client, PacketType.NOTICE_PLAYER);
                    break;

                case GMNoticeFactionPacket gmNoticeFactionPacket:
                    if (!IsAdmin)
                        return;

                    _noticeManager.SendFactionNotice(gmNoticeFactionPacket.Message, this.Country, gmNoticeFactionPacket.TimeInterval);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    break;

                case GMNoticeMapPacket gmNoticeMapPacket:
                    if (!IsAdmin)
                        return;

                    _noticeManager.SendMapNotice(gmNoticeMapPacket.Message, this.MapId, gmNoticeMapPacket.TimeInterval);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    break;

                case GMNoticeAdminsPacket gmNoticeAdminsPacket:
                    if (!IsAdmin)
                        return;

                    _noticeManager.SendAdminNotice(gmNoticeAdminsPacket.Message);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    break;

                case GMCurePlayerPacket gmCurePlayerPacket:
                    if (!IsAdmin)
                        return;

                    HandleGMCurePlayerPacket(gmCurePlayerPacket);
                    break;

                case GMWarningPacket gmWarningPacket:
                    if (!IsAdmin)
                        return;

                    HandleGMWarningPlayer(gmWarningPacket);
                    break;

                case GMTeleportPlayerPacket gmTeleportPlayerPacket:
                    if (!IsAdmin)
                        return;

                    HandleGMTeleportPlayer(gmTeleportPlayerPacket);
                    break;

                case BankClaimItemPacket bankClaimItemPacket:
                    var result = TryClaimBankItem(bankClaimItemPacket.Slot, out _);
                    if (!result)
                        _packetsHelper.SendFullInventory(Client);
                    break;
            }
        }
    }
}
