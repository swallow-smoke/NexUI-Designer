using System.Collections.Generic;
using emiteat.NexUI.Abstractions;

namespace emiteat.NexUI.Designer.Editor.InputPreview
{
    /// <summary>
    /// Design-time analysis of how a screen behaves under an input mode, plus a summary
    /// of what changes per mode (focus indicator, hover motion, prompt glyphs).
    /// </summary>
    public static class InputModeService
    {
        public static List<string> Analyze(DesignerMetadataAsset asset, UIInputMode mode)
        {
            var messages = new List<string>();
            if (asset == null) return messages;

            bool anyInteractive = false, anyHover = false, anyFocus = false;
            foreach (var e in asset.elements)
            {
                if (e?.binding == null) continue;
                if (!string.IsNullOrEmpty(e.binding.commandKey) || !string.IsNullOrEmpty(e.binding.interactableKey))
                    anyInteractive = true;
                if (e.motion != null && !string.IsNullOrEmpty(e.motion.hoverVariant)) anyHover = true;
                if (e.motion != null && !string.IsNullOrEmpty(e.motion.focusVariant)) anyFocus = true;
            }

            switch (mode)
            {
                case UIInputMode.Touch:
                    if (anyInteractive)
                        messages.Add("• Touch: verify each button's hit area is large enough (min ~44px).");
                    break;
                case UIInputMode.Gamepad:
                case UIInputMode.SteamDeck:
                    if (anyInteractive && !anyFocus)
                        messages.Add("• Gamepad: interactive elements but no focus motion/graph — navigation may be unclear.");
                    break;
                case UIInputMode.KeyboardMouse:
                    if (anyInteractive && !anyHover)
                        messages.Add("• Keyboard/Mouse: no hover state on interactive elements.");
                    break;
            }
            return messages;
        }

        public static string[] Summary(UIInputMode mode)
        {
            switch (mode)
            {
                case UIInputMode.Touch:
                    return new[] { "focus indicator: hidden", "hover motion: off", "prompt glyph: touch" };
                case UIInputMode.Gamepad:
                    return new[] { "focus indicator: shown", "hover motion: off", "prompt glyph: gamepad" };
                case UIInputMode.SteamDeck:
                    return new[] { "focus indicator: shown", "hover motion: off", "prompt glyph: steamdeck" };
                default:
                    return new[] { "focus indicator: optional", "hover motion: on", "prompt glyph: keyboard" };
            }
        }
    }
}
