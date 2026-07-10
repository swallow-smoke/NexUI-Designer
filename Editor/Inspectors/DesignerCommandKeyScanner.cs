using System.Collections.Generic;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// Lists candidate <c>binding.commandKey</c> values for the Binding inspector's command
    /// picker (C1: drag-and-drop-first event-&gt;action wiring - a non-programmer picks from a
    /// list instead of typing/knowing a class name). There's no formal command-key registry in
    /// the runtime (a key is just resolved through <c>UIActionResolver</c> at runtime, by
    /// design - see <c>UICommandBinder</c>), so this offers the small set of built-in pipeline
    /// keys plus every commandKey already used anywhere in the project's
    /// <see cref="DesignerMetadataAsset"/>s, which doubles as project-wide autocomplete/consistency
    /// (reusing an existing key instead of a near-duplicate typo).
    /// </summary>
    internal static class DesignerCommandKeyScanner
    {
        /// <summary>Keys the built-in Command Pipeline always understands (see emiteat.NexUI.Core.UICommands).</summary>
        public static readonly string[] BuiltIn = { "nexui.close", "nexui.back", "nexui.toggle" };

        public static IReadOnlyList<string> Find()
        {
            var keys = new SortedSet<string>(System.StringComparer.Ordinal);
            foreach (var key in BuiltIn) keys.Add(key);

            foreach (var guid in AssetDatabase.FindAssets("t:" + nameof(DesignerMetadataAsset)))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<DesignerMetadataAsset>(path);
                if (asset == null) continue;
                foreach (var element in asset.elements)
                {
                    var key = element?.binding?.commandKey;
                    if (!string.IsNullOrEmpty(key)) keys.Add(key);
                }
            }

            return new List<string>(keys);
        }
    }
}
