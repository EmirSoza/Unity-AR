using System.Text.RegularExpressions;

namespace LivelyTextGlyphs
{
    internal static class StringExtensions
    {
        internal static string SubstringUpToMatch(this string s, Match m)
        {
            return s.Substring(0, m.Index);
        }

        internal static string SubstringBetweenMatches(this string s, Match m1, Match m2)
        {
            return s.Substring(m1.Index + m1.Length, m2.Index - (m1.Index + m1.Length));
        }

        internal static string SubstringAfterMatch(this string s, Match m)
        {
            return s.Substring(m.Index + m.Length);
        }
    }
}
