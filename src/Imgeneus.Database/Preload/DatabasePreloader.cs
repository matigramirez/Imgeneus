using Imgeneus.Database.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Imgeneus.Database.Preload
{
    /// <inheritdoc />
    public class DatabasePreloader : IDatabasePreloader
    {
        private readonly ILogger<DatabasePreloader> _logger;
        private readonly IDatabase _database;

        /// <inheritdoc />
        public Dictionary<(byte Type, byte TypeId), DbItem> Items { get; private set; } = new Dictionary<(byte Type, byte TypeId), DbItem>();

        /// <inheritdoc />
        public Dictionary<ushort, List<DbItem>> ItemsByGrade { get; } = new Dictionary<ushort, List<DbItem>>();

        /// <inheritdoc />
        public Dictionary<(ushort SkillId, byte SkillLevel), DbSkill> Skills { get; private set; } = new Dictionary<(ushort SkillId, byte SkillLevel), DbSkill>();

        /// <inheritdoc />
        public Dictionary<ushort, DbMob> Mobs { get; private set; } = new Dictionary<ushort, DbMob>();

        /// <inheritdoc />
        public Dictionary<(ushort MobId, byte ItemOrder), DbMobItems> MobItems { get; private set; } = new Dictionary<(ushort MobId, byte ItemOrder), DbMobItems>();

        /// <inheritdoc />
        public Dictionary<(byte Type, ushort TypeId), DbNpc> NPCs { get; private set; } = new Dictionary<(byte Type, ushort TypeId), DbNpc>();

        /// <inheritdoc />
        public Dictionary<ushort, DbQuest> Quests { get; private set; } = new Dictionary<ushort, DbQuest>();

        public DatabasePreloader(ILogger<DatabasePreloader> logger, IDatabase database)
        {
            _logger = logger;
            _database = database;

            Preload();
        }

        /// <summary>
        /// Preloads all needed game definitions from database.
        /// </summary>
        private void Preload()
        {
            try
            {
                PreloadItems(_database);
                PreloadSkills(_database);
                PreloadMobs(_database);
                PreloadMobItems(_database);
                PrealodNpcs(_database);
                PreloadQuests(_database);

                _logger.LogInformation("Database was successfully preloaded.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during preloading database: {ex.Message}");
            }

        }

        /// <summary>
        /// Preloads all available items from database.
        /// </summary>
        private void PreloadItems(IDatabase database)
        {
            var items = database.Items;
            foreach (var item in items)
            {
                Items.Add((item.Type, item.TypeId), item);
                if (ItemsByGrade.ContainsKey(item.Grade))
                {
                    ItemsByGrade[item.Grade].Add(item);
                }
                else
                {
                    ItemsByGrade.Add(item.Grade, new List<DbItem>() { item });
                }
            }
        }

        /// <summary>
        /// Preloads all available skills from database.
        /// </summary>
        private void PreloadSkills(IDatabase database)
        {
            var skills = database.Skills;
            foreach (var skill in skills)
            {
                Skills.Add((skill.SkillId, skill.SkillLevel), skill);
            }
        }

        /// <summary>
        /// Preloads all available mobs from database.
        /// </summary>
        private void PreloadMobs(IDatabase database)
        {
            var mobs = database.Mobs;
            foreach (var mob in mobs)
            {
                Mobs.Add(mob.Id, mob);
            }
        }

        /// <summary>
        /// Preloads all available mob drops from database.
        /// </summary>
        private void PreloadMobItems(IDatabase database)
        {
            var mobItems = database.MobItems;
            foreach (var item in mobItems)
            {
                MobItems.Add((item.MobId, item.ItemOrder), item);
            }
        }

        /// <summary>
        /// Preloads all available npcs from database.
        /// </summary>
        private void PrealodNpcs(IDatabase database)
        {
            var npcs = database.Npcs;
            foreach (var npc in npcs)
            {
                NPCs.Add((npc.Type, npc.TypeId), npc);
            }
        }

        /// <summary>
        /// Preloads all available quests from database.
        /// </summary>
        private void PreloadQuests(IDatabase database)
        {
            var quests = database.Quests;
            foreach (var quest in quests)
            {
                Quests.Add(quest.Id, quest);
            }
        }
    }
}
