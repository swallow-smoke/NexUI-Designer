using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.InputPreview
{
    /// <summary>Full UI for input mode preview (§12).</summary>
    public sealed class InputPreviewWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        private UIInputMode _mode;
        private List<string> _analysis;

        protected override string TitleKey => "panel.inputPreview";
        protected override string TooltipKey => "tooltip.inputPreview";

        [MenuItem("Tools/NexUI/Designer/Advanced/Input Mode Preview")]
        public static void Open() => GetWindow<InputPreviewWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);

            Section("panel.inputPreview");
            _mode = (UIInputMode)EditorGUILayout.EnumPopup("input mode", _mode);

            foreach (var line in InputModeService.Summary(_mode))
                EditorGUILayout.LabelField("• " + line);

            using (new EditorGUI.DisabledScope(_asset == null))
                if (GUILayout.Button(LC("toolbar.validate"), GUILayout.Width(120)))
                    _analysis = InputModeService.Analyze(_asset, _mode);

            if (_analysis != null)
                EditorGUILayout.HelpBox(_analysis.Count == 0
                    ? T("message.validationPassed") : string.Join("\n", _analysis),
                    _analysis.Count == 0 ? MessageType.Info : MessageType.Warning);
        }
    }
}
