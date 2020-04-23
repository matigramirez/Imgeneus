using Imgeneus.Network.Data;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.Network.Packets.Game
{
    public struct SkillBarPacket : IDeserializedPacket
    {
        public QuickSkillBarItem[] QuickItems;

        public SkillBarPacket(IPacketStream packet)
        {
            var count = packet.Read<byte>();
            var unknown = packet.Read<int>();

            QuickItems = new QuickSkillBarItem[count - 1];

            for (var i = 0; i < count - 1; i++)
            {
                var bar = packet.Read<byte>();
                var slot = packet.Read<byte>();
                var bag = packet.Read<byte>();
                var number = packet.Read<ushort>();
                var unknown2 = packet.Read<int>(); // cooldown?

                QuickItems[i] = new QuickSkillBarItem(bar, slot, bag, number);
            }

            // There are still 5 bytes after all. But they are always the same.
            // These are 21, 1, 255, 0, 0. I leave it unimplemented, since i have no idea why they are needed.
        }
    }
}
