using System.Collections.Generic;
using UnityEngine;
using emiteat.NexUI.Theme;

namespace emiteat.NexUI.Designer.Editor.Contrast
{
    /// <summary>
    /// Designer wrapper around <see cref="ThemeContrastChecker"/>: single-pair checks and
    /// a scan of a screen's theme token overrides for conventional fg/bg color pairs (§20).
    /// </summary>
    public static class ContrastService
    {
        public static (float ratio, bool aa, bool aaLarge) Check(Color fg, Color bg)
        {
            float r = ThemeContrastChecker.ContrastRatio(fg, bg);
            return (r, r >= ThemeContrastChecker.AaNormalText, r >= ThemeContrastChecker.AaLargeText);
        }

        private static readonly (string fg, string bg)[] Pairs =
        {
            ("text", "surface"), ("text", "bg"), ("danger", "bg"),
            ("button.text", "button.bg"), ("toast.text", "toast.bg")
        };

        public static List<string> ScanTokens(DesignerMetadataAsset asset)
        {
            var messages = new List<string>();
            if (asset == null) return messages;

            var colors = new Dictionary<string, Color>();
            foreach (var e in asset.elements)
            {
                if (e?.theme == null) continue;
                foreach (var tok in e.theme.tokenOverrides)
                    if (!string.IsNullOrEmpty(tok.key) && ColorUtility.TryParseHtmlString(tok.value, out var c))
                        colors[tok.key] = c;
            }

            foreach (var pair in Pairs)
            {
                if (!colors.TryGetValue(pair.fg, out var fg) || !colors.TryGetValue(pair.bg, out var bg))
                    continue;
                float ratio = ThemeContrastChecker.ContrastRatio(fg, bg);
                if (ratio < ThemeContrastChecker.AaNormalText)
                    messages.Add($"• {pair.fg} on {pair.bg}: contrast {ratio:F2}:1 (< 4.5:1)");
            }
            return messages;
        }
    }
}
