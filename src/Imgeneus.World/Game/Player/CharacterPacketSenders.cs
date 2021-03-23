using System;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Zone.Obelisks;
using Imgeneus.World.Game.Zone.Portals;
using System.Linq;
using Imgeneus.World.Game.Guild;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Sends to client character start-up information.
        /// </summary>
        private void SendCharacterInfo()
        {
            // SendWorldDay(); TODO: why do we need it?
            SendGuildList();
            SendDetails();
            SendAdditionalStats();
            SendCurrentHitpoints();
            SendInventoryItems();
            SendLearnedSkills();
            SendOpenQuests();
            SendFinishedQuests();
            SendActiveBuffs();
            SendMoveAndAttackSpeed();
            SendFriends();
            SendBlessAmount();
            SendBankItems();
            SendAutoStats();
#if EP8_V1
            SendAccountPoints(); // WARNING: This is necessary if you have an in-game item mall.
#endif
        }

        private void SendWorldDay() => _packetsHelper.SendWorldDay(Client);

        private void SendDetails() => _packetsHelper.SendDetails(Client, this);

        protected override void SendCurrentHitpoints() => _packetsHelper.SendCurrentHitpoints(Client, this);

        private void SendInventoryItems()
        {
            var inventoryItems = InventoryItems.Values.ToArray();
            _packetsHelper.SendInventoryItems(Client, inventoryItems);

            foreach (var item in inventoryItems.Where(i => i.ExpirationTime != null))
                SendItemExpiration(item);
        }

        private void SendItemExpiration(Item item) => _packetsHelper.SendItemExpiration(Client, item);

        private void SendLearnedSkills() => _packetsHelper.SendLearnedSkills(Client, this);

        private void SendOpenQuests() => _packetsHelper.SendQuests(Client, Quests.Where(q => !q.IsFinished));

        private void SendFinishedQuests() => _packetsHelper.SendFinishedQuests(Client, Quests.Where(q => q.IsFinished));

        private void SendQuestStarted(Quest quest, int npcId = 0) => _packetsHelper.SendQuestStarted(Client, quest.Id, npcId);

        private void SendQuestFinished(Quest quest, int npcId = 0) => _packetsHelper.SendQuestFinished(Client, quest, npcId);

        private void SendFriendRequest(Character requester) => _packetsHelper.SendFriendRequest(Client, requester);

        private void SendFriendOnline(int friendId, bool isOnline) => _packetsHelper.SendFriendOnline(Client, friendId, isOnline);

        private void SendFriends() => _packetsHelper.SendFriends(Client, Friends.Values);

        private void SendFriendAdd(Character friend) => _packetsHelper.SendFriendAdded(Client, friend);

        private void SendFriendResponse(bool accepted) => _packetsHelper.SendFriendResponse(Client, accepted);

        private void SendFriendDelete(int id) => _packetsHelper.SendFriendDelete(Client, id);

        private void SendQuestCountUpdate(ushort questId, byte index, byte count) => _packetsHelper.SendQuestCountUpdate(Client, questId, index, count);

        private void SendActiveBuffs() => _packetsHelper.SendActiveBuffs(Client, ActiveBuffs);

        private void SendAddBuff(ActiveBuff buff) => _packetsHelper.SendAddBuff(Client, buff);

        private void SendRemoveBuff(ActiveBuff buff) => _packetsHelper.SendRemoveBuff(Client, buff);

        private void SendSkillBar() => _packetsHelper.SendSkillBar(Client, QuickItems);

        protected override void SendAdditionalStats()
        {
            if (Client != null) _packetsHelper.SendAdditionalStats(Client, this);
        }

        private void SendAutoStats() => _packetsHelper.SendAutoStats(Client, this);

        private void SendMaxHP() => _packetsHelper.SendMaxHitpoints(Client, this, HitpointType.HP);

        private void SendMaxSP() => _packetsHelper.SendMaxHitpoints(Client, this, HitpointType.SP);

        private void SendMaxMP() => _packetsHelper.SendMaxHitpoints(Client, this, HitpointType.MP);

        private void SendAttackStart() => _packetsHelper.SendAttackStart(Client);

        private void SendAutoAttackWrongTarget(IKillable target) => _packetsHelper.SendAutoAttackWrongTarget(Client, this, target);

        private void SendAutoAttackWrongEquipment(IKillable target) =>
            _packetsHelper.SendAutoAttackWrongEquipment(Client, this, target);

        private void SendAutoAttackCanNotAttack(IKillable target) => _packetsHelper.SendAutoAttackCanNotAttack(Client, this, target);

        private void SendSkillAttackCanNotAttack(IKillable target, Skill skill) => _packetsHelper.SendSkillAttackCanNotAttack(Client, this, skill, target);

        private void SendSkillWrongTarget(IKillable target, Skill skill) => _packetsHelper.SendSkillWrongTarget(Client, this, skill, target);

        private void SendSkillWrongEquipment(IKillable target, Skill skill) => _packetsHelper.SendSkillWrongEquipment(Client, this, target, skill);

        private void SendNotEnoughMPSP(IKillable target, Skill skill) => _packetsHelper.SendNotEnoughMPSP(Client, this, target, skill);

        private void SendUseSMMP(ushort needMP, ushort needSP) => _packetsHelper.SendUseSMMP(Client, needMP, needSP);

        private void SendCooldownNotOver(IKillable target, Skill skill) => _packetsHelper.SendCooldownNotOver(Client, this, target, skill);

        protected override void SendMoveAndAttackSpeed()
        {
            if (Client != null) _packetsHelper.SendMoveAndAttackSpeed(Client, this);
        }

        private void SendRunMode() => _packetsHelper.SendRunMode(Client, this);

        private void SendTargetAddBuff(IKillable target, ActiveBuff buff) => _packetsHelper.SendTargetAddBuff(Client, target, buff);

        private void SendTargetRemoveBuff(IKillable target, ActiveBuff buff) => _packetsHelper.SendTargetRemoveBuff(Client, target, buff);

        public void SendAddItemToInventory(Item item)
        {
            _packetsHelper.SendAddItem(Client, item);

            if (item.ExpirationTime != null)
                _packetsHelper.SendItemExpiration(Client, item);
        }

        public void SendRemoveItemFromInventory(Item item, bool fullRemove) => _packetsHelper.SendRemoveItem(Client, item, fullRemove);

        public void SendWeather() => _packetsHelper.SendWeather(Client, Map);

        public void SendObelisks() => _packetsHelper.SendObelisks(Client, Map.Obelisks.Values);

        public void SendObeliskBroken(Obelisk obelisk) => _packetsHelper.SendObeliskBroken(Client, obelisk);

        public void SendPortalTeleportNotAllowed(PortalTeleportNotAllowedReason reason) => _packetsHelper.SendPortalTeleportNotAllowed(Client, reason);

        public void SendTeleportViaNpc(NpcTeleportNotAllowedReason reason) => _packetsHelper.SendTeleportViaNpc(Client, reason, Gold);

        public void SendUseVehicle(bool success, bool status) => _packetsHelper.SendUseVehicle(Client, success, status);

        public void SendMyShape() => _packetsHelper.SendCharacterShape(Client, this);

        private void TargetChanged(IKillable target)
        {
            if (target is Mob)
            {
                _packetsHelper.SetMobInTarget(Client, (Mob)target);
            }
            else
            {
                _packetsHelper.SetPlayerInTarget(Client, (Character)target);
            }
        }

        public void SendAttribute(CharacterAttributeEnum attribute) =>
            _packetsHelper.SendAttribute(Client, attribute, GetAttributeValue(attribute));

        public void SendExperienceGain(ushort expAmount) => _packetsHelper.SendExperienceGain(Client, expAmount);

        public void SendWarning(string message) => _packetsHelper.SendWarning(Client, message);

        public void SendBankItems() => _packetsHelper.SendBankItems(Client, BankItems.Values.ToList());

        public void SendBankItemClaim(byte bankSlot, Item item) => _packetsHelper.SendBankItemClaim(Client, bankSlot, item);
        public void SendAccountPoints() => _packetsHelper.SendAccountPoints(Client, Points);

        public void SendResetSkills() => _packetsHelper.SendResetSkills(Client, SkillPoint);

        public void SendGuildCreateFailed(GuildCreateFailedReason reason) => _packetsHelper.SendGuildCreateFailed(Client, reason);

        public void SendGuildCreateSuccess(int guildId, byte rank, string guildName, string guildMessage) => _packetsHelper.SendGuildCreateSuccess(Client, guildId, rank, guildName, guildMessage);

        public void SendGuildCreateRequest(int creatorId, string guildName, string guildMessage) => _packetsHelper.SendGuildCreateRequest(Client, creatorId, guildName, guildMessage);

        public void SendGuildMemberIsOnline(int playerId) => _packetsHelper.SendGuildMemberIsOnline(Client, playerId);

        public void SendGuildMemberIsOffline(int playerId) => _packetsHelper.SendGuildMemberIsOffline(Client, playerId);

        public void SendGoldUpdate() => _packetsHelper.SendGoldUpdate(Client, Gold);
    }
}
