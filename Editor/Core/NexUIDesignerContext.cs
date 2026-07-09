using System;
using System.Collections.Generic;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Designer.Editor.Serialization;
using emiteat.NexUI.Designer.Editor.Validation;
using emiteat.NexUI.Motion;
using emiteat.NexUI.State;
using emiteat.NexUI.Theme;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed class NexUIDesignerContext : IDisposable
    {
        private readonly List<string> _validationMessages = new List<string>();
        private readonly List<DesignerValidationIssue> _validationIssues = new List<DesignerValidationIssue>();
        private int _elementCounter = 1;

        public UIScreenDefinition CurrentScreen { get; private set; }
        public DesignerMetadataAsset Metadata { get; private set; }
        public IUISurface PreviewSurface { get; private set; }
        public IUIElementHandle SelectedElement { get; private set; }
        public DesignerElementMetadata SelectedMetadata { get; private set; }
        public UIRenderBackend Backend { get; private set; }
        public UIStateStore PreviewStateStore { get; private set; }
        public IUIMotionPlayer PreviewMotionPlayer { get; private set; }
        public ThemeRegistry PreviewThemeRegistry { get; private set; }
        public INexUIDesignerBackend CurrentBackend { get; private set; }
        public Vector2Int Resolution { get; private set; }
        public float Zoom { get; private set; }
        public bool SnapEnabled { get; private set; }
        public float GridSize { get; private set; }
        public string PreviewState { get; private set; }
        public string InputMode { get; private set; }
        public IReadOnlyList<string> ValidationMessages => _validationMessages;
        public IReadOnlyList<DesignerValidationIssue> ValidationIssues => _validationIssues;
        public DesignerSaveReport LastSaveReport { get; private set; }
        public int ErrorCount { get; private set; }
        public int WarningCount { get; private set; }

        public event Action<UIScreenDefinition> ScreenChanged;
        public event Action<DesignerSaveReport> SaveCompleted;
        public event Action<DesignerMetadataAsset> MetadataChanged;
        public event Action<IUIElementHandle> SelectionChanged;
        public event Action<DesignerElementMetadata> MetadataSelectionChanged;
        public event Action PreviewRebuilt;
        public event Action ValidationChanged;
        public event Action CanvasChanged;

        public NexUIDesignerContext()
        {
            PreviewStateStore = new UIStateStore();
            PreviewMotionPlayer = new BuiltInMotionPlayer();
            PreviewThemeRegistry = new ThemeRegistry();
            Resolution = new Vector2Int(1920, 1080);
            Zoom = 0.5f;
            SnapEnabled = true;
            GridSize = 8f;
            PreviewState = "Normal";
            InputMode = "Keyboard";
            DesignerBackendRegistry.RegisterDefaults();
        }

        public void Open(UIScreenDefinition definition)
        {
            CurrentScreen = definition;
            Backend = definition != null ? definition.backendAsset.backend : UIRenderBackend.UIToolkit;
            ScreenChanged?.Invoke(definition);
            RebuildPreview();
        }

        public void SetMetadata(DesignerMetadataAsset metadata)
        {
            Metadata = metadata;
            if (Metadata != null && CurrentScreen != null && string.IsNullOrEmpty(Metadata.screenId))
                Metadata.screenId = CurrentScreen.ScreenId;
            SelectedMetadata = null;
            MetadataChanged?.Invoke(metadata);
            MetadataSelectionChanged?.Invoke(null);
            CanvasChanged?.Invoke();
            Validate();
        }

        public DesignerMetadataAsset CreateMetadataAsset()
        {
            var asset = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            asset.screenId = CurrentScreen != null ? CurrentScreen.ScreenId : string.Empty;

            var folder = "Assets";
            if (CurrentScreen != null)
            {
                var screenPath = AssetDatabase.GetAssetPath(CurrentScreen);
                if (!string.IsNullOrEmpty(screenPath))
                    folder = System.IO.Path.GetDirectoryName(screenPath).Replace("\\", "/");
            }

            var baseName = !string.IsNullOrEmpty(asset.screenId) ? asset.screenId : "NexUIDesigner";
            var path = AssetDatabase.GenerateUniqueAssetPath(folder + "/" + baseName + ".Metadata.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            SetMetadata(asset);
            EditorGUIUtility.PingObject(asset);
            return asset;
        }

        public void Select(IUIElementHandle handle)
        {
            SelectedElement = handle;
            SelectionChanged?.Invoke(handle);
        }

        public void SelectMetadata(DesignerElementMetadata element)
        {
            SelectedMetadata = element;
            if (element != null && TryFindElement(element.elementId, out var handle))
                SelectedElement = handle;
            else
                SelectedElement = null;
            SelectionChanged?.Invoke(SelectedElement);
            MetadataSelectionChanged?.Invoke(element);
        }

        public void SelectMetadata(string elementId)
            => SelectMetadata(Metadata != null ? Metadata.Find(elementId) : null);

        public void ClearSelection()
        {
            SelectedElement = null;
            SelectedMetadata = null;
            SelectionChanged?.Invoke(null);
            MetadataSelectionChanged?.Invoke(null);
        }

        public void RebuildPreview()
        {
            if (PreviewSurface != null && CurrentBackend != null)
                CurrentBackend.DestroyPreviewSurface(PreviewSurface);

            PreviewSurface = null;
            CurrentBackend = null;

            if (CurrentScreen != null && DesignerBackendRegistry.TryGet(CurrentScreen.backendAsset.backend, out var backend))
            {
                CurrentBackend = backend;
                PreviewSurface = backend.CreatePreviewSurface(CurrentScreen);
            }

            ClearSelection();
            PreviewRebuilt?.Invoke();
            Validate();
        }

        /// <summary>
        /// Persists the screen through the backend-appropriate serializer. The returned
        /// report (also stored in <see cref="LastSaveReport"/> and logged) states exactly
        /// what was written to disk and what was preview-only/skipped.
        /// </summary>
        public DesignerSaveReport Save()
        {
            var report = new DesignerSaveReport();

            if (CurrentScreen == null)
            {
                report.Warn("No screen is open; nothing was saved.");
                LastSaveReport = report;
                SaveCompleted?.Invoke(report);
                return report;
            }

            var serializer = DesignerSerializerRegistry.Get(CurrentScreen.backendAsset.backend);
            report.Merge(serializer.Save(CurrentScreen, Metadata));

            if (report.HasErrors)
                Debug.LogError("[NexUI Designer] " + report.Details());
            else if (report.HasWarnings)
                Debug.LogWarning("[NexUI Designer] " + report.Details());
            else
                Debug.Log("[NexUI Designer] " + report.Details());

            LastSaveReport = report;
            SaveCompleted?.Invoke(report);
            Validate();
            return report;
        }

        public void Validate()
        {
            _validationIssues.Clear();
            _validationIssues.AddRange(DesignerValidationService.Validate(CurrentScreen, Metadata));

            _validationMessages.Clear();
            ErrorCount = 0;
            WarningCount = 0;
            foreach (var issue in _validationIssues)
            {
                _validationMessages.Add(issue.ToString());
                if (issue.Severity == DesignerValidationSeverity.Error) ErrorCount++;
                else if (issue.Severity == DesignerValidationSeverity.Warning) WarningCount++;
            }

            ValidationChanged?.Invoke();
        }

        public void SetResolution(Vector2Int resolution)
        {
            Resolution = resolution;
            CanvasChanged?.Invoke();
            PreviewRebuilt?.Invoke();
        }

        public void SetZoom(float zoom)
        {
            Zoom = Mathf.Clamp(zoom, 0.15f, 2.0f);
            CanvasChanged?.Invoke();
        }

        public void ZoomBy(float delta) => SetZoom(Zoom + delta);

        public void SetSnap(bool enabled)
        {
            SnapEnabled = enabled;
            CanvasChanged?.Invoke();
        }

        public void SetGridSize(float size)
        {
            GridSize = Mathf.Clamp(size, 1f, 64f);
            CanvasChanged?.Invoke();
        }

        public void SetPreviewState(string state)
        {
            PreviewState = state;
            CanvasChanged?.Invoke();
        }

        public void SetInputMode(string mode)
        {
            InputMode = mode;
            CanvasChanged?.Invoke();
        }

        public void SetTheme(string themeId)
        {
            if (!string.IsNullOrEmpty(themeId))
                NexUITheme.Use(themeId);
            PreviewRebuilt?.Invoke();
        }

        public bool TryFindElement(string elementId, out IUIElementHandle handle)
        {
            handle = PreviewSurface?.TryFind(elementId);
            return handle != null;
        }

        /// <summary>
        /// Adds metadata entries for named backend elements (UXML names / prefab GameObject
        /// names) that have no metadata yet. Returns the number of elements added.
        /// </summary>
        public int SyncMetadataFromBackend()
        {
            if (Metadata == null || CurrentScreen == null) return 0;
            var asset = CurrentScreen.backendAsset.asset;
            var added = 0;

            if (CurrentScreen.backendAsset.backend == UIRenderBackend.UIToolkit && asset is UnityEngine.UIElements.VisualTreeAsset vta)
            {
                added = UIToolkitAssetSerializer.SyncMetadataFromUxml(Metadata, vta);
            }
            else if (CurrentScreen.backendAsset.backend == UIRenderBackend.UGUI && asset is GameObject prefab)
            {
                Undo.RecordObject(Metadata, "Sync Metadata From Prefab");
                foreach (var t in prefab.GetComponentsInChildren<Transform>(true))
                {
                    if (t == prefab.transform) continue;
                    if (Metadata.Find(t.name) != null) continue;
                    Metadata.elements.Add(new DesignerElementMetadata { elementId = t.name, displayName = t.name, elementType = "Custom" });
                    added++;
                }
                if (added > 0) EditorUtility.SetDirty(Metadata);
            }

            if (added > 0)
            {
                MetadataChanged?.Invoke(Metadata);
                MetadataSelectionChanged?.Invoke(SelectedMetadata);
            }
            CanvasChanged?.Invoke();
            Validate();
            return added;
        }

        /// <summary>
        /// Applies Designer metadata to the live preview surface only (names, classes,
        /// position, size, visibility, binding). This is preview-only and is NOT written to
        /// disk until the user saves.
        /// </summary>
        public void ApplyMetadataToPreview()
        {
            if (Metadata == null || CurrentBackend == null || PreviewSurface == null) return;
            foreach (var element in Metadata.elements)
            {
                if (element == null || string.IsNullOrEmpty(element.elementId)) continue;
                if (!TryFindElement(element.elementId, out var handle))
                    handle = CurrentBackend.CreateElement(PreviewSurface, element.parentId,
                        new DesignerElementCreateInfo { elementId = element.elementId, displayName = element.displayName });
                if (handle == null) continue;

                CurrentBackend.SetPosition(handle, element.rect.position);
                CurrentBackend.SetSize(handle, element.rect.size);
                CurrentBackend.SetVisible(handle, !element.hiddenInDesigner);
                CurrentBackend.SetBinding(handle, element.binding);
                foreach (var cls in element.classes)
                    CurrentBackend.AddClass(handle, cls);
            }
            PreviewRebuilt?.Invoke();
        }

        public DesignerElementMetadata CreateMetadataElement(DesignerElementType type)
        {
            if (Metadata == null) return null;
            RecordMetadata("Create NexUI Element");
            var element = new DesignerElementMetadata
            {
                elementId = NextElementId(type),
                displayName = type.ToString(),
                elementType = type.ToString(),
                rect = DefaultRectFor(type),
                text = DefaultTextFor(type),
                tint = DefaultTintFor(type),
                textColor = Color.white,
                fontSize = type == DesignerElementType.Label ? 18 : 14
            };
            Metadata.elements.Add(element);
            MarkMetadataDirty();
            SelectMetadata(element);
            return element;
        }

        public void DeleteSelectedMetadata()
        {
            if (Metadata == null || SelectedMetadata == null) return;
            RecordMetadata("Delete NexUI Element");
            Metadata.elements.Remove(SelectedMetadata);
            SelectedMetadata = null;
            MarkMetadataDirty();
            ClearSelection();
        }

        public DesignerElementMetadata DuplicateSelectedMetadata()
        {
            if (Metadata == null || SelectedMetadata == null) return null;
            RecordMetadata("Duplicate NexUI Element");
            var src = SelectedMetadata;
            var copy = new DesignerElementMetadata
            {
                elementId = UniqueElementId(src.elementId + "Copy"),
                parentId = src.parentId,
                displayName = string.IsNullOrEmpty(src.displayName) ? src.elementId + " Copy" : src.displayName + " Copy",
                elementType = src.elementType,
                rect = new Rect(src.rect.x + GridSize * 2f, src.rect.y + GridSize * 2f, src.rect.width, src.rect.height),
                anchorPreset = src.anchorPreset,
                text = src.text,
                tint = src.tint,
                textColor = src.textColor,
                fontSize = src.fontSize,
                locked = src.locked,
                hiddenInDesigner = src.hiddenInDesigner
            };
            copy.classes.AddRange(src.classes);
            copy.binding.valueKey = src.binding.valueKey;
            copy.binding.commandKey = src.binding.commandKey;
            copy.binding.textKey = src.binding.textKey;
            copy.binding.visibilityKey = src.binding.visibilityKey;
            copy.binding.classKey = src.binding.classKey;
            copy.binding.interactableKey = src.binding.interactableKey;
            Metadata.elements.Add(copy);
            MarkMetadataDirty();
            SelectMetadata(copy);
            return copy;
        }

        public void UpdateSelectedRect(Rect rect)
        {
            if (SelectedMetadata == null) return;
            RecordMetadata("Edit NexUI Element Rect");
            SelectedMetadata.rect = SnapRect(rect);
            MarkMetadataDirty();
        }

        /// <summary>
        /// Persists an anchor preset on the selected element's metadata (undo-tracked) and,
        /// when a matching live element exists, applies it to the preview surface through the
        /// backend so the viewport reflects the choice immediately. The saved prefab picks up
        /// the same preset via <c>UGUIAssetSerializer.ApplyRect</c>.
        /// </summary>
        public void SetSelectedAnchor(DesignerAnchorPreset preset)
        {
            if (SelectedMetadata == null) return;
            RecordMetadata("Set NexUI Element Anchor");
            SelectedMetadata.anchorPreset = preset;
            MarkMetadataDirty();
            if (CurrentBackend != null && TryFindElement(SelectedMetadata.elementId, out var handle))
                CurrentBackend.SetAnchor(handle, preset);
        }

        public void MoveSelected(Vector2 delta)
        {
            if (SelectedMetadata == null || SelectedMetadata.locked) return;
            var r = SelectedMetadata.rect;
            r.position += delta;
            UpdateSelectedRect(r);
        }

        public void ResizeSelected(Vector2 delta)
        {
            if (SelectedMetadata == null || SelectedMetadata.locked) return;
            var r = SelectedMetadata.rect;
            r.width = Mathf.Max(24f, r.width + delta.x);
            r.height = Mathf.Max(24f, r.height + delta.y);
            UpdateSelectedRect(r);
        }

        public void AlignSelected(string mode)
        {
            if (SelectedMetadata == null) return;
            var r = SelectedMetadata.rect;
            switch (mode)
            {
                case "left": r.x = 0f; break;
                case "centerX": r.x = (Resolution.x - r.width) * 0.5f; break;
                case "right": r.x = Resolution.x - r.width; break;
                case "top": r.y = 0f; break;
                case "centerY": r.y = (Resolution.y - r.height) * 0.5f; break;
                case "bottom": r.y = Resolution.y - r.height; break;
                case "fill": r = new Rect(0f, 0f, Resolution.x, Resolution.y); break;
            }
            UpdateSelectedRect(r);
        }

        public void UpdateSelectedElement(Action<DesignerElementMetadata> change, string undoName)
        {
            if (SelectedMetadata == null || change == null) return;
            RecordMetadata(undoName);
            change(SelectedMetadata);
            MarkMetadataDirty();
        }

        /// <summary>
        /// Screen-level counterpart to <see cref="UpdateSelectedElement"/>. Mutates the open
        /// <see cref="UIScreenDefinition"/> (e.g. its <c>policy</c> struct) under undo, marks it
        /// dirty and re-validates. Mirrors the element-level record/dirty idiom used above.
        /// </summary>
        public void UpdateScreen(Action<UIScreenDefinition> change, string undoName)
        {
            if (CurrentScreen == null || change == null) return;
            Undo.RecordObject(CurrentScreen, undoName);
            change(CurrentScreen);
            EditorUtility.SetDirty(CurrentScreen);
            CanvasChanged?.Invoke();
            Validate();
        }

        public Rect SnapRect(Rect rect)
        {
            if (!SnapEnabled || GridSize <= 0f) return rect;
            rect.x = Mathf.Round(rect.x / GridSize) * GridSize;
            rect.y = Mathf.Round(rect.y / GridSize) * GridSize;
            rect.width = Mathf.Round(rect.width / GridSize) * GridSize;
            rect.height = Mathf.Round(rect.height / GridSize) * GridSize;
            return rect;
        }

        private void RecordMetadata(string name)
        {
            if (Metadata != null)
                Undo.RecordObject(Metadata, name);
        }

        private void MarkMetadataDirty()
        {
            if (Metadata != null)
                EditorUtility.SetDirty(Metadata);
            CanvasChanged?.Invoke();
            Validate();
        }

        private string NextElementId(DesignerElementType type)
        {
            while (true)
            {
                var id = char.ToLowerInvariant(type.ToString()[0]) + type.ToString().Substring(1) + _elementCounter++;
                if (Metadata == null || Metadata.Find(id) == null)
                    return id;
            }
        }

        private string UniqueElementId(string baseId)
        {
            var id = string.IsNullOrEmpty(baseId) ? "element" : baseId;
            var candidate = id;
            var index = 1;
            while (Metadata != null && Metadata.Find(candidate) != null)
                candidate = id + index++;
            return candidate;
        }

        private static Rect DefaultRectFor(DesignerElementType type)
        {
            switch (type)
            {
                case DesignerElementType.Button: return new Rect(96, 96, 220, 56);
                case DesignerElementType.Label: return new Rect(96, 96, 260, 44);
                case DesignerElementType.Image: return new Rect(96, 96, 160, 120);
                case DesignerElementType.Modal: return new Rect(420, 240, 640, 360);
                case DesignerElementType.List: return new Rect(96, 96, 360, 420);
                case DesignerElementType.Slot: return new Rect(96, 96, 88, 88);
                default: return new Rect(96, 96, 280, 120);
            }
        }

        private static string DefaultTextFor(DesignerElementType type)
        {
            switch (type)
            {
                case DesignerElementType.Button: return "Button";
                case DesignerElementType.Label: return "Label";
                case DesignerElementType.Toast: return "Toast message";
                default: return string.Empty;
            }
        }

        private static Color DefaultTintFor(DesignerElementType type)
        {
            switch (type)
            {
                case DesignerElementType.Button: return new Color(0.12f, 0.36f, 0.85f, 1f);
                case DesignerElementType.Label: return new Color(0.12f, 0.15f, 0.2f, 0.65f);
                case DesignerElementType.Modal: return new Color(0.08f, 0.1f, 0.14f, 0.96f);
                case DesignerElementType.Image: return new Color(0.19f, 0.25f, 0.34f, 1f);
                default: return new Color(0.13f, 0.18f, 0.26f, 1f);
            }
        }

        public void Dispose()
        {
            if (PreviewSurface != null && CurrentBackend != null)
                CurrentBackend.DestroyPreviewSurface(PreviewSurface);
        }
    }
}
