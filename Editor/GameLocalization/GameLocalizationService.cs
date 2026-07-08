using System.Collections.Generic;
using emiteat.NexUI.Localization;

namespace emiteat.NexUI.Designer.Editor.GameLocalization
{
    /// <summary>
    /// Authoring + checking for a game-facing <see cref="UIGameLocalizationTable"/>:
    /// add entries, find missing translations and detect too-long strings that may overflow.
    /// </summary>
    public static class GameLocalizationService
    {
        public static UIGameLocalizationEntry Add(UIGameLocalizationTable table, string key)
        {
            var e = new UIGameLocalizationEntry { key = key };
            table.entries.Add(e);
            return e;
        }

        public static List<string> FindMissing(UIGameLocalizationTable table)
        {
            var messages = new List<string>();
            if (table == null) return messages;
            var seen = new HashSet<string>();
            foreach (var e in table.entries)
            {
                if (string.IsNullOrEmpty(e.key)) { messages.Add("• empty key"); continue; }
                if (!seen.Add(e.key)) messages.Add($"• duplicate key: {e.key}");
                if (string.IsNullOrEmpty(e.koKR)) messages.Add($"• {e.key}: missing ko-KR");
                if (string.IsNullOrEmpty(e.enUS)) messages.Add($"• {e.key}: missing en-US");
            }
            return messages;
        }

        public static List<string> FindOverflow(UIGameLocalizationTable table, int maxChars)
        {
            var messages = new List<string>();
            if (table == null) return messages;
            foreach (var e in table.entries)
            {
                if (!string.IsNullOrEmpty(e.koKR) && e.koKR.Length > maxChars)
                    messages.Add($"• {e.key}: ko-KR is {e.koKR.Length} chars (> {maxChars})");
                if (!string.IsNullOrEmpty(e.enUS) && e.enUS.Length > maxChars)
                    messages.Add($"• {e.key}: en-US is {e.enUS.Length} chars (> {maxChars})");
            }
            return messages;
        }
    }
}
