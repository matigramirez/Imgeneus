using BinarySerialization;
using System.IO;

namespace Imgeneus.Network.Serialization
{
    /// <summary>
    /// Base class, that transforms object into byte array.
    /// </summary>
    public abstract class BaseSerializable
    {
        public byte[] Serialize()
        {
            using var ms = new MemoryStream();
            var serializer = new BinarySerializer();
            serializer.Serialize(ms, this);
            return ms.ToArray();
        }
    }
}
