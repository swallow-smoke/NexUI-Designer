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

        [MenuItem("Tools/NexUI/Designer/UI Profiler")]
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

            if (_warnings != null && _warnings.Count > 0)
                EditorGUILayout.HelpBox(string.Join("\n", _warnings), MessageType.Warning);
        }

        private void ExportJson()
        {
            string path = EditorUtility.SaveFilePanel("Export Profiler JSON", "", "ui-profiler.json", "json");
            if (string.IsNullOrEmpty(path)) return;
            System.IO.File.WriteAllText(path, UIProfilerService.ToJson(_snapshot));
        }
    }
}
