using emiteat.NexUI.Designer.Editor.Viewport;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.UI.Shell
{
    public sealed class NexUIDesignerShell : VisualElement
    {
        public NexUIDesignerContext Context { get; }

        private readonly NexUIDesignerViewport _viewport;
        private readonly NexUICommandPalette _commandPalette;

        public NexUIDesignerShell(NexUIDesignerContext context)
        {
            Context = context;
            AddToClassList("nexui-shell");
            focusable = true;

            var globalToolbar = new NexUIGlobalToolbar(context);
            Add(globalToolbar);

            _viewport = new NexUIDesignerViewport(context);

            var canvasColumn = new VisualElement();
            canvasColumn.AddToClassList("nexui-canvas-column");
            canvasColumn.Add(new NexUICanvasToolbar(context, () => _viewport.FitToView()));
            canvasColumn.Add(_viewport);

            var left = new NexUILeftSidebar(context);
            var right = new NexUIRightInspector(context);

            var centerRightSplit = new TwoPaneSplitView(1, 340, TwoPaneSplitViewOrientation.Horizontal)
            {
                name = "NexUICenterRightSplit",
                viewDataKey = "NexUI.Rebuild.Split.CenterRight"
            };
            centerRightSplit.Add(canvasColumn);
            centerRightSplit.Add(right);

            var bodySplit = new TwoPaneSplitView(0, 260, TwoPaneSplitViewOrientation.Horizontal)
            {
                name = "NexUIBodySplit",
                viewDataKey = "NexUI.Rebuild.Split.LeftBody"
            };
            bodySplit.AddToClassList("nexui-rebuild-body");
            bodySplit.Add(left);
            bodySplit.Add(centerRightSplit);

            var bodyAndDrawer = new VisualElement();
            bodyAndDrawer.AddToClassList("nexui-body-and-drawer");
            bodyAndDrawer.Add(bodySplit);
            bodyAndDrawer.Add(new NexUIBottomDrawer(context));

            Add(bodyAndDrawer);

            _commandPalette = new NexUICommandPalette(context);
            Add(_commandPalette);

            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            var commandPaletteShortcut = evt.keyCode == UnityEngine.KeyCode.K && (evt.ctrlKey || evt.commandKey);
            var vscodeShortcut = evt.keyCode == UnityEngine.KeyCode.P && (evt.ctrlKey || evt.commandKey) && evt.shiftKey;
            if (!commandPaletteShortcut && !vscodeShortcut) return;

            _commandPalette.Toggle();
            evt.StopPropagation();
        }
    }
}
