using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace emiteat.NexUI.Designer.Editor.FontCheck
{
    /// <summary>
    /// Checks whether a font can render a sample string (Korean / special glyphs) and
    /// whether a TMP font asset has a fallback configured (§15).
    /// </summary>
    public static class FontGlyphService
    {
        public const string DefaultSample = "가나다한글ABCabc0123 ★→♥①";

        public static List<string> CheckFont(Font font, string sample)
        {
            var messages = new List<string>();
            if (font == null) { messages.Add("• no Font assigned"); return messages; }

            foreach (char c in Distinct(sample))
                if (!char.IsWhiteSpace(c) && !font.HasCharacter(c))
                    messages.Add($"• Font missing glyph: '{c}' (U+{(int)c:X4})");
            return messages;
        }

        public static List<string> CheckTMP(TMP_FontAsset font, string sample)
        {
            var messages = new List<string>();
            if (font == null) { messages.Add("• no TMP Font Asset assigned"); return messages; }

            foreach (char c in Distinct(sample))
                if (!char.IsWhiteSpace(c) && !font.HasCharacter(c))
                    messages.Add($"• TMP font missing glyph: '{c}' (U+{(int)c:X4})");

            if (font.fallbackFontAssetTable == null || font.fallbackFontAssetTable.Count == 0)
                messages.Add("• TMP Font Asset fallback is empty.");
            return messages;
        }

        private static IEnumerable<char> Distinct(string s)
        {
            var seen = new HashSet<char>();
            foreach (char c in s ?? string.Empty)
                if (seen.Add(c)) yield return c;
        }
    }
}
