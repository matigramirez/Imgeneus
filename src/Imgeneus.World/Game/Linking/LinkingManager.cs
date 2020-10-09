using Imgeneus.Database.Constants;
using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Player;
using System;
using System.Collections.Generic;

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
                    slot = 0;
                    item.Gem1 = new Gem(_databasePreloader, gem.TypeId, slot);
                }
                else if (item.Gem2 is null)
                {
                    slot = 1;
                    item.Gem2 = new Gem(_databasePreloader, gem.TypeId, slot);
                }
                else if (item.Gem3 is null)
                {
                    slot = 2;
                    item.Gem3 = new Gem(_databasePreloader, gem.TypeId, slot);
                }
                else if (item.Gem4 is null)
                {
                    slot = 3;
                    item.Gem4 = new Gem(_databasePreloader, gem.TypeId, slot);
                }
                else if (item.Gem2 is null)
                {
                    slot = 4;
                    item.Gem5 = new Gem(_databasePreloader, gem.TypeId, slot);
                }
                else if (item.Gem2 is null)
                {
                    slot = 5;
                    item.Gem6 = new Gem(_databasePreloader, gem.TypeId, slot);
                }
            }
            gem.Count--;
            return (success, slot);
        }

        public bool RemoveGem(Item item, Gem gem, Item hammer)
        {
            var rate = GetRemoveRate(gem, hammer);
            var rand = _random.Next(1, 101);
            var success = rate >= rand;

            return success;
        }

        public double GetRate(Item gem, Item hammer)
        {
            double rate = GetRateByReqIg(gem.ReqIg);

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
            int gold = GetGoldByReqIg(gem.ReqIg);
            return gold;
        }

        public double GetRemoveRate(Gem gem, Item hammer)
        {
            double rate = GetRateByReqIg(gem.ReqIg);

            if (hammer != null)
            {
                if (hammer.Special == SpecialEffect.ExtractionHammer)
                {
                    if (hammer.ReqVg == 40) // Small extracting hammer.
                        rate = 40;

                    if (hammer.ReqVg >= 80) // Big extracting hammer.
                        rate = 80;
                }

                if (hammer.Special == SpecialEffect.PerfectExtractionHammer)
                    rate = 100; // GM exrracting hammer. Usually it's item with Type = 44 and TypeId = 237 or create your own.
            }

            return rate;
        }

        public int GetRemoveGold(Gem gem)
        {
            int gold = GetGoldByReqIg(gem.ReqIg);
            return gold;
        }

        private double GetRateByReqIg(byte ReqIg)
        {
            double rate;
            switch (ReqIg)
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

                case 255: // ONLY FOR TESTS!
                    rate = 100;
                    break;

                default:
                    rate = 0;
                    break;
            }

            return rate;
        }

        private int GetGoldByReqIg(byte reqIg)
        {
            int gold;
            switch (reqIg)
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
                    gold = 0;
                    break;
            }

            return gold;
        }

        public void Compose(Item item)
        {
            item.ComposedStr = 0;
            item.ComposedDex = 0;
            item.ComposedRec = 0;
            item.ComposedInt = 0;
            item.ComposedWis = 0;
            item.ComposedLuc = 0;
            item.ComposedHP = 0;
            item.ComposedMP = 0;
            item.ComposedSP = 0;

            var indexes = new List<int>();
            do
            {
                var index = _random.Next(0, 9);
                if (!indexes.Contains(index))
                    indexes.Add(index);
            }
            while (indexes.Count != 3);

            foreach (var i in indexes)
            {
                switch (i)
                {
                    case 0:
                        item.ComposedStr = _random.Next(1, item.ReqWis + 1);
                        break;

                    case 1:
                        item.ComposedDex = _random.Next(1, item.ReqWis + 1);
                        break;

                    case 2:
                        item.ComposedRec = _random.Next(1, item.ReqWis + 1);
                        break;

                    case 3:
                        item.ComposedInt = _random.Next(1, item.ReqWis + 1);
                        break;

                    case 4:
                        item.ComposedWis = _random.Next(1, item.ReqWis + 1);
                        break;

                    case 5:
                        item.ComposedLuc = _random.Next(1, item.ReqWis + 1);
                        break;

                    case 6:
                        item.ComposedHP = _random.Next(1, item.ReqWis + 1) * 100;
                        break;

                    case 7:
                        item.ComposedMP = _random.Next(1, item.ReqWis + 1) * 100;
                        break;

                    case 8:
                        item.ComposedSP = _random.Next(1, item.ReqWis + 1) * 100;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
