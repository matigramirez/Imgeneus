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
        CHARACTER_ATTACK_MOVEMENT_SPEED = 0x051C, // 1308
        CHARACTER_SHAPE_UPDATE = 0x051D, // 1309

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
        CASH_POINT = 0x2605, // 9733
        CHANGE_ENCRYPTION = 0xB106, // I'm not really sure, that exactly this packet starts another encryption, but for now let's assume so.
        CHARACTER_SHAPE = 0x0303,
        MAP_ADD_ITEM = 0x0401, // 1025
        MAP_REMOVE_ITEM = 0x0402, // 1026
        CHARACTER_DEAD = 0x0405,
        CHARACTER_MOTION = 0x0506,
        CHARACTER_CURRENT_HITPOINTS = 0x0521, // 1313
        CHARACTER_ABSORPTION_DAMAGE = 0x0525, // 1317

        RUN_MODE = 0x0210, // 528
        BLESS_AMOUNT = 0x0211, // 529

        // Attack
        ATTACK_START = 0x0212, // 530
        SAVEPOS_RESULT = 0x0222, // 546

        // Mobs
        MOB_ENTER = 0x0601, // 1537
        MOB_OUT_OF_RADAR = 0x0602, // 1538
        MOB_MOVE = 0x0603, // 1539
        MOB_ATTACK = 0x0605, // 1541
        MOB_DEATH = 0x0606, // 1542
        MOB_SET_SPEED = 0x0609, // 1545
        MOB_RECOVER = 0x0610, // 1552
        MOB_SKILL_USE = 0x060B, // 1547

        // Char skills
        LEARN_NEW_SKILL = 0x209,

        // Target
        TARGET_SELECT_CHARACTER = 0x0302, // 770
        TARGET_MOB_GET_STATE = 0x0304, // 772
        TARGET_SELECT_MOB = 0x0305, // 773
        TARGET_GET_CHARACTER_BUFFS = 0x0308, // 776
        TARGET_GET_MOB_BUFFS = 0x0309, // 777
        TARGET_BUFFS = 0x030B, // 779
        TARGET_BUFF_ADD = 0x030C, // 780
        TARGET_BUFF_REMOVE = 0x030D, // 781

        // PvP
        CHARACTER_MOVE = 0x0501, // 1281
        CHARACTER_CHARACTER_AUTO_ATTACK = 0x0502, // 1282
        CHARACTER_MOB_AUTO_ATTACK = 0x0503, // 1283
        CHARACTER_DEATH = 0x0504, // 1284
        CHARACTER_RECOVER = 0x0505, // 1285
        USE_ITEM = 0x050A, // 1290
        CHARACTER_MAX_HITPOINTS = 0x050B, // 1291
        USED_SP_MP = 0x050C, // 1292
        BUFF_ADD = 0x050D, // 1293
        BUFF_REMOVE = 0x050E, // 1294
        CHARACTER_SKILL_KEEP = 0x050F, // 1295
        CHARACTER_SKILL_CASTING = 0x0510, // 1296
        USE_CHARACTER_TARGET_SKILL = 0x0511, // 1297
        USE_CHARACTER_RANGE_SKILL = 0x0513, // 1299
        MOB_SKILL_CASTING = 0x0516, // 1302
        USE_MOB_TARGET_SKILL = 0x0517, // 1303
        USE_MOB_RANGE_SKILL = 0x0519, // 1305
        DEAD_REBIRTH = 0x0551,// 1361
        REBIRTH_TO_NEAREST_TOWN = 0x0553, // 1363
        CHARACTER_LEAVE_DEAD = 0x0406, // 1030

        // GM commands
        GM_COMMAND_GET_ITEM = 0xF702,
        GM_CREATE_MOB = 0xF704, // 63236
        GM_TELEPORT = 0xFA0A, // 64010
        GM_CMD_ERROR = 0xF501, // -2815
        GM_CREATE_NPC = 0xF70C, // 63244, -2292
        GM_REMOVE_NPC = 0xF70D, // 63245, -2291

        // Map
        CHARACTER_ENTERED_MAP = 0x0201, // 513
        CHARACTER_LEFT_MAP = 0x0202,  // 514
        CHARACTER_MAP_TELEPORT = 0x020B, // 523
        MAP_NPC_ENTER = 0x0E01, // 3585
        MAP_NPC_LEAVE = 0x0E02, // 3586

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
        PARTY_CHARACTER_SP_MP = 0x0C01, // 3073
        PARTY_SET_MAX = 0x0C02, // 3074
        PARTY_ADDED_BUFF = 0x0C04, // 3076
        MAP_PARTY_SET = 0x0520, // 1312

        // Raid
        RAID_LIST = 0x0B0B, // 2827
        RAID_ENTER = 0x0B0C, // 2828
        RAID_LEAVE = 0x0B0D, // 2829
        RAID_CREATE = 0x0B0E, // 2830
        RAID_CHANGE_LOOT = 0x0B0F, // 2831
        RAID_CHANGE_AUTOINVITE = 0x0B10, // 2832
        RAID_JOIN = 0x0B11, // 2833
        RAID_MOVE_PLAYER = 0x0B12, // 2834
        RAID_CHANGE_SUBLEADER = 0x0B13, // 2835
        RAID_INVITE = 0x0B14, // 2836
        RAID_DISMANTLE = 0x0B16, // 2838
        RAID_CHANGE_LEADER = 0x0B17, // 2839
        RAID_PARTY_ERROR = 0x0B19, // 2841
        RAID_KICK = 0x0B1A, // 2842
        RAID_CHARACTER_SP_MP = 0x0C0A, // 3082
        RAID_SET_MAX = 0x0C0B, // 3083
        RAID_ADDED_BUFF = 0x0C0E, // 3086

        // Chat
        CHAT_NORMAL_ADMIN = 0xF101, // 61697, -3837
        CHAT_WHISPER_ADMIN = 0xF102, // 61698, -3838
        CHAT_GUILD_ADMIN = 0xF104, //  61700, -3836
        CHAT_PARTY_ADMIN = 0xF105, // 61701, -3835

        CHAT_NORMAL = 0x1101, // 4353
        CHAT_WHISPER = 0x1102, // 4354
        CHAT_WORLD = 0x1103, // 4355
        CHAT_PARTY = 0x1105, // 4357
        CHAT_ANOUNCEMENT = 0x1108, // 4360
        CHAT_MAP = 0x1111, // 4369

        // Duel
        DUEL_REQUEST = 0x2401, // 9217
        DUEL_RESPONSE = 0x2402, // 9218
        DUEL_READY = 0x2403, // 9219
        DUEL_START = 0x2404, // 9220
        DUEL_WIN_LOSE = 0x2405, // 9221
        DUEL_CANCEL = 0x2406, // 9222
        DUEL_TRADE = 0x2407, // 9223
        DUEL_CLOSE_TRADE = 0x2408, // 9224
        DUEL_TRADE_OK = 0x2409, // 9225
        DUEL_TRADE_ADD_ITEM = 0x240A, // 9226
        DUEL_TRADE_REMOVE_ITEM = 0x240B, // 9227
        DUEL_TRADE_ADD_MONEY = 0x240C, // 9228
        DUEL_TRADE_OPPONENT_ADD_ITEM = 0x240D, // 9229

    }
}
