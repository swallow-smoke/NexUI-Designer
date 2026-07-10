using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Commands;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public sealed class NexUIDesignerViewport : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly Label _label;
        private readonly Label _hint;
        private readonly Label _zoomBadge;
        private readonly ScrollView _previewFrame;
        private readonly VisualElement _previewCanvas;
        private readonly VisualElement _gridLayer;
        private readonly VisualElement _elementLayer;
        private readonly VisualElement _selectionRectOverlay;
        private readonly Label _emptyState;
        private readonly Dictionary<DesignerElementMetadata, VisualElement> _views = new Dictionary<DesignerElementMetadata, VisualElement>();

        // Single element drag/resize state (also used as the "grabbed" element during a group move).
        private DesignerElementMetadata _dragElement;
        private Vector2 _dragStart;
        private Rect _dragStartRect;
        private Rect _pendingDragRect;
        private bool _resizing;
        private Vector2 _lastDragDelta;
        private Dictionary<DesignerElementMetadata, Rect> _groupDragStartRects;

        // Drag-box (rectangle) selection state, in unscaled canvas coordinates.
        private Vector2? _boxSelectStart;
        private bool _boxSelectShift;
        private bool _boxSelectCtrl;

        private TextField _renameField;

        public NexUIDesignerViewport(NexUIDesignerContext context)
        {
            _context = context;
            name = "NexUIDesignerViewport";
            focusable = true;
            AddToClassList("nexui-viewport");
            style.flexGrow = 1;

            var header = new VisualElement();
            header.AddToClassList("nexui-viewport-header");
            Add(header);

            var titleBlock = new VisualElement();
            titleBlock.style.flexGrow = 1;
            header.Add(titleBlock);

            _label = new Label();
            _label.AddToClassList("nexui-viewport-title");
            titleBlock.Add(_label);

            _hint = new Label();
            _hint.AddToClassList("nexui-viewport-hint");
            titleBlock.Add(_hint);

            _zoomBadge = new Label();
            _zoomBadge.AddToClassList("nexui-zoom-badge");
            header.Add(_zoomBadge);

            _previewFrame = new ScrollView();
            _previewFrame.AddToClassList("nexui-preview-frame");
            _previewCanvas = new VisualElement();
            _previewCanvas.AddToClassList("nexui-preview-canvas");
            _previewCanvas.focusable = true;

            _gridLayer = new VisualElement();
            _gridLayer.AddToClassList("nexui-grid-layer");
            _elementLayer = new VisualElement();
            _elementLayer.AddToClassList("nexui-element-layer");
            _selectionRectOverlay = new VisualElement();
            _selectionRectOverlay.AddToClassList("nexui-selection-rect");
            _selectionRectOverlay.style.position = Position.Absolute;
            _selectionRectOverlay.style.display = DisplayStyle.None;
            _selectionRectOverlay.pickingMode = PickingMode.Ignore;
            _emptyState = new Label();
            _emptyState.AddToClassList("nexui-canvas-empty-state");

            _previewCanvas.Add(_gridLayer);
            _previewCanvas.Add(_elementLayer);
            _previewCanvas.Add(_selectionRectOverlay);
            _previewCanvas.Add(_emptyState);
            _previewFrame.Add(_previewCanvas);
            Add(_previewFrame);

            _previewFrame.RegisterCallback<WheelEvent>(OnWheel);
            _previewCanvas.RegisterCallback<PointerDownEvent>(OnCanvasPointerDown);
            _previewCanvas.RegisterCallback<PointerMoveEvent>(OnCanvasPointerMove);
            _previewCanvas.RegisterCallback<PointerUpEvent>(OnCanvasPointerUp);
            _previewCanvas.RegisterCallback<ContextClickEvent>(OnCanvasContextClick);
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            context.PreviewRebuilt += RefreshAll;
            context.MetadataChanged += _ => RefreshAll();
            context.MetadataSelectionChanged += _ => RefreshSelection();
            context.CanvasChanged += RefreshAll;
            context.ElementChanged += FlashElement;
            RefreshAll();
        }

        public NexUIDesignerContext Context => _context;

        /// <summary>
        /// C1: briefly highlights the element's viewport view so a property/style/theme change
        /// is visibly confirmed instead of relying on the user to notice a value changed in an
        /// inspector field. <see cref="MarkMetadataDirty"/> already rebuilt <see cref="_views"/>
        /// synchronously before this fires (via CanvasChanged), so the lookup below always sees
        /// the current view.
        /// </summary>
        private void FlashElement(DesignerElementMetadata element)
        {
            if (element == null || !_views.TryGetValue(element, out var view) || view == null) return;
            view.AddToClassList("nexui-element-flash");
            view.schedule.Execute(() => view.RemoveFromClassList("nexui-element-flash")).ExecuteLater(300);
        }

        private void RefreshAll()
        {
            RefreshHeaderAndCanvas();
            RebuildElements();
            RefreshSelection();
        }

        private void RefreshHeaderAndCanvas()
        {
            _label.text = _context.CurrentScreen == null
                ? DesignerLocalization.T("message.noScreenSelected")
                : _context.CurrentScreen.ScreenId;
            _hint.text = BuildHint();
            _zoomBadge.text = Mathf.RoundToInt(_context.Zoom * 100f) + "%";

            var width = Mathf.Max(320, _context.Resolution.x * _context.Zoom);
            var height = Mathf.Max(220, _context.Resolution.y * _context.Zoom);
            _previewCanvas.style.width = width;
            _previewCanvas.style.height = height;
            _gridLayer.style.opacity = _context.SnapEnabled ? 1f : 0.25f;
            BuildGrid(width, height);
        }

        private void RebuildElements()
        {
            _elementLayer.Clear();
            _views.Clear();

            if (_context.Metadata == null || _context.Metadata.elements.Count == 0)
            {
                _emptyState.text = _context.Metadata == null
                    ? "Select a Metadata asset to edit design elements."
                    : "Add elements from the component palette.";
                _emptyState.style.display = DisplayStyle.Flex;
                return;
            }

            _emptyState.style.display = DisplayStyle.None;
            foreach (var element in _context.Metadata.elements)
            {
                if (element == null || element.hiddenInDesigner) continue;
                var view = CreateElementView(element);
                _views[element] = view;
                _elementLayer.Add(view);
            }
        }

        private void BuildGrid(float width, float height)
        {
            _gridLayer.Clear();
            var spacing = Mathf.Max(4f, _context.GridSize * _context.Zoom);
            var verticalCount = Mathf.Min(220, Mathf.CeilToInt(width / spacing));
            var horizontalCount = Mathf.Min(160, Mathf.CeilToInt(height / spacing));

            for (int i = 0; i <= verticalCount; i++)
            {
                var line = new VisualElement();
                line.AddToClassList(i % 8 == 0 ? "nexui-grid-line-major" : "nexui-grid-line");
                line.style.left = i * spacing;
                line.style.top = 0;
                line.style.width = 1;
                line.style.height = height;
                _gridLayer.Add(line);
            }

            for (int i = 0; i <= horizontalCount; i++)
            {
                var line = new VisualElement();
                line.AddToClassList(i % 8 == 0 ? "nexui-grid-line-major" : "nexui-grid-line");
                line.style.left = 0;
                line.style.top = i * spacing;
                line.style.width = width;
                line.style.height = 1;
                _gridLayer.Add(line);
            }
        }

        private string BuildHint()
        {
            if (_context.CurrentScreen == null)
                return "Select a Screen and Metadata, then add elements from the palette.";
            return _context.Backend + " / " + _context.Resolution.x + "x" + _context.Resolution.y + " / " + _context.PreviewState + " / " + _context.InputMode;
        }

        private VisualElement CreateElementView(DesignerElementMetadata element)
        {
            var view = new VisualElement();
            view.AddToClassList("nexui-design-element");
            view.AddToClassList("type-" + element.elementType.ToLowerInvariant());
            view.AddToClassList("shape-" + element.shape.ToString().ToLowerInvariant());
            view.EnableInClassList("is-locked", element.locked);
            view.style.position = Position.Absolute;
            ApplyRect(view, element.rect);
            view.style.backgroundColor = new StyleColor(element.tint);
            view.style.borderTopColor = new StyleColor(Lighten(element.tint, 0.18f));
            view.style.borderRightColor = new StyleColor(Lighten(element.tint, 0.18f));
            view.style.borderBottomColor = new StyleColor(Darken(element.tint, 0.18f));
            view.style.borderLeftColor = new StyleColor(Lighten(element.tint, 0.18f));

            AddTypeSpecificPreview(view, element);

            var name = new Label(string.IsNullOrEmpty(element.displayName) ? element.elementId : element.displayName);
            name.AddToClassList("nexui-element-name");
            view.Add(name);

            if (!string.IsNullOrEmpty(element.text))
            {
                var text = new Label(element.text);
                text.AddToClassList("nexui-element-text");
                text.style.color = new StyleColor(element.textColor);
                text.style.fontSize = Mathf.Max(9, element.fontSize) * _context.Zoom;
                view.Add(text);
            }

            var meta = new Label(element.elementId);
            meta.AddToClassList("nexui-element-meta");
            view.Add(meta);

            var handle = new VisualElement();
            handle.AddToClassList("nexui-resize-handle");
            view.Add(handle);

            view.RegisterCallback<PointerDownEvent>(evt => BeginDrag(evt, element, view));
            view.RegisterCallback<PointerMoveEvent>(evt => ContinueDrag(evt, view));
            view.RegisterCallback<PointerUpEvent>(evt => EndDrag(evt, view));
            view.RegisterCallback<PointerCancelEvent>(evt => CancelDrag(evt, view));
            return view;
        }

        /// <summary>
        /// Builds type-specific internal preview structure so the canvas shows an actual
        /// filled progress bar / radial ring / list rows instead of the same bare tinted box
        /// for every component type. All fills use percentage sizing so they stay correct as
        /// the element is resized, without any per-drag recomputation.
        /// </summary>
        private void AddTypeSpecificPreview(VisualElement view, DesignerElementMetadata element)
        {
            switch (element.elementType)
            {
                case "ProgressBar":
                case "StatBar":
                    AddLinearFill(view, element);
                    break;
                case "RadialFill":
                    AddRadialFill(view, element, spin: false);
                    break;
                case "Spinner":
                    AddRadialFill(view, element, spin: true);
                    break;
                case "ChoiceList":
                    AddChoiceRows(view);
                    break;
                case "List":
                    AddListRows(view, grid: false);
                    break;
                case "Grid":
                    AddListRows(view, grid: true);
                    break;
                case "Skeleton":
                    AddSkeletonBars(view);
                    break;
                case "Image":
                    ApplyPreviewImage(view, element, fullBleed: true);
                    break;
                case "IconButton":
                    ApplyPreviewImage(view, element, fullBleed: false);
                    break;
            }
        }

        private void AddLinearFill(VisualElement view, DesignerElementMetadata element)
        {
            var direction = element.fill.direction;
            var horizontal = direction == DesignerFillDirection.LeftToRight || direction == DesignerFillDirection.RightToLeft;

            var track = new VisualElement();
            track.AddToClassList("nexui-preview-fill-track");
            track.EnableInClassList("is-vertical", !horizontal);
            view.Add(track);

            var fraction = Mathf.Clamp01(Mathf.InverseLerp(element.fill.minValue, element.fill.maxValue, element.previewValue));

            var fillBar = new VisualElement();
            fillBar.AddToClassList("nexui-preview-fill-bar");
            fillBar.style.position = Position.Absolute;
            fillBar.style.backgroundColor = new StyleColor(Lighten(element.tint, 0.4f));

            // Unity-Image-style fill: anchor two opposite sides at 0, set the free axis's
            // explicit percent size, and clear the *other* inset on that axis to Auto so the
            // bar grows/shrinks from the correct edge instead of stretching both ways.
            if (horizontal)
            {
                fillBar.style.top = 0;
                fillBar.style.bottom = 0;
                fillBar.style.width = new Length(fraction * 100f, LengthUnit.Percent);
                if (direction == DesignerFillDirection.RightToLeft)
                {
                    fillBar.style.right = 0;
                    fillBar.style.left = StyleKeyword.Auto;
                }
                else
                {
                    fillBar.style.left = 0;
                    fillBar.style.right = StyleKeyword.Auto;
                }
            }
            else
            {
                fillBar.style.left = 0;
                fillBar.style.right = 0;
                fillBar.style.height = new Length(fraction * 100f, LengthUnit.Percent);
                if (direction == DesignerFillDirection.TopToBottom)
                {
                    fillBar.style.top = 0;
                    fillBar.style.bottom = StyleKeyword.Auto;
                }
                else
                {
                    fillBar.style.bottom = 0;
                    fillBar.style.top = StyleKeyword.Auto;
                }
            }
            track.Add(fillBar);
        }

        private void AddRadialFill(VisualElement view, DesignerElementMetadata element, bool spin)
        {
            var ring = new RadialFillPreview
            {
                Value = element.previewValue,
                Spin = spin,
                Clockwise = element.fill.clockwise,
                FillColor = Lighten(element.tint, 0.4f)
            };
            ring.AddToClassList("nexui-preview-radial");
            view.Add(ring);
        }

        /// <summary>Real Texture2D preview for Image (full-bleed background) / IconButton (small centered icon) - no sprite/atlas needed, just a direct texture assignment.</summary>
        private static void ApplyPreviewImage(VisualElement view, DesignerElementMetadata element, bool fullBleed)
        {
            if (element.previewImage == null) return;

            if (fullBleed)
            {
                view.style.backgroundImage = new StyleBackground(element.previewImage);
                view.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(ScaleMode.ScaleToFit);
                return;
            }

            var icon = new VisualElement();
            icon.AddToClassList("nexui-preview-icon");
            icon.style.backgroundImage = new StyleBackground(element.previewImage);
            icon.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(ScaleMode.ScaleToFit);
            view.Add(icon);
        }

        private static void AddChoiceRows(VisualElement view)
        {
            var list = new VisualElement();
            list.AddToClassList("nexui-preview-choice-list");
            for (int i = 0; i < 3; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("nexui-preview-choice-row");
                var box = new VisualElement();
                box.AddToClassList("nexui-preview-choice-box");
                box.EnableInClassList("is-checked", i == 0);
                row.Add(box);
                list.Add(row);
            }
            view.Add(list);
        }

        private static void AddListRows(VisualElement view, bool grid)
        {
            var container = new VisualElement();
            container.AddToClassList(grid ? "nexui-preview-grid" : "nexui-preview-list");
            var count = grid ? 6 : 3;
            for (int i = 0; i < count; i++)
            {
                var cell = new VisualElement();
                cell.AddToClassList(grid ? "nexui-preview-grid-cell" : "nexui-preview-list-row");
                container.Add(cell);
            }
            view.Add(container);
        }

        private static void AddSkeletonBars(VisualElement view)
        {
            var container = new VisualElement();
            container.AddToClassList("nexui-preview-skeleton");
            for (int i = 0; i < 3; i++)
            {
                var bar = new VisualElement();
                bar.AddToClassList("nexui-preview-skeleton-bar");
                container.Add(bar);

                var dim = false;
                bar.schedule.Execute(() =>
                {
                    dim = !dim;
                    bar.style.opacity = dim ? 0.35f : 0.85f;
                }).Every(450);
            }
            view.Add(container);
        }

        private void ApplyRect(VisualElement view, Rect rect)
        {
            view.style.left = rect.x * _context.Zoom;
            view.style.top = rect.y * _context.Zoom;
            view.style.width = Mathf.Max(16, rect.width * _context.Zoom);
            view.style.height = Mathf.Max(16, rect.height * _context.Zoom);
        }

        // ---- Element drag/resize (also drives group move when the dragged element is part of a
        // multi-selection) ------------------------------------------------------------------

        private void BeginDrag(PointerDownEvent evt, DesignerElementMetadata element, VisualElement view)
        {
            if (evt.button != 0) return;
            Focus();

            if (evt.shiftKey)
                _context.AddToSelection(element);
            else if (evt.ctrlKey || evt.commandKey)
                _context.ToggleSelection(element);
            else if (!_context.IsSelected(element))
                _context.SelectMetadata(element);

            if (!_context.IsSelected(element)) return; // e.g. a ctrl-click just removed it
            if (element.locked) return;

            _dragElement = element;
            _dragStart = new Vector2(evt.position.x, evt.position.y);
            _dragStartRect = element.rect;
            _pendingDragRect = element.rect;
            _lastDragDelta = Vector2.zero;

            var local = view.WorldToLocal(evt.position);
            _resizing = local.x >= view.resolvedStyle.width - 16f && local.y >= view.resolvedStyle.height - 16f;

            _groupDragStartRects = null;
            if (!_resizing && _context.SelectedElements.Count > 1)
            {
                _groupDragStartRects = new Dictionary<DesignerElementMetadata, Rect>();
                foreach (var selected in _context.SelectedElements)
                    _groupDragStartRects[selected] = selected.rect;
            }

            view.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void ContinueDrag(PointerMoveEvent evt, VisualElement view)
        {
            if (_dragElement == null || !view.HasPointerCapture(evt.pointerId)) return;

            var current = new Vector2(evt.position.x, evt.position.y);
            var delta = (current - _dragStart) / Mathf.Max(0.01f, _context.Zoom);

            if (_resizing)
            {
                var rect = _dragStartRect;
                rect.width = Mathf.Max(24f, rect.width + delta.x);
                rect.height = Mathf.Max(24f, rect.height + delta.y);
                _pendingDragRect = _context.SnapRect(rect);
                ApplyRect(view, _pendingDragRect);
            }
            else
            {
                // Shift held while moving locks the drag to whichever axis has the larger delta.
                if (evt.shiftKey)
                {
                    if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y)) delta.y = 0f;
                    else delta.x = 0f;
                }
                _lastDragDelta = delta;

                if (_groupDragStartRects != null)
                {
                    foreach (var pair in _groupDragStartRects)
                    {
                        var r = pair.Value;
                        r.position += delta;
                        if (_views.TryGetValue(pair.Key, out var elementView))
                            ApplyRect(elementView, r);
                    }
                }
                else
                {
                    var rect = _dragStartRect;
                    rect.position += delta;
                    _pendingDragRect = _context.SnapRect(rect);
                    ApplyRect(view, _pendingDragRect);
                }
            }

            evt.StopPropagation();
        }

        private void EndDrag(PointerUpEvent evt, VisualElement view)
        {
            if (view.HasPointerCapture(evt.pointerId))
                view.ReleasePointer(evt.pointerId);
            CommitDrag();
            evt.StopPropagation();
        }

        private void CancelDrag(PointerCancelEvent evt, VisualElement view)
        {
            if (view.HasPointerCapture(evt.pointerId))
                view.ReleasePointer(evt.pointerId);
            _dragElement = null;
            _resizing = false;
            _groupDragStartRects = null;
            evt.StopPropagation();
        }

        private void CommitDrag()
        {
            if (_dragElement != null)
            {
                if (_groupDragStartRects != null)
                {
                    var rects = new Dictionary<DesignerElementMetadata, Rect>();
                    foreach (var pair in _groupDragStartRects)
                    {
                        var r = pair.Value;
                        r.position += _lastDragDelta;
                        rects[pair.Key] = r;
                    }
                    _context.SetElementsRects(rects, "Move NexUI Elements");
                }
                else
                {
                    _context.UpdateElementRect(_dragElement, _pendingDragRect);
                }
            }

            _dragElement = null;
            _resizing = false;
            _groupDragStartRects = null;
            _lastDragDelta = Vector2.zero;
        }

        // ---- Drag-box (rectangle) selection --------------------------------------------------

        private void OnCanvasPointerDown(PointerDownEvent evt)
        {
            Focus();
            if (evt.button != 0) return;
            if (evt.target != _previewCanvas && evt.target != _gridLayer && evt.target != _elementLayer) return;

            _boxSelectStart = _previewCanvas.WorldToLocal(evt.position);
            _boxSelectShift = evt.shiftKey;
            _boxSelectCtrl = evt.ctrlKey || evt.commandKey;
            _previewCanvas.CapturePointer(evt.pointerId);
            ShowSelectionRect(_boxSelectStart.Value, _boxSelectStart.Value);
            evt.StopPropagation();
        }

        private void OnCanvasPointerMove(PointerMoveEvent evt)
        {
            if (!_boxSelectStart.HasValue || !_previewCanvas.HasPointerCapture(evt.pointerId)) return;
            ShowSelectionRect(_boxSelectStart.Value, _previewCanvas.WorldToLocal(evt.position));
            evt.StopPropagation();
        }

        private void OnCanvasPointerUp(PointerUpEvent evt)
        {
            if (!_boxSelectStart.HasValue) return;
            if (_previewCanvas.HasPointerCapture(evt.pointerId))
                _previewCanvas.ReleasePointer(evt.pointerId);

            var start = _boxSelectStart.Value;
            var current = _previewCanvas.WorldToLocal(evt.position);
            _boxSelectStart = null;
            HideSelectionRect();

            if (Vector2.Distance(start, current) < 3f)
            {
                // A plain click (no drag) on empty canvas clears the selection unless a modifier is held.
                if (!_boxSelectShift && !_boxSelectCtrl)
                    _context.ClearSelection();
                evt.StopPropagation();
                return;
            }

            var selectionRect = Rect.MinMaxRect(
                Mathf.Min(start.x, current.x), Mathf.Min(start.y, current.y),
                Mathf.Max(start.x, current.x), Mathf.Max(start.y, current.y));
            var matches = HitTestRect(selectionRect);

            if (_boxSelectShift)
                foreach (var element in matches) _context.AddToSelection(element);
            else if (_boxSelectCtrl)
                foreach (var element in matches) _context.ToggleSelection(element);
            else
                _context.SelectMany(matches);

            evt.StopPropagation();
        }

        private List<DesignerElementMetadata> HitTestRect(Rect selectionRectScaled)
        {
            var result = new List<DesignerElementMetadata>();
            if (_context.Metadata == null) return result;
            foreach (var element in _context.Metadata.elements)
            {
                if (element == null || element.hiddenInDesigner) continue;
                var scaled = new Rect(
                    element.rect.x * _context.Zoom, element.rect.y * _context.Zoom,
                    element.rect.width * _context.Zoom, element.rect.height * _context.Zoom);
                if (scaled.Overlaps(selectionRectScaled))
                    result.Add(element);
            }
            return result;
        }

        private void ShowSelectionRect(Vector2 a, Vector2 b)
        {
            _selectionRectOverlay.style.display = DisplayStyle.Flex;
            _selectionRectOverlay.style.left = Mathf.Min(a.x, b.x);
            _selectionRectOverlay.style.top = Mathf.Min(a.y, b.y);
            _selectionRectOverlay.style.width = Mathf.Abs(a.x - b.x);
            _selectionRectOverlay.style.height = Mathf.Abs(a.y - b.y);
        }

        private void HideSelectionRect() => _selectionRectOverlay.style.display = DisplayStyle.None;

        // ---- Context menu + rename ------------------------------------------------------------

        private void OnCanvasContextClick(ContextClickEvent evt)
        {
            var local = _previewCanvas.WorldToLocal(evt.mousePosition);
            var canvasPoint = local / Mathf.Max(0.01f, _context.Zoom);
            NexUIDesignerContextMenu.Show(_context, canvasPoint, BeginRename);
            evt.StopPropagation();
        }

        private void BeginRename(DesignerElementMetadata element)
        {
            if (element == null || !_views.TryGetValue(element, out var view)) return;

            _renameField?.RemoveFromHierarchy();
            var field = new TextField { value = string.IsNullOrEmpty(element.displayName) ? element.elementId : element.displayName };
            field.AddToClassList("nexui-rename-field");
            field.style.position = Position.Absolute;
            field.style.left = view.style.left;
            field.style.top = view.style.top;
            field.style.width = view.style.width;

            void Commit()
            {
                var newName = field.value;
                field.RemoveFromHierarchy();
                if (_renameField == field) _renameField = null;
                if (!string.IsNullOrEmpty(newName) && newName != element.displayName)
                    _context.UpdateElement(element, m => m.displayName = newName, "Rename NexUI Element");
            }

            field.RegisterCallback<FocusOutEvent>(_ => Commit());
            field.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Escape)
                {
                    Commit();
                    e.StopPropagation();
                }
            });

            _renameField = field;
            _elementLayer.Add(field);
            field.Focus();
            field.SelectAll();
        }

        // ---- Selection styling / zoom / shortcuts ---------------------------------------------

        private void RefreshSelection()
        {
            foreach (var pair in _views)
                pair.Value.EnableInClassList("is-selected", _context.IsSelected(pair.Key));
        }

        private void OnWheel(WheelEvent evt)
        {
            if (!evt.ctrlKey && !evt.commandKey) return;
            _context.ZoomBy(evt.delta.y > 0 ? -0.08f : 0.08f);
            evt.StopPropagation();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (UIDesignerCommandDispatcher.TryDispatch(evt, _context))
            {
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.F && _context.SelectedMetadata != null)
            {
                var r = _context.SelectedMetadata.rect;
                _previewFrame.scrollOffset = new Vector2(
                    Mathf.Max(0f, r.center.x * _context.Zoom - _previewFrame.resolvedStyle.width * 0.5f),
                    Mathf.Max(0f, r.center.y * _context.Zoom - _previewFrame.resolvedStyle.height * 0.5f));
                evt.StopPropagation();
            }
        }

        private static Color Lighten(Color color, float amount)
            => Color.Lerp(color, Color.white, amount);

        private static Color Darken(Color color, float amount)
            => Color.Lerp(color, Color.black, amount);
    }
}
