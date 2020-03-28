using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MoveCharacterPacket
    {
        /// <summary>
        /// If it's 132 character stopped moving, if it's 129 character is continuously moving.
        /// </summary>
        public MovementType MovementType { get; }

        public ushort Angle { get; }

        public float X { get; }

        public float Y { get; }

        public float Z { get; }

        public MoveCharacterPacket(IPacketStream packet)
        {
            MovementType = (MovementType)packet.Read<byte>();
            Angle = packet.Read<ushort>();
            X = packet.Read<float>();
            Y = packet.Read<float>();
            Z = packet.Read<float>();
        }
    }

    public enum MovementType : byte
    {
        Stopped = 132,
        Moving = 129
    }
}
