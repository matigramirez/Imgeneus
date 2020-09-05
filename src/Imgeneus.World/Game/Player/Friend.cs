using Imgeneus.Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imgeneus.World.Game.Player
{
    public class Friend
    {
        /// <summary>
        /// Character id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Friend name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Friend job.
        /// </summary>
        public CharacterProfession Job { get; private set; }

        /// <summary>
        /// Friend is online?
        /// </summary>
        public bool IsOnline { get; set; }

        public Friend(int id, string name, CharacterProfession job, bool isOnline = false)
        {
            Id = id;
            Name = name;
            Job = job;
            IsOnline = isOnline;
        }
    }
}
