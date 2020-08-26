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
using Imgeneus.World.Serialization;

namespace Imgeneus.World.Packets
{
    internal class CharacterPacketsHelper
    {
        internal void SendInventoryItems(WorldClient client, ICollection<Item> inventoryItems)
        {
            using var packet = new Packet(PacketType.CHARACTER_ITEMS);
            packet.Write(new InventoryItems(inventoryItems).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCurrentHitpoints(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_CURRENT_HITPOINTS);
            packet.Write(new CharacterHitpoints(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendDetails(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_DETAILS);
            packet.Write(new CharacterDetails(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendLearnedNewSkill(WorldClient client, Skill skill)
        {
            using var answerPacket = new Packet(PacketType.LEARN_NEW_SKILL);
            answerPacket.Write(new LearnedSkill(skill).Serialize());
            client.SendPacket(answerPacket);
        }

        internal void SendLearnedSkills(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SKILLS);
            packet.Write(new CharacterSkills(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendActiveBuffs(WorldClient client, ICollection<ActiveBuff> activeBuffs)
        {
            using var packet = new Packet(PacketType.CHARACTER_ACTIVE_BUFFS);
            packet.Write(new CharacterActiveBuffs(activeBuffs).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAddBuff(WorldClient client, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.BUFF_ADD);
            packet.Write(new SerializedActiveBuff(buff).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRemoveBuff(WorldClient client, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.BUFF_REMOVE);
            packet.Write(buff.Id);
            client.SendPacket(packet);
        }

        internal void SendMoveItemInInventory(WorldClient client, Item sourceItem, Item destinationItem)
        {
            // Send move item.
            using var packet = new Packet(PacketType.INVENTORY_MOVE_ITEM);

            var bytes = new MovedItem(sourceItem).Serialize();
            packet.Write(bytes);

            bytes = new MovedItem(destinationItem).Serialize();
            packet.Write(bytes);

            client.SendPacket(packet);
        }

        internal void SetMobInTarget(WorldClient client, Mob target)
        {
            using var packet = new Packet(PacketType.TARGET_SELECT_MOB);
            packet.Write(new MobInTarget(target).Serialize());
            client.SendPacket(packet);
        }

        internal void SetPlayerInTarget(WorldClient client, Character target)
        {
            using var packet = new Packet(PacketType.TARGET_SELECT_CHARACTER);
            packet.Write(new CharacterInTarget(target).Serialize());
            client.SendPacket(packet);
        }

        internal void SendSkillBar(WorldClient client, IEnumerable<DbQuickSkillBarItem> quickItems)
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

        internal void SendAddItem(WorldClient client, Item item)
        {
            using var packet = new Packet(PacketType.ADD_ITEM);
            packet.Write(new AddedInventoryItem(item).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRemoveItem(WorldClient client, Item item, bool fullRemove)
        {
            using var packet = new Packet(PacketType.REMOVE_ITEM);
            packet.Write(new RemovedInventoryItem(item, fullRemove).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAdditionalStats(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_ADDITIONAL_STATS);
            packet.Write(new CharacterAdditionalStats(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMaxHitpoints(WorldClient client, Character character, HitpointType type)
        {
            using var packet = new Packet(PacketType.CHARACTER_MAX_HITPOINTS);
            packet.Write(new MaxHitpoint(character, type).Serialize());
            client.SendPacket(packet);
        }

        internal void SendPartyInfo(WorldClient client, IEnumerable<Character> partyMembers, byte leaderIndex)
        {
            using var packet = new Packet(PacketType.PARTY_LIST);
            packet.Write(new UsualParty(partyMembers, leaderIndex).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRaidInfo(WorldClient client, Raid raid)
        {
            using var packet = new Packet(PacketType.RAID_LIST);
            packet.Write(new RaidParty(raid).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAttackStart(WorldClient client)
        {
            using var packet = new Packet(PacketType.ATTACK_START);
            client.SendPacket(packet);
        }

        internal void SendAutoAttackWrongTarget(WorldClient client, Character sender, IKillable target)
        {
            PacketType type = target is Character ? PacketType.CHARACTER_CHARACTER_AUTO_ATTACK : PacketType.CHARACTER_MOB_AUTO_ATTACK;
            using var packet = new Packet(type);
            packet.Write(new UsualAttack(sender.Id, 0, new AttackResult() { Success = AttackSuccess.WrongTarget }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendAutoAttackCanNotAttack(WorldClient client, Character sender, IKillable target)
        {
            PacketType type = target is Character ? PacketType.CHARACTER_CHARACTER_AUTO_ATTACK : PacketType.CHARACTER_MOB_AUTO_ATTACK;
            using var packet = new Packet(type);
            packet.Write(new UsualAttack(sender.Id, 0, new AttackResult() { Success = AttackSuccess.CanNotAttack }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendItemDoesNotBelong(WorldClient client)
        {
            using var packet = new Packet(PacketType.ADD_ITEM);
            packet.WriteByte(0);
            packet.WriteByte(0); // Item doesn't belong to player.
            client.SendPacket(packet);
        }

        internal void SendFullInventory(WorldClient client)
        {
            using var packet = new Packet(PacketType.ADD_ITEM);
            packet.WriteByte(0);
            packet.WriteByte(1); // Inventory is full.
            client.SendPacket(packet);
        }

        internal void SendBoughtItem(WorldClient client, Item boughtItem, uint gold)
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

        internal void SendSoldItem(WorldClient client, Item soldItem, uint gold)
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

        internal void SendBuyItemIssue(WorldClient client, byte issue)
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

        internal void SendSkillWrongTarget(WorldClient client, Character sender, Skill skill, IKillable target)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.WrongTarget }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendSkillAttackCanNotAttack(WorldClient client, Character sender, Skill skill, IKillable target)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.CanNotAttack }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendGmCommandSuccess(WorldClient client)
        {
            using var packet = new Packet(PacketType.GM_CMD_ERROR);
            packet.Write<ushort>(0); // 0 == no error
            client.SendPacket(packet);
        }

        internal void SendGmCommandError(WorldClient client, PacketType error)
        {
            using var packet = new Packet(PacketType.GM_CMD_ERROR);
            packet.Write((ushort)error);
            client.SendPacket(packet);
        }

        internal void SendGmTeleport(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.GM_TELEPORT);
            packet.Write(character.Id);
            packet.Write(character.Map.Id);
            packet.Write(character.PosX);
            packet.Write(character.PosY);
            packet.Write(character.PosZ);
            client.SendPacket(packet);
        }

        internal void SendSkillWrongEquipment(WorldClient client, Character sender, IKillable target, Skill skill)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.WrongEquipment }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendNotEnoughMPSP(WorldClient client, Character sender, IKillable target, Skill skill)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.NotEnoughMPSP }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCooldownNotOver(WorldClient client, Character sender, IKillable target, Skill skill)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.NotEnoughMPSP }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendUseSMMP(WorldClient client, ushort MP, ushort SP)
        {
            using var packet = new Packet(PacketType.USED_SP_MP);
            packet.Write(new UseSPMP(SP, MP).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMoveAndAttackSpeed(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_ATTACK_MOVEMENT_SPEED);
            packet.Write(new CharacterAttackAndMovement(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCurrentBuffs(WorldClient client, IKillable target)
        {
            using var packet = new Packet(PacketType.TARGET_BUFFS);
            packet.Write(new TargetBuffs(target).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterShape(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SHAPE);
            packet.Write(new CharacterShape(character).Serialize());
            client.SendPacket(packet);
        }

        internal void SendRunMode(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.RUN_MODE);
            packet.Write(character.MoveMotion);
            client.SendPacket(packet);
        }

        internal void SendTargetAddBuff(WorldClient client, IKillable target, ActiveBuff buff)
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

        internal void SendTargetRemoveBuff(WorldClient client, IKillable target, ActiveBuff buff)
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

        internal void SendQuests(WorldClient client, IEnumerable<Quest> quests)
        {
            using var packet = new Packet(PacketType.QUEST_LIST);
            packet.Write(new CharacterQuests(quests).Serialize());
            client.SendPacket(packet);
        }

        internal void SendFinishedQuests(WorldClient client, IEnumerable<Quest> quests)
        {
            using var packet = new Packet(PacketType.QUEST_FINISHED_LIST);
            packet.Write(new CharacterFinishedQuests(quests).Serialize());
            client.SendPacket(packet);
        }
    }
}
