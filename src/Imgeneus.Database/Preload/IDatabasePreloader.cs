using Imgeneus.Database.Entities;
using System.Collections.Generic;

namespace Imgeneus.Database.Preload
{
    /// <summary>
    /// Database preloader loads game definitions from database, that not gonna change during game server lifetime.
    /// E.g. item definitions, mob definitions, buff/dbuff definitions etc.
    /// </summary>
    public interface IDatabasePreloader
    {
        /// <summary>
        /// Preloaded items.
        /// </summary>
        Dictionary<(byte Type, byte TypeId), DbItem> Items { get; }

        /// <summary>
        /// Preloaded skills.
        /// </summary>
        Dictionary<(ushort SkillId, byte SkillLevel), DbSkill> Skills { get; }

        /// <summary>
        /// Preloaded mobs.
        /// </summary>
        Dictionary<ushort, DbMob> Mobs { get; }

        /// <summary>
        /// Preloaded NPCs.
        /// </summary>
        Dictionary<(byte Type, ushort TypeId), DbNpc> NPCs { get; }

        /// <summary>
        /// Preloaded quests.
        /// </summary>
        Dictionary<ushort, DbQuest> Quests { get; }

        /// <summary>
        /// Preloads all needed info.
        /// </summary>
        void Preload();
    }
}
