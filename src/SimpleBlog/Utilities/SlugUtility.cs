using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleBlog.Utilities
{
    public static class SlugUtility
    {
        public static string ToSlug(this string input, int maxLength = 72)
        {
            var str = input.RemoveDiacritics().ToLowerInvariant();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        public static bool IsSlug(this string input)
        {
            return !string.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, "^[a-z0-9]+(?:-[a-z0-9]+)*$");
        }

        private static string RemoveDiacritics(this string text)
        {
            var s = new string(text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            return s.Normalize(NormalizationForm.FormC);
        }
    }
}
