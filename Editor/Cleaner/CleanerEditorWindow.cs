using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.Cleaner
{
    /// <summary>Full UI for the dead reference cleaner (§10).</summary>
    public sealed class CleanerEditorWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        private List<DeadReference> _dead;

        protected override string TitleKey => "panel.cleaner";
        protected override string TooltipKey => "tooltip.cleaner";

        [MenuItem("Tools/NexUI/Designer/Dead Reference Cleaner")]
        public static void Open() => GetWindow<CleanerEditorWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);
            if (_asset == null)
            {
                EditorGUILayout.HelpBox(T("message.noScreenSelected"), MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(LC("button.findDeadReferences", "tooltip.findDeadReferences"), GUILayout.Height(24)))
                _dead = CleanerService.Find(_asset);
            using (new EditorGUI.DisabledScope(_dead == null || _dead.Count == 0))
                if (GUILayout.Button(LC("button.clean", "tooltip.clean"), GUILayout.Height(24), GUILayout.Width(120)))
                {
                    Undo.RecordObject(_asset, "Clean Dead References");
                    CleanerService.CleanAll(_dead);
                    MarkDirty(_asset);
                    _dead = CleanerService.Find(_asset);
                }
            EditorGUILayout.EndHorizontal();

            if (_dead == null) return;

            Section("panel.cleaner");
            if (_dead.Count == 0)
            {
                EditorGUILayout.HelpBox(T("message.validationPassed"), MessageType.Info);
                return;
            }

            for (int i = 0; i < _dead.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(T("validation.deadReference"), GUILayout.Width(140));
                EditorGUILayout.LabelField(_dead[i].description);
                if (GUILayout.Button(LC("button.delete"), GUILayout.Width(60)))
                {
                    Undo.RecordObject(_asset, "Remove Dead Reference");
                    _dead[i].fix?.Invoke();
                    MarkDirty(_asset);
                    _dead = CleanerService.Find(_asset);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
