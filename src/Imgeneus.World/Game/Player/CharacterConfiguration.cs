using Imgeneus.Database.Entities;
using Newtonsoft.Json;

namespace Imgeneus.World.Game.Player
{
    public sealed class CharacterConfiguration : ICharacterConfiguration
    {
        /// <summary>
        /// Config for each job and level.
        /// </summary>
        public Character_HP_SP_MP[] Configs { get; set; }

        /// <summary>
        /// Default starting stats.
        /// </summary>
        public DefaultStat[] DefaultStats { get; set; }

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

        public Character_HP_SP_MP GetConfig(int index);
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
