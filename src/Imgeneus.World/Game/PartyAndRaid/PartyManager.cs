using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Player;
using System;
using System.Linq;

namespace Imgeneus.World.Game.PartyAndRaid
{
    /// <summary>
    /// Party manager handles all party packets.
    /// </summary>
    public class PartyManager : IDisposable
    {
        private readonly IGameWorld _gameWorld;
        private readonly Character _player;

        public PartyManager(IGameWorld gameWorld, Character player)
        {
            _gameWorld = gameWorld;
            _player = player;
            _player.Client.OnPacketArrived += Client_OnPacketArrived;
        }

        public void Dispose()
        {
            _player.Client.OnPacketArrived -= Client_OnPacketArrived;
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            var worldSender = (WorldClient)sender;

            switch (packet)
            {
                case PartyRequestPacket partyRequestPacket:
                    if (_gameWorld.Players.TryGetValue(partyRequestPacket.CharacterId, out var requestedPlayer))
                    {
                        requestedPlayer.PartyInviterId = worldSender.CharID;
                        SendPartyRequest(requestedPlayer.Client, worldSender.CharID);
                    }
                    break;

                case PartyResponsePacket responsePartyPacket:
                    if (_gameWorld.Players.TryGetValue(responsePartyPacket.CharacterId, out var partyResponser))
                    {
                        if (responsePartyPacket.IsDeclined)
                        {
                            if (_gameWorld.Players.TryGetValue(partyResponser.PartyInviterId, out var partyRequester))
                            {
                                SendDeclineParty(partyRequester.Client, worldSender.CharID);
                            }
                        }
                        else
                        {
                            if (_gameWorld.Players.TryGetValue(partyResponser.PartyInviterId, out var partyRequester))
                            {
                                if (partyRequester.Party is null)
                                {
                                    var party = new Party();
                                    partyRequester.Party = party;
                                    partyResponser.Party = party;
                                }
                                else
                                {
                                    partyResponser.Party = partyRequester.Party;
                                }
                            }
                        }

                        partyResponser.PartyInviterId = 0;
                    }
                    break;

                case PartyLeavePacket partyLeavePacket:
                    _player.Party.LeaveParty(_player);
                    break;

                case PartyKickPacket partyKickPacket:
                    if (!_player.IsPartyLead)
                        return;

                    var playerToKick = _player.Party.Members.FirstOrDefault(m => m.Id == partyKickPacket.CharacterId);
                    if (playerToKick != null)
                        _player.Party.KickMember(playerToKick);
                    break;

                case PartyChangeLeaderPacket changeLeaderPacket:
                    if (!_player.IsPartyLead)
                        return;

                    var newLeader = _player.Party.Members.FirstOrDefault(m => m.Id == changeLeaderPacket.CharacterId);
                    if (newLeader != null)
                        _player.Party.SetLeader(newLeader);
                    break;

                case RaidCreatePacket raidCreatePacket:
                    if (!_player.IsPartyLead)
                        return;
                    var raid = new Raid(raidCreatePacket.AutoJoin, (RaidDropType)raidCreatePacket.DropType);
                    foreach (var member in _player.Party.Members.ToList())
                    {
                        member.Party = raid;
                    }
                    raid.SetLeader(_player);
                    foreach (var m in raid.Members)
                    {
                        SendRaidCreated(m.Client, raid);
                    }

                    break;
            }
        }

        private void SendPartyRequest(WorldClient client, int requesterId)
        {
            using var packet = new Packet(PacketType.PARTY_REQUEST);
            packet.Write(requesterId);
            client.SendPacket(packet);
        }

        private void SendDeclineParty(WorldClient client, int charID)
        {
            using var packet = new Packet(PacketType.PARTY_RESPONSE);
            packet.Write(false);
            packet.Write(charID);
            client.SendPacket(packet);
        }

        private void SendRaidCreated(WorldClient client, Raid raid)
        {
            using var packet = new Packet(PacketType.RAID_CREATE);
            packet.Write(true); // raid type ?
            packet.Write(raid.AutoJoin);
            packet.Write((int)raid.DropType);
            packet.Write(raid.Members.IndexOf(raid.Leader));
            packet.Write(raid.Members.IndexOf(raid.SubLeader));
            client.SendPacket(packet);
        }
    }
}
