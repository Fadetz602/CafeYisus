using System.Text.RegularExpressions;

namespace CafeYisus.Helpers
{
    public class RegexHelper
    {
        public static bool IsMatch(string input, string pattern)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, pattern);
        }
    }
}
