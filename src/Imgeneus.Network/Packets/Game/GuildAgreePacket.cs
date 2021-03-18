using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildAgreePacket : IDeserializedPacket
    {
        /// <summary>
        /// Player agrees to create a guild.
        /// </summary>
        public bool Ok { get; }

        public GuildAgreePacket(IPacketStream packet)
        {
            Ok = packet.Read<bool>();
        }
    }
}
