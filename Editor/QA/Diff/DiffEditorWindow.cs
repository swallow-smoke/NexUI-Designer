using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.Diff
{
    /// <summary>Full UI for metadata visual diff (§8).</summary>
    public sealed class DiffEditorWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _before;
        [SerializeField] private DesignerMetadataAsset _after;
        private List<string> _diff;

        protected override string TitleKey => "panel.diff";
        protected override string TooltipKey => "tooltip.diff";

        public static void Open() => GetWindow<DiffEditorWindow>();

        protected override void DrawBody()
        {
            _before = (DesignerMetadataAsset)EditorGUILayout.ObjectField("Before", _before, typeof(DesignerMetadataAsset), false);
            _after = (DesignerMetadataAsset)EditorGUILayout.ObjectField("After", _after, typeof(DesignerMetadataAsset), false);

            using (new EditorGUI.DisabledScope(_before == null || _after == null))
                if (GUILayout.Button(LC("button.compare", "tooltip.diff"), GUILayout.Height(24)))
                    _diff = DesignerDiffService.DiffMetadata(_before, _after);

            if (_diff != null)
            {
                Section("panel.diff");
                EditorGUILayout.HelpBox(_diff.Count == 0 ? "(no change)" : string.Join("\n", _diff),
                    _diff.Count == 0 ? MessageType.Info : MessageType.None);
            }
        }
    }
}
