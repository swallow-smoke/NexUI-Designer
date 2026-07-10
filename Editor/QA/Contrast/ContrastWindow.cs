using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.Contrast
{
    /// <summary>Full UI for the theme contrast checker (§20).</summary>
    public sealed class ContrastWindow : NexUIToolWindow
    {
        [SerializeField] private Color _foreground = Color.white;
        [SerializeField] private Color _background = new Color(0.12f, 0.12f, 0.14f);
        [SerializeField] private DesignerMetadataAsset _asset;
        private List<string> _scan;

        protected override string TitleKey => "panel.contrastChecker";
        protected override string TooltipKey => "tooltip.contrastChecker";

        [MenuItem("Tools/NexUI/Designer/QA/Contrast Checker")]
        public static void Open() => GetWindow<ContrastWindow>();

        protected override void DrawBody()
        {
            Section("panel.contrastChecker");
            _foreground = EditorGUILayout.ColorField("foreground", _foreground);
            _background = EditorGUILayout.ColorField("background", _background);

            var (ratio, aa, aaLarge) = ContrastService.Check(_foreground, _background);
            EditorGUILayout.LabelField("contrast ratio", $"{ratio:F2} : 1");
            EditorGUILayout.HelpBox(
                $"AA normal (4.5): {(aa ? "PASS" : "FAIL")}   •   AA large (3.0): {(aaLarge ? "PASS" : "FAIL")}",
                aa ? MessageType.Info : MessageType.Warning);

            EditorGUILayout.Space(6);
            Section("panel.validation");
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);
            using (new EditorGUI.DisabledScope(_asset == null))
                if (GUILayout.Button("Scan theme tokens", GUILayout.Width(160)))
                    _scan = ContrastService.ScanTokens(_asset);

            if (_scan != null)
                EditorGUILayout.HelpBox(_scan.Count == 0
                    ? T("message.validationPassed")
                    : T("validation.lowContrast") + "\n" + string.Join("\n", _scan),
                    _scan.Count == 0 ? MessageType.Info : MessageType.Warning);
        }
    }
}
