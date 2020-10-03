using Imgeneus.World.Game.Player;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Imgeneus.World.Game.Dyeing
{
    public class DyeingManager : IDyeingManager
    {
        public Item DyeingItem { get; set; }

        public List<DyeColor> AvailableColors { get; private set; } = new List<DyeColor>();

        private Dictionary<int, Color> _colors = new Dictionary<int, Color>()
        {
            { 0, Color.White },
            { 1, Color.Black },
            { 2, Color.Red },
            { 3, Color.Orange },
            { 4, Color.Yellow },
            { 5, Color.LightBlue },
            { 6, Color.Blue },
            { 7, Color.Green },
            { 8, Color.DarkGreen },
            { 9, Color.Azure },
            { 10, Color.Violet },
            { 11, Color.PaleVioletRed },
            { 12, Color.Brown },
            { 13, Color.DeepPink },
            { 14, Color.Gray },
            { 15, Color.LightSeaGreen },
            { 16, Color.MintCream },
            { 17, Color.YellowGreen },
            { 18, Color.OrangeRed },
            { 19, Color.Chocolate },
            { 20, Color.Crimson },
        };

        private Random _random = new Random();

        public void Reroll()
        {
            AvailableColors.Clear();

            // Always generate 5 random colors.
            Color color;
            byte saturation = 35;

            color = _colors[_random.Next(0, _colors.Keys.Count)];
            AvailableColors.Add(new DyeColor(200, saturation, color.R, color.G, color.B));

            color = _colors[_random.Next(0, _colors.Keys.Count)];
            saturation += 15;
            AvailableColors.Add(new DyeColor(200, saturation, color.R, color.G, color.B));

            color = _colors[_random.Next(0, _colors.Keys.Count)];
            saturation += 50;
            AvailableColors.Add(new DyeColor(200, saturation, color.R, color.G, color.B));

            color = _colors[_random.Next(0, _colors.Keys.Count)];
            saturation += 50;
            AvailableColors.Add(new DyeColor(200, saturation, color.R, color.G, color.B));

            color = _colors[_random.Next(0, _colors.Keys.Count)];
            saturation += 50;
            AvailableColors.Add(new DyeColor(200, saturation, color.R, color.G, color.B));
        }
    }
}
