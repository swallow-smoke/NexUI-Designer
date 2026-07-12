using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Designer.Editor.Commands;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.MotionClipEditor;
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
        private readonly VisualElement _guideLayer;
        private readonly VisualElement _selectionRectOverlay;
        private readonly VisualElement _inlineMenuLayer;
        private readonly VisualElement _floatingToolbar;
        private readonly Label _emptyState;
        private readonly Label _distanceLabel;
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
            _guideLayer = new VisualElement();
            _guideLayer.AddToClassList("nexui-guide-layer");
            _guideLayer.pickingMode = PickingMode.Ignore;
            _selectionRectOverlay = new VisualElement();
            _selectionRectOverlay.AddToClassList("nexui-selection-rect");
            _selectionRectOverlay.style.position = Position.Absolute;
            _selectionRectOverlay.style.display = DisplayStyle.None;
            _selectionRectOverlay.pickingMode = PickingMode.Ignore;
            _inlineMenuLayer = new VisualElement();
            _inlineMenuLayer.AddToClassList("nexui-inline-menu-layer");
            _inlineMenuLayer.style.display = DisplayStyle.None;
            _floatingToolbar = new VisualElement();
            _floatingToolbar.AddToClassList("nexui-floating-toolbar");
            _floatingToolbar.style.display = DisplayStyle.None;
            _distanceLabel = new Label();
            _distanceLabel.AddToClassList("nexui-distance-label");
            _distanceLabel.style.display = DisplayStyle.None;
            _distanceLabel.pickingMode = PickingMode.Ignore;
            _emptyState = new Label();
            _emptyState.AddToClassList("nexui-canvas-empty-state");
            _emptyState.pickingMode = PickingMode.Ignore;

            _previewCanvas.Add(_gridLayer);
            _previewCanvas.Add(_elementLayer);
            _previewCanvas.Add(_guideLayer);
            _previewCanvas.Add(_selectionRectOverlay);
            _previewCanvas.Add(_floatingToolbar);
            _previewCanvas.Add(_distanceLabel);
            _previewCanvas.Add(_inlineMenuLayer);
            _previewCanvas.Add(_emptyState);
            _previewFrame.Add(_previewCanvas);
            Add(_previewFrame);

            _previewFrame.RegisterCallback<WheelEvent>(OnWheel);
            _previewCanvas.RegisterCallback<PointerDownEvent>(OnCanvasPointerDown);
            _previewCanvas.RegisterCallback<PointerMoveEvent>(OnCanvasPointerMove);
            _previewCanvas.RegisterCallback<PointerUpEvent>(OnCanvasPointerUp);
            _previewCanvas.RegisterCallback<ContextClickEvent>(OnCanvasContextClick);
            _previewCanvas.RegisterCallback<PointerDownEvent>(OnDismissInlineMenuPointerDown, TrickleDown.TrickleDown);
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            context.PreviewRebuilt += RefreshAll;
            context.MetadataChanged += _ => RefreshAll();
            context.MetadataSelectionChanged += _ => RefreshSelection();
            context.MultiSelectionChanged += _ => RefreshSelection();
            context.CanvasChanged += RefreshAll;
            context.ElementChanged += FlashElement;
            RefreshAll();
        }

        public NexUIDesignerContext Context => _context;

        public void FitToView()
        {
            if (_previewFrame.resolvedStyle.width <= 1f || _previewFrame.resolvedStyle.height <= 1f) return;
            var x = (_previewFrame.resolvedStyle.width - 48f) / Mathf.Max(1f, _context.Resolution.x);
            var y = (_previewFrame.resolvedStyle.height - 48f) / Mathf.Max(1f, _context.Resolution.y);
            _context.SetZoom(Mathf.Clamp(Mathf.Min(x, y), 0.15f, 2f));
        }

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
            HideSmartGuides();
        }

        private void RebuildElements()
        {
            _elementLayer.Clear();
            _views.Clear();
            HideInlineMenu();

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
            var hasPreviewImage = IsImagePreviewElement(element);
            view.EnableInClassList("has-preview-image", hasPreviewImage);
            view.EnableInClassList("is-locked", element.locked);
            view.style.position = Position.Absolute;
            ApplyRect(view, element.rect);
            if (hasPreviewImage)
            {
                var outline = ImageOutlineColor(element.tint);
                view.style.backgroundColor = new StyleColor(Color.clear);
                view.style.borderTopColor = new StyleColor(outline);
                view.style.borderRightColor = new StyleColor(outline);
                view.style.borderBottomColor = new StyleColor(outline);
                view.style.borderLeftColor = new StyleColor(outline);
            }
            else
            {
                view.style.backgroundColor = new StyleColor(element.tint);
                view.style.borderTopColor = new StyleColor(Lighten(element.tint, 0.18f));
                view.style.borderRightColor = new StyleColor(Lighten(element.tint, 0.18f));
                view.style.borderBottomColor = new StyleColor(Darken(element.tint, 0.18f));
                view.style.borderLeftColor = new StyleColor(Lighten(element.tint, 0.18f));
            }

            AddTypeSpecificPreview(view, element);

            if (!hasPreviewImage)
            {
                var name = new Label(string.IsNullOrEmpty(element.displayName) ? element.elementId : element.displayName);
                name.AddToClassList("nexui-element-name");
                view.Add(name);
            }

            if (!hasPreviewImage && !string.IsNullOrEmpty(element.text))
            {
                var text = new Label(element.text);
                text.AddToClassList("nexui-element-text");
                text.style.color = new StyleColor(element.textColor);
                text.style.fontSize = Mathf.Max(9, element.fontSize) * _context.Zoom;
                view.Add(text);
            }

            if (!hasPreviewImage)
            {
                var meta = new Label(element.elementId);
                meta.AddToClassList("nexui-element-meta");
                view.Add(meta);
            }

            var handle = new VisualElement();
            handle.AddToClassList("nexui-resize-handle");
            view.Add(handle);

            AddSelectionHandles(view);

            view.RegisterCallback<PointerDownEvent>(evt => BeginDrag(evt, element, view));
            view.RegisterCallback<PointerMoveEvent>(evt => ContinueDrag(evt, view));
            view.RegisterCallback<PointerUpEvent>(evt => EndDrag(evt, view));
            view.RegisterCallback<PointerCancelEvent>(evt => CancelDrag(evt, view));
            return view;
        }

        private static void AddSelectionHandles(VisualElement view)
        {
            foreach (var name in new[] { "nw", "n", "ne", "e", "se", "s", "sw", "w" })
            {
                var handle = new VisualElement();
                handle.AddToClassList("nexui-selection-handle");
                handle.AddToClassList("handle-" + name);
                handle.pickingMode = PickingMode.Ignore;
                view.Add(handle);
            }

            var rotate = new VisualElement();
            rotate.AddToClassList("nexui-rotation-handle");
            rotate.pickingMode = PickingMode.Ignore;
            view.Add(rotate);
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

        private static bool IsImagePreviewElement(DesignerElementMetadata element)
            => element.previewImage != null && (element.elementType == "Image" || element.elementType == "IconButton");

        private static Color ImageOutlineColor(Color tint)
        {
            if (tint.a > 0.05f)
                return new Color(tint.r, tint.g, tint.b, 0.8f);
            return new Color(0.35f, 0.66f, 1f, 0.75f);
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
            else if (_context.SelectedElements.Count > 1 && !evt.shiftKey && !evt.ctrlKey && !evt.commandKey)
                _context.SetKeyObject(element);

            if (!_context.IsSelected(element)) return; // e.g. a ctrl-click just removed it
            if (element.locked) return;

            if (evt.altKey)
            {
                var copies = _context.DuplicateSelectionAtDragStart();
                if (copies.Count > 0)
                {
                    element = copies[copies.Count - 1];
                    if (_views.TryGetValue(element, out var duplicateView))
                        view = duplicateView;
                }
            }

            _dragElement = element;
            _dragStart = new Vector2(evt.position.x, evt.position.y);
            _dragStartRect = element.rect;
            _pendingDragRect = element.rect;
            _lastDragDelta = Vector2.zero;

            var local = view.WorldToLocal(evt.position);
            _resizing = local.x >= view.resolvedStyle.width - 16f && local.y >= view.resolvedStyle.height - 16f;

            // For a move (not a resize), drag the whole subtree: the dragged element (or the full
            // multi-selection when it is part of one) plus every descendant, so children visually
            // follow their parent. Rects are absolute canvas space, so each node translates by the
            // same delta. Resizing only affects the single dragged element.
            _groupDragStartRects = null;
            if (!_resizing)
            {
                System.Collections.Generic.IEnumerable<DesignerElementMetadata> roots =
                    (_context.SelectedElements.Count > 1 && _context.IsSelected(element))
                        ? _context.SelectedElements
                        : new[] { element };
                var closure = _context.MoveClosure(roots);
                if (closure.Count > 1)
                {
                    _groupDragStartRects = new Dictionary<DesignerElementMetadata, Rect>();
                    foreach (var node in closure)
                        _groupDragStartRects[node] = node.rect;
                }
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
                    _pendingDragRect = SnapWithSmartGuides(rect, _dragElement);
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
            HideSmartGuides();
        }

        private Rect SnapWithSmartGuides(Rect rect, DesignerElementMetadata moving)
        {
            var snapped = _context.SnapRect(rect);
            if (_context.Metadata == null) return snapped;

            var guide = NexUISmartGuideUtility.Snap(snapped, _context.Metadata.elements, moving, Mathf.Max(4f, 8f / Mathf.Max(0.01f, _context.Zoom)));
            ShowSmartGuides(guide, snapped);
            return guide.Rect;
        }

        private void ShowSmartGuides(NexUISmartGuideResult guide, Rect moving)
        {
            _guideLayer.Clear();
            if (guide.VerticalGuide.HasValue)
            {
                var line = new VisualElement();
                line.AddToClassList("nexui-smart-guide-line");
                line.AddToClassList("is-vertical");
                line.pickingMode = PickingMode.Ignore;
                line.style.left = guide.VerticalGuide.Value * _context.Zoom;
                line.style.height = _previewCanvas.resolvedStyle.height;
                _guideLayer.Add(line);
            }

            if (guide.HorizontalGuide.HasValue)
            {
                var line = new VisualElement();
                line.AddToClassList("nexui-smart-guide-line");
                line.AddToClassList("is-horizontal");
                line.pickingMode = PickingMode.Ignore;
                line.style.top = guide.HorizontalGuide.Value * _context.Zoom;
                line.style.width = _previewCanvas.resolvedStyle.width;
                _guideLayer.Add(line);
            }

            if (string.IsNullOrEmpty(guide.DistanceLabel))
            {
                _distanceLabel.style.display = DisplayStyle.None;
                return;
            }

            _distanceLabel.text = guide.DistanceLabel;
            _distanceLabel.style.display = DisplayStyle.Flex;
            _distanceLabel.style.left = moving.center.x * _context.Zoom + 8f;
            _distanceLabel.style.top = moving.center.y * _context.Zoom + 8f;
        }

        private void HideSmartGuides()
        {
            _guideLayer?.Clear();
            if (_distanceLabel != null)
                _distanceLabel.style.display = DisplayStyle.None;
        }

        // ---- Drag-box (rectangle) selection --------------------------------------------------

        private void OnCanvasPointerDown(PointerDownEvent evt)
        {
            Focus();
            if (evt.button != 0) return;
            HideInlineMenu();
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
            ShowInlineMenu(local, canvasPoint);
            evt.StopPropagation();
        }

        private void OnDismissInlineMenuPointerDown(PointerDownEvent evt)
        {
            if (_inlineMenuLayer.resolvedStyle.display == DisplayStyle.None) return;
            if (IsInsideInlineMenu(evt.target as VisualElement)) return;
            if (evt.button == 1) return;
            HideInlineMenu();
        }

        private bool IsInsideInlineMenu(VisualElement target)
        {
            if (target == null || target == _inlineMenuLayer) return false;
            while (target != null)
            {
                if (target.parent == _inlineMenuLayer) return true;
                target = target.parent;
            }
            return false;
        }

        private void ShowInlineMenu(Vector2 localPoint, Vector2 canvasPoint)
        {
            _inlineMenuLayer.Clear();
            _inlineMenuLayer.style.display = DisplayStyle.Flex;
            _inlineMenuLayer.pickingMode = PickingMode.Position;

            var hits = HitTestPoint(canvasPoint);
            var panel = new VisualElement();
            panel.AddToClassList("nexui-context-popover");
            _inlineMenuLayer.Add(panel);

            if (hits.Count == 0)
                BuildCanvasInlineMenu(panel, canvasPoint);
            else
                BuildElementInlineMenu(panel, hits);

            panel.RegisterCallback<GeometryChangedEvent>(_ => ClampInlineMenu(panel, localPoint));
            ClampInlineMenu(panel, localPoint);
        }

        private void ClampInlineMenu(VisualElement panel, Vector2 localPoint)
        {
            var width = Mathf.Max(236f, panel.resolvedStyle.width);
            var height = Mathf.Max(120f, panel.resolvedStyle.height);
            var left = Mathf.Clamp(localPoint.x + 8f, 8f, Mathf.Max(8f, _previewCanvas.resolvedStyle.width - width - 8f));
            var top = Mathf.Clamp(localPoint.y + 8f, 8f, Mathf.Max(8f, _previewCanvas.resolvedStyle.height - height - 8f));
            panel.style.left = left;
            panel.style.top = top;
        }

        private void HideInlineMenu()
        {
            if (_inlineMenuLayer == null) return;
            _inlineMenuLayer.Clear();
            _inlineMenuLayer.style.display = DisplayStyle.None;
            _inlineMenuLayer.pickingMode = PickingMode.Ignore;
        }

        private List<DesignerElementMetadata> HitTestPoint(Vector2 point)
        {
            var result = new List<DesignerElementMetadata>();
            if (_context.Metadata == null) return result;
            for (int i = _context.Metadata.elements.Count - 1; i >= 0; i--)
            {
                var element = _context.Metadata.elements[i];
                if (element == null || element.hiddenInDesigner) continue;
                if (element.rect.Contains(point))
                    result.Add(element);
            }
            return result;
        }

        private void BuildCanvasInlineMenu(VisualElement panel, Vector2 canvasPoint)
        {
            AddMenuHeader(panel, "Canvas", "Create or paste at " + Mathf.RoundToInt(canvasPoint.x) + ", " + Mathf.RoundToInt(canvasPoint.y));
            var createGrid = AddMenuGrid(panel, "Create");
            AddMenuButton(createGrid, "Panel", () => CreateAt(DesignerElementType.Panel, canvasPoint), true);
            AddMenuButton(createGrid, "Button", () => CreateAt(DesignerElementType.Button, canvasPoint), true);
            AddMenuButton(createGrid, "Text", () => CreateAt(DesignerElementType.Label, canvasPoint), true);
            AddMenuButton(createGrid, "Image", () => CreateAt(DesignerElementType.Image, canvasPoint), true);

            var editGrid = AddMenuGrid(panel, "Edit");
            AddMenuButton(editGrid, "Paste", () => _context.PasteSelection(), _context.HasClipboard);
            AddMenuButton(editGrid, "Select All", _context.SelectAll, _context.Metadata != null && _context.Metadata.elements.Count > 0);
            AddMenuButton(editGrid, "Clear", _context.ClearSelection, _context.SelectedElements.Count > 0);
        }

        private void BuildElementInlineMenu(VisualElement panel, List<DesignerElementMetadata> hits)
        {
            if (hits.Count > 1)
            {
                AddMenuHeader(panel, "Pick Layer", hits.Count + " overlapping elements");
                var hitList = new VisualElement();
                hitList.AddToClassList("nexui-context-hit-list");
                panel.Add(hitList);
                foreach (var hit in hits)
                {
                    var captured = hit;
                    AddMenuButton(hitList, Label(captured), () => _context.SelectMetadata(captured), true);
                }
            }

            var primary = hits[0];
            if (!_context.IsSelected(primary))
                _context.SelectMetadata(primary);

            AddMenuHeader(panel, Label(primary), primary.elementType + " / " + Mathf.RoundToInt(primary.rect.width) + "x" + Mathf.RoundToInt(primary.rect.height));

            var quickGrid = AddMenuGrid(panel, "Quick");
            AddMenuButton(quickGrid, "Rename", () => BeginRenameAndHide(primary), true);
            AddMenuButton(quickGrid, "Copy", () => _context.CopySelection(), _context.SelectedElements.Count > 0);
            AddMenuButton(quickGrid, "Duplicate", () => _context.DuplicateSelection(), _context.SelectedElements.Count > 0);
            AddMenuButton(quickGrid, "Delete", () => _context.DeleteSelection(), _context.SelectedElements.Count > 0, "is-danger");

            var layerGrid = AddMenuGrid(panel, "Layer");
            AddMenuButton(layerGrid, "Forward", _context.BringSelectionForward, _context.SelectedElements.Count > 0);
            AddMenuButton(layerGrid, "Backward", _context.SendSelectionBackward, _context.SelectedElements.Count > 0);
            AddMenuButton(layerGrid, "To Front", _context.BringSelectionToFront, _context.SelectedElements.Count > 0);
            AddMenuButton(layerGrid, "To Back", _context.SendSelectionToBack, _context.SelectedElements.Count > 0);

            var alignGrid = AddMenuGrid(panel, "Align");
            AddMenuButton(alignGrid, "Left", () => _context.AlignSelection("left"), true);
            AddMenuButton(alignGrid, "Center X", () => _context.AlignSelection("centerX"), true);
            AddMenuButton(alignGrid, "Right", () => _context.AlignSelection("right"), true);
            AddMenuButton(alignGrid, "Top", () => _context.AlignSelection("top"), true);
            AddMenuButton(alignGrid, "Center Y", () => _context.AlignSelection("centerY"), true);
            AddMenuButton(alignGrid, "Bottom", () => _context.AlignSelection("bottom"), true);

            var arrangeGrid = AddMenuGrid(panel, "Group & Motion");
            AddMenuButton(arrangeGrid, "Group", () => _context.GroupSelection(), _context.SelectedElements.Count >= 2);
            AddMenuButton(arrangeGrid, "Ungroup", () => _context.UngroupSelection(), _context.GetChildren(primary).Count > 0);
            AddMenuButton(arrangeGrid, "Motion Clip", () => MotionClipEditorWindow.Open(_context.PreviewSurface, primary.elementId), true);
        }

        private static void AddMenuHeader(VisualElement panel, string title, string subtitle)
        {
            var header = new VisualElement();
            header.AddToClassList("nexui-context-header");
            header.Add(new Label(title) { name = "ContextTitle" });
            header.Add(new Label(subtitle) { name = "ContextSubtitle" });
            panel.Add(header);
        }

        private static VisualElement AddMenuGrid(VisualElement panel, string label)
        {
            var group = new VisualElement();
            group.AddToClassList("nexui-context-group");
            group.Add(new Label(label) { name = "ContextGroupLabel" });
            var grid = new VisualElement();
            grid.AddToClassList("nexui-context-grid");
            group.Add(grid);
            panel.Add(group);
            return grid;
        }

        private void AddMenuButton(VisualElement parent, string label, System.Action action, bool enabled, string extraClass = null)
        {
            var button = new Button(() =>
            {
                action?.Invoke();
                HideInlineMenu();
            })
            {
                text = label
            };
            button.SetEnabled(enabled);
            button.AddToClassList("nexui-context-button");
            if (!string.IsNullOrEmpty(extraClass))
                button.AddToClassList(extraClass);
            parent.Add(button);
        }

        private static string Label(DesignerElementMetadata element)
            => string.IsNullOrEmpty(element.displayName) ? element.elementId : element.displayName;

        private void CreateAt(DesignerElementType type, Vector2 canvasPoint)
        {
            var element = _context.CreateMetadataElement(type);
            if (element == null) return;
            var r = element.rect;
            r.position = canvasPoint;
            _context.UpdateSelectedRect(r);
        }

        private void BeginRenameAndHide(DesignerElementMetadata element)
        {
            HideInlineMenu();
            BeginRename(element);
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
            {
                pair.Value.EnableInClassList("is-selected", _context.IsSelected(pair.Key));
                pair.Value.EnableInClassList("is-key-object", pair.Key == _context.KeyObject);
            }
            RefreshFloatingToolbar();
        }

        private void RefreshFloatingToolbar()
        {
            _floatingToolbar.Clear();
            if (_context.SelectedElements.Count == 0)
            {
                _floatingToolbar.style.display = DisplayStyle.None;
                return;
            }

            _floatingToolbar.style.display = DisplayStyle.Flex;
            var bounds = UIAlignmentUtility.GetBounds(_context.SelectedElements);
            _floatingToolbar.style.left = Mathf.Max(8f, bounds.xMin * _context.Zoom);
            _floatingToolbar.style.top = Mathf.Max(8f, bounds.yMin * _context.Zoom - 38f);

            var label = new Label(_context.SelectedElements.Count == 1
                ? Label(_context.SelectedMetadata)
                : _context.SelectedElements.Count + " selected");
            label.AddToClassList("nexui-floating-label");
            _floatingToolbar.Add(label);

            if (_context.SelectedElements.Count > 1)
            {
                AddFloatingButton("Left", () => _context.AlignSelection("left"), "Align left.");
                AddFloatingButton("Center", () => _context.AlignSelection("centerX"), "Align center.");
                AddFloatingButton("Dist", _context.DistributeSelectionHorizontal, "Distribute horizontally.");
                AddFloatingButton("Group", () => _context.GroupSelection(), "Group selected elements.");
            }
            else
            {
                AddFloatingButton("Copy", () => _context.CopySelection(), "Copy selection.");
                AddFloatingButton("Dup", () => _context.DuplicateSelection(), "Duplicate selection.");
                AddFloatingButton("Lock", () => _context.UpdateSelectedElement(e => e.locked = !e.locked, "Toggle NexUI Element Lock"), "Toggle lock.");
                AddFloatingButton("Hide", () => _context.UpdateSelectedElement(e => e.hiddenInDesigner = !e.hiddenInDesigner, "Toggle NexUI Element Hidden"), "Toggle visibility.");
                AddFloatingButton("Motion", () => MotionClipEditorWindow.Open(_context.PreviewSurface, _context.SelectedMetadata.elementId), "Open Motion Clip Editor.");
            }
        }

        private void AddFloatingButton(string text, System.Action action, string tooltip)
        {
            var button = new Button(action) { text = text, tooltip = tooltip };
            button.AddToClassList("nexui-floating-button");
            _floatingToolbar.Add(button);
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
