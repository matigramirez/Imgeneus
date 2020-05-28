using System;
using Imgeneus.Database.Constants;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;

namespace Imgeneus.World.Packets
{
    /// <summary>
    /// Helps to creates packets and send them to players.
    /// </summary>
    internal class MapPacketsHelper
    {
        internal void SendCharacterLeftMap(WorldClient client, Character removedCharacter)
        {
            using var packet = new Packet(PacketType.CHARACTER_LEFT_MAP);
            packet.Write(removedCharacter.Id);
            client.SendPacket(packet);
        }

        internal void SendCharacterMoves(WorldClient client, Character movedPlayer)
        {
            using var packet = new Packet(PacketType.CHARACTER_MOVE);
            packet.Write(new CharacterMove(movedPlayer).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterMotion(WorldClient client, int characterId, Motion motion)
        {
            using var packet = new Packet(PacketType.CHARACTER_MOTION);
            packet.Write(characterId);
            packet.WriteByte((byte)motion);
            client.SendPacket(packet);
        }

        internal void SendCharacterUsedSkill(WorldClient client, Character sender, IKillable target, Skill skill, AttackResult attackResult)
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

        internal void SendCharacterConnectedToMap(WorldClient client, Character character)
        {
            using var packet0 = new Packet(PacketType.CHARACTER_ENTERED_MAP);
            packet0.Write(new CharacterEnteredMap(character).Serialize());
            client.SendPacket(packet0);

            using var packet1 = new Packet(PacketType.CHARACTER_MOVE);
            packet1.Write(new CharacterMove(character).Serialize());
            client.SendPacket(packet1);
        }

        internal void SendMobEntered(WorldClient client, Mob mob)
        {
            using var packet = new Packet(PacketType.MOB_ENTER);
            packet.Write(new MobEnter(mob).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMobMove(WorldClient client, Mob mob)
        {
            using var packet = new Packet(PacketType.MOB_MOVE);
            packet.Write(new MobMove(mob).Serialize());
            client.SendPacket(packet);
        }

        internal void SendMobAttack(WorldClient client, Mob mob, int targetId)
        {
            using var packet = new Packet(PacketType.MOB_ATTACK);
            packet.Write(new MobAttack(mob, targetId).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterChangedEquipment(WorldClient client, int characterId, Item equipmentItem, byte slot)
        {
            using var packet = new Packet(PacketType.SEND_EQUIPMENT);
            packet.Write(characterId);

            packet.WriteByte(slot);
            packet.WriteByte(equipmentItem is null ? (byte)0 : equipmentItem.Type);
            packet.WriteByte(equipmentItem is null ? (byte)0 : equipmentItem.TypeId);
            packet.WriteByte(equipmentItem is null ? (byte)0 : (byte)20); // TODO: implement enchant here.

            if (equipmentItem != null && equipmentItem.IsCloakSlot)
            {
                for (var i = 0; i < 6; i++) // Something about cloak.
                {
                    packet.WriteByte(0);
                }
            }

            client.SendPacket(packet);
        }

        internal void SendCharacterUsualAttack(WorldClient client, Character sender, IKillable target, AttackResult attackResult)
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

        internal void SendCharacterPartyChanged(WorldClient client, int characterId, PartyMemberType type)
        {
            using var packet = new Packet(PacketType.MAP_PARTY_SET);
            packet.Write(characterId);
            packet.Write((byte)type);
            client.SendPacket(packet);
        }

        internal void SendAttackAndMovementSpeed(WorldClient client, Character sender)
        {
            using var packet = new Packet(PacketType.CHARACTER_ATTACK_MOVEMENT_SPEED);
            packet.Write(new CharacterAttackAndMovement(sender).Serialize());
            client.SendPacket(packet);
        }

        internal void SendCharacterKilled(WorldClient client, Character character, IKiller killer)
        {
            using var packet = new Packet(PacketType.CHARACTER_DEATH);
            packet.Write(character.Id);
            packet.WriteByte(1); // killer type. 1 - another player.
            packet.Write(killer.Id);
            client.SendPacket(packet);
        }

        internal void SendCharacterAddedBuff(WorldClient client, Character sender, IKillable receiver, ActiveBuff buff)
        {
            using var packet = new Packet(PacketType.CHARACTER_ADDED_BUFF);
            packet.Write(false); // IsFailed?
            packet.Write(sender.Id);
            packet.Write(receiver.Id);
            packet.Write(buff.SkillId);
            packet.Write(buff.SkillLevel);
            client.SendPacket(packet);
        }

        internal void SendSkillCastStarted(WorldClient client, Character sender, IKillable target, Skill skill)
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

        internal void SendUsedItem(WorldClient client, Character sender, Item item)
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

        internal void SendRecoverCharacter(WorldClient client, Character sender)
        {
            using var packet = new Packet(PacketType.CHARACTER_RECOVER);
            packet.Write(sender.Id);
            packet.Write(sender.CurrentHP);
            packet.Write(sender.CurrentMP);
            packet.Write(sender.CurrentSP);
            client.SendPacket(packet);
        }

        internal void Send_Max_HP(WorldClient client, int id, int value)
        {
            using var packet = new Packet(PacketType.CHARACTER_MAX_HITPOINTS);
            packet.Write(id);
            packet.WriteByte(0); // 0 means max hp type.
            packet.Write(value);
            client.SendPacket(packet);
        }
    }
}
