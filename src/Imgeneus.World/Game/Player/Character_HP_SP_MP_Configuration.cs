using Imgeneus.Database.Entities;
using Newtonsoft.Json;

namespace Imgeneus.World.Game.Player
{
    public sealed class Character_HP_SP_MP_Configuration
    {
        /// <summary>
        /// Config for each job and level.
        /// </summary>
        [JsonProperty("Configs")]
        public Character_HP_SP_MP[] Configs { get; set; }
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
