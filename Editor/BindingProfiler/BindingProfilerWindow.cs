using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.BindingProfiler
{
    /// <summary>Full UI for the binding profiler (§17).</summary>
    public sealed class BindingProfilerWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        private List<string> _issues;
        private int _bindingCount;

        protected override string TitleKey => "panel.bindingProfiler";
        protected override string TooltipKey => "tooltip.bindingProfiler";

        [MenuItem("Tools/NexUI/Designer/Binding Profiler")]
        public static void Open() => GetWindow<BindingProfilerWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);

            using (new EditorGUI.DisabledScope(_asset == null))
                if (GUILayout.Button(LC("toolbar.validate"), GUILayout.Height(22), GUILayout.Width(120)))
                    _issues = BindingProfilerService.Analyze(_asset, out _bindingCount);

            if (_issues == null) return;
            Section("panel.bindingProfiler");
            EditorGUILayout.LabelField("total bindings", _bindingCount.ToString());
            EditorGUILayout.HelpBox(_issues.Count == 0 ? T("message.validationPassed") : string.Join("\n", _issues),
                _issues.Count == 0 ? MessageType.Info : MessageType.Warning);
        }
    }
}
