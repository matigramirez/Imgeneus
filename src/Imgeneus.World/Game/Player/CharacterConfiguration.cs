using System.Linq;
using Imgeneus.Core.Helpers;
using Imgeneus.Database.Entities;
using Newtonsoft.Json;

namespace Imgeneus.World.Game.Player
{
    public sealed class CharacterConfiguration : ICharacterConfiguration
    {
        private const string CharacterConfigFile = "config/character.json";

        public static CharacterConfiguration LoadFromConfigFile()
        {
            return ConfigurationHelper.Load<CharacterConfiguration>(CharacterConfigFile);
        }

        /// <summary>
        /// Config for each job and level.
        /// </summary>
        public Character_HP_SP_MP[] Configs { get; set; }

        /// <summary>
        /// Default starting stats.
        /// </summary>
        public DefaultStat[] DefaultStats { get; set; }

        /// <summary>
        /// Default maximum level for each mode
        /// </summary>
        public DefaultMaxLevel[] DefaultMaxLevels { get; set; }

        /// <summary>
        /// Default stat and skill points received per level
        /// </summary>
        public DefaultLevelStatSkillPoints[] DefaultLevelStatSkillPoints { get; set; }

        /// <summary>
        /// Gets hp, mp, sp config by index calculation.
        /// </summary>
        public Character_HP_SP_MP GetConfig(int index)
        {
            if (Configs.Length < index)
            {
                return Configs[index];
            }
            else
            {
                return Configs[Configs.Length - 1];
            }
        }

        public DefaultMaxLevel GetMaxLevelConfig(Mode mode) => DefaultMaxLevels.FirstOrDefault(dml => dml.Mode == mode);

        public DefaultLevelStatSkillPoints GetLevelStatSkillPoints(Mode mode) => DefaultLevelStatSkillPoints.FirstOrDefault(dsp => dsp.Mode == mode);
    }

    public interface ICharacterConfiguration
    {
        /// <summary>
        /// Config for each job and level.
        /// </summary>
        public Character_HP_SP_MP[] Configs { get; }

        /// <summary>
        /// Default starting stats.
        /// </summary>
        public DefaultStat[] DefaultStats { get; set; }

        /// <summary>
        /// Default maximum level for each mode
        /// </summary>
        public DefaultMaxLevel[] DefaultMaxLevels { get; set; }

        /// <summary>
        /// Default stat and skill points received per level
        /// </summary>
        public DefaultLevelStatSkillPoints[] DefaultLevelStatSkillPoints { get; set; }

        public Character_HP_SP_MP GetConfig(int index);

        public DefaultMaxLevel GetMaxLevelConfig(Mode mode);

        public DefaultLevelStatSkillPoints GetLevelStatSkillPoints(Mode mode);
    }

    public sealed class Character_HP_SP_MP
    {
        /// <summary>
        /// Character level.
        /// </summary>
        [JsonProperty("Level")]
        public int Level { get; set; }

        /// <summary>
        /// Character job.
        /// </summary>
        [JsonProperty("Job")]
        public CharacterProfession Job { get; set; }

        /// <summary>
        /// Const HP.
        /// </summary>
        [JsonProperty("HP")]
        public int HP { get; set; }

        /// <summary>
        /// Const SP.
        /// </summary>
        [JsonProperty("HP")]
        public int SP { get; set; }

        /// <summary>
        /// Const MP.
        /// </summary>
        [JsonProperty("MP")]
        public int MP { get; set; }
    }
}
