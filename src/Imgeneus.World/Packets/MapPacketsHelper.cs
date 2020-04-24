using System;
using Imgeneus.Database.Constants;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Monster;
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

        internal void SendCharacterUsedSkill(WorldClient client, PacketType skillType, int senderId, int targetId, Skill skill, Damage damage)
        {
            Packet packet = new Packet(skillType);
            packet.Write(new SkillRange(true, senderId, targetId, skill, new ushort[3] { damage.HP, damage.SP, damage.MP }).Serialize());
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

            using var packet2 = new Packet(PacketType.CHARACTER_SHAPE);
            packet2.Write(new CharacterShape(character).Serialize());
            client.SendPacket(packet2);
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

        internal void SendCharacterChangedEquipment(WorldClient client, int characterId, Item equipmentItem)
        {
            using var packet = new Packet(PacketType.SEND_EQUIPMENT);
            packet.Write(characterId);

            packet.WriteByte(equipmentItem.Slot);
            packet.WriteByte(equipmentItem.Type);
            packet.WriteByte(equipmentItem.TypeId);
            packet.WriteByte(20); // TODO: implement enchant here.

            if (equipmentItem.IsCloakSlot)
            {
                for (var i = 0; i < 6; i++) // Something about cloak.
                {
                    packet.WriteByte(0);
                }
            }

            client.SendPacket(packet);
        }
    }
}
