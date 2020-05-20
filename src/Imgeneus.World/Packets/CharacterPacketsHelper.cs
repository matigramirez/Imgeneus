using System;
using System.Collections.Generic;
using System.Linq;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Monster;
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

        internal void SendAttackStart(WorldClient client)
        {
            using var packet = new Packet(PacketType.ATTACK_START);
            client.SendPacket(packet);
        }

        internal void SendAutoAttackWrongTarget(WorldClient client, Character sender, IKillable target)
        {
            PacketType type;
            if (target is Character)
            {
                type = PacketType.CHARACTER_CHARACTER_AUTO_ATTACK;
            }
            else
            {
                type = PacketType.CHARACTER_MOB_AUTO_ATTACK;
            }

            using var packet = new Packet(type);
            packet.Write(new UsualAttack(sender.Id, 0, new AttackResult() { Success = AttackSuccess.WrongTarget }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendSkillWrongTarget(WorldClient client, Character sender, Skill skill, IKillable target)
        {
            PacketType type = target is Character ? PacketType.USE_CHARACTER_TARGET_SKILL : PacketType.USE_MOB_TARGET_SKILL;
            using var packet = new Packet(type);
            packet.Write(new SkillRange(sender.Id, 0, skill, new AttackResult() { Success = AttackSuccess.WrongTarget }).Serialize());
            client.SendPacket(packet);
        }

        internal void SendNotEnoughMPSP(WorldClient client, Character sender, IKillable target, Skill skill)
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

        internal void SendCharacterBuffs(WorldClient client, Character target)
        {
            using var packet = new Packet(PacketType.TARGET_CHARACTER_BUFFS);
            packet.Write(new TargetBuffs(target).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterShape(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_SHAPE);
            packet.Write(new CharacterShape(character).Serialize());
            client.SendPacket(packet);
        }
    }
}
