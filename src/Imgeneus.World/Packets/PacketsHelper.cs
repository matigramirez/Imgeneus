using System.Collections.Generic;
using System.Linq;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Dyeing;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.Obelisks;
using Imgeneus.World.Game.Zone.Portals;
using Imgeneus.World.Serialization;

#if EP8_V1
using Imgeneus.World.Serialization.EP_8_V1;
#elif EP8_V2
using Imgeneus.World.Serialization.EP_8_V2;
#else
using Imgeneus.World.Serialization.EP_8_V1;
#endif

namespace Imgeneus.World.Packets
{
    /// <summary>
    /// Helps to creates packets and send them to players.
    /// </summary>
    internal class PacketsHelper
    {
        internal void SendInventoryItems(IWorldClient client, Item[] inventoryItems)
        {
            var steps = inventoryItems.Length / 50;
            var left = inventoryItems.Length % 50;

            for (var i = 0; i <= steps; i++)
            {
                var startIndex = i * 50;
                var length = i == steps ? left : 50;
                var endIndex = startIndex + length;

                using var packet = new Packet(PacketType.CHARACTER_ITEMS);
                packet.Write(new InventoryItems(inventoryItems[startIndex..endIndex]).Serialize());
                client.SendPacket(packet);
            }
        }

        internal void SendCharacterTeleport(IWorldClient client, Character player, bool teleportedByAdmin)
        {
            using var packet = new Packet(teleportedByAdmin ? PacketType.CHARACTER_MAP_TELEPORT : PacketType.GM_TELEPORT_MAP_COORDINATES);
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

#if (EP8_V2 || SHAIYA_US)
            packet.Write(0); // Unknown int in V2.
#endif

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

        internal void SendPortalTeleportNotAllowed(IWorldClient client, PortalTeleportNotAllowedReason reason)
        {
            using var packet = new Packet(PacketType.CHARACTER_ENTERED_PORTAL);
            packet.Write(false); // success
            packet.Write((byte)reason);
            client.SendPacket(packet);
        }

        internal void SendRemoveItem(IWorldClient client, Item item, bool fullRemove)
        {
            using var packet = new Packet(PacketType.REMOVE_ITEM);
            packet.Write(new RemovedInventoryItem(item, fullRemove).Serialize());
            client.SendPacket(packet);
        }

        internal void SendTeleportViaNpc(IWorldClient client, NpcTeleportNotAllowedReason reason, uint money)
        {
            using var packet = new Packet(PacketType.CHARACTER_TELEPORT_VIA_NPC);
            packet.Write((byte)reason);
            packet.Write(money);
            client.SendPacket(packet);
        }

        internal void SendItemExpiration(IWorldClient client, Item item)
        {
            using var packet = new Packet(PacketType.ITEM_EXPIRATION);
            packet.Write(new InventoryItemExpiration(item).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAdditionalStats(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_ADDITIONAL_STATS);
            packet.Write(new CharacterAdditionalStats(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAutoStats(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.AUTO_STATS_LIST);
            packet.Write(character.AutoStr);
            packet.Write(character.AutoDex);
            packet.Write(character.AutoRec);
            packet.Write(character.AutoInt);
            packet.Write(character.AutoWis);
            packet.Write(character.AutoLuc);
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

        internal void SendResetSkills(IWorldClient client, ushort skillPoint)
        {
            using var packet = new Packet(PacketType.RESET_SKILLS);
            packet.Write(true); // is success?
            packet.Write(skillPoint);
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

        internal void SendAutoAttackWrongEquipment(IWorldClient client, Character sender, IKillable target)
        {
            PacketType type = target is Character ? PacketType.CHARACTER_CHARACTER_AUTO_ATTACK : PacketType.CHARACTER_MOB_AUTO_ATTACK;
            using var packet = new Packet(type);
            packet.Write(new UsualAttack(sender.Id, 0, new AttackResult() { Success = AttackSuccess.WrongEquipment }).Serialize());
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

        internal void SendAbsorbValue(IWorldClient client, ushort absorb)
        {
            using var packet = new Packet(PacketType.CHARACTER_ABSORPTION_DAMAGE);
            packet.Write(absorb);
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

        internal void SendResetStats(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.STATS_RESET);
            packet.Write(true); // success
            packet.Write(character.StatPoint);
            packet.Write(character.Strength);
            packet.Write(character.Reaction);
            packet.Write(character.Intelligence);
            packet.Write(character.Wisdom);
            packet.Write(character.Dexterity);
            packet.Write(character.Luck);
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
            using var packet = new Packet(PacketType.GEM_ADD_POSSIBILITY);
            packet.WriteByte(1); // TODO: unknown, maybe bool, that we can link?
            packet.Write(rate);
            packet.Write(gold);
            client.SendPacket(packet);
        }

        internal void SendGemRemovePossibility(IWorldClient client, double rate, int gold)
        {
            using var packet = new Packet(PacketType.GEM_REMOVE_POSSIBILITY);
            packet.WriteByte(1); // TODO: unknown, maybe bool, that we can link?
            packet.Write(rate);
            packet.Write(gold);
            client.SendPacket(packet);
        }

        internal void SendRemoveGem(IWorldClient client, bool success, Item item, byte gemPosition, List<Item> savedGems, uint gold)
        {
            using var packet = new Packet(PacketType.GEM_REMOVE);
            packet.Write(success);
            packet.Write(item.Bag);
            packet.Write(item.Slot);
            packet.Write(gemPosition);

            for (var i = 0; i < 6; i++)
                if (savedGems[i] is null)
                    packet.WriteByte(0); // bag
                else
                    packet.Write(savedGems[i].Bag);

            for (var i = 0; i < 6; i++)
                if (savedGems[i] is null)
                    packet.WriteByte(0); // slot
                else
                    packet.Write(savedGems[i].Slot);

            for (var i = 0; i < 6; i++) // NB! in old eps this value was byte.
                if (savedGems[i] is null)
                    packet.Write(0); // type id
                else
                    packet.Write((int)savedGems[i].TypeId);

            for (var i = 0; i < 6; i++)
                if (savedGems[i] is null)
                    packet.WriteByte(0); // count
                else
                    packet.Write(savedGems[i].Count);

            packet.Write(gold);
            client.SendPacket(packet);
        }

        internal void SendSelectDyeItem(IWorldClient client, bool success)
        {
            using var packet = new Packet(PacketType.DYE_SELECT_ITEM);
            packet.Write(success);
            client.SendPacket(packet);
        }

        internal void SendDyeColors(IWorldClient client, IEnumerable<DyeColor> availableColors)
        {
            using var packet = new Packet(PacketType.DYE_REROLL);
            foreach (var color in availableColors)
            {
                packet.Write(color.IsEnabled);
                packet.Write(color.Alpha);
                packet.Write(color.Saturation);
                packet.Write(color.R);
                packet.Write(color.G);
                packet.Write(color.B);

                for (var i = 0; i < 21; i++)
                    packet.WriteByte(0); // unknown bytes.
            }
            client.SendPacket(packet);
        }

        internal void SendDyeConfirm(IWorldClient client, bool success, DyeColor color)
        {
            using var packet = new Packet(PacketType.DYE_CONFIRM);
            packet.Write(success);
            if (success)
            {
                packet.Write(color.Alpha);
                packet.Write(color.Saturation);
                packet.Write(color.R);
                packet.Write(color.G);
                packet.Write(color.B);
            }
            else
            {
                packet.WriteByte(0);
                packet.WriteByte(0);
                packet.WriteByte(0);
                packet.WriteByte(0);
                packet.WriteByte(0);
            }
            client.SendPacket(packet);
        }

        internal void SendAbsoluteComposition(IWorldClient client, bool isFailure, string craftName)
        {
            using var packet = new Packet(PacketType.ITEM_COMPOSE_ABSOLUTE);
            packet.Write(isFailure);
            packet.Write(new CraftName(craftName).Serialize());
            packet.Write(true); // ?

            client.SendPacket(packet);
        }

        internal void SendComposition(IWorldClient client, bool isFailure, Item item)
        {
            using var packet = new Packet(PacketType.ITEM_COMPOSE);
            packet.Write(isFailure);
            packet.Write(item.Bag);
            packet.Write(item.Slot);
            packet.Write(new CraftName(item.GetCraftName()).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCanNotUseItem(IWorldClient client, int characterId)
        {
            using var packet = new Packet(PacketType.USE_ITEM);
            packet.Write(characterId);
            packet.WriteByte(0); // bag
            packet.WriteByte(0); // slot
            packet.WriteByte(0); // type
            packet.WriteByte(0); // type id
            packet.WriteByte(0); // count
            client.SendPacket(packet);
        }

        internal void SendStatsUpdate(IWorldClient client, ushort str, ushort dex, ushort rec, ushort intl, ushort wis, ushort luc)
        {
            using var packet = new Packet(PacketType.UPDATE_STATS);
            packet.Write(str);
            packet.Write(dex);
            packet.Write(rec);
            packet.Write(intl);
            packet.Write(wis);
            packet.Write(luc);
            client.SendPacket(packet);
        }

        internal void SendCharacterLeave(IWorldClient client, Character removedCharacter)
        {
            using var packet = new Packet(PacketType.CHARACTER_LEFT_MAP);
            packet.Write(removedCharacter.Id);
            client.SendPacket(packet);
        }

        internal void SendCharacterMoves(IWorldClient client, Character movedPlayer)
        {
            using var packet = new Packet(PacketType.CHARACTER_MOVE);
            packet.Write(new CharacterMove(movedPlayer).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterMotion(IWorldClient client, int characterId, Motion motion)
        {
            using var packet = new Packet(PacketType.CHARACTER_MOTION);
            packet.Write(characterId);
            packet.WriteByte((byte)motion);
            client.SendPacket(packet);
        }

        internal void SendCharacterUsedSkill(IWorldClient client, Character sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            PacketType skillType;
            if (target is Character)
            {
                skillType = PacketType.USE_CHARACTER_TARGET_SKILL;
            }
            else if (target is Mob)
            {
                skillType = PacketType.USE_MOB_TARGET_SKILL;
            }
            else
            {
                skillType = PacketType.USE_CHARACTER_TARGET_SKILL;
            }

            Packet packet = new Packet(skillType);
            var targetId = target is null ? 0 : target.Id;
            packet.Write(new SkillRange(sender.Id, targetId, skill, attackResult).Serialize());
            client.SendPacket(packet);
            packet.Dispose();
        }

        internal void SendCharacterEnter(IWorldClient client, Character character)
        {
            using var packet0 = new Packet(PacketType.CHARACTER_ENTERED_MAP);
            packet0.Write(new CharacterEnteredMap(character).Serialize());
            client.SendPacket(packet0);

            using var packet1 = new Packet(PacketType.CHARACTER_MOVE);
            packet1.Write(new CharacterMove(character).Serialize());
            client.SendPacket(packet1);

            using var packet2 = new Packet(PacketType.CHARACTER_SHAPE_UPDATE);
            packet2.Write(character.Id);
            packet2.Write((byte)character.Shape);
            client.SendPacket(packet2);
        }

        internal void SendMobEnter(IWorldClient client, Mob mob, bool isNew)
        {
            using var packet = new Packet(PacketType.MOB_ENTER);
            packet.Write(new MobEnter(mob, isNew).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMobLeave(IWorldClient client, Mob mob)
        {
            using var packet = new Packet(PacketType.MOB_LEAVE);
            packet.Write(mob.Id);
            client.SendPacket(packet);
        }

        internal void SendMobMove(IWorldClient client, Mob mob)
        {
            using var packet = new Packet(PacketType.MOB_MOVE);
            packet.Write(new MobMove(mob).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMobAttack(IWorldClient client, Mob mob, int targetId, AttackResult attackResult)
        {
            using var packet = new Packet(PacketType.MOB_ATTACK);
            packet.Write(new MobAttack(mob, targetId, attackResult).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMobUsedSkill(IWorldClient client, Mob mob, int targetId, Skill skill, AttackResult attackResult)
        {
            using var packet = new Packet(PacketType.MOB_SKILL_USE);
            packet.Write(new MobSkillAttack(mob, targetId, skill, attackResult).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterChangedEquipment(IWorldClient client, int characterId, Item equipmentItem, byte slot)
        {
            using var packet = new Packet(PacketType.SEND_EQUIPMENT);
            packet.Write(new CharacterEquipmentChange(characterId, slot, equipmentItem).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterUsualAttack(IWorldClient client, IKiller sender, IKillable target, AttackResult attackResult)
        {
            PacketType attackType;
            if (target is Character)
            {
                attackType = PacketType.CHARACTER_CHARACTER_AUTO_ATTACK;
            }
            else if (target is Mob)
            {
                attackType = PacketType.CHARACTER_MOB_AUTO_ATTACK;
            }
            else
            {
                attackType = PacketType.CHARACTER_CHARACTER_AUTO_ATTACK;
            }
            using var packet = new Packet(attackType);
            packet.Write(new UsualAttack(sender.Id, target.Id, attackResult).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterPartyChanged(IWorldClient client, int characterId, PartyMemberType type)
        {
            using var packet = new Packet(PacketType.MAP_PARTY_SET);
            packet.Write(characterId);
            packet.Write((byte)type);
            client.SendPacket(packet);
        }

        internal void SendAttackAndMovementSpeed(IWorldClient client, IKillable sender)
        {
            using var packet = new Packet(PacketType.CHARACTER_ATTACK_MOVEMENT_SPEED);
            packet.Write(new CharacterAttackAndMovement(sender).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterKilled(IWorldClient client, Character character, IKiller killer)
        {
            using var packet = new Packet(PacketType.CHARACTER_DEATH);
            packet.Write(character.Id);
            packet.WriteByte(1); // killer type. 1 - another player.
            packet.Write(killer.Id);
            client.SendPacket(packet);
        }

        internal void SendSkillCastStarted(IWorldClient client, Character sender, IKillable target, Skill skill)
        {
            PacketType type;
            if (target is Character)
                type = PacketType.CHARACTER_SKILL_CASTING;
            else if (target is Mob)
                type = PacketType.MOB_SKILL_CASTING;
            else
                type = PacketType.CHARACTER_SKILL_CASTING;

            using var packet = new Packet(type);
            packet.Write(new SkillCasting(sender.Id, target is null ? 0 : target.Id, skill).Serialize());
            client.SendPacket(packet);
        }

        internal void SendUsedItem(IWorldClient client, Character sender, Item item)
        {
            using var packet = new Packet(PacketType.USE_ITEM);
            packet.Write(sender.Id);
            packet.Write(item.Bag);
            packet.Write(item.Slot);
            packet.Write(item.Type);
            packet.Write(item.TypeId);
            packet.Write(item.Count);
            client.SendPacket(packet);
        }

        internal void SendRecoverCharacter(IWorldClient client, IKillable sender, int hp, int mp, int sp)
        {
            // NB!!! In previous episodes and in china ep 8 with recover packet it's sent how much hitpoints recovered.
            // But in os ep8 this packet sends current hitpoints.
            using var packet = new Packet(PacketType.CHARACTER_RECOVER);
            packet.Write(sender.Id);
            packet.Write(sender.CurrentHP); // old eps: packet.Write(hp);
            packet.Write(sender.CurrentMP); // old eps: packet.Write(mp);
            packet.Write(sender.CurrentSP); // old eps: packet.Write(sp);
            client.SendPacket(packet);
        }

        internal void Send_Max_HP(IWorldClient client, int id, int value)
        {
            using var packet = new Packet(PacketType.CHARACTER_MAX_HITPOINTS);
            packet.Write(id);
            packet.WriteByte(0); // 0 means max hp type.
            packet.Write(value);
            client.SendPacket(packet);
        }

        internal void SendSkillKeep(IWorldClient client, int id, ushort skillId, byte skillLevel, AttackResult result)
        {
            using var packet = new Packet(PacketType.CHARACTER_SKILL_KEEP);
            packet.Write(id);
            packet.Write(skillId);
            packet.Write(skillLevel);
            packet.Write(result.Damage.HP);
            packet.Write(result.Damage.MP);
            packet.Write(result.Damage.SP);
            client.SendPacket(packet);
        }

        internal void SendShapeUpdate(IWorldClient client, Character sender)
        {
            using var packet = new Packet(PacketType.CHARACTER_SHAPE_UPDATE);
            packet.Write(sender.Id);
            packet.Write((byte)sender.Shape);

            // Only for ep 8.
            if (sender.Shape == CharacterShapeEnum.EP_8_Vehicles)
            {
                packet.Write((int)sender.Mount.Type);
                packet.Write((int)sender.Mount.TypeId);
            }

            client.SendPacket(packet);
        }

        internal void SendUsedRangeSkill(IWorldClient client, Character sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            PacketType type;
            if (target is Character)
                type = PacketType.USE_CHARACTER_RANGE_SKILL;
            else if (target is Mob)
                type = PacketType.USE_MOB_RANGE_SKILL;
            else
                type = PacketType.USE_CHARACTER_RANGE_SKILL;

            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, target.Id, skill, attackResult).Serialize());
            client.SendPacket(packet);
        }

        internal void SendDeadRebirth(IWorldClient client, Character sender)
        {
            using var packet = new Packet(PacketType.DEAD_REBIRTH);
            packet.Write(sender.Id);
            packet.WriteByte(4); // rebirth type.
            packet.Write(sender.Exp);
            packet.Write(sender.PosX);
            packet.Write(sender.PosY);
            packet.Write(sender.PosZ);
            client.SendPacket(packet);
        }

        internal void SendCharacterRebirth(IWorldClient client, IKillable sender)
        {
            using var packet = new Packet(PacketType.CHARACTER_LEAVE_DEAD);
            packet.Write(sender.Id);
            client.SendPacket(packet);
        }

        internal void SendMobDead(IWorldClient client, IKillable sender, IKiller killer)
        {
            using var packet = new Packet(PacketType.MOB_DEATH);
            packet.Write(sender.Id);
            packet.WriteByte(1); // killer type. Always 1, since only player can kill the mob.
            packet.Write(killer.Id);
            client.SendPacket(packet);
        }

        internal void SendMobRecover(IWorldClient client, IKillable sender)
        {
            using var packet = new Packet(PacketType.MOB_RECOVER);
            packet.Write(sender.Id);
            packet.Write(sender.CurrentHP);
            client.SendPacket(packet);
        }

        internal void SendAddItem(IWorldClient client, MapItem mapItem)
        {
            using var packet = new Packet(PacketType.MAP_ADD_ITEM);
            packet.Write(mapItem.Id);
            packet.WriteByte(1); // kind of item
            packet.Write(mapItem.Item.Type);
            packet.Write(mapItem.Item.TypeId);
            packet.Write(mapItem.Item.Count);
            packet.Write(mapItem.PosX);
            packet.Write(mapItem.PosY);
            packet.Write(mapItem.PosZ);
            if (mapItem.Item.Type != Item.MONEY_ITEM_TYPE && mapItem.Item.ReqDex > 4) // Highlights valuable items.
                packet.Write(mapItem.Owner.Id);
            else
                packet.Write(0);
            client.SendPacket(packet);
        }

        internal void SendRemoveItem(IWorldClient client, MapItem mapItem)
        {
            using var packet = new Packet(PacketType.MAP_REMOVE_ITEM);
            packet.Write(mapItem.Id);
            client.SendPacket(packet);
        }

        internal void SendNpcEnter(IWorldClient client, Npc npc)
        {
            using var packet = new Packet(PacketType.MAP_NPC_ENTER);
            packet.Write(npc.Id);
            packet.Write(npc.Type);
            packet.Write(npc.TypeId);
            packet.Write(npc.PosX);
            packet.Write(npc.PosY);
            packet.Write(npc.PosZ);
            packet.Write(npc.Angle);
            client.SendPacket(packet);
        }

        internal void SendNpcLeave(IWorldClient client, Npc npc)
        {
            using var packet = new Packet(PacketType.MAP_NPC_LEAVE);
            packet.Write(npc.Id);
            client.SendPacket(packet);
        }

        internal void SendAppearanceChanged(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHANGE_APPEARANCE);
            packet.Write(character.Id);
            packet.Write(character.Hair);
            packet.Write(character.Face);
            packet.Write(character.Height);
            packet.Write((byte)character.Gender);
            client.SendPacket(packet);
        }

        internal void SendStartSummoningVehicle(IWorldClient client, Character sender)
        {
            using var packet = new Packet(PacketType.USE_VEHICLE_READY);
            packet.Write(sender.Id);
            client.SendPacket(packet);
        }

        internal void SendAttribute(IWorldClient client, CharacterAttributeEnum attribute, uint attributeValue)
        {
            using var packet = new Packet(PacketType.CHARACTER_ATTRIBUTE_SET);
            packet.Write(new CharacterAttribute(attribute, attributeValue).Serialize());
            client.SendPacket(packet);
        }

        internal void SendExperienceGain(IWorldClient client, uint exp)
        {
            using var packet = new Packet(PacketType.EXPERIENCE_GAIN);
            packet.Write(new CharacterExperienceGain(exp).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMax_HP_MP_SP(IWorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_MAX_HP_MP_SP);
            packet.Write(new CharacterMax_HP_MP_SP(character));
            client.SendPacket(packet);
        }

        internal void SendLevelUp(IWorldClient client, Character character, bool isAdminLevelUp = false)
        {
            var type = isAdminLevelUp ? PacketType.GM_CHARACTER_LEVEL_UP : PacketType.CHARACTER_LEVEL_UP;
            using var packet = new Packet(type);
            packet.Write(new CharacterLevelUp(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendWarning(IWorldClient client, string message)
        {
            using var packet = new Packet(PacketType.GM_WARNING_PLAYER);
            packet.WriteByte((byte)(message.Length + 1));
            packet.Write(message);
            packet.WriteByte(0);
            client.SendPacket(packet);
        }

        internal void SendBankItems(IWorldClient client, ICollection<BankItem> bankItems)
        {
            using var packet = new Packet(PacketType.BANK_ITEM_LIST);
            packet.Write(new BankItemList(bankItems).Serialize());
            client.SendPacket(packet);
        }

        internal void SendBankItemClaim(IWorldClient client, byte bankSlot, Item item)
        {
            using var packet = new Packet(PacketType.BANK_CLAIM_ITEM);
            packet.Write(new BankItemClaim(bankSlot, item).Serialize());
            client.SendPacket(packet);
        }
        internal void SendAccountPoints(IWorldClient client, uint points)
        {
            using var packet = new Packet(PacketType.ACCOUNT_POINTS);
            packet.Write(new AccountPoints(points).Serialize());
            client.SendPacket(packet);
        }
    }
}
