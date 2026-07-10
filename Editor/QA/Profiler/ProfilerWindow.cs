using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.Profiler
{
    /// <summary>Full UI for the UI performance profiler (§16).</summary>
    public sealed class ProfilerWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        private UIProfilerSnapshot _snapshot;
        private List<string> _warnings;

        protected override string TitleKey => "panel.profiler";
        protected override string TooltipKey => "tooltip.profiler";

        [MenuItem("Tools/NexUI/Designer/QA/UI Profiler")]
        public static void Open() => GetWindow<ProfilerWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);

            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(_asset == null))
            {
                if (GUILayout.Button("Capture", GUILayout.Height(22)))
                {
                    _snapshot = UIProfilerService.CaptureStatic(_asset);
                    _warnings = UIProfilerService.Warnings(_snapshot);
                }
                using (new EditorGUI.DisabledScope(_snapshot == null))
                    if (GUILayout.Button(LC("button.exportManifest", "tooltip.exportManifest"), GUILayout.Height(22), GUILayout.Width(160)))
                        ExportJson();
            }
            EditorGUILayout.EndHorizontal();

            if (_snapshot == null) return;
            Section("panel.profiler");
            EditorGUILayout.LabelField("opened screens", _snapshot.openedScreenCount.ToString());
            EditorGUILayout.LabelField("elements", _snapshot.elementCount.ToString());
            EditorGUILayout.LabelField("bindings", _snapshot.bindingCount.ToString());
            EditorGUILayout.LabelField("active motions", _snapshot.activeMotionCount.ToString());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("estimated vertices (heuristic)", _snapshot.estimatedVertexCount.ToString());
            EditorGUILayout.LabelField("recommended Vertex Budget", _snapshot.recommendedVertexBudget.ToString());
            EditorGUILayout.LabelField("estimated batch groups (proxy)", _snapshot.estimatedBatchGroups.ToString());
            EditorGUILayout.LabelField("overdraw estimate", _snapshot.overdrawScore.ToString("P0"));
            EditorGUILayout.LabelField("largest cost contributor", UIProfilerService.ClassifyBottleneck(_snapshot));

            if (_warnings != null && _warnings.Count > 0)
                EditorGUILayout.HelpBox(string.Join("\n", _warnings), MessageType.Warning);

            EditorGUILayout.HelpBox(
                "Vertex/batch/overdraw numbers are design-time estimates from element type + rect + tint, " +
                "not a live GPU/Frame Debugger measurement. Use them to catch obviously expensive screens " +
                "early; verify real numbers with the Unity Profiler before shipping a performance claim.",
                MessageType.Info);
        }

        private void ExportJson()
        {
            string path = EditorUtility.SaveFilePanel("Export Profiler JSON", "", "ui-profiler.json", "json");
            if (string.IsNullOrEmpty(path)) return;
            System.IO.File.WriteAllText(path, UIProfilerService.ToJson(_snapshot));
        }
    }
}
