using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    public static class NexUIDesignerMenu
    {
        [MenuItem("Tools/NexUI/Designer")]
        public static void OpenDesigner() => NexUIDesigner.Open();

        [MenuItem("Tools/NexUI/Designer/Open Selected Screen")]
        public static void OpenSelectedScreen()
        {
            var definition = Selection.activeObject as UIScreenDefinition;
            if (definition != null) NexUIDesigner.Open(definition);
            else NexUIDesigner.Open();
        }

        [MenuItem("Tools/NexUI/Designer/Rebuild Preview")]
        public static void RebuildPreview() => NexUIDesigner.RebuildPreview();

        [MenuItem("Tools/NexUI/Designer/Validate Current Screen")]
        public static void ValidateCurrent() => NexUIDesigner.ValidateCurrent();

        [MenuItem("Tools/NexUI/Designer/Save Current Screen")]
        public static void SaveCurrent() => NexUIDesigner.SaveCurrent();

        [MenuItem("Tools/NexUI/Designer/Backend/Sync Metadata From Backend")]
        public static void SyncMetadataFromBackend()
        {
            var context = NexUIDesigner.Open().Context;
            var added = context.SyncMetadataFromBackend();
            Debug.Log($"[NexUI Designer] Synced metadata from backend: {added} element(s) added.");
        }

        [MenuItem("Tools/NexUI/Designer/Backend/Apply Metadata To Preview")]
        public static void ApplyMetadataToPreview()
        {
            NexUIDesigner.Open().Context.ApplyMetadataToPreview();
        }

        [MenuItem("Tools/NexUI/Designer/Backend/Open Backend Asset In UI Builder")]
        public static void OpenBackendAsset()
        {
            var asset = NexUIDesigner.Open().Context.CurrentScreen?.backendAsset.asset;
            if (asset == null) { Debug.LogWarning("[NexUI Designer] No backend asset assigned."); return; }
            AssetDatabase.OpenAsset(asset);
        }

        [MenuItem("Tools/NexUI/Designer/Backend/Ping Backend Asset")]
        public static void PingBackendAsset()
        {
            var asset = NexUIDesigner.Open().Context.CurrentScreen?.backendAsset.asset;
            if (asset == null) { Debug.LogWarning("[NexUI Designer] No backend asset assigned."); return; }
            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
        }

        [MenuItem("Tools/NexUI/Designer/Language/Korean")]
        public static void Korean() => DesignerLocalization.SetLanguage(DesignerLanguage.Korean);

        [MenuItem("Tools/NexUI/Designer/Language/English")]
        public static void English() => DesignerLocalization.SetLanguage(DesignerLanguage.English);
    }
}
