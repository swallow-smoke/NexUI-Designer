using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using emiteat.NexUI.Designer.Editor.Serialization;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor.Sync
{
    /// <summary>Combined sync status of a screen's two generated files.</summary>
    public struct ScreenSyncStatus
    {
        public DesignerMetadataAsset Metadata;
        public string ScreenId;
        public string UxmlPath;
        public string UssPath;
        public SyncState UxmlState;
        public SyncState UssState;
        public string DesignerUxml;
        public string DesignerUss;
        public string FileUxml;
        public string FileUss;

        /// <summary>Worst-of the two file states: a screen needs resolution if either file conflicts or
        /// was backend-edited, is changed if either is New/DesignerChanged, else in sync.</summary>
        public SyncState Combined =>
            UxmlState == SyncState.Conflict || UssState == SyncState.Conflict ? SyncState.Conflict :
            UxmlState == SyncState.BackendChanged || UssState == SyncState.BackendChanged ? SyncState.BackendChanged :
            UxmlState == SyncState.New || UssState == SyncState.New ? SyncState.New :
            UxmlState == SyncState.DesignerChanged || UssState == SyncState.DesignerChanged ? SyncState.DesignerChanged :
            SyncState.InSync;
    }

    /// <summary>One line of a changed-only publish report.</summary>
    public struct PublishAction
    {
        public string ScreenId;
        public string Result; // Published / Skipped (in sync) / Needs resolution / ...
    }

    /// <summary>
    /// Bidirectional-sync + changed-only publish over generated UXML/USS files (brief §32/§39.3).
    /// Generation reuses <see cref="UIToolkitCodeGenerator"/>; the baseline lives in a
    /// <see cref="DesignerPublishManifest"/>. Writing always targets the separate <c>.g.uxml</c>/
    /// <c>.g.uss</c> files and refuses to overwrite a file that lost the generated banner, so a
    /// hand-edited file is never silently clobbered — it surfaces as BackendChanged/Conflict instead.
    /// </summary>
    public static class DesignerPublishService
    {
        private const string GeneratedMarker = "NEXUI:GENERATED";

        public static ScreenSyncStatus Evaluate(DesignerMetadataAsset metadata, DesignerPublishManifest manifest)
        {
            var status = new ScreenSyncStatus
            {
                Metadata = metadata,
                ScreenId = metadata != null ? metadata.screenId : string.Empty
            };
            if (metadata == null) return status;

            (status.UxmlPath, status.UssPath) = TargetPaths(metadata);
            status.DesignerUxml = UIToolkitCodeGenerator.GenerateUxml(metadata);
            status.DesignerUss = UIToolkitCodeGenerator.GenerateUss(metadata);
            status.FileUxml = File.Exists(status.UxmlPath) ? File.ReadAllText(status.UxmlPath) : null;
            status.FileUss = File.Exists(status.UssPath) ? File.ReadAllText(status.UssPath) : null;

            var entry = manifest != null ? manifest.Find(metadata.screenId) : null;
            status.UxmlState = SyncStateResolver.Resolve(status.FileUxml != null, Hash(status.DesignerUxml), Hash(status.FileUxml), entry?.uxmlHash);
            status.UssState = SyncStateResolver.Resolve(status.FileUss != null, Hash(status.DesignerUss), Hash(status.FileUss), entry?.ussHash);
            return status;
        }

        public static List<ScreenSyncStatus> EvaluateAll(DesignerPublishManifest manifest)
        {
            var result = new List<ScreenSyncStatus>();
            foreach (var guid in AssetDatabase.FindAssets("t:DesignerMetadataAsset"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var metadata = AssetDatabase.LoadAssetAtPath<DesignerMetadataAsset>(path);
                if (metadata != null) result.Add(Evaluate(metadata, manifest));
            }
            return result;
        }

        /// <summary>"Use Designer": writes the generated files and records them as the new baseline.
        /// Returns false (without writing) if a target file exists without the generated banner.</summary>
        public static bool WriteAndRecord(ScreenSyncStatus status, DesignerPublishManifest manifest)
        {
            if (!CanWrite(status.UxmlPath) || !CanWrite(status.UssPath)) return false;

            File.WriteAllText(status.UxmlPath, status.DesignerUxml);
            File.WriteAllText(status.UssPath, status.DesignerUss);
            AssetDatabase.ImportAsset(status.UxmlPath);
            AssetDatabase.ImportAsset(status.UssPath);
            Record(status, manifest, status.DesignerUxml, status.DesignerUss);
            return true;
        }

        /// <summary>"Use Backend": accepts the on-disk file as the new baseline without regenerating,
        /// clearing the conflict/backend-changed state until the Designer diverges again.</summary>
        public static void AdoptBackend(ScreenSyncStatus status, DesignerPublishManifest manifest)
            => Record(status, manifest, status.FileUxml ?? string.Empty, status.FileUss ?? string.Empty);

        /// <summary>Changed-only publish over every screen: writes only New/DesignerChanged screens,
        /// skips InSync, and reports BackendChanged/Conflict as needing resolution. Dry run reports
        /// without writing.</summary>
        public static List<PublishAction> PublishChanged(DesignerPublishManifest manifest, bool dryRun)
        {
            var actions = new List<PublishAction>();
            foreach (var status in EvaluateAll(manifest))
            {
                var combined = status.Combined;
                string result;
                switch (combined)
                {
                    case SyncState.InSync:
                        result = "In sync (skipped)";
                        break;
                    case SyncState.Conflict:
                        result = "Conflict - needs resolution";
                        break;
                    case SyncState.BackendChanged:
                        result = "Backend changed - needs resolution";
                        break;
                    default: // New or DesignerChanged
                        if (dryRun) result = "Would publish";
                        else result = WriteAndRecord(status, manifest) ? "Published" : "Blocked (file not generated)";
                        break;
                }
                actions.Add(new PublishAction { ScreenId = status.ScreenId, Result = result });
            }
            if (!dryRun) AssetDatabase.Refresh();
            return actions;
        }

        private static void Record(ScreenSyncStatus status, DesignerPublishManifest manifest, string uxml, string uss)
        {
            if (manifest == null || status.Metadata == null) return;
            Undo.RecordObject(manifest, "Publish");
            var entry = manifest.GetOrCreate(status.Metadata.screenId);
            entry.uxmlHash = Hash(uxml);
            entry.ussHash = Hash(uss);
            EditorUtility.SetDirty(manifest);
        }

        public static (string uxml, string uss) TargetPaths(DesignerMetadataAsset metadata)
        {
            var assetPath = AssetDatabase.GetAssetPath(metadata);
            var dir = string.IsNullOrEmpty(assetPath) ? "Assets" : Path.GetDirectoryName(assetPath);
            var baseName = string.IsNullOrEmpty(metadata.screenId) ? metadata.name : metadata.screenId;
            foreach (var c in Path.GetInvalidFileNameChars()) baseName = baseName.Replace(c, '_');
            return (
                (dir + "/" + baseName + ".g.uxml").Replace('\\', '/'),
                (dir + "/" + baseName + ".g.uss").Replace('\\', '/'));
        }

        private static bool CanWrite(string path)
        {
            if (!File.Exists(path)) return true;
            return File.ReadAllText(path).Contains(GeneratedMarker);
        }

        public static string Hash(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            using (var sha = SHA1.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(content));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
