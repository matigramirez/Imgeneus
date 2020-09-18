using System;

namespace Imgeneus.Core.Extensions
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Generates new float between min and max value.
        /// </summary>
        public static float NextFloat(
            this Random random,
            float minValue,
            float maxValue)
        {
            return (float)random.NextDouble() * (maxValue - minValue) + minValue;
        }

        /// <summary>
        /// Generates random double value between min and max values.
        /// </summary>
        public static double NextDouble(
            this Random random,
            double minValue,
            double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
