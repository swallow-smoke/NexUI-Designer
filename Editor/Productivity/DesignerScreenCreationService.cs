using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Productivity
{
    public enum DesignerScreenTemplate { FullScreen, Popup, Modal, HUD, Overlay }

    [Serializable]
    public sealed class DesignerScreenCreationRequest
    {
        public string ScreenName = "New Screen";
        public string ScreenId = "NewScreen";
        public UIRenderBackend Backend = UIRenderBackend.UIToolkit;
        public DesignerScreenTemplate Template = DesignerScreenTemplate.FullScreen;
        public Vector2Int ReferenceResolution = new Vector2Int(1920, 1080);
        public string Folder = "Assets/UI/Screens";
        public bool CreateMetadata = true;
        public bool CreateRoot = true;
        public bool CreateTransition = true;
        public bool CreateSampleElements;
    }

    public sealed class DesignerScreenCreationResult
    {
        public UIScreenDefinition Screen;
        public DesignerMetadataAsset Metadata;
        public readonly List<string> CreatedPaths = new List<string>();
        public string Error;
        public bool Success => Screen != null && string.IsNullOrEmpty(Error);
    }

    /// <summary>Creates one connected Screen/Metadata/Backend bundle without overwriting existing assets.</summary>
    public static class DesignerScreenCreationService
    {
        private static readonly HashSet<string> CachedScreenIds = new HashSet<string>(StringComparer.Ordinal);
        private static double nextScreenIdRefresh;

        public static bool Validate(DesignerScreenCreationRequest request, out string error)
        {
            error = null;
            if (request == null) { error = "생성 설정이 없습니다."; return false; }
            if (!DesignerMetadataUtility.IsValidElementId(request.ScreenId)) { error = "Screen ID는 문자 또는 '_'로 시작하고 문자, 숫자, '_', '-'만 사용할 수 있습니다."; return false; }
            if (string.IsNullOrWhiteSpace(request.ScreenName)) { error = "Screen Name을 입력하세요."; return false; }
            request.Folder = NormalizeFolder(request.Folder);
            if (!request.Folder.StartsWith("Assets/", StringComparison.Ordinal) && request.Folder != "Assets") { error = "저장 폴더는 Assets 안이어야 합니다."; return false; }
            RefreshScreenIdCache();
            if (CachedScreenIds.Contains(request.ScreenId))
            { error = $"Screen ID '{request.ScreenId}'가 이미 존재합니다."; return false; }
            foreach (var path in PlannedPaths(request)) if (File.Exists(path)) { error = "기존 파일을 덮어쓸 수 없습니다: " + path; return false; }
            return true;
        }

        public static DesignerScreenCreationResult Create(DesignerScreenCreationRequest request)
        {
            var result = new DesignerScreenCreationResult();
            if (!Validate(request, out result.Error)) return result;
            Undo.IncrementCurrentGroup();
            var group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create NexUI Screen");
            try
            {
                EnsureFolders(request.Folder);
                var baseName = SanitizeFileName(request.ScreenId);
                var screenPath = $"{request.Folder}/{baseName}.asset";
                var metadataPath = $"{request.Folder}/{baseName}.Metadata.asset";
                var backendPath = $"{request.Folder}/{baseName}." + (request.Backend == UIRenderBackend.UGUI ? "prefab" : "uxml");

                UnityEngine.Object backendAsset = request.Backend == UIRenderBackend.UGUI
                    ? CreateUguiPrefab(backendPath, request)
                    : CreateUxml(backendPath, request);
                result.CreatedPaths.Add(backendPath);
                if (request.Backend == UIRenderBackend.UIToolkit)
                    result.CreatedPaths.Add(Path.ChangeExtension(backendPath, ".uss").Replace('\\', '/'));

                var screen = ScriptableObject.CreateInstance<UIScreenDefinition>();
                screen.name = request.ScreenName;
                screen.identity = new UIScreenIdentity { screenId = request.ScreenId, accessibilityLabel = request.ScreenName };
                screen.backendAsset = new UIScreenBackendAsset { backend = request.Backend, asset = backendAsset };
                ApplyTemplateDefaults(screen, request.Template);
                AssetDatabase.CreateAsset(screen, screenPath);
                Undo.RegisterCreatedObjectUndo(screen, "Create NexUI Screen Definition");
                result.Screen = screen;
                CachedScreenIds.Add(request.ScreenId);
                result.CreatedPaths.Add(screenPath);

                if (request.CreateMetadata)
                {
                    var metadata = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
                    metadata.screenId = request.ScreenId;
                    if (request.CreateRoot) AddTemplateElements(metadata, request);
                    AssetDatabase.CreateAsset(metadata, metadataPath);
                    Undo.RegisterCreatedObjectUndo(metadata, "Create NexUI Screen Metadata");
                    result.Metadata = metadata;
                    result.CreatedPaths.Add(metadataPath);
                }

                if (request.CreateTransition && result.Metadata != null)
                {
                    var motionFolder = request.Folder + "/Motions";
                    EnsureFolders(motionFolder);
                    var preset = request.Template == DesignerScreenTemplate.Popup || request.Template == DesignerScreenTemplate.Modal
                        ? DesignerTransitionPreset.Modal : DesignerTransitionPreset.Fade;
                    var pair = DesignerTransitionPresetService.CreateAssetPair(result.Metadata, RootId(request.Template), preset,
                        motionFolder + "/" + baseName, new DesignerTransitionSettings());
                    result.CreatedPaths.AddRange(pair.CreatedPaths);
                    screen.motion = new UIScreenMotionConfig { openMotion = pair.Open, closeMotion = pair.Close };
                    result.Metadata.screenMotion.entryClip = pair.Open;
                    result.Metadata.screenMotion.exitClip = pair.Close;
                    EditorUtility.SetDirty(screen);
                    EditorUtility.SetDirty(result.Metadata);
                }

                AssetDatabase.SaveAssetIfDirty(screen);
                if (result.Metadata != null) AssetDatabase.SaveAssetIfDirty(result.Metadata);
                Undo.CollapseUndoOperations(group);
                return result;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                CachedScreenIds.Remove(request.ScreenId);
                for (var i = result.CreatedPaths.Count - 1; i >= 0; i--) AssetDatabase.DeleteAsset(result.CreatedPaths[i]);
                Debug.LogException(ex);
                return result;
            }
        }

        private static IEnumerable<string> PlannedPaths(DesignerScreenCreationRequest r)
        {
            var n = SanitizeFileName(r.ScreenId);
            yield return $"{r.Folder}/{n}.asset";
            if (r.CreateMetadata) yield return $"{r.Folder}/{n}.Metadata.asset";
            yield return $"{r.Folder}/{n}." + (r.Backend == UIRenderBackend.UGUI ? "prefab" : "uxml");
            if (r.Backend == UIRenderBackend.UIToolkit) yield return $"{r.Folder}/{n}.uss";
        }

        private static void RefreshScreenIdCache()
        {
            if (EditorApplication.timeSinceStartup < nextScreenIdRefresh) return;
            CachedScreenIds.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t:UIScreenDefinition"))
            {
                var screen = AssetDatabase.LoadAssetAtPath<UIScreenDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (screen != null && !string.IsNullOrEmpty(screen.ScreenId)) CachedScreenIds.Add(screen.ScreenId);
            }
            nextScreenIdRefresh = EditorApplication.timeSinceStartup + 2d;
        }

        private static UnityEngine.Object CreateUguiPrefab(string path, DesignerScreenCreationRequest r)
        {
            var root = new GameObject(RootId(r.Template), typeof(RectTransform), typeof(CanvasGroup));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            if (prefab == null) throw new InvalidOperationException("uGUI Prefab 생성에 실패했습니다.");
            return prefab;
        }

        private static UnityEngine.Object CreateUxml(string path, DesignerScreenCreationRequest r)
        {
            var root = RootId(r.Template);
            var ussPath = Path.ChangeExtension(path, ".uss").Replace('\\', '/');
            var ussName = Path.GetFileName(ussPath);
            var content = "<!-- NEXUI:GENERATED -->\n<ui:UXML xmlns:ui=\"UnityEngine.UIElements\">\n  <Style src=\"" + ussName + "\" />\n  <ui:VisualElement name=\"" + root + "\" class=\"nexui-screen-root\" />\n</ui:UXML>\n";
            var uss = "/* NEXUI:GENERATED */\n.nexui-screen-root { flex-grow: 1; }\n";
            var write = new GeneratedAssetWriter().Write(new[] { new GeneratedAssetFile(path, content), new GeneratedAssetFile(ussPath, uss) });
            if (!write.Success) throw new IOException(string.Join("\n", write.Errors));
            return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path) ?? throw new InvalidOperationException("생성한 UXML을 불러오지 못했습니다.");
        }

        private static void AddTemplateElements(DesignerMetadataAsset m, DesignerScreenCreationRequest r)
        {
            var size = (Vector2)r.ReferenceResolution;
            var rootId = RootId(r.Template);
            var rootRect = r.Template == DesignerScreenTemplate.Popup ? new Rect((size.x - 640) / 2, (size.y - 420) / 2, 640, 420) : new Rect(Vector2.zero, size);
            m.elements.Add(new DesignerElementMetadata { elementId = rootId, displayName = r.ScreenName + " Root", elementType = r.Template == DesignerScreenTemplate.Modal ? "Modal" : "Panel", rect = rootRect, anchorPreset = r.Template == DesignerScreenTemplate.Popup || r.Template == DesignerScreenTemplate.Modal ? DesignerAnchorPreset.Center : DesignerAnchorPreset.Stretch });
            if (r.Template == DesignerScreenTemplate.Modal)
                m.elements.Insert(0, new DesignerElementMetadata { elementId = "dim", displayName = "Dim Background", elementType = "Panel", rect = new Rect(Vector2.zero, size), anchorPreset = DesignerAnchorPreset.Stretch, tint = new Color(0, 0, 0, .55f) });
            if (r.CreateSampleElements)
            {
                m.elements.Add(new DesignerElementMetadata { elementId = "title", parentId = rootId, displayName = "Title", elementType = "Label", text = r.ScreenName, rect = new Rect(40, 40, 360, 56), fontSize = 28 });
                m.elements.Add(new DesignerElementMetadata { elementId = "primaryButton", parentId = rootId, displayName = "Primary Button", elementType = "Button", text = "확인", rect = new Rect(40, 120, 220, 56) });
            }
        }

        private static void ApplyTemplateDefaults(UIScreenDefinition s, DesignerScreenTemplate t)
        {
            var layer = t == DesignerScreenTemplate.Modal ? UILayerType.Modal : t == DesignerScreenTemplate.HUD ? UILayerType.HUD : t == DesignerScreenTemplate.Overlay ? UILayerType.Overlay : UILayerType.Window;
            s.layer = new UIScreenLayerConfig { layerType = layer, openPolicy = t == DesignerScreenTemplate.Modal ? UIOpenPolicy.StackPush : UIOpenPolicy.Single };
            s.policy = new UIScreenPolicyConfig { blockInputBehind = t == DesignerScreenTemplate.Modal, closeOnBack = t != DesignerScreenTemplate.HUD, focusPolicy = t == DesignerScreenTemplate.Modal ? UIFocusPolicy.TrapFocus : UIFocusPolicy.None, lifetimePolicy = UILifetimePolicy.KeepAlive };
            s.focus = new UIScreenFocusConfig { trapFocus = t == DesignerScreenTemplate.Modal, restoreFocusOnClose = true };
        }

        private static string RootId(DesignerScreenTemplate t) => t == DesignerScreenTemplate.Modal ? "modalRoot" : t == DesignerScreenTemplate.Popup ? "popupRoot" : t == DesignerScreenTemplate.HUD ? "safeAreaRoot" : t == DesignerScreenTemplate.Overlay ? "overlayRoot" : "screenRoot";
        private static string NormalizeFolder(string p) => (p ?? "Assets/UI/Screens").Replace('\\', '/').TrimEnd('/');
        private static string SanitizeFileName(string value) { foreach (var c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_'); return value; }
        private static void EnsureFolders(string path) { var parts = path.Split('/'); var current = parts[0]; for (var i = 1; i < parts.Length; i++) { var next = current + "/" + parts[i]; if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]); current = next; } }
    }
}
