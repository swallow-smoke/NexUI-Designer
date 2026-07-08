using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.Tools;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public sealed class NexUIDesignerViewport : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly Label _label;
        private readonly Label _hint;
        private readonly VisualElement _previewFrame;
        private readonly VisualElement _previewCanvas;
        private readonly Label _canvasTitle;
        private readonly Label _canvasMeta;

        public NexUIDesignerViewport(NexUIDesignerContext context)
        {
            _context = context;
            name = "NexUIDesignerViewport";
            AddToClassList("nexui-viewport");
            style.flexGrow = 1;
            _label = new Label();
            _label.AddToClassList("nexui-viewport-title");
            Add(_label);

            _hint = new Label();
            _hint.AddToClassList("nexui-viewport-hint");
            Add(_hint);

            _previewFrame = new VisualElement();
            _previewFrame.AddToClassList("nexui-preview-frame");
            _previewCanvas = new VisualElement();
            _previewCanvas.AddToClassList("nexui-preview-canvas");
            _canvasTitle = new Label();
            _canvasTitle.AddToClassList("nexui-canvas-title");
            _canvasMeta = new Label();
            _canvasMeta.AddToClassList("nexui-canvas-meta");
            _previewCanvas.Add(_canvasTitle);
            _previewCanvas.Add(_canvasMeta);
            _previewFrame.Add(_previewCanvas);
            Add(_previewFrame);

            Add(new DesignerGridOverlay());
            Add(new DesignerSafeAreaOverlay());
            Add(new DesignerLayerOverlay(context));
            Add(new DesignerSelectionOverlay(context));
            Add(new NexUIDesignerOverlay(context));
            context.PreviewRebuilt += Refresh;
            Refresh();
        }

        public NexUIDesignerContext Context => _context;

        private void Refresh()
        {
            if (_context.CurrentScreen == null)
            {
                _label.text = DesignerLocalization.T("message.noScreenSelected");
                _hint.text = "Select a UIScreenDefinition from the toolbar or Project window.";
                _canvasTitle.text = "Empty Preview";
                _canvasMeta.text = "Waiting for a screen";
                _previewCanvas.AddToClassList("is-empty");
                return;
            }

            _label.text = _context.CurrentScreen.ScreenId;
            _hint.text = "Previewing " + _context.Backend + " at " + _context.Resolution.x + "x" + _context.Resolution.y;
            _canvasTitle.text = _context.CurrentScreen.ScreenId;
            _canvasMeta.text = _context.Backend + " / " + _context.Resolution.x + "x" + _context.Resolution.y;
            _previewCanvas.RemoveFromClassList("is-empty");
        }
    }
}
