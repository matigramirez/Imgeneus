using Imgeneus.Database.Constants;
using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Player;
using System;

namespace Imgeneus.World.Game.Linking
{
    public class LinkingManager : ILinkingManager
    {
        private readonly Random _random = new Random();
        private readonly IDatabasePreloader _databasePreloader;

        public LinkingManager(IDatabasePreloader databasePreloader)
        {
            _databasePreloader = databasePreloader;
        }

        public (bool Success, byte Slot) AddGem(Item item, Item gem, Item hammer)
        {
            double rate = GetRate(gem, hammer);
            var rand = _random.Next(1, 101);
            var success = rate >= rand;
            byte slot = 0;
            if (success)
            {
                if (item.Gem1 is null)
                {
                    item.Gem1 = new Gem(_databasePreloader, gem.TypeId);
                    slot = 0;
                }
                else if (item.Gem2 is null)
                {
                    item.Gem2 = new Gem(_databasePreloader, gem.TypeId);
                    slot = 1;
                }
                else if (item.Gem3 is null)
                {
                    item.Gem3 = new Gem(_databasePreloader, gem.TypeId);
                    slot = 2;
                }
                else if (item.Gem4 is null)
                {
                    item.Gem4 = new Gem(_databasePreloader, gem.TypeId);
                    slot = 3;
                }
                else if (item.Gem2 is null)
                {
                    item.Gem5 = new Gem(_databasePreloader, gem.TypeId);
                    slot = 4;
                }
                else if (item.Gem2 is null)
                {
                    item.Gem6 = new Gem(_databasePreloader, gem.TypeId);
                    slot = 5;
                }
            }
            gem.Count--;
            return (success, slot);
        }

        public double GetRate(Item gem, Item hammer)
        {
            double rate;
            switch (gem.ReqIg)
            {
                case 30:
                    rate = 50;
                    break;

                case 31:
                    rate = 46;
                    break;

                case 32:
                    rate = 40;
                    break;

                case 33:
                    rate = 32;
                    break;

                case 34:
                    rate = 24;
                    break;

                case 35:
                    rate = 16;
                    break;

                case 36:
                    rate = 8;
                    break;

                case 37:
                    rate = 2;
                    break;

                case 38:
                    rate = 1;
                    break;

                case 39:
                    rate = 1;
                    break;

                case 40:
                    rate = 1;
                    break;

                case 99:
                    rate = 1;
                    break;

                default:
                    rate = 1;
                    break;
            }

            if (hammer != null)
            {
                if (hammer.Special == SpecialEffect.LinkingHammer)
                {
                    rate = rate * (hammer.ReqVg / 100);
                    if (rate > 50)
                        rate = 50;
                }

                if (hammer.Special == SpecialEffect.PerfectLinkingHammer)
                    rate = 100;
            }

            return rate;
        }

        public int GetGold(Item gem)
        {
            int gold;
            switch (gem.ReqIg)
            {
                case 30:
                    gold = 1000;
                    break;

                case 31:
                    gold = 4095;
                    break;

                case 32:
                    gold = 11250;
                    break;

                case 33:
                    gold = 22965;
                    break;

                case 34:
                    gold = 41280;
                    break;

                case 35:
                    gold = 137900;
                    break;

                case 36:
                    gold = 365000;
                    break;

                case 37:
                    gold = 480000;
                    break;

                case 38:
                    gold = 627000;
                    break;

                case 39:
                    gold = 814000;
                    break;

                case 40:
                    gold = 1040000;
                    break;

                case 99:
                    gold = 7500000;
                    break;

                default:
                    gold = 1;
                    break;
            }

            return gold;
        }
    }
}
