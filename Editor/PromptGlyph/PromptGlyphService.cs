using System;
using System.Collections.Generic;
using emiteat.NexUI.Prompt;

namespace emiteat.NexUI.Designer.Editor.PromptGlyph
{
    /// <summary>
    /// Authoring + coverage checking for input prompt glyphs. Coverage is tracked per
    /// (action, device); missing-glyph validation reports devices that lack a glyph.
    /// </summary>
    public static class PromptGlyphService
    {
        public static readonly UIPromptDevice[] AllDevices =
            (UIPromptDevice[])Enum.GetValues(typeof(UIPromptDevice));

        public static DesignerPromptActionMetadata AddAction(DesignerMetadataAsset asset, string actionId)
        {
            var a = new DesignerPromptActionMetadata { actionId = actionId, displayName = actionId };
            asset.prompts.actions.Add(a);
            EnsureDevices(a);
            return a;
        }

        public static void RemoveAction(DesignerMetadataAsset asset, DesignerPromptActionMetadata action)
            => asset.prompts.actions.Remove(action);

        public static void EnsureDevices(DesignerPromptActionMetadata action)
        {
            foreach (var device in AllDevices)
            {
                bool found = false;
                foreach (var d in action.devices)
                    if (d.device == device) { found = true; break; }
                if (!found)
                    action.devices.Add(new DesignerPromptDeviceMetadata { device = device });
            }
        }

        public static List<string> ValidateMissing(DesignerMetadataAsset asset)
        {
            var messages = new List<string>();
            if (asset?.prompts == null) return messages;

            foreach (var action in asset.prompts.actions)
            {
                foreach (var device in AllDevices)
                {
                    var entry = action.devices.Find(d => d.device == device);
                    if (entry == null || (!entry.hasGlyph && string.IsNullOrEmpty(entry.textFallback)))
                        messages.Add($"• '{action.actionId}' missing glyph/fallback for {device}");
                }
            }
            return messages;
        }
    }
}
