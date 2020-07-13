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
    }
}
