using System;
using System.Collections.Generic;
using System.Linq;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.Obelisks;
using Imgeneus.World.Serialization;

namespace Imgeneus.World.Packets
{
    internal class CharacterPacketsHelper
    {
        internal void SendInventoryItems(IWorldClient client, ICollection<Item> inventoryItems)
        {
            using var packet = new Packet(PacketType.CHARACTER_ITEMS);
            packet.Write(new InventoryItems(inventoryItems).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterTeleport(IWorldClient client, Character player)
        {
            using var packet = new Packet(PacketType.CHARACTER_MAP_TELEPORT);
            packet.Write(player.Id);
            packet.Write(player.MapId);
            packet.Write(player.PosX);
            packet.Write(player.PosY);
            packet.Write(player.PosZ);
            client.SendPacket(packet);
        }

        internal void SendCurrentHitpoints(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_CURRENT_HITPOINTS);
            packet.Write(new CharacterHitpoints(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendDetails(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_DETAILS);
            packet.Write(new CharacterDetails(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendLearnedNewSkill(IWorldClient client, Skill skill)
        {
            using var answerPacket = new Packet(PacketType.LEARN_NEW_SKILL);
            answerPacket.Write(new LearnedSkill(skill).Serialize());
            client.SendPacket(answerPacket);
        }

        internal void SendLearnedSkills(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SKILLS);
            packet.Write(new CharacterSkills(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendActiveBuffs(IWorldClient client, ICollection<ActiveBuff> activeBuffs)
        {
            using var packet = new Packet(PacketType.CHARACTER_ACTIVE_BUFFS);
            packet.Write(new CharacterActiveBuffs(activeBuffs).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAddBuff(IWorldClient client, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.BUFF_ADD);
            packet.Write(new SerializedActiveBuff(buff).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRemoveBuff(IWorldClient client, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.BUFF_REMOVE);
            packet.Write(buff.Id);
            client.SendPacket(packet);
        }

        internal void SendMoveItemInInventory(IWorldClient client, Item sourceItem, Item destinationItem)
        {
            // Send move item.
            using var packet = new Packet(PacketType.INVENTORY_MOVE_ITEM);

            var bytes = new MovedItem(sourceItem).Serialize();
            packet.Write(bytes);

            bytes = new MovedItem(destinationItem).Serialize();
            packet.Write(bytes);

            client.SendPacket(packet);
        }

        internal void SetMobInTarget(IWorldClient client, Mob target)
        {
            using var packet = new Packet(PacketType.TARGET_SELECT_MOB);
            packet.Write(new MobInTarget(target).Serialize());
            client.SendPacket(packet);
        }

        internal void SetPlayerInTarget(IWorldClient client, Character target)
        {
            using var packet = new Packet(PacketType.TARGET_SELECT_CHARACTER);
            packet.Write(new CharacterInTarget(target).Serialize());
            client.SendPacket(packet);
        }

        internal void SendSkillBar(IWorldClient client, IEnumerable<DbQuickSkillBarItem> quickItems)
        {
            using var packet = new Packet(PacketType.CHARACTER_SKILL_BAR);
            packet.Write((byte)quickItems.Count());
            packet.Write(0); // Unknown int.

            foreach (var item in quickItems)
            {
                packet.Write(item.Bar);
                packet.Write(item.Slot);
                packet.Write(item.Bag);
                packet.Write(item.Number);
                packet.Write(0); // Unknown int.
            }

            client.SendPacket(packet);
            client.CryptoManager.UseExpandedKey = true;
        }

        internal void SendAddItem(IWorldClient client, Item item)
        {
            using var packet = new Packet(PacketType.ADD_ITEM);
            packet.Write(new AddedInventoryItem(item).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRemoveItem(IWorldClient client, Item item, bool fullRemove)
        {
            using var packet = new Packet(PacketType.REMOVE_ITEM);
            packet.Write(new RemovedInventoryItem(item, fullRemove).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAdditionalStats(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_ADDITIONAL_STATS);
            packet.Write(new CharacterAdditionalStats(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMaxHitpoints(IWorldClient client, Character character, HitpointType type)
        {
            using var packet = new Packet(PacketType.CHARACTER_MAX_HITPOINTS);
            packet.Write(new MaxHitpoint(character, type).Serialize());
            client.SendPacket(packet);
        }

        internal void SendPartyInfo(IWorldClient client, IEnumerable<Character> partyMembers, byte leaderIndex)
        {
            using var packet = new Packet(PacketType.PARTY_LIST);
            packet.Write(new UsualParty(partyMembers, leaderIndex).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRaidInfo(IWorldClient client, Raid raid)
        {
            using var packet = new Packet(PacketType.RAID_LIST);
            packet.Write(new RaidParty(raid).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAttackStart(IWorldClient client)
        {
            using var packet = new Packet(PacketType.ATTACK_START);
            client.SendPacket(packet);
        }

        internal void SendAutoAttackWrongTarget(IWorldClient client, Character sender, IKillable target)
        {
            PacketType type = target is Character ? PacketType.CHARACTER_CHARACTER_AUTO_ATTACK : PacketType.CHARACTER_MOB_AUTO_ATTACK;
            using var packet = new Packet(type);
            packet.Write(new UsualAttack(sender.Id, 0, new AttackResult() { Success = AttackSuccess.WrongTarget }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAutoAttackCanNotAttack(IWorldClient client, Character sender, IKillable target)
        {
            PacketType type = target is Character ? PacketType.CHARACTER_CHARACTER_AUTO_ATTACK : PacketType.CHARACTER_MOB_AUTO_ATTACK;
            using var packet = new Packet(type);
            packet.Write(new UsualAttack(sender.Id, 0, new AttackResult() { Success = AttackSuccess.CanNotAttack }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendItemDoesNotBelong(IWorldClient client)
        {
            using var packet = new Packet(PacketType.ADD_ITEM);
            packet.WriteByte(0);
            packet.WriteByte(0); // Item doesn't belong to player.
            client.SendPacket(packet);
        }

        internal void SendFullInventory(IWorldClient client)
        {
            using var packet = new Packet(PacketType.ADD_ITEM);
            packet.WriteByte(0);
            packet.WriteByte(1); // Inventory is full.
            client.SendPacket(packet);
        }

        internal void SendBoughtItem(IWorldClient client, Item boughtItem, uint gold)
        {
            using var packet = new Packet(PacketType.NPC_BUY_ITEM);
            packet.WriteByte(0); // success
            packet.Write(boughtItem.Bag);
            packet.Write(boughtItem.Slot);
            packet.Write(boughtItem.Type);
            packet.Write(boughtItem.TypeId);
            packet.Write(boughtItem.Count);
            packet.Write(gold);
            client.SendPacket(packet);
        }

        internal void SendSoldItem(IWorldClient client, Item soldItem, uint gold)
        {
            using var packet = new Packet(PacketType.NPC_SELL_ITEM);
            packet.WriteByte(0); // success
            packet.Write(soldItem.Bag);
            packet.Write(soldItem.Slot);
            packet.Write(soldItem.Type);
            packet.Write(soldItem.TypeId);
            packet.Write(soldItem.Count);
            packet.Write(gold);
            client.SendPacket(packet);
        }

        internal void SendBuyItemIssue(IWorldClient client, byte issue)
        {
            using var packet = new Packet(PacketType.NPC_BUY_ITEM);
            packet.Write(issue);
            // empty fields about item, because it wasn't bought.
            packet.WriteByte(0); // bag
            packet.WriteByte(0); // slot
            packet.WriteByte(0); // type
            packet.WriteByte(0); // type id
            packet.WriteByte(0); // count
            packet.Write(0); // gold
            client.SendPacket(packet);
        }

        internal void SendSkillWrongTarget(IWorldClient client, Character sender, Skill skill, IKillable target)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.WrongTarget }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendSkillAttackCanNotAttack(IWorldClient client, Character sender, Skill skill, IKillable target)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.CanNotAttack }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendGmCommandSuccess(IWorldClient client)
        {
            using var packet = new Packet(PacketType.GM_CMD_ERROR);
            packet.Write<ushort>(0); // 0 == no error
            client.SendPacket(packet);
        }

        internal void SendGmCommandError(IWorldClient client, PacketType error)
        {
            using var packet = new Packet(PacketType.GM_CMD_ERROR);
            packet.Write((ushort)error);
            client.SendPacket(packet);
        }

        internal void SendGmTeleport(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.GM_TELEPORT_MAP);
            packet.Write(character.Id);
            packet.Write(character.MapId);
            packet.Write(character.PosX);
            packet.Write(character.PosY);
            packet.Write(character.PosZ);
            client.SendPacket(packet);
        }

        internal void SendSkillWrongEquipment(IWorldClient client, Character sender, IKillable target, Skill skill)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.WrongEquipment }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendNotEnoughMPSP(IWorldClient client, Character sender, IKillable target, Skill skill)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.NotEnoughMPSP }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCooldownNotOver(IWorldClient client, Character sender, IKillable target, Skill skill)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.NotEnoughMPSP }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendUseSMMP(IWorldClient client, ushort MP, ushort SP)
        {
            using var packet = new Packet(PacketType.USED_SP_MP);
            packet.Write(new UseSPMP(SP, MP).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMoveAndAttackSpeed(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_ATTACK_MOVEMENT_SPEED);
            packet.Write(new CharacterAttackAndMovement(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCurrentBuffs(IWorldClient client, IKillable target)
        {
            using var packet = new Packet(PacketType.TARGET_BUFFS);
            packet.Write(new TargetBuffs(target).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterShape(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SHAPE);
            packet.Write(new CharacterShape(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRunMode(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.RUN_MODE);
            packet.Write(character.MoveMotion);
            client.SendPacket(packet);
        }

        internal void SendTargetAddBuff(IWorldClient client, IKillable target, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.TARGET_BUFF_ADD);
            if (target is Mob)
            {
                packet.WriteByte(2);
            }
            else
            {
                packet.WriteByte(1);
            }
            packet.Write(target.Id);
            packet.Write(buff.SkillId);
            packet.Write(buff.SkillLevel);

            client.SendPacket(packet);
        }

        internal void SendTargetRemoveBuff(IWorldClient client, IKillable target, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.TARGET_BUFF_REMOVE);
            if (target is Mob)
            {
                packet.WriteByte(2);
            }
            else
            {
                packet.WriteByte(1);
            }
            packet.Write(target.Id);
            packet.Write(buff.SkillId);
            packet.Write(buff.SkillLevel);

            client.SendPacket(packet);
        }

        internal void SendMobPosition(IWorldClient client, Mob mob)
        {
            using var packet = new Packet(PacketType.MOB_MOVE);
            packet.Write(new MobMove(mob).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMobState(IWorldClient client, Mob target)
        {
            using var packet = new Packet(PacketType.TARGET_MOB_GET_STATE);
            packet.Write(target.Id);
            packet.Write(target.CurrentHP);
            packet.Write((byte)target.AttackSpeed);
            packet.Write((byte)target.MoveSpeed);
            client.SendPacket(packet);
        }

        internal void SendQuests(IWorldClient client, IEnumerable<Quest> quests)
        {
            using var packet = new Packet(PacketType.QUEST_LIST);
            packet.Write(new CharacterQuests(quests).Serialize());
            client.SendPacket(packet);
        }

        internal void SendFinishedQuests(IWorldClient client, IEnumerable<Quest> quests)
        {
            using var packet = new Packet(PacketType.QUEST_FINISHED_LIST);
            packet.Write(new CharacterFinishedQuests(quests).Serialize());
            client.SendPacket(packet);
        }

        internal void SendQuestStarted(IWorldClient client, ushort questId, int npcId)
        {
            using var packet = new Packet(PacketType.QUEST_START);
            packet.Write(npcId);
            packet.Write(questId);
            client.SendPacket(packet);
        }

        internal void SendQuestFinished(IWorldClient client, Quest quest, int npcId)
        {
            using var packet = new Packet(PacketType.QUEST_END);
            packet.Write(npcId);
            packet.Write(quest.Id);
            packet.Write(quest.IsSuccessful);
            packet.WriteByte(0); // ResultType
            packet.Write(quest.IsSuccessful ? quest.XP : 0);
            packet.Write(quest.IsSuccessful ? quest.Gold : 0);
            packet.WriteByte(0); // bag
            packet.WriteByte(0); // slot
            packet.WriteByte(0); // item type
            packet.WriteByte(0); // item id
            client.SendPacket(packet);
        }

        internal void SendQuestCountUpdate(IWorldClient client, ushort questId, byte index, byte count)
        {
            using var packet = new Packet(PacketType.QUEST_UPDATE_COUNT);
            packet.Write(questId);
            packet.Write(index);
            packet.Write(count);
            client.SendPacket(packet);
        }

        internal void SendFriendRequest(IWorldClient client, Character requester)
        {
            using var packet = new Packet(PacketType.FRIEND_REQUEST);
            packet.WriteString(requester.Name, 21);
            client.SendPacket(packet);
        }

        internal void SendFriendAdded(IWorldClient client, Character friend)
        {
            using var packet = new Packet(PacketType.FRIEND_ADD);
            packet.Write(friend.Id);
            packet.Write((byte)friend.Class);
            packet.WriteString(friend.Name);
            client.SendPacket(packet);
        }

        internal void SendFriendDelete(IWorldClient client, int id)
        {
            using var packet = new Packet(PacketType.FRIEND_DELETE);
            packet.Write(id);
            client.SendPacket(packet);
        }

        internal void SendFriends(IWorldClient client, IEnumerable<Friend> friends)
        {
            using var packet = new Packet(PacketType.FRIEND_LIST);
            packet.Write(new FriendsList(friends).Serialize());
            client.SendPacket(packet);
        }

        internal void SendFriendOnline(IWorldClient client, int id, bool isOnline)
        {
            using var packet = new Packet(PacketType.FRIEND_ONLINE);
            packet.Write(id);
            packet.Write(isOnline);
            client.SendPacket(packet);
        }

        internal void SendFriendResponse(IWorldClient client, bool accepted)
        {
            using var packet = new Packet(PacketType.FRIEND_RESPONSE);
            packet.Write(accepted);
            client.SendPacket(packet);
        }

        internal void SendWeather(IWorldClient client, Map map)
        {
            using var packet = new Packet(PacketType.MAP_WEATHER);
            packet.Write(new MapWeather(map).Serialize());
            client.SendPacket(packet);
        }

        internal void SendObelisks(IWorldClient client, IEnumerable<Obelisk> obelisks)
        {
            using var packet = new Packet(PacketType.OBELISK_LIST);
            packet.Write(new ObeliskList(obelisks).Serialize());
            client.SendPacket(packet);
        }

        internal void SendObeliskBroken(IWorldClient client, Obelisk obelisk)
        {
            using var packet = new Packet(PacketType.OBELISK_CHANGE);
            packet.Write(obelisk.Id);
            packet.Write((byte)obelisk.ObeliskCountry);
            client.SendPacket(packet);
        }

        internal void SendRegisteredInPartySearch(IWorldClient client, bool isSuccess)
        {
            using var packet = new Packet(PacketType.PARTY_SEARCH_REGISTRATION);
            packet.Write(isSuccess);
            client.SendPacket(packet);
        }

        internal void SendPartySearchList(IWorldClient client, IEnumerable<Character> partySearchers)
        {
            using var packet = new Packet(PacketType.PARTY_SEARCH_LIST);
            packet.Write(new PartySearchList(partySearchers).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterPosition(IWorldClient client, Character player)
        {
            using var packet = new Packet(PacketType.GM_FIND_PLAYER);
            packet.Write(player.MapId);
            packet.Write(player.PosX);
            packet.Write(player.PosY);
            packet.Write(player.PosZ);
            client.SendPacket(packet);
        }

        internal void SendGmSummon(IWorldClient client, Character player)
        {
            using var packet = new Packet(PacketType.GM_SUMMON_PLAYER);
            packet.Write(player.Id);
            packet.Write(player.MapId);
            packet.Write(player.PosX);
            packet.Write(player.PosY);
            packet.Write(player.PosZ);
            client.SendPacket(packet);
        }

        internal void SendGmTeleportToPlayer(IWorldClient client, Character player)
        {
            using var packet = new Packet(PacketType.GM_TELEPORT_TO_PLAYER);
            packet.Write(player.Id);
            packet.Write(player.MapId);
            packet.Write(player.PosX);
            packet.Write(player.PosY);
            packet.Write(player.PosZ);
            client.SendPacket(packet);
        }

        internal void SendUseVehicle(IWorldClient client, bool success, bool status)
        {
            using var packet = new Packet(PacketType.USE_VEHICLE);
            packet.Write(success);
            packet.Write(status);
            client.SendPacket(packet);
        }

        internal void SendAddGem(IWorldClient client, bool success, Item gem, Item destinationItem, byte gemSlot, uint gold, Item saveItem, Item hammer)
        {
            using var packet = new Packet(PacketType.GEM_ADD);
            packet.Write(success);
            packet.Write(gem.Bag);
            packet.Write(gem.Slot);
            packet.Write(gem.Count);
            packet.Write(destinationItem.Bag);
            packet.Write(destinationItem.Slot);
            packet.Write(gemSlot);
            packet.Write(gem.TypeId);
            packet.WriteByte(0); // unknown, old eps: byBag
            packet.WriteByte(0); // unknown, old eps: bySlot
            packet.WriteByte(0); // unknown, old eps: byTypeID; maybe in new ep TypeId is int?
            packet.Write(gold);
            if (hammer is null)
            {
                packet.WriteByte(0);
                packet.WriteByte(0);
            }
            else
            {
                packet.Write(hammer.Bag);
                packet.Write(hammer.Slot);
            }

            client.SendPacket(packet);
        }

        internal void SendGemPossibility(IWorldClient client, double rate, int gold)
        {
            using var packet = new Packet(PacketType.GEM_POSSIBILITY);
            packet.WriteByte(1); // TODO: unknown, maybe bool, that we can link?
            packet.Write(rate);
            packet.Write(gold);
            client.SendPacket(packet);
        }
    }
}
