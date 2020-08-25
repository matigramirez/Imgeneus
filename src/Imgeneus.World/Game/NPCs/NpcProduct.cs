namespace Imgeneus.World.Game.NPCs
{
    /// <summary>
    /// Item, that npc sells.
    /// </summary>
    public class NpcProduct
    {
        /// <summary>
        /// Item type.
        /// </summary>
        public byte Type { get; }

        /// <summary>
        /// Item type id.
        /// </summary>
        public byte TypeId { get; }

        public NpcProduct(byte type, byte typeId)
        {
            Type = type;
            TypeId = typeId;
        }
    }
}
