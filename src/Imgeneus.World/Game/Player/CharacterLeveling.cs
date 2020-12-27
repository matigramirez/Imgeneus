using System;
using System.Linq;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Imgeneus.Network.Packets.Game;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Minimum experience needed for current player's level
        /// </summary>
        public uint MinLevelExp => Level > 1 ? _databasePreloader.Levels[(Mode, (ushort) (Level - 1))].Exp : 0;

        /// <summary>
        /// Experience needed to level up to next level
        /// </summary>
        public uint NextLevelExp => _databasePreloader.Levels[(Mode, Level)].Exp;

        /// <summary>
        /// Event that's fired when a player level's up
        /// </summary>
        public event Action<Character> OnLevelUp;

        // Event that's fired when an admin set's a player's level
        public event Action<Character> OnAdminLevelUp;

        /// <summary>
        /// Sets character's new level.
        /// </summary>
        private void SetLevel(ushort newLevel)
        {
            Level = newLevel;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_LEVEL, Id, Level);
        }

        /// <summary>
        /// Attempts to set a new level for a character
        /// </summary>
        /// <param name="newLevel">New player level</param>
        /// <returns>Success status indicating whether it's possible to set the new level or not.</returns>
        private bool TrySetLevel(ushort newLevel)
        {
            if (Level == newLevel)
                return false;

            // Check minimum level boundary
            if (newLevel < 1)
                return false;

            // Check maximum level boundary
            var maxLevel = _characterConfig.GetMaxLevelConfig(Mode).Level;

            if (newLevel > maxLevel) return false;

            SetLevel(newLevel);

            return true;
        }

        /// <summary>
        /// Attempts to set a new level for a character and handles the levelling logic (exp, stat points, skill points, etc)
        /// </summary>
        /// <param name="newLevel">New player level</param>
        /// <param name="changedByAdmin">Indicates whether the level change was issued by an admin or not.</param>
        /// <returns>Success status indicating whether it's possible to set the new level or not.</returns>
        public bool TryChangeLevel(ushort newLevel, bool changedByAdmin = false)
        {
            var previousLevel = Level;

            if (!TrySetLevel(newLevel))
                return false;

            if (changedByAdmin)
            {
                // Change player experience to 0% of current level
                SetExperience(MinLevelExp);

                // Send player experience
                SendAttribute(CharacterAttributeEnum.Exp);

                OnAdminLevelUp?.Invoke(this);
            }
            else
            {
                // Increase stats and skill points
                var levelStats = _characterConfig.GetLevelStatSkillPoints(Mode);
                IncreaseStatPoint(levelStats.StatPoint);
                IncreaseSkillPoint(levelStats.SkillPoint);

                OnLevelUp?.Invoke(this);
            }

            // Send max hp mp and sp
            OnMax_HP_MP_SP_Changed?.Invoke(this);

            // Recover
            FullRecover();

            // Update primary attribute
            if (changedByAdmin)
            {
                var levelDifference = newLevel - previousLevel;

                if (levelDifference > 0)
                    IncrementPrimaryAttribute((ushort) levelDifference);
                else
                    DecreasePrimaryAttribute((ushort) Math.Abs(levelDifference));
            }
            else
            {
                IncrementPrimaryAttribute(1);
            }

            // Send primary attribute
            SendAttribute(GetPrimaryAttribute());

            // Send new level
            SendAttribute(CharacterAttributeEnum.Level);

            return true;
        }

        /// <summary>
        /// Sets character's experience.
        /// </summary>
        private void SetExperience(uint exp)
        {
            Exp = exp;

            _taskQueue.Enqueue(ActionType.SAVE_CHARACTER_EXPERIENCE, Id, Exp);
        }

        /// <summary>
        /// Attempts to set the experience of a player and updates the player's level if necessary.
        /// </summary>
        /// <param name="exp">New player experience</param>
        /// <param name="changedByAdmin">Indicates whether the level change was issued by an admin or not.</param>
        /// <returns>Success status indicating whether it's possible to set the new level or not.</returns>
        public bool TryChangeExperience(uint exp, bool changedByAdmin = false)
        {
            if (!CanSetExperience(exp)) return false;

            SetExperience(exp);

            SendAttribute(CharacterAttributeEnum.Exp);

            var currentLevelExp = _databasePreloader.Levels[(Mode, Level)].Exp;

            uint lowerLevelExp = 0;

            if (Level > 1)
            {
                lowerLevelExp = _databasePreloader.Levels[(Mode, (ushort) (Level - 1))].Exp;
            }

            if (Exp >= currentLevelExp || Exp < lowerLevelExp)
            {
                TryChangeLevel(GetLevelByExperience(Exp), changedByAdmin);
            }

            return true;
        }

        /// <summary>
        /// Attempts to add experience to a player.
        /// </summary>
        /// <param name="expAmount"></param>
        /// <returns></returns>
        public bool TryAddExperience(ushort expAmount)
        {
            var newExp = Exp + expAmount;

            if (!CanSetExperience(newExp)) return false;

            SetExperience(newExp);

            SendExperienceGain(expAmount);

            if (Exp >= NextLevelExp)
            {
                TryChangeLevel(GetLevelByExperience(Exp));
            }

            return true;
        }

        /// <summary>
        /// Checks if an experience value can be set, verifying it doesn't exceed the max level's experience
        /// </summary>
        /// <param name="exp"></param>
        /// <returns>Success status indicating whether it is possible to set an experience value or not.</returns>
        private bool CanSetExperience(uint exp)
        {
            var maxLevel = _characterConfig.GetMaxLevelConfig(Mode).Level;

            var maxLevelInfo = _databasePreloader.Levels[(Mode, maxLevel)];

            // Exp can't be superior than max level's experience
            return exp <= maxLevelInfo.Exp;
        }

        /// <summary>
        /// Helper method that calculates the level that corresponds to a certain experience value.
        /// </summary>
        private ushort GetLevelByExperience(uint exp)
        {
            var levelInfo = _databasePreloader.Levels.Values
                .Where(l => l.Exp > exp)
                .OrderBy(l => l.Level)
                .First();

            return levelInfo.Level;
        }
    }
}