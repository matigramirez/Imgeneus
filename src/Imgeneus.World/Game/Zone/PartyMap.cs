using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;
using System;

namespace Imgeneus.World.Game.Zone
{
    /// <summary>
    /// Instance map, only for parties.
    /// </summary>
    public class PartyMap : Map, IPartyMap
    {
        private readonly IParty _party;

        /// <inheritdoc/>
        public override bool IsInstance { get => true; }

        /// <inheritdoc/>
        public Guid PartyId => _party.Id;

        /// <inheritdoc/>
        public event Action<IPartyMap> OnAllMembersLeft;

        public PartyMap(IParty party, ushort id, MapDefinition definition, MapConfiguration config, ILogger<Map> logger, IDatabasePreloader databasePreloader, IMobFactory mobFactory, INpcFactory npcFactory, IObeliskFactory obeliskFactory)
            : base(id, definition, config, logger, databasePreloader, mobFactory, npcFactory, obeliskFactory)
        {
            _party = party;
            _party.AllMembersLeft += Party_AllMembersLeft;
        }

        private void Party_AllMembersLeft()
        {
            _party.AllMembersLeft -= Party_AllMembersLeft;
            if (Players.Count == 0)
            {
                OnAllMembersLeft?.Invoke(this);
                Dispose();
            }
        }

        public override bool UnloadPlayer(Character character)
        {
            var result = base.UnloadPlayer(character);

            if (_party.Members.Count <= 1 && Players.Count == 0)
            {
                OnAllMembersLeft?.Invoke(this);
                Dispose();
            }

            return result;
        }
    }
}
