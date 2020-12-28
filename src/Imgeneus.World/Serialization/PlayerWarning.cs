using BinarySerialization;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class PlayerWarning : BaseSerializable
    {
        [FieldOrder(0)]
        public byte MessageLength;

        [FieldOrder(1)]
        public string Message;

        public PlayerWarning(string message)
        {
            MessageLength = (byte)message.Length;
            Message = message;
        }
    }
}