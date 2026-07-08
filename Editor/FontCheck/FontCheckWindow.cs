using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.FontCheck
{
    /// <summary>Full UI for the font / Korean glyph checker (§15).</summary>
    public sealed class FontCheckWindow : NexUIToolWindow
    {
        [SerializeField] private Font _font;
        [SerializeField] private TMP_FontAsset _tmpFont;
        [SerializeField] private string _sample = FontGlyphService.DefaultSample;
        private List<string> _result;

        protected override string TitleKey => "panel.fontChecker";
        protected override string TooltipKey => "tooltip.fontChecker";

        [MenuItem("Tools/NexUI/Designer/Font Glyph Checker")]
        public static void Open() => GetWindow<FontCheckWindow>();

        protected override void DrawBody()
        {
            Section("panel.fontChecker");
            _sample = EditorGUILayout.TextField("sample text", _sample);
            _font = (Font)EditorGUILayout.ObjectField("Unity Font", _font, typeof(Font), false);
            _tmpFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font Asset", _tmpFont, typeof(TMP_FontAsset), false);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Check Font", GUILayout.Width(120)))
                _result = FontGlyphService.CheckFont(_font, _sample);
            if (GUILayout.Button("Check TMP", GUILayout.Width(120)))
                _result = FontGlyphService.CheckTMP(_tmpFont, _sample);
            EditorGUILayout.EndHorizontal();

            if (_result != null)
                EditorGUILayout.HelpBox(_result.Count == 0
                    ? T("message.validationPassed")
                    : T("validation.missingGlyph") + "\n" + string.Join("\n", _result),
                    _result.Count == 0 ? MessageType.Info : MessageType.Warning);
        }
    }
}
