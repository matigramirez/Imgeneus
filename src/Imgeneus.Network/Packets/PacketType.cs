namespace Imgeneus.Network.Packets
{
    public enum PacketType : ushort
    {
        // Internal Server
        AUTH_SERVER = 5,
        SERVER_INFO = 6,
        UPDATE_SERVER = 7,
        DISSCONNECT_USER = 8,
        AES_KEY_RESPONSE = 9,
        AES_KEY_REQUEST = 10,

        // Character
        CHARACTER_LIST = 0x0101,
        CREATE_CHARACTER = 0x0102,
        DELETE_CHARACTER = 0x0103,
        SELECT_CHARACTER = 0x0104,
        CHARACTER_DETAILS = 0x0105,
        CHARACTER_ITEMS = 0x0106,
        CHECK_CHARACTER_AVAILABLE_NAME = 0x0119,
        CHARACTER_ADDITIONAL_STATS = 0x0526,
        CHARACTER_ATTACK_MOVEMENT_SPEED = 0x051C,

        // Common
        LOGOUT = 0x0107, // 263
        CHARACTER_SKILLS = 0x0108,
        ACCOUNT_FACTION = 0x0109,
        CHARACTER_ACTIVE_BUFFS = 0x010A,
        CHARACTER_SKILL_BAR = 0x010B,
        RENAME_CHARACTER = 0x010E,
        RESTORE_CHARACTER = 0x010F,

        // Login Server
        LOGIN_HANDSHAKE = 0xA101, // 41217
        LOGIN_REQUEST = 0xA102,
        SERVER_LIST = 0xA201,
        SELECT_SERVER = 0xA202,

        // Game 
        GAME_HANDSHAKE = 0xA301,
        PING = 0xA303,
        CHANGE_ENCRYPTION = 0xB106, // I'm not really sure, that exactly this packet starts another encryption, but for now let's assume so.
        CHARACTER_ENTERED_MAP = 0x0201, // 513
        CHARACTER_LEFT_MAP = 0x0202,  // 514
        CHARACTER_SHAPE = 0x0303,
        CHARACTER_DEAD = 0x0405,
        CHARACTER_MOVE = 0x0501,
        CHARACTER_MOTION = 0x0506,
        CHARACTER_CURRENT_HITPOINTS = 0x0521, // 1313
        CHARACTER_MAX_HITPOINTS = 0x050B, // 1291
        CHARACTER_ABSORPTION_DAMAGE = 0x0525, // 1317
        TARGET_SELECT_MOB = 0x0305, // 773
        TARGET_SELECT_CHARACTER = 0x302, // 770
        BUFF_SELF = 0x050D, // 1293
        BUFF_PARTY = 0x0C04, // 3076
        BUFF_RAID = 0x0C0E, // 3086
        BLESS_AMOUNT = 0x0211,

        // Attack
        ATTACK_START = 0x0212, // 530
        SAVEPOS_RESULT = 0x0222, // 546

        // Mobs
        MOB_ENTER = 0x0601, // 1537
        MOB_GET_STATE = 0x0304, // 772
        MOB_MOVE = 0x0603, // 1539
        MOB_ATTACK = 0x605, // 1541

        // Char skills
        LEARN_NEW_SKILL = 0x209,
        USE_SKILL_LOG = 0x0309, // 777
        USE_CHARACTER_TARGET_SKILL = 0x0511, // 1297
        USE_MOB_TARGET_SKILL = 0x0517, // 1303

        // PvP
        CHARACTER_CHARACTER_AUTO_ATTACK = 0x0502, // 1282
        CHARACTER_MOB_AUTO_ATTACK = 0x0503, // 1283
        CHARACTER_DEATH = 0x0504, // 1284

        // GM commands
        GM_COMMAND_GET_ITEM = 0xF702,
        GM_CREATE_MOB = 0xF704, // 63236

        // Inventory
        INVENTORY_MOVE_ITEM = 0x204,
        ADD_ITEM = 0x0205, // 517
        REMOVE_ITEM = 0x0206, // 518
        SEND_EQUIPMENT = 0x0507,

        // Trade
        TRADE_REQUEST = 0x0A01, // 2561
        TRADE_RESPONSE = 0x0A02, // 2562
        TRADE_START = 0x0A03, // 2563
        TRADE_STOP = 0x0A04, // 2564
        TRADE_FINISH = 0x0A05, // 2565
        TRADE_OWNER_ADD_ITEM = 0x0A06, // 2566
        TRADE_ADD_MONEY = 0x0A08, //
        TRADE_RECEIVER_ADD_ITEM = 0x0A09, // 2569
        TRADE_DECIDE = 0xA0A, // 2570

        // Party
        PARTY_LIST = 0x0B01, // 2817
        PARTY_REQUEST = 0x0B02, // 2818
        PARTY_RESPONSE = 0x0B03, // 2819
        PARTY_ENTER = 0x0B04, // 2820
        PARTY_LEAVE = 0x0B05, // 2821
        PARTY_KICK = 0x0B06, // 2822
        PARTY_CHANGE_LEADER = 0x0B07,
        MAP_PARTY_SET = 0x0520, // 1312
    }
}
