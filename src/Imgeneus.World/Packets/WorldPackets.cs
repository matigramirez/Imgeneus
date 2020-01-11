using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imgeneus.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendSelectedCharacter(WorldClient client, DbCharacter character)
        {
            using var packet = new Packet(PacketType.SELECT_CHARACTER);
            packet.WriteByte(0); // ok response
            packet.Write(character.Id);
            client.SendPacket(packet);

            SendCharacterDetails(client, character);
        }

        private static void SendCharacterDetails(WorldClient client, DbCharacter character)
        {
            using var packet = new Packet(PacketType.CHARACTER_DETAILS);

            packet.Write(character.Strength);
            packet.Write(character.Dexterity);
            packet.Write(character.Rec);
            packet.Write(character.Intelligence);
            packet.Write(character.Wisdom);
            packet.Write(character.Luck);
            packet.Write(character.StatPoint);
            packet.Write(character.SkillPoint);

            packet.Write(100); // Max HP
            packet.Write(200); // Max MP
            packet.Write(300); // Max SP

            packet.Write(character.Angle);

            // Client should display 200 / 1500 for EXP
            packet.Write(100); // EXP Values are multiplied by 10
            packet.Write(250); // Next EXP is at 2500. Client takes the previous value, calculates the difference = 1500
            packet.Write(120); // Current EXP is at 1200

            packet.Write(character.Gold);

            packet.Write(character.PosX);
            packet.Write(character.PosY);
            packet.Write(character.PosZ);

            packet.Write((int)character.Kills);
            packet.Write((int)character.Deaths);
            packet.Write((int)character.Victories);
            packet.Write((int)character.Defeats);

            if (true) // Has guild?
            {
                packet.Write(true);
                packet.WriteString("TestGuild");
            }
            else // no guild
            {
                packet.Write(false);
            }

            // Write here guild name
            client.SendPacket(packet);
        }
    }
}
