using System.IO;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Common;
using emiteat.NexUI.Designer.Editor.Serialization;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Backend
{
    /// <summary>
    /// Dry-run preview + explicit write of generated UXML/USS from the open screen's Designer metadata
    /// (brief §31/§39.2). Generation is automatic; the file write is a deliberate button press and
    /// always targets SEPARATE generated files (<c>&lt;screen&gt;.g.uxml</c> / <c>.g.uss</c>) next to
    /// the metadata asset — the hand-authored UXML is never touched. Writing refuses to overwrite any
    /// existing file that does not carry the generated banner, so a user's file can never be clobbered.
    /// </summary>
    public sealed class UIToolkitGenerationWindow : NexUIToolWindow
    {
        private const string GeneratedMarker = "NEXUI:GENERATED";

        private string _uxml;
        private string _uss;
        private bool _generated;
        private Vector2 _uxmlScroll;
        private Vector2 _ussScroll;

        protected override string TitleKey => "uxmlGen.window.title";
        protected override string TooltipKey => "uxmlGen.window.description";

        [MenuItem("Tools/NexUI/Designer/Advanced/Generate UXML + USS")]
        public static void Open()
        {
            var window = GetWindow<UIToolkitGenerationWindow>();
            window.minSize = new Vector2(520f, 480f);
            window.Show();
        }

        private static NexUIDesignerContext ResolveContext()
            => Resources.FindObjectsOfTypeAll<NexUIDesignerWindow>().FirstOrDefault()?.Context;

        protected override void DrawBody()
        {
            var context = ResolveContext();
            var metadata = context?.Metadata;

            if (metadata == null)
            {
                Badge(T("uxmlGen.status.disconnected"), BadgeKind.Warning);
                EditorGUILayout.HelpBox(T("uxmlGen.help.noScreen"), MessageType.Info);
                if (GUILayout.Button(T("uxmlGen.status.openDesigner"), GUILayout.ExpandWidth(false)))
                    EditorApplication.ExecuteMenuItem("Tools/NexUI/Designer");
                return;
            }

            Badge($"{T("uxmlGen.status.target")}: {metadata.screenId}", BadgeKind.Ok);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(LC("uxmlGen.button.generate", "tooltip.uxmlGen.generate")))
                {
                    _uxml = UIToolkitCodeGenerator.GenerateUxml(metadata);
                    _uss = UIToolkitCodeGenerator.GenerateUss(metadata);
                    _generated = true;
                }

                using (new EditorGUI.DisabledScope(!_generated))
                {
                    if (GUILayout.Button(LC("uxmlGen.button.write", "tooltip.uxmlGen.write")))
                        WriteFiles(metadata);
                }
            }

            EditorGUILayout.HelpBox(T("uxmlGen.help.safety"), MessageType.Info);

            if (!_generated) return;

            var (uxmlPath, ussPath) = TargetPaths(metadata);
            EditorGUILayout.LabelField(T("uxmlGen.field.uxmlPath"), uxmlPath, EditorStyles.miniLabel);
            EditorGUILayout.LabelField(T("uxmlGen.field.ussPath"), ussPath, EditorStyles.miniLabel);

            Section("uxmlGen.section.uxml");
            _uxmlScroll = EditorGUILayout.BeginScrollView(_uxmlScroll, GUILayout.MaxHeight(180f));
            EditorGUILayout.TextArea(_uxml, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            Section("uxmlGen.section.uss");
            _ussScroll = EditorGUILayout.BeginScrollView(_ussScroll, GUILayout.MaxHeight(180f));
            EditorGUILayout.TextArea(_uss, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private static (string uxml, string uss) TargetPaths(DesignerMetadataAsset metadata)
        {
            var assetPath = AssetDatabase.GetAssetPath(metadata);
            var dir = string.IsNullOrEmpty(assetPath) ? "Assets" : Path.GetDirectoryName(assetPath);
            var baseName = string.IsNullOrEmpty(metadata.screenId) ? metadata.name : metadata.screenId;
            baseName = Sanitize(baseName);
            return (
                (dir + "/" + baseName + ".g.uxml").Replace('\\', '/'),
                (dir + "/" + baseName + ".g.uss").Replace('\\', '/'));
        }

        private void WriteFiles(DesignerMetadataAsset metadata)
        {
            var (uxmlPath, ussPath) = TargetPaths(metadata);
            if (!CanWrite(uxmlPath) || !CanWrite(ussPath))
            {
                EditorUtility.DisplayDialog(T("uxmlGen.window.title"), T("uxmlGen.dialog.refuseOverwrite"), "OK");
                return;
            }

            File.WriteAllText(uxmlPath, _uxml);
            File.WriteAllText(ussPath, _uss);
            AssetDatabase.ImportAsset(uxmlPath);
            AssetDatabase.ImportAsset(ussPath);
            AssetDatabase.Refresh();

            var written = AssetDatabase.LoadAssetAtPath<Object>(uxmlPath);
            if (written != null) EditorGUIUtility.PingObject(written);
            Debug.Log($"[NexUI] Generated {uxmlPath} + {ussPath}");
        }

        /// <summary>Only write to a path that is empty or already a NexUI-generated file — never over a
        /// user's own UXML/USS.</summary>
        private static bool CanWrite(string path)
        {
            if (!File.Exists(path)) return true;
            var existing = File.ReadAllText(path);
            return existing.Contains(GeneratedMarker);
        }

        private static string Sanitize(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Screen";
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}
