using System;

namespace Imgeneus.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static int ToShaiyaTime(this DateTime time)
        {
            int num1 = 16 * (time.Year - 16);
            int num2 = 32 * (time.Month + num1);
            return time.Second + (time.Minute + (time.Hour + 32 * (time.Day + num2) << 6) << 6);
        }
    }
}
