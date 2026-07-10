using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.PromptGlyph
{
    /// <summary>Full UI for the prompt glyph system (§13).</summary>
    public sealed class PromptGlyphWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        private List<string> _missing;

        protected override string TitleKey => "panel.promptGlyph";
        protected override string TooltipKey => "tooltip.promptGlyph";

        [MenuItem("Tools/NexUI/Designer/Advanced/Prompt Glyphs")]
        public static void Open() => GetWindow<PromptGlyphWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);
            if (_asset == null)
            {
                EditorGUILayout.HelpBox(T("message.noScreenSelected"), MessageType.Info);
                return;
            }

            Section("panel.promptGlyph");
            foreach (var action in new List<DesignerPromptActionMetadata>(_asset.prompts.actions))
                DrawAction(action);

            if (GUILayout.Button(LC("button.create", "tooltip.promptGlyph"), GUILayout.Height(22)))
            {
                Undo.RecordObject(_asset, "Add Prompt Action");
                PromptGlyphService.AddAction(_asset, "Action" + (_asset.prompts.actions.Count + 1));
                MarkDirty(_asset);
            }

            Section("panel.validation");
            if (GUILayout.Button(LC("toolbar.validate"), GUILayout.Width(120)))
                _missing = PromptGlyphService.ValidateMissing(_asset);
            if (_missing != null)
                EditorGUILayout.HelpBox(_missing.Count == 0
                    ? T("message.validationPassed")
                    : T("validation.missingPromptGlyph") + "\n" + string.Join("\n", _missing),
                    _missing.Count == 0 ? MessageType.Info : MessageType.Warning);
        }

        private void DrawAction(DesignerPromptActionMetadata action)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            action.actionId = EditorGUILayout.TextField("actionId", action.actionId);
            if (GUILayout.Button(LC("button.delete"), GUILayout.Width(60)))
            {
                Undo.RecordObject(_asset, "Remove Prompt Action");
                PromptGlyphService.RemoveAction(_asset, action); MarkDirty(_asset);
                EditorGUILayout.EndHorizontal(); EditorGUILayout.EndVertical(); return;
            }
            EditorGUILayout.EndHorizontal();
            action.displayName = EditorGUILayout.TextField("displayName", action.displayName);

            PromptGlyphService.EnsureDevices(action);
            foreach (var d in action.devices)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(d.device.ToString(), GUILayout.Width(120));
                d.hasGlyph = GUILayout.Toggle(d.hasGlyph, "glyph", GUILayout.Width(60));
                d.textFallback = EditorGUILayout.TextField(d.textFallback);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
