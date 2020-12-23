namespace Imgeneus.DatabaseBackgroundService.Handlers
{
    /// <summary>
    /// All possible actions to database.
    /// </summary>
    public enum ActionType
    {
        // Character
        SAVE_CHARACTER_MOVE,
        SAVE_CHARACTER_HP_MP_SP,
        SAVE_CHARACTER_LEVEL,
        SAVE_CHARACTER_EXPERIENCE,
        UPDATE_CHARACTER_MODE,
        SAVE_CHARACTER_KILLS,
        SAVE_CHARACTER_DEATHS,
        SAVE_CHARACTER_STATPOINT,
        SAVE_CHARACTER_SKILLPOINT,

        // Inventory
        SAVE_ITEM_IN_INVENTORY,
        REMOVE_ITEM_FROM_INVENTORY,
        UPDATE_ITEM_COUNT_IN_INVENTORY,
        UPDATE_GOLD,
        CREATE_DYE_COLOR,
        UPDATE_CRAFT_NAME,

        // Gems
        UPDATE_GEM,

        // Stats
        UPDATE_STATS,

        // Skills
        SAVE_SKILL,
        REMOVE_SKILL,

        // Buffs
        SAVE_BUFF,
        REMOVE_BUFF,
        REMOVE_BUFF_ALL,
        UPDATE_BUFF_RESET_TIME,

        // Quests
        QUEST_START,
        QUEST_UPDATE,

        // Appearance
        SAVE_APPEARANCE,
        SAVE_IS_RENAME,

        // Friends
        SAVE_FRIENDS,
        DELETE_FRIENDS,

        // Quick bar
        SAVE_QUICK_BAR,

        // Map
        SAVE_MAP_ID,

        // Logs
        LOG_SAVE_CHAT_MESSAGE
    }
}
