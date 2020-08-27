using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        protected override IList<Item> GenerateDrop(IKiller killer)
        {
            if (killer is Mob || killer is Npc)
                return new List<Item>();

            // TODO: generate drop, if character was killed by another character.
            return new List<Item>();
        }
    }
}
