using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.Viewport;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using emiteat.NexUI.Designer.Editor.Productivity;

namespace emiteat.NexUI.Designer.Editor.UI.Shell
{
    public sealed class NexUICanvasToolbar : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly Label _zoom;

        public NexUICanvasToolbar(NexUIDesignerContext context, System.Action fitCanvas)
        {
            _context = context;
            AddToClassList("nexui-canvas-toolbar");

            AddTool(context, DesignerTool.Select, "Select", "Select and edit elements.");
            AddTool(context, DesignerTool.Move, "Move", "Move selected elements.");
            AddTool(context, DesignerTool.Frame, "Frame", "Prepare frame editing.");
            AddTool(context, DesignerTool.Hand, "Hand", "Pan the canvas.");

            Add(Divider());

            var resolution = new PopupField<string>("Frame") { tooltip = DesignerLocalization.T("tooltip.toolbar.preview") };
            foreach (var preset in DesignerResolutionPreset.Defaults)
                resolution.choices.Add(preset.Name);
            resolution.value = "1920x1080";
            resolution.AddToClassList("nexui-canvas-field");
            resolution.RegisterValueChangedCallback(evt =>
            {
                foreach (var preset in DesignerResolutionPreset.Defaults)
                    if (preset.Name == evt.newValue)
                        context.SetResolution(preset.Resolution);
            });
            Add(resolution);

            var state = new PopupField<string>("State", new List<string> { "Normal", "Hover", "Pressed", "Disabled", "Focused" }, context.PreviewState)
            {
                tooltip = DesignerLocalization.T("tooltip.toolbar.state")
            };
            state.AddToClassList("nexui-mini-popup");
            state.RegisterValueChangedCallback(evt => context.SetPreviewState(evt.newValue));
            Add(state);

            var input = new PopupField<string>("Input", new List<string> { "Keyboard", "Gamepad", "Touch", "SteamDeck" }, context.InputMode)
            {
                tooltip = DesignerLocalization.T("tooltip.toolbar.input")
            };
            input.AddToClassList("nexui-mini-popup");
            input.RegisterValueChangedCallback(evt => context.SetInputMode(evt.newValue));
            Add(input);

            Add(Divider());

            var snap = new Toggle("Snap") { value = context.SnapEnabled, tooltip = DesignerLocalization.T("tooltip.toolbar.snap") };
            snap.AddToClassList("nexui-toolbar-toggle");
            snap.RegisterValueChangedCallback(evt => context.SetSnap(evt.newValue));
            Add(snap);

            var grid = new FloatField("Grid") { value = context.GridSize, tooltip = "Grid size in pixels." };
            grid.AddToClassList("nexui-grid-size-field");
            grid.RegisterValueChangedCallback(evt => context.SetGridSize(evt.newValue));
            Add(grid);

            Add(MakeButton(() => context.ZoomBy(-0.1f), "-", DesignerLocalization.T("tooltip.toolbar.zoomOut")));
            _zoom = new Label();
            _zoom.AddToClassList("nexui-zoom-readout");
            Add(_zoom);
            Add(MakeButton(() => context.ZoomBy(0.1f), "+", DesignerLocalization.T("tooltip.toolbar.zoomIn")));
            Add(MakeButton(fitCanvas, "Fit", "Fit canvas to the available viewport."));
            Add(MakeButton(context.RebuildPreview, "Rebuild", DesignerLocalization.T("tooltip.toolbar.rebuild")));

            Add(Divider());
            Add(MakeButton(() => DesignerLayoutConversionWindow.Open(context), DesignerLocalization.T("productivity.layout"), DesignerLocalization.T("productivity.tooltip.layout")));
            Add(MakeButton(() => DesignerTransitionPresetWindow.Open(context), DesignerLocalization.T("productivity.transition"), DesignerLocalization.T("productivity.tooltip.transition")));

            var subscriptions = new ContextBoundSubscriptions(this);
            subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, Refresh);
            subscriptions.Add(h => context.UIStateChanged += h, h => context.UIStateChanged -= h, RefreshTools);
            Refresh();
            RefreshTools();
        }

        private void AddTool(NexUIDesignerContext context, DesignerTool tool, string label, string tooltip)
        {
            var button = new Button(() => context.SetTool(tool)) { text = label, tooltip = tooltip };
            button.AddToClassList("nexui-tool-button");
            button.userData = tool;
            Add(button);
        }

        private void Refresh()
        {
            _zoom.text = UnityEngine.Mathf.RoundToInt(_context.Zoom * 100f) + "%";
        }

        private void RefreshTools()
        {
            foreach (var child in Children())
                if (child is Button button && button.userData is DesignerTool tool)
                    button.EnableInClassList("is-active", tool == _context.CurrentTool);
            Refresh();
        }

        private static Button MakeButton(System.Action action, string text, string tooltip)
        {
            var button = new Button(action) { text = text, tooltip = tooltip };
            button.AddToClassList("nexui-toolbar-button");
            button.AddToClassList("nexui-button-secondary");
            return button;
        }

        private static VisualElement Divider()
        {
            var divider = new VisualElement();
            divider.AddToClassList("nexui-toolbar-divider");
            return divider;
        }
    }
}
