using System;
using System.Collections.Generic;
using System.Linq;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Serialization;
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

        internal void SendActiveBuffs(WorldClient client, List<ActiveBuff> activeBuffs)
        {
            using var packet = new Packet(PacketType.CHARACTER_ACTIVE_BUFFS);
            packet.Write(new CharacterActiveBuffs(activeBuffs).Serialize());
            client.SendPacket(packet);
        }

        internal void SendNewActiveBuff(WorldClient client, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.BUFF_SELF);
            packet.Write(new SerializedActiveBuff(buff).Serialize());
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
    }
}
