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

        /// <summary>
        /// All party members.
        /// </summary>
        public IEnumerable<Character> Members { get; private set; }

        /// <summary>
        /// Guild name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Guild remark.
        /// </summary>
        public string Message { get; private set; }

        public GuildCreateRequest(Character guildCreator, IEnumerable<Character> members, string name, string message)
        {
            GuildCreator = guildCreator;
            Members = members;
            Name = name;
            Message = message;

            foreach (var m in members)
                Acceptance.Add(m.Id, false);
        }

        public void Dispose()
        {
            GuildCreator = null;
            Acceptance.Clear();
            Acceptance = null;
        }
    }
}
