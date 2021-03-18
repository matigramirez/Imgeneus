using Imgeneus.World.Game.Player;
using System;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Guild
{
    /// <summary>
    /// Request, that is sent to party members, when player wants to create guild.
    /// </summary>
    public class GuildCreateRequest : IDisposable
    {
        /// <summary>
        /// Acceptance of all party members.
        /// Key is character id.
        /// Value if accepted or not.
        /// </summary>
        public Dictionary<int, bool> Acceptance { get; private set; } = new Dictionary<int, bool>();

        /// <summary>
        /// Initiator of request.
        /// </summary>
        public Character GuildCreator { get; private set; }

        public GuildCreateRequest(Character guildCreator, IEnumerable<Character> members)
        {
            GuildCreator = guildCreator;

            foreach (var m in members)
                Acceptance.Add(m.Id, false);

            Acceptance[guildCreator.Id] = true;
        }

        public void Dispose()
        {
            GuildCreator = null;
            Acceptance.Clear();
            Acceptance = null;
        }
    }
}
