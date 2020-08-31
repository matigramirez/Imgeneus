using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.World.Game.Blessing;
using System;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        private void OnDarkBlessChanged(BlessArgs args)
        {
            if (Country == Fraction.Dark)
                AddBlessBonuses(args);

            if (Client != null)
                SendBlessUpdate(1, args.NewValue);
        }

        private void OnLightBlessChanged(BlessArgs args)
        {
            if (Country == Fraction.Light)
                AddBlessBonuses(args);

            if (Client != null)
                SendBlessUpdate(0, args.NewValue);
        }

        /// <summary>
        /// Sends update of bonuses, based on bless amount change.
        /// </summary>
        /// <param name="args">bless args</param>
        private void AddBlessBonuses(BlessArgs args)
        {
            if (args.OldValue >= Bless.MAX_HP_SP_MP && args.NewValue < Bless.MAX_HP_SP_MP)
            {
                ExtraHP -= ConstHP / 5;
                ExtraMP -= ConstMP / 5;
                ExtraSP -= ConstSP / 5;
            }
            if (args.OldValue < Bless.MAX_HP_SP_MP && args.NewValue >= Bless.MAX_HP_SP_MP)
            {
                ExtraHP += ConstHP / 5;
                ExtraMP += ConstMP / 5;
                ExtraSP += ConstSP / 5;
            }
        }

        /// <summary>
        /// Sends new bless amount.
        /// </summary>
        private void SendBlessUpdate(byte fraction, int amount)
        {
            using var packet = new Packet(PacketType.BLESS_UPDATE);
            packet.Write(fraction);
            packet.Write(amount);
            Client.SendPacket(packet);
        }

        /// <summary>
        /// Sends initial bless amount.
        /// </summary>
        private void SendBlessAmount()
        {
            using var packet = new Packet(PacketType.BLESS_INIT);
            packet.Write((byte)Country);

            var blessAmount = Country == Fraction.Light ? Bless.Instance.LightAmount : Bless.Instance.DarkAmount;
            packet.Write(blessAmount);
            packet.Write(Bless.Instance.RemainingTime);

            Client.SendPacket(packet);
        }
    }
}
