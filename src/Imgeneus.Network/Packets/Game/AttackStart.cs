using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct AttackStart : IDeserializedPacket
    {
        public AttackStart(IPacketStream packet)
        {
            // Looks like this packet client sends in order to get permission to attack some target(mob?).
        }
    }
}
