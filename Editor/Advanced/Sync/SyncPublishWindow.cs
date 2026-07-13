using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Sync
{
    /// <summary>
    /// Bidirectional sync + changed-only publish UI (brief §32/§39.3). Shows the sync state of the open
    /// screen's generated UXML/USS against the on-disk files and the published baseline, offers Use
    /// Designer / Use Backend / Review Diff, and runs a changed-only publish (dry-run or write) over
    /// every screen in the project.
    /// </summary>
    public sealed class SyncPublishWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerPublishManifest _manifest;
        private bool _showDiff;
        private Vector2 _diffScroll;
        private List<PublishAction> _lastReport;

        protected override string TitleKey => "sync.window.title";
        protected override string TooltipKey => "sync.window.description";

        public static void Open()
        {
            var window = GetWindow<SyncPublishWindow>();
            window.minSize = new Vector2(540f, 500f);
            window.Show();
        }

        private static NexUIDesignerContext ResolveContext()
            => DesignerSessions.ActiveContext;

        protected override void DrawBody()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var picked = (DesignerPublishManifest)EditorGUILayout.ObjectField(
                    LC("sync.field.manifest", "tooltip.sync.manifest"), _manifest, typeof(DesignerPublishManifest), false);
                if (check.changed) _manifest = picked;
            }
            if (GUILayout.Button(LC("sync.button.createManifest", "tooltip.sync.createManifest")))
                _manifest = CreateManifest();

            if (_manifest == null)
                EditorGUILayout.HelpBox(T("sync.help.noManifest"), MessageType.Info);

            DrawCurrentScreen();
            DrawPublishAll();
        }

        private void DrawCurrentScreen()
        {
            Section("sync.section.current");
            var metadata = ResolveContext()?.Metadata;
            if (metadata == null)
            {
                EditorGUILayout.HelpBox(T("sync.help.noScreen"), MessageType.Info);
                return;
            }

            var status = DesignerPublishService.Evaluate(metadata, _manifest);
            Badge($"{metadata.screenId}: {StateLabel(status.Combined)}", BadgeFor(status.Combined));
            EditorGUILayout.LabelField("UXML", $"{StateLabel(status.UxmlState)}  ({status.UxmlPath})", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("USS", $"{StateLabel(status.UssState)}  ({status.UssPath})", EditorStyles.miniLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(LC("sync.button.useDesigner", "tooltip.sync.useDesigner")))
                {
                    if (!DesignerPublishService.WriteAndRecord(status, _manifest))
                        EditorUtility.DisplayDialog(T("sync.window.title"), T("sync.dialog.blocked"), "OK");
                }
                using (new EditorGUI.DisabledScope(status.Combined == SyncState.New || status.Combined == SyncState.InSync))
                {
                    if (GUILayout.Button(LC("sync.button.useBackend", "tooltip.sync.useBackend")))
                        DesignerPublishService.AdoptBackend(status, _manifest);
                }
                _showDiff = GUILayout.Toggle(_showDiff, LC("sync.button.reviewDiff", "tooltip.sync.reviewDiff"), EditorStyles.miniButton);
            }

            if (_showDiff)
                DrawDiff(status);
        }

        private void DrawDiff(ScreenSyncStatus status)
        {
            _diffScroll = EditorGUILayout.BeginScrollView(_diffScroll, GUILayout.MaxHeight(240f));
            DrawFileDiff("UXML", status.DesignerUxml, status.FileUxml);
            DrawFileDiff("USS", status.DesignerUss, status.FileUss);
            EditorGUILayout.EndScrollView();
        }

        private void DrawFileDiff(string label, string designer, string file)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            // Diff is oriented file (A) -> designer (B): '+' = the Designer would add, '-' = would remove.
            var diff = TextLineDiff.Diff(file ?? string.Empty, designer ?? string.Empty);
            if (TextLineDiff.ChangeCount(diff) == 0)
            {
                EditorGUILayout.LabelField(T("sync.diff.identical"), EditorStyles.miniLabel);
                return;
            }
            var previous = GUI.color;
            foreach (var line in diff)
            {
                switch (line.Type)
                {
                    case DiffLine.Kind.Added: GUI.color = new Color(0.6f, 1f, 0.6f); EditorGUILayout.LabelField("+ " + line.Text, EditorStyles.miniLabel); break;
                    case DiffLine.Kind.Removed: GUI.color = new Color(1f, 0.6f, 0.6f); EditorGUILayout.LabelField("- " + line.Text, EditorStyles.miniLabel); break;
                    default: GUI.color = previous; EditorGUILayout.LabelField("  " + line.Text, EditorStyles.miniLabel); break;
                }
            }
            GUI.color = previous;
        }

        private void DrawPublishAll()
        {
            Section("sync.section.publishAll");
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(LC("sync.button.dryRun", "tooltip.sync.dryRun")))
                    _lastReport = DesignerPublishService.PublishChanged(_manifest, dryRun: true);
                if (GUILayout.Button(LC("sync.button.publish", "tooltip.sync.publish")))
                    _lastReport = DesignerPublishService.PublishChanged(_manifest, dryRun: false);
            }

            if (_lastReport == null) return;
            if (_lastReport.Count == 0)
            {
                EditorGUILayout.HelpBox(T("sync.report.noScreens"), MessageType.Info);
                return;
            }
            foreach (var action in _lastReport)
                EditorGUILayout.LabelField(action.ScreenId, action.Result);
        }

        private BadgeKind BadgeFor(SyncState state)
            => state == SyncState.InSync ? BadgeKind.Ok
             : state == SyncState.Conflict || state == SyncState.BackendChanged ? BadgeKind.Warning
             : BadgeKind.Muted;

        private static string StateLabel(SyncState state) => DesignerLocalizationKey(state);

        private static string DesignerLocalizationKey(SyncState state)
        {
            switch (state)
            {
                case SyncState.New: return T("sync.state.new");
                case SyncState.InSync: return T("sync.state.inSync");
                case SyncState.DesignerChanged: return T("sync.state.designerChanged");
                case SyncState.BackendChanged: return T("sync.state.backendChanged");
                default: return T("sync.state.conflict");
            }
        }

        private static DesignerPublishManifest CreateManifest()
        {
            var asset = ScriptableObject.CreateInstance<DesignerPublishManifest>();
            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/NexUIPublishManifest.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(asset);
            return asset;
        }
    }
}
