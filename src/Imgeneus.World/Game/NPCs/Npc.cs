using Imgeneus.Database.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Imgeneus.World.Game.NPCs
{
    public class Npc : IMapMember
    {
        private readonly ILogger _logger;
        private readonly DbNpc _dbNpc;

        public Npc(ILogger<Npc> logger, DbNpc dbNpc, float x, float y, float z)
        {
            _logger = logger;
            _dbNpc = dbNpc;
            PosX = x;
            PosY = y;
            PosZ = z;

            // Set products.
            var items = dbNpc.Products.Split(" | ");
            foreach (var item in items)
            {
                try
                {
                    var itemTypeAndId = item.Split(".");
                    byte itemType = byte.Parse(itemTypeAndId[0]);
                    byte itemTypeId = byte.Parse(itemTypeAndId[1]);
                    _products.Add(new NpcProduct(itemType, itemTypeId));
                }
                catch
                {
                    _logger.LogError($"Couldn't parse npc item definition, plase check this npc: {_dbNpc.Id}.");
                }
            }
        }

        public int Id { get; set; }

        /// <inheritdoc />
        public float PosX { get; set; }

        /// <inheritdoc />
        public float PosY { get; set; }

        /// <inheritdoc />
        public float PosZ { get; set; }

        /// <inheritdoc />
        public ushort Angle { get; set; }

        /// <summary>
        /// Type of NPC.
        /// </summary>
        public byte Type { get => _dbNpc.Type; }

        /// <summary>
        /// Type id of NPC.
        /// </summary>
        public ushort TypeId { get => _dbNpc.TypeId; }

        private readonly IList<NpcProduct> _products = new List<NpcProduct>();
        private IList<NpcProduct> _readonlyProducts;

        /// <summary>
        /// Items, that npc sells.
        /// </summary>
        public IList<NpcProduct> Products
        {
            get
            {
                if (_readonlyProducts is null)
                    _readonlyProducts = new ReadOnlyCollection<NpcProduct>(_products);
                return _readonlyProducts;
            }
        }

        /// <summary>
        /// Checks if Product list contains product at index. Logs warning, if product is not found.
        /// </summary>
        /// <param name="index">index, that we want to check.</param>
        /// <returns>return true, if there is some product at index</returns>
        public bool ContainsProduct(byte index)
        {
            if (Products.Count <= index)
            {
                _logger.LogWarning($"NPC {_dbNpc.Id} doesn't contain product at index {index}. Check it out.");
                return false;
            }

            return true;
        }
    }
}
