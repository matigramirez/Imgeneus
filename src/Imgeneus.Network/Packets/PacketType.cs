namespace Imgeneus.Network.Packets
{
    public enum PacketType : ushort
    {
        // Internal Server
        AUTH_SERVER = 5,
        SERVER_INFO = 6,
        UPDATE_SERVER = 7,
        DISSCONNECT_USER = 8,

        // Character
        CHARACTER_LIST = 0x0101,
        CREATE_CHARACTER = 0x0102,
        DELETE_CHARACTER = 0x0103,
        SELECT_CHARACTER = 0x0104,
        CHARACTER_DETAILS = 0x0105,
        CHARACTER_ITEMS = 0x0106,
        CHECK_CHARACTER_AVAILABLE_NAME = 0x0119,

        // Common
        CLOSE_CONNECTION = 0x0107,
        CHARACTER_SKILLS = 0x0108,
        ACCOUNT_FACTION = 0x0109,
        CHARACTER_ACTIVE_BUFFS = 0x010A,
        CHARACTER_SKILL_BAR = 0x010B,
        RENAME_CHARACTER = 0x010E,
        RESTORE_CHARACTER = 0x010F,

        // Login Server
        LOGIN_HANDSHAKE = 0xA101,
        LOGIN_REQUEST = 0xA102,
        SERVER_LIST = 0xA201,
        SELECT_SERVER = 0xA202,

        // Game 
        GAME_HANDSHAKE = 0xA301,
        PING = 0xA303,
        CHARACTER_ENTERED_MAP = 0x0201, // 513
        CHARACTER_LEFT_MAP = 0x0202,  // 514
        CHARACTER_SHAPE = 0x0303,
        CHARACTER_DEAD = 0x0405,
        CHARACTER_MOVE = 0x0501,
        CHARACTER_MOTION = 0x0506,
        USE_SKILL = 0x0511,
        BUFF_SELF = 0x050D, // 1293
        BUFF_PARTY = 0x0C04, // 3076
        BUFF_RAID = 0x0C0E, // 3086
        SKILL_RANGE = 0x0511, // 1297
        BLESS_AMOUNT = 0x0211,

        // Char skills
        LEARN_NEW_SKILL = 0x209,

        // GM commands
        GM_COMMAND_GET_ITEM = 0xF702,

        // Inventory
        INVENTORY_MOVE_ITEM = 0x204,
        SEND_EQUIPMENT = 0x0507,
    }
}
