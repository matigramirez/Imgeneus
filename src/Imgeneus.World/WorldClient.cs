using Imgeneus.Core.DependencyInjection;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using static Imgeneus.Network.Server.IServerClient;

namespace Imgeneus.World
{
    public sealed class WorldClient : ServerClient, IWorldClient
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Gets the client's logged user id.
        /// </summary>
        public int UserID { get; private set; }

        /// <summary>
        /// Gets the client's logged char id.
        /// </summary>
        public int CharID { get; set; }

        /// <summary>
        /// Check if the client is connected.
        /// </summary>
        public bool IsConnected => this.UserID != 0;

        /// <inheritdoc />
        public override event Action<ServerClient, IDeserializedPacket> OnPacketArrived;

        public WorldClient(IServer server, Socket acceptedSocket, ILogger logger)
            : base(server, acceptedSocket)
        {
            _logger = logger;
        }

        public override void HandlePacket(IPacketStream packet)
        {
            if (this.Socket == null)
            {
                _logger.LogTrace("Skip to handle packet. Reason: client is no more connected.");
                return;
            }

            try
            {
                PacketDeserializeHandler handler;

                if (PacketHandlers.TryGetValue(packet.PacketType, out handler))
                {
                    var deserializedPacket = handler.Invoke(packet);
                    OnPacketArrived?.Invoke(this, deserializedPacket);
                }
                else
                {
                    if (Enum.IsDefined(typeof(PacketType), packet.PacketType))
                    {
                        _logger.LogWarning("Received an unimplemented packet {0} from {1}.", packet.PacketType, this.RemoteEndPoint);
                    }
                    else
                    {
                        _logger.LogWarning("Received an unknown packet 0x{0} from {1}.", ((ushort)packet.PacketType).ToString("X2"), this.RemoteEndPoint);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Packet handle error from {0}. {1}", this.RemoteEndPoint, exception.Message);
                _logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }

        /// <summary>
        /// Sets the client's user id.
        /// </summary>
        /// <param name="userID">The client user id.</param>
        public void SetClientUserID(int userID)
        {
            if (this.UserID != 0)
            {
                throw new InvalidOperationException("Client user ID already set.");
            }

            this.UserID = userID;
        }

        /// <summary>
        /// All packets transformations available for world server. Add new packets here.
        /// </summary>
        private readonly Dictionary<PacketType, PacketDeserializeHandler> _packetHandlers = new Dictionary<PacketType, PacketDeserializeHandler>()
        {
            { PacketType.GAME_HANDSHAKE, (s) => new HandshakePacket(s) },
            { PacketType.PING, (s) => new PingPacket(s) },
            { PacketType.CASH_POINT, (s) => new CashPointPacket(s) },
            { PacketType.CHANGE_ENCRYPTION, (s) => new ChangeEncryptionPacket(s) },
            { PacketType.LOGOUT, (s) => new LogOutPacket(s) },
            { PacketType.ACCOUNT_FACTION, (s) => new AccountFractionPacket(s) },
            { PacketType.CHECK_CHARACTER_AVAILABLE_NAME, (s) => new CheckCharacterAvailableNamePacket(s) },
            { PacketType.CREATE_CHARACTER, (s) => new CreateCharacterPacket(s) },
            { PacketType.DELETE_CHARACTER, (s) => new DeleteCharacterPacket(s) },
            { PacketType.RESTORE_CHARACTER, (s) => new RestoreCharacterPacket(s) },
            { PacketType.SELECT_CHARACTER, (s) => new SelectCharacterPacket(s) },
            { PacketType.LEARN_NEW_SKILL, (s) => new LearnNewSkillPacket(s) },
            { PacketType.INVENTORY_MOVE_ITEM, (s) => new MoveItemInInventoryPacket(s) },
            { PacketType.CHARACTER_MOVE, (s) => new MoveCharacterPacket(s) },
            { PacketType.CHARACTER_ENTERED_MAP, (s) => new CharacterEnteredMapPacket(s) },
            { PacketType.CHARACTER_ENTERED_PORTAL, (s) => new CharacterEnteredPortalPacket(s) },
            { PacketType.CHARACTER_MOTION, (s) => new MotionPacket(s) },
            { PacketType.USE_CHARACTER_TARGET_SKILL, (s) => new CharacterSkillAttackPacket(s) },
            { PacketType.TARGET_SELECT_MOB, (s) => new MobInTargetPacket(s) },
            { PacketType.TARGET_SELECT_CHARACTER, (s) => new PlayerInTargetPacket(s) },
            { PacketType.TARGET_GET_CHARACTER_BUFFS, (s) => new TargetCharacterGetBuffs(s) },
            { PacketType.TARGET_GET_MOB_BUFFS, (s) => new TargetMobGetBuffs(s) },
            { PacketType.TARGET_MOB_GET_STATE, (s) => new TargetGetMobStatePacket(s) },
            { PacketType.GM_CREATE_MOB, (s) => new GMCreateMobPacket(s) },
            { PacketType.GM_COMMAND_GET_ITEM, (s) => new GMGetItemPacket(s) },
            { PacketType.CHARACTER_SKILL_BAR, (s) => new SkillBarPacket(s) },
            { PacketType.CHARACTER_MOB_AUTO_ATTACK, (s) => new MobAutoAttackPacket(s) },
            { PacketType.CHARACTER_CHARACTER_AUTO_ATTACK, (s) => new CharacterAutoAttackPacket(s) },
            { PacketType.ATTACK_START, (s) => new AttackStart(s) },
            { PacketType.USE_MOB_TARGET_SKILL, (s) => new MobSkillAttackPacket(s) },
            { PacketType.TRADE_REQUEST, (s) => new TradeRequestPacket(s) },
            { PacketType.TRADE_RESPONSE, (s) => new TradeResponsePacket(s) },
            { PacketType.TRADE_FINISH, (s) => new TradeFinishPacket(s) },
            { PacketType.TRADE_OWNER_ADD_ITEM, (s) => new TradeAddItemPacket(s) },
            { PacketType.TRADE_ADD_MONEY, (s) => new TradeAddMoneyPacket(s) },
            { PacketType.TRADE_DECIDE, (s) => new TradeDecidePacket(s) },
            { PacketType.PARTY_REQUEST, (s) => new PartyRequestPacket(s) },
            { PacketType.PARTY_RESPONSE, (s) => new PartyResponsePacket(s) },
            { PacketType.PARTY_LEAVE, (s) => new PartyLeavePacket(s) },
            { PacketType.PARTY_KICK, (s) => new PartyKickPacket(s) },
            { PacketType.PARTY_CHANGE_LEADER, (s) => new PartyChangeLeaderPacket(s) },
            { PacketType.CHARACTER_SHAPE, (s) => new CharacterShapePacket(s)},
            { PacketType.USE_ITEM, (s) => new UseItemPacket(s) },
            { PacketType.REBIRTH_TO_NEAREST_TOWN, (s) => new RebirthPacket(s) },
            { PacketType.CHAT_NORMAL_ADMIN, (s) => new ChatNormalPacket(s) },
            { PacketType.CHAT_NORMAL, (s) => new ChatNormalPacket(s) },
            { PacketType.CHAT_WHISPER_ADMIN, (s) => new ChatWhisperPacket(s) },
            { PacketType.CHAT_WHISPER, (s) => new ChatWhisperPacket(s) },
            { PacketType.CHAT_PARTY_ADMIN, (s) => new ChatPartyPacket(s) },
            { PacketType.CHAT_PARTY, (s) => new ChatPartyPacket(s) },
            { PacketType.CHAT_MAP, (s) => new ChatMapPacket(s) },
            { PacketType.CHAT_WORLD, (s) => new ChatWorldPacket(s) },
            { PacketType.DUEL_REQUEST, (s) => new DuelRequestPacket(s) },
            { PacketType.DUEL_RESPONSE, (s) => new DuelResponsePacket(s) },
            { PacketType.DUEL_TRADE_ADD_ITEM, (s) => new DuelAddItemPacket(s) },
            { PacketType.DUEL_TRADE_REMOVE_ITEM, (s) => new DuelRemoveItemPacket(s) },
            { PacketType.DUEL_TRADE_ADD_MONEY, (s) => new DuelAddMoneyPacket(s) },
            { PacketType.DUEL_TRADE_OK, (s) => new DuelOkPacket(s) },
            { PacketType.DUEL_CANCEL, (s) => new DuelDefeatPacket(s) },
            { PacketType.ADD_ITEM, (s) => new MapPickUpItemPacket(s) },
            { PacketType.REMOVE_ITEM, (s) => new RemoveItemPacket(s) },
            { PacketType.RAID_CREATE, (s) => new RaidCreatePacket(s) },
            { PacketType.RAID_DISMANTLE, (s) => new RaidDismantlePacket(s) },
            { PacketType.RAID_LEAVE, (s) => new RaidLeavePacket(s) },
            { PacketType.RAID_CHANGE_AUTOINVITE, (s) => new RaidChangeAutoInvitePacket(s) },
            { PacketType.RAID_CHANGE_LOOT, (s) => new RaidChangeLootPacket(s) },
            { PacketType.RAID_JOIN, (s) => new RaidJoinPacket(s) },
            { PacketType.RAID_CHANGE_LEADER, (s) => new RaidChangeLeaderPacket(s) },
            { PacketType.RAID_CHANGE_SUBLEADER, (s) => new RaidChangeSubLeaderPacket(s) },
            { PacketType.RAID_KICK, (s) => new RaidKickPacket(s) },
            { PacketType.RAID_MOVE_PLAYER, (s) => new RaidMovePlayerPacket(s) },
            { PacketType.GM_TELEPORT_MAP_COORDINATES, (s) => new GMTeleportMapCoordinatesPacket(s) },
            { PacketType.GM_TELEPORT_MAP, (s) => new GMTeleportMapPacket(s) },
            { PacketType.GM_CREATE_NPC, (s) => new GMCreateNpcPacket(s) },
            { PacketType.GM_REMOVE_NPC, (s) => new GMRemoveNpcPacket(s) },
            { PacketType.NPC_BUY_ITEM, (s) => new NpcBuyItemPacket(s) },
            { PacketType.NPC_SELL_ITEM, (s) => new NpcSellItemPacket(s) },
            { PacketType.QUEST_START, (s) => new QuestStartPacket(s) },
            { PacketType.QUEST_END, (s) => new QuestEndPacket(s) },
            { PacketType.QUEST_QUIT, (s) => new QuestQuitPacket(s) },
            { PacketType.CHANGE_APPEARANCE, (s) => new ChangeAppearancePacket(s) },
            { PacketType.FRIEND_REQUEST, (s) => new FriendRequestPacket(s) },
            { PacketType.FRIEND_RESPONSE, (s) => new FriendResponsePacket(s) },
            { PacketType.FRIEND_DELETE, (s) => new FriendDeletePacket(s) },
            { PacketType.PARTY_SEARCH_REGISTRATION, (s) => new PartySearchRegistrationPacket(s) },
            { PacketType.PARTY_SEARCH_INVITE, (s) => new PartySearchInvitePacket(s) },
            { PacketType.GM_FIND_PLAYER, (s) => new GMFindPlayerPacket(s) },
            { PacketType.GM_SUMMON_PLAYER, (s) => new GMSummonPlayerPacket(s) },
            { PacketType.GM_TELEPORT_PLAYER, (s) => new GMTeleportPlayerPacket(s)},
            { PacketType.GM_TELEPORT_TO_PLAYER, (s) => new GMTeleportToPlayerPacket(s) },
            { PacketType.USE_VEHICLE, (s) => new UseVehiclePacket(s) },
            { PacketType.GEM_ADD, (s) => new GemAddPacket(s) },
            { PacketType.GEM_ADD_POSSIBILITY, (s) => new GemAddPossibilityPacket(s) },
            { PacketType.GEM_REMOVE, (s) => new GemRemovePacket(s) },
            { PacketType.GEM_REMOVE_POSSIBILITY, (s) => new GemRemovePossibilityPacket(s) },
            { PacketType.DYE_SELECT_ITEM, (s) => new DyeSelectItemPacket(s) },
            { PacketType.DYE_REROLL, (s) => new DyeRerollPacket(s) },
            { PacketType.DYE_CONFIRM, (s) => new DyeConfirmPacket(s) },
            { PacketType.ITEM_COMPOSE_ABSOLUTE, (s) => new ItemComposeAbsolutePacket(s) },
            { PacketType.ITEM_COMPOSE, (s) => new ItemComposePacket(s) },
            { PacketType.ITEM_COMPOSE_ABSOLUTE_SELECT, (s) => new ItemComposeAbsoluteSelectPacket(s) },
            { PacketType.UPDATE_STATS, (s) => new UpdateStatsPacket(s) },
            { PacketType.CHARACTER_ATTRIBUTE_SET, (s) => new GMSetAttributePacket(s) },
            { PacketType.RENAME_CHARACTER, (s) => new RenameCharacterPacket(s) },
            { PacketType.NOTICE_WORLD, (s) => new GMNoticeWorldPacket(s) },
            { PacketType.NOTICE_PLAYER, (s) => new GMNoticePlayerPacket(s) },
            { PacketType.NOTICE_FACTION, (s) => new GMNoticeFactionPacket(s) },
            { PacketType.NOTICE_MAP, (s) => new GMNoticeMapPacket(s) },
            { PacketType.NOTICE_ADMINS, (s) => new GMNoticeAdminsPacket(s) },
            { PacketType.GM_CURE_PLAYER, (s) => new GMCurePlayerPacket(s) },
            { PacketType.GM_WARNING_PLAYER, (s) => new GMWarningPacket(s) }
        };

        /// <inheritdoc />
        public override Dictionary<PacketType, PacketDeserializeHandler> PacketHandlers => _packetHandlers;
    }
}
