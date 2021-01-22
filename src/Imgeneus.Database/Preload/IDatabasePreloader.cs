using Imgeneus.Database.Entities;
using System.Collections.Generic;

namespace Imgeneus.Database.Preload
{
    /// <summary>
    /// Database preloader loads game definitions from database, that not gonna change during game server lifetime.
    /// E.g. item definitions, mob definitions, buff/debuff definitions etc.
    /// </summary>
    public interface IDatabasePreloader
    {
        /// <summary>
        /// Preloaded items.
        /// </summary>
        Dictionary<(byte Type, byte TypeId), DbItem> Items { get; }

        /// <summary>
        /// Preloaded items based by grade.
        /// </summary>
        Dictionary<ushort, List<DbItem>> ItemsByGrade { get; }

        /// <summary>
        /// Preloaded skills.
        /// </summary>
        Dictionary<(ushort SkillId, byte SkillLevel), DbSkill> Skills { get; }

        /// <summary>
        /// Preloaded mobs.
        /// </summary>
        Dictionary<ushort, DbMob> Mobs { get; }

        /// <summary>
        /// Preloaded mob items.
        /// </summary>
        Dictionary<(ushort MobId, byte ItemOrder), DbMobItems> MobItems { get; }

        /// <summary>
        /// Preloaded NPCs.
        /// </summary>
        Dictionary<(byte Type, ushort TypeId), DbNpc> NPCs { get; }

        /// <summary>
        /// Preloaded quests.
        /// </summary>
        Dictionary<ushort, DbQuest> Quests { get; }

        /// <summary>
        /// Preloaded levels.
        /// </summary>
        Dictionary<(Mode Mode, ushort Level), DbLevel> Levels { get; }
    }
}
