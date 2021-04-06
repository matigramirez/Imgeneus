namespace Imgeneus.World.Game.Guild
{
    public class GuildHouseNpcInfo
    {
        /// <summary>
        /// NPC type, e.g. Merchant is 1, Gate keeper is 2, Blacksmith is 3, Warehouse is 6.
        /// </summary>
        public byte NpcType { get; set; }

        /// <summary>
        /// NPC type id, only for lights.
        /// </summary>
        public ushort LightNpcTypeId { get; set; }

        /// <summary>
        /// NPC type id, only for darks.
        /// </summary>
        public ushort DarkNpcTypeId { get; set; }

        /// <summary>
        /// NPC group. Works only for Merchants. Like weapons, accessories etc.
        /// </summary>
        public byte Group { get; set; }

        /// <summary>
        /// NPC level.
        /// </summary>
        public byte NpcLvl { get; set; }

        /// <summary>
        /// Upgrade price for the next level.
        /// </summary>
        public ushort UpPrice { get; set; }

        /// <summary>
        /// Decreases price in %. E.g. weapons merchant sells items for less price.
        /// </summary>
        public byte PriceRate { get; set; }

        /// <summary>
        /// Used only for Blacksmith. % for linkings/extracting gems.
        /// </summary>
        public byte RapiceMixPercentRate { get; set; }

        /// <summary>
        /// Used only for Blacksmith. Reduces cost for repair/link items.
        /// </summary>
        public byte RapiceMixDecreRate { get; set; }

        /// <summary>
        /// Guild's min rank in order to upgrade/use npc.
        /// </summary>
        public byte MinRank { get; set; }
    }
}
