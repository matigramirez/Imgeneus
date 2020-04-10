using System;
using System.Linq;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendSelectedCharacter(WorldClient client, Character character)
        {
            client.CharID = character.Id;

            using var packet = new Packet(PacketType.SELECT_CHARACTER);
            packet.WriteByte(0); // ok response
            packet.Write(character.Id);
            client.SendPacket(packet);

            SendCharacterDetails(client, character);
            SendCharacterItems(client, character.InventoryItems);
            SendLearnedSkills(client, character);
            SendActiveBuffs(client, character);
            SendBlessAmount(client);
        }

        private static void SendCharacterDetails(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_DETAILS);
            var bytes = new CharacterDetails(character).Serialize();
            packet.Write(bytes);
            client.SendPacket(packet);
        }

        private static void SendBlessAmount(WorldClient client)
        {
            using var packet = new Packet(PacketType.BLESS_AMOUNT);
            packet.Write((byte)Fraction.Light);

            // Set bless amount to anu of these values.
            // 150  + sp, mp during break
            // 300  + xp during break
            // 1200 + link and extrack lapis 
            // 1350 + cast time of dispoable items
            // 1500 + exp loss
            // 2100 + shooting/magic defence power
            // 2250 + physical defence power
            // 2700 + repair cost
            // 8400 + sp, mp during battle
            // 10200 + max sp, mp
            // 12000 + increase in all stats (str, dex rec etc.)
            // 12288 + full bless: increase in critical hit rate, evasion of all attacks (shooting/magic/physical)

            var blessAmount = 12288;
            packet.Write(blessAmount);

            if (blessAmount >= 12288)
            {
                // Bless duration is 10 minutes.
                var fullBlessDuration = TimeSpan.FromMinutes(10);
                var timeElapsed = TimeSpan.FromMinutes(5);

                // Remaning time in milliseconds.
                uint remainingTime = (uint)(fullBlessDuration - timeElapsed).TotalMilliseconds;
                packet.Write(remainingTime);
            }
            else
            {
                // Write no remaing time if it's not full bless.
                packet.Write(0);
            }

            client.SendPacket(packet);
        }

        public static void CharacterConnectedToMap(WorldClient client, Character character)
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

        public static void CharacterLeftMap(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_LEFT_MAP);
            packet.Write(character.Id);
            client.SendPacket(packet);
        }

        public static void CharacterMoves(WorldClient client, Character character)
        {
            using var packet = new Packet(PacketType.CHARACTER_MOVE);
            packet.Write(new CharacterMove(character).Serialize());
            client.SendPacket(packet);
        }

        public static void CharacterMotion(WorldClient client, int characterId, Motion motion)
        {
            using var packet = new Packet(PacketType.CHARACTER_MOTION);
            packet.Write(characterId);
            packet.WriteByte((byte)motion);
            client.SendPacket(packet);
        }
    }
}
