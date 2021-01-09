using System;

namespace Imgeneus.Core.Extensions
{
    public static class MathExtensions
    {
        /// <summary>
        /// Calculates distance between 2 points.
        /// </summary>
        /// <param name="x1">point1 x coordinate</param>
        /// <param name="x2">point2 x coordinate</param>
        /// <param name="z1">point1 z coordinate</param>
        /// <param name="z2">point2 z coordinate</param>
        /// <returns></returns>
        public static double Distance(float x1, float x2, float z1, float z2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(z2 - z1, 2));
        }

        /// <summary>
        /// Rounds a number to the nearest multiple of 10.
        /// </summary>
        /// <param name="number">Number to round</param>
        public static uint RoundToTenMultiple(uint number)
        {
            var remaining = number % 10;
            return remaining >= 5 ? (number - remaining + 10) : (number - remaining);
        }
    }
}
