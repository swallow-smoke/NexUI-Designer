using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.FlowSimulator
{
    /// <summary>Full UI for the interaction flow simulator (§11).</summary>
    public sealed class FlowSimulatorWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        private int _elementIndex;
        private List<InteractionFlowStep> _steps;

        protected override string TitleKey => "panel.flowSimulator";
        protected override string TooltipKey => "tooltip.flowSimulator";

        [MenuItem("Tools/NexUI/Designer/Flow Simulator")]
        public static void Open() => GetWindow<FlowSimulatorWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);
            if (_asset == null)
            {
                EditorGUILayout.HelpBox(T("message.noScreenSelected"), MessageType.Info);
                return;
            }

            var ids = FlowSimulatorService.InteractiveElementIds(_asset);
            if (ids.Count == 0)
            {
                EditorGUILayout.HelpBox("No interactive elements (bind a command or interactable key).", MessageType.Info);
                return;
            }

            Section("panel.flowSimulator");
            EditorGUILayout.BeginHorizontal();
            _elementIndex = EditorGUILayout.Popup(Mathf.Clamp(_elementIndex, 0, ids.Count - 1), ids.ToArray());
            if (GUILayout.Button(LC("button.simulate", "tooltip.simulate"), GUILayout.Width(120)))
                _steps = FlowSimulatorService.Simulate(_asset, ids[Mathf.Clamp(_elementIndex, 0, ids.Count - 1)]);
            EditorGUILayout.EndHorizontal();

            if (_steps != null)
                foreach (var s in _steps)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"{s.order}", GUILayout.Width(24));
                    EditorGUILayout.LabelField($"[{s.kind}]", GUILayout.Width(110));
                    EditorGUILayout.LabelField(s.description);
                    EditorGUILayout.EndHorizontal();
                }
        }
    }
}
