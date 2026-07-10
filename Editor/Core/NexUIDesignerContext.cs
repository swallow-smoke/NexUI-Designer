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
        private readonly List<DesignerElementMetadata> _selection = new List<DesignerElementMetadata>();
        private readonly List<DesignerElementMetadata> _clipboard = new List<DesignerElementMetadata>();
        private readonly List<string> _recentActions = new List<string>();
        private int _elementCounter = 1;
        private int _groupCounter = 1;
        private const int MaxRecentActions = 50;

        /// <summary>
        /// C2: read-only log of the last <see cref="MaxRecentActions"/> undo-recorded edit
        /// names, newest first - session-only (cleared on domain reload), not a jump-to-any-
        /// point history. Unity's public Undo API does not expose an enumerable/random-access
        /// undo stack, so this is visibility into what changed rather than a true steppable
        /// history browser.
        /// </summary>
        public IReadOnlyList<string> RecentActions => _recentActions;

        public event Action RecentActionsChanged;

        private void LogAction(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            _recentActions.Insert(0, name);
            if (_recentActions.Count > MaxRecentActions)
                _recentActions.RemoveAt(_recentActions.Count - 1);
            RecentActionsChanged?.Invoke();
        }

        public UIScreenDefinition CurrentScreen { get; private set; }
        public DesignerMetadataAsset Metadata { get; private set; }
        public IUISurface PreviewSurface { get; private set; }
        public IUIElementHandle SelectedElement { get; private set; }

        /// <summary>All currently selected elements. Empty when nothing is selected.</summary>
        public IReadOnlyList<DesignerElementMetadata> SelectedElements => _selection;

        /// <summary>
        /// The "primary" selected element - the last one added to the selection. Kept for every
        /// pre-existing single-select caller (inspectors, rect/anchor edits); resolves to the
        /// same element a single click would have selected before multi-select existed.
        /// </summary>
        public DesignerElementMetadata SelectedMetadata => _selection.Count > 0 ? _selection[_selection.Count - 1] : null;

        public bool HasClipboard => _clipboard.Count > 0;
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
        public event Action<IReadOnlyList<DesignerElementMetadata>> MultiSelectionChanged;
        public event Action PreviewRebuilt;
        public event Action ValidationChanged;
        public event Action CanvasChanged;

        /// <summary>
        /// C1 (visible style-apply feedback): raised whenever a single element's fields change
        /// through <see cref="UpdateElement"/>/<see cref="UpdateSelectedElement"/> (style, theme,
        /// binding, accessibility, policy, motion - every per-element Inspector routes through
        /// here). The Viewport uses this to briefly flash the affected element so "did that
        /// apply?" is never in doubt.
        /// </summary>
        public event Action<DesignerElementMetadata> ElementChanged;

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
            ClearSelection();
            MetadataChanged?.Invoke(metadata);
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

        /// <summary>Replaces the whole selection with a single element (or clears it if null).</summary>
        public void SelectMetadata(DesignerElementMetadata element)
        {
            _selection.Clear();
            if (element != null)
                _selection.Add(element);
            RaiseSelectionChanged();
        }

        public void SelectMetadata(string elementId)
            => SelectMetadata(Metadata != null ? Metadata.Find(elementId) : null);

        /// <summary>Alias of <see cref="SelectMetadata(DesignerElementMetadata)"/> matching the requested selection-service shape.</summary>
        public void Select(DesignerElementMetadata element) => SelectMetadata(element);

        public void AddToSelection(DesignerElementMetadata element)
        {
            if (element == null || _selection.Contains(element)) return;
            _selection.Add(element);
            RaiseSelectionChanged();
        }

        public void RemoveFromSelection(DesignerElementMetadata element)
        {
            if (element == null || !_selection.Remove(element)) return;
            RaiseSelectionChanged();
        }

        public void ToggleSelection(DesignerElementMetadata element)
        {
            if (element == null) return;
            if (!_selection.Remove(element))
                _selection.Add(element);
            RaiseSelectionChanged();
        }

        public void SelectMany(IEnumerable<DesignerElementMetadata> elements)
        {
            _selection.Clear();
            if (elements != null)
                foreach (var e in elements)
                    if (e != null && !_selection.Contains(e))
                        _selection.Add(e);
            RaiseSelectionChanged();
        }

        public void SelectAll()
        {
            if (Metadata == null) return;
            SelectMany(Metadata.elements);
        }

        public bool IsSelected(DesignerElementMetadata element) => element != null && _selection.Contains(element);

        public List<DesignerElementMetadata> GetChildren(DesignerElementMetadata element)
        {
            var result = new List<DesignerElementMetadata>();
            if (Metadata == null || element == null) return result;
            foreach (var e in Metadata.elements)
                if (e != null && e.parentId == element.elementId)
                    result.Add(e);
            return result;
        }

        public void SelectChildren(DesignerElementMetadata element)
        {
            var children = GetChildren(element);
            if (children.Count > 0)
                SelectMany(children);
        }

        public void SelectParent(DesignerElementMetadata element)
        {
            if (Metadata == null || element == null || string.IsNullOrEmpty(element.parentId)) return;
            var parent = Metadata.Find(element.parentId);
            if (parent != null)
                SelectMetadata(parent);
        }

        public void ClearSelection()
        {
            if (_selection.Count == 0 && SelectedElement == null) return;
            _selection.Clear();
            RaiseSelectionChanged();
        }

        private void RaiseSelectionChanged()
        {
            var primary = SelectedMetadata;
            SelectedElement = primary != null && TryFindElement(primary.elementId, out var handle) ? handle : null;
            SelectionChanged?.Invoke(SelectedElement);
            MetadataSelectionChanged?.Invoke(primary);
            MultiSelectionChanged?.Invoke(_selection);
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

            // B8: keep the git-friendly companion JSON in sync with every save so it's always
            // the reviewable diff for a PR, never stale relative to the .asset.
            if (Metadata != null)
            {
                var jsonPath = DesignerMetadataJsonSerializer.Export(Metadata);
                if (!string.IsNullOrEmpty(jsonPath))
                    report.MarkChanged($"Companion JSON: {jsonPath}");
            }

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
        /// B8: overwrites <see cref="Metadata"/> with the contents of its companion JSON file
        /// (Undo-tracked) - use after resolving a Git merge conflict in the JSON to push the
        /// merged result back into the <c>.asset</c>. Returns false if there's no JSON file yet
        /// (nothing has been saved through this screen) or it failed to parse.
        /// </summary>
        public bool SyncMetadataFromJson()
        {
            if (Metadata == null) return false;
            var applied = DesignerMetadataJsonSerializer.Import(Metadata);
            if (applied)
            {
                MetadataChanged?.Invoke(Metadata);
                MetadataSelectionChanged?.Invoke(SelectedMetadata);
                CanvasChanged?.Invoke();
                Validate();
            }
            return applied;
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
                shape = DefaultShapeFor(type),
                textColor = Color.white,
                fontSize = type == DesignerElementType.Label ? 18 : 14
            };
            Metadata.elements.Add(element);
            MarkMetadataDirty();
            SelectMetadata(element);
            return element;
        }

        /// <summary>Deletes every currently selected element (multi-select aware).</summary>
        public void DeleteSelectedMetadata()
        {
            if (Metadata == null || _selection.Count == 0) return;
            RecordMetadata("Delete NexUI Element");
            foreach (var element in _selection)
                Metadata.elements.Remove(element);
            _selection.Clear();
            MarkMetadataDirty();
            RaiseSelectionChanged();
        }

        public void DeleteSelection() => DeleteSelectedMetadata();

        private static DesignerElementMetadata CloneElement(DesignerElementMetadata src, string elementId, Vector2 offset)
        {
            var copy = new DesignerElementMetadata
            {
                elementId = elementId,
                parentId = src.parentId,
                displayName = string.IsNullOrEmpty(src.displayName) ? src.elementId + " Copy" : src.displayName + " Copy",
                elementType = src.elementType,
                rect = new Rect(src.rect.x + offset.x, src.rect.y + offset.y, src.rect.width, src.rect.height),
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
            return copy;
        }

        public DesignerElementMetadata DuplicateSelectedMetadata()
        {
            var copies = DuplicateSelection();
            return copies.Count > 0 ? copies[copies.Count - 1] : null;
        }

        /// <summary>Duplicates every selected element (offset by two grid cells) and selects the copies.</summary>
        public List<DesignerElementMetadata> DuplicateSelection()
        {
            var copies = new List<DesignerElementMetadata>();
            if (Metadata == null || _selection.Count == 0) return copies;
            RecordMetadata("Duplicate NexUI Element");
            var offset = new Vector2(GridSize * 2f, GridSize * 2f);
            foreach (var src in _selection)
            {
                var copy = CloneElement(src, UniqueElementId(src.elementId + "Copy"), offset);
                Metadata.elements.Add(copy);
                copies.Add(copy);
            }
            MarkMetadataDirty();
            SelectMany(copies);
            return copies;
        }

        /// <summary>Copies the current selection into an in-memory clipboard (survives across Paste calls).</summary>
        public void CopySelection()
        {
            _clipboard.Clear();
            foreach (var e in _selection)
                _clipboard.Add(e);
        }

        /// <summary>Pastes the clipboard as new elements offset from their originals, and selects the pasted copies.</summary>
        public List<DesignerElementMetadata> PasteSelection()
        {
            var copies = new List<DesignerElementMetadata>();
            if (Metadata == null || _clipboard.Count == 0) return copies;
            RecordMetadata("Paste NexUI Element");
            var offset = new Vector2(GridSize * 2f, GridSize * 2f);
            foreach (var src in _clipboard)
            {
                var copy = CloneElement(src, UniqueElementId(src.elementId + "Copy"), offset);
                Metadata.elements.Add(copy);
                copies.Add(copy);
            }
            MarkMetadataDirty();
            SelectMany(copies);
            return copies;
        }

        public void UpdateSelectedRect(Rect rect) => UpdateElementRect(SelectedMetadata, rect);

        /// <summary>
        /// Rect update targeted at a specific element rather than the "primary" selection - used
        /// by the viewport's resize/move-drag commit, where the dragged element isn't always the
        /// primary selection (e.g. dragging a non-primary member of an existing multi-selection).
        /// </summary>
        public void UpdateElementRect(DesignerElementMetadata element, Rect rect)
        {
            if (element == null || element.locked) return;
            RecordMetadata("Edit NexUI Element Rect");
            element.rect = SnapRect(rect);
            MarkMetadataDirty();
            ElementChanged?.Invoke(element);
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

        /// <summary>Moves every selected (and unlocked) element by the same delta as a single undo step.</summary>
        public void MoveSelection(Vector2 delta)
        {
            if (_selection.Count == 0) return;
            var rects = new Dictionary<DesignerElementMetadata, Rect>();
            foreach (var element in _selection)
            {
                if (element.locked) continue;
                var r = element.rect;
                r.position += delta;
                rects[element] = r;
            }
            SetElementsRects(rects, "Move NexUI Elements");
        }

        /// <summary>
        /// Applies a batch of rect changes (from group move, align, or distribute) as a single
        /// undo step. All elements live on the same <see cref="Metadata"/> asset, so one
        /// <c>Undo.RecordObject</c> call before the loop is enough to collapse the whole batch.
        /// </summary>
        public void SetElementsRects(IReadOnlyDictionary<DesignerElementMetadata, Rect> rects, string undoName)
        {
            if (Metadata == null || rects == null || rects.Count == 0) return;
            RecordMetadata(undoName);
            foreach (var pair in rects)
            {
                if (pair.Key == null || pair.Key.locked) continue;
                pair.Key.rect = SnapRect(pair.Value);
            }
            MarkMetadataDirty();
        }

        public void AlignSelection(string mode)
        {
            if (_selection.Count == 0) return;
            if (_selection.Count == 1)
            {
                AlignSelected(mode);
                return;
            }

            var bounds = UIAlignmentUtility.GetBounds(_selection);
            Dictionary<DesignerElementMetadata, Rect> rects = mode switch
            {
                "left" => UIAlignmentUtility.AlignLeft(_selection, bounds),
                "centerX" => UIAlignmentUtility.AlignCenterX(_selection, bounds),
                "right" => UIAlignmentUtility.AlignRight(_selection, bounds),
                "top" => UIAlignmentUtility.AlignTop(_selection, bounds),
                "centerY" => UIAlignmentUtility.AlignCenterY(_selection, bounds),
                "bottom" => UIAlignmentUtility.AlignBottom(_selection, bounds),
                _ => null
            };
            if (rects != null)
                SetElementsRects(rects, "Align NexUI Elements");
        }

        public void DistributeSelectionHorizontal()
            => SetElementsRects(UIAlignmentUtility.DistributeHorizontal(_selection), "Distribute NexUI Elements Horizontally");

        public void DistributeSelectionVertical()
            => SetElementsRects(UIAlignmentUtility.DistributeVertical(_selection), "Distribute NexUI Elements Vertically");

        public void BringSelectionForward()
        {
            if (_selection.Count == 0) return;
            RecordMetadata("Bring Forward");
            UIElementLayerOrder.BringForward(Metadata, _selection);
            MarkMetadataDirty();
        }

        public void SendSelectionBackward()
        {
            if (_selection.Count == 0) return;
            RecordMetadata("Send Backward");
            UIElementLayerOrder.SendBackward(Metadata, _selection);
            MarkMetadataDirty();
        }

        public void BringSelectionToFront()
        {
            if (_selection.Count == 0) return;
            RecordMetadata("Bring To Front");
            UIElementLayerOrder.BringToFront(Metadata, _selection);
            MarkMetadataDirty();
        }

        public void SendSelectionToBack()
        {
            if (_selection.Count == 0) return;
            RecordMetadata("Send To Back");
            UIElementLayerOrder.SendToBack(Metadata, _selection);
            MarkMetadataDirty();
        }

        /// <summary>
        /// Wraps the current selection in a new Panel element sized to their bounding box and
        /// reassigns their <c>parentId</c> to it. Rects stay in the same absolute canvas space
        /// the viewport already renders in (element rects are not parent-relative), so no
        /// coordinate conversion is needed - grouping only changes the saved hierarchy.
        /// </summary>
        public DesignerElementMetadata GroupSelection()
        {
            if (Metadata == null || _selection.Count < 2) return null;
            RecordMetadata("Group NexUI Elements");

            var bounds = UIAlignmentUtility.GetBounds(_selection);
            var group = new DesignerElementMetadata
            {
                elementId = UniqueElementId("group" + _groupCounter++),
                displayName = "Group",
                elementType = "Panel",
                rect = bounds,
                tint = new Color(0f, 0f, 0f, 0f)
            };

            var members = new List<DesignerElementMetadata>(_selection);
            var insertIndex = int.MaxValue;
            foreach (var member in members)
                insertIndex = Mathf.Min(insertIndex, Metadata.elements.IndexOf(member));
            if (insertIndex == int.MaxValue || insertIndex > Metadata.elements.Count)
                insertIndex = Metadata.elements.Count;

            Metadata.elements.Insert(insertIndex, group);
            foreach (var member in members)
                member.parentId = group.elementId;

            MarkMetadataDirty();
            SelectMetadata(group);
            return group;
        }

        /// <summary>Removes the group's parentId from its direct children and deletes the group wrapper.</summary>
        public void UngroupSelection()
        {
            if (Metadata == null || SelectedMetadata == null) return;
            var group = SelectedMetadata;
            var children = Metadata.elements.FindAll(e => e != null && e.parentId == group.elementId);
            if (children.Count == 0) return;

            RecordMetadata("Ungroup NexUI Elements");
            foreach (var child in children)
                child.parentId = group.parentId;
            Metadata.elements.Remove(group);
            MarkMetadataDirty();
            SelectMany(children);
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
            => UpdateElement(SelectedMetadata, change, undoName);

        /// <summary>Element-targeted counterpart to <see cref="UpdateSelectedElement"/> (see <see cref="UpdateElementRect"/>).</summary>
        public void UpdateElement(DesignerElementMetadata element, Action<DesignerElementMetadata> change, string undoName)
        {
            if (element == null || change == null) return;
            RecordMetadata(undoName);
            change(element);
            MarkMetadataDirty();
            ElementChanged?.Invoke(element);
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
            LogAction(undoName);
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
            LogAction(name);
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

        /// <summary>Gives naturally round/pill-shaped component types a matching canvas preview shape by default, instead of every new element starting as the same rounded box.</summary>
        private static DesignerElementShape DefaultShapeFor(DesignerElementType type)
        {
            switch (type)
            {
                case DesignerElementType.Spinner:
                case DesignerElementType.RadialFill:
                    return DesignerElementShape.Circle;
                case DesignerElementType.Toast:
                case DesignerElementType.Tooltip:
                case DesignerElementType.IconButton:
                    return DesignerElementShape.Pill;
                default:
                    return DesignerElementShape.Rounded;
            }
        }

        public void Dispose()
        {
            if (PreviewSurface != null && CurrentBackend != null)
                CurrentBackend.DestroyPreviewSurface(PreviewSurface);
        }
    }
}
