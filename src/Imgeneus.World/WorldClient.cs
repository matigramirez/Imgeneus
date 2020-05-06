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
    public sealed class WorldClient : ServerClient
    {
        private readonly ILogger<WorldClient> logger;

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

        public WorldClient(IServer server, Socket acceptedSocket)
            : base(server, acceptedSocket)
        {
            this.logger = DependencyContainer.Instance.Resolve<ILogger<WorldClient>>();
        }

        public override void HandlePacket(IPacketStream packet)
        {
            if (this.Socket == null)
            {
                this.logger.LogTrace("Skip to handle packet. Reason: client is no more connected.");
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
                        this.logger.LogWarning("Received an unimplemented packet {0} from {2}.", packet.PacketType, this.RemoteEndPoint);
                    }
                    else
                    {
                        this.logger.LogWarning("Received an unknown packet 0x{0} from {1}.", ((ushort)packet.PacketType).ToString("X2"), this.RemoteEndPoint);
                    }
                }
            }
            catch (Exception exception)
            {
                this.logger.LogError("Packet handle error from {0}. {1}", this.RemoteEndPoint, exception.Message);
                this.logger.LogDebug(exception.InnerException?.StackTrace);
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
            { PacketType.CHANGE_ENCRYPTION, (s) => new ChangeEncryptionPacket(s) },
            { PacketType.LOGOUT, (s) => new LogOutPacket(s) },
            { PacketType.ACCOUNT_FACTION, (s) => new AccountFractionPacket(s) },
            { PacketType.CHECK_CHARACTER_AVAILABLE_NAME, (s) => new CheckCharacterAvailableNamePacket(s) },
            { PacketType.CREATE_CHARACTER, (s) => new CreateCharacterPacket(s) },
            { PacketType.SELECT_CHARACTER, (s) => new SelectCharacterPacket(s) },
            { PacketType.LEARN_NEW_SKILL, (s) => new LearnNewSkillPacket(s) },
            { PacketType.INVENTORY_MOVE_ITEM, (s) => new MoveItemInInventoryPacket(s) },
            { PacketType.CHARACTER_MOVE, (s) => new MoveCharacterPacket(s) },
            { PacketType.CHARACTER_ENTERED_MAP, (s) => new CharacterEnteredMapPacket(s) },
            { PacketType.CHARACTER_MOTION, (s) => new MotionPacket(s) },
            { PacketType.USE_NON_TARGET_SKILL, (s) => new UsedNonTargetSkillPacket(s) },
            { PacketType.TARGET_SELECT_MOB, (s) => new MobInTargetPacket(s) },
            { PacketType.TARGET_SELECT_CHARACTER, (s) => new PlayerInTargetPacket(s) },
            { PacketType.GM_CREATE_MOB, (s) => new GMCreateMobPacket(s) },
            { PacketType.GM_COMMAND_GET_ITEM, (s) => new GMGetItemPacket(s) },
            { PacketType.CHARACTER_SKILL_BAR, (s) => new SkillBarPacket(s) },
            { PacketType.CHARACTER_ATTACK, (s) => new CharacterAttackPacket(s) },
            { PacketType.ATTACK_START, (s) => new AttackStart(s) },
            { PacketType.USE_ATTACK_SKILL, (s) => new UsedSkillAttackPacket(s) },
            { PacketType.TRADE_REQUEST, (s) => new TradeRequestPacket(s) },
            { PacketType.TRADE_RESPONSE, (s) => new TradeResponsePacket(s) },
            { PacketType.TRADE_FINISH, (s) => new TradeFinishPacket(s) },
            { PacketType.TRADE_OWNER_ADD_ITEM, (s) => new TradeAddItemPacket(s) },
            { PacketType.TRADE_ADD_MONEY, (s) => new TradeAddMoneyPacket(s) },
            { PacketType.TRADE_DECIDE, (s) => new TradeDecidePacket(s) }
        };

        /// <inheritdoc />
        public override Dictionary<PacketType, PacketDeserializeHandler> PacketHandlers => _packetHandlers;
    }
}
