using System.Linq;
using System.Text.RegularExpressions;

namespace Imgeneus.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidCharacterName(this string name)
        {
            // Validate length
            if (name.Length < 3 || name.Length > 20)
                return false;

            // Validate allowed characters
            var validCharsPattern = @"[a-zA-Z0-9-_.]";

            if (!Regex.IsMatch(name, validCharsPattern))
                return false;

            // Don't allow more than 6 digits or names that only have digits
            var digitCount = name.Count(char.IsDigit);

            if (digitCount > 6 || name.Length == digitCount)
                return false;

            // Don't allow more than 6 symbols or names that only have symbols
            var symbolCount = name.Count(c => c == '-' || c == '_' || c == '.');

            if (symbolCount > 6 || name.Length == symbolCount)
                return false;

            // Don't allow two consecutive equal symbols
            if (name.Contains("..") || name.Contains("--") || name.Contains("__"))
                return false;

            // Don't allow more than 6 I and l because cheaters usually use names that look like "||||||||" in-game so that it's harder for them to get banned.
            var combinationCount = name.Count(c => c == 'l' || c == 'I');

            if (combinationCount > 6 || name.Length == combinationCount)
                return false;

            // Validate prohibited starting tags
            var staffPattern = @"^(\[(?:DEVELOPER|ADM(?:IN)?|DEV|TGS|G(?:MA|S)|GM)\]|DEVELOPER|ADMIN|DEV|ADM|TGS|G(?:MA|S)|GM)";

            if (Regex.IsMatch(name, staffPattern))
                return false;

            return true;
        }
    }
}
