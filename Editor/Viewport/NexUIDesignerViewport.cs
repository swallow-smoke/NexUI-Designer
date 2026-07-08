using System.Collections.Generic;
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
        private readonly Label _emptyState;
        private readonly Dictionary<DesignerElementMetadata, VisualElement> _views = new Dictionary<DesignerElementMetadata, VisualElement>();
        private DesignerElementMetadata _dragElement;
        private Vector2 _dragStart;
        private Rect _dragStartRect;
        private Rect _pendingDragRect;
        private bool _resizing;

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
            _emptyState = new Label();
            _emptyState.AddToClassList("nexui-canvas-empty-state");

            _previewCanvas.Add(_gridLayer);
            _previewCanvas.Add(_elementLayer);
            _previewCanvas.Add(_emptyState);
            _previewFrame.Add(_previewCanvas);
            Add(_previewFrame);

            _previewFrame.RegisterCallback<WheelEvent>(OnWheel);
            _previewCanvas.RegisterCallback<PointerDownEvent>(OnCanvasPointerDown);
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            context.PreviewRebuilt += RefreshAll;
            context.MetadataChanged += _ => RefreshAll();
            context.MetadataSelectionChanged += _ => RefreshSelection();
            context.CanvasChanged += RefreshAll;
            RefreshAll();
        }

        public NexUIDesignerContext Context => _context;

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
            view.EnableInClassList("is-locked", element.locked);
            view.style.position = Position.Absolute;
            ApplyRect(view, element.rect);
            view.style.backgroundColor = new StyleColor(element.tint);
            view.style.borderTopColor = new StyleColor(Lighten(element.tint, 0.18f));
            view.style.borderRightColor = new StyleColor(Lighten(element.tint, 0.18f));
            view.style.borderBottomColor = new StyleColor(Darken(element.tint, 0.18f));
            view.style.borderLeftColor = new StyleColor(Lighten(element.tint, 0.18f));

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

        private void ApplyRect(VisualElement view, Rect rect)
        {
            view.style.left = rect.x * _context.Zoom;
            view.style.top = rect.y * _context.Zoom;
            view.style.width = Mathf.Max(16, rect.width * _context.Zoom);
            view.style.height = Mathf.Max(16, rect.height * _context.Zoom);
        }

        private void OnCanvasPointerDown(PointerDownEvent evt)
        {
            Focus();
            if (evt.target == _previewCanvas || evt.target == _gridLayer || evt.target == _elementLayer)
                _context.ClearSelection();
        }

        private void BeginDrag(PointerDownEvent evt, DesignerElementMetadata element, VisualElement view)
        {
            if (evt.button != 0) return;
            Focus();
            _context.SelectMetadata(element);
            if (element.locked) return;

            _dragElement = element;
            _dragStart = new Vector2(evt.position.x, evt.position.y);
            _dragStartRect = element.rect;
            _pendingDragRect = element.rect;

            var local = view.WorldToLocal(evt.position);
            _resizing = local.x >= view.resolvedStyle.width - 16f && local.y >= view.resolvedStyle.height - 16f;
            view.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void ContinueDrag(PointerMoveEvent evt, VisualElement view)
        {
            if (_dragElement == null || !view.HasPointerCapture(evt.pointerId)) return;

            var current = new Vector2(evt.position.x, evt.position.y);
            var delta = (current - _dragStart) / Mathf.Max(0.01f, _context.Zoom);
            var rect = _dragStartRect;
            if (_resizing)
            {
                rect.width = Mathf.Max(24f, rect.width + delta.x);
                rect.height = Mathf.Max(24f, rect.height + delta.y);
            }
            else
            {
                rect.x += delta.x;
                rect.y += delta.y;
            }

            _pendingDragRect = _context.SnapRect(rect);
            ApplyRect(view, _pendingDragRect);
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
            evt.StopPropagation();
        }

        private void CommitDrag()
        {
            if (_dragElement != null)
                _context.UpdateSelectedRect(_pendingDragRect);
            _dragElement = null;
            _resizing = false;
        }

        private void RefreshSelection()
        {
            foreach (var pair in _views)
                pair.Value.EnableInClassList("is-selected", pair.Key == _context.SelectedMetadata);
        }

        private void OnWheel(WheelEvent evt)
        {
            if (!evt.ctrlKey && !evt.commandKey) return;
            _context.ZoomBy(evt.delta.y > 0 ? -0.08f : 0.08f);
            evt.StopPropagation();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace)
            {
                _context.DeleteSelectedMetadata();
                evt.StopPropagation();
            }
            else if ((evt.ctrlKey || evt.commandKey) && evt.keyCode == KeyCode.D)
            {
                _context.DuplicateSelectedMetadata();
                evt.StopPropagation();
            }
            else if (evt.keyCode == KeyCode.F && _context.SelectedMetadata != null)
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
