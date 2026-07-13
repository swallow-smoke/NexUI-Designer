using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    /// <summary>
    /// Bottom-dock panel showing the Interactive-Preview log: simulated commands, forced-state
    /// changes and interactions. Read-only record - nothing here executes real game logic.
    /// </summary>
    public sealed class NexUIPreviewLogPanel : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly ScrollView _list;
        private readonly Label _empty;

        public NexUIPreviewLogPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-preview-log-panel");

            var bar = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
            bar.Add(new Label("Preview Log") { name = "PanelTitle", style = { flexGrow = 1 } });
            var clear = new Button(() => _context.PreviewLog.Clear()) { text = "Clear", tooltip = "Clear the preview log." };
            clear.AddToClassList("nexui-toolbar-button");
            bar.Add(clear);
            Add(bar);

            _empty = new Label("No preview activity yet. Switch the canvas to Interactive and click a component.");
            _empty.AddToClassList("nexui-empty-note");
            Add(_empty);

            _list = new ScrollView { style = { flexGrow = 1 } };
            Add(_list);

            var subscriptions = new ContextBoundSubscriptions(this);
            subscriptions.Add(h => _context.PreviewLog.Changed += h, h => _context.PreviewLog.Changed -= h, Refresh);
            Refresh();
        }

        private void Refresh()
        {
            _list.Clear();
            var entries = _context.PreviewLog.Entries;
            _empty.style.display = entries.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            foreach (var e in entries)
            {
                var row = new Label(e.ToString());
                row.style.whiteSpace = WhiteSpace.Normal;
                row.style.fontSize = 11;
                row.style.paddingTop = 1; row.style.paddingBottom = 1;
                row.style.color = ColorFor(e.Kind);
                _list.Add(row);
            }
        }

        private static Color ColorFor(DesignerPreviewLogKind kind)
        {
            switch (kind)
            {
                case DesignerPreviewLogKind.Command: return new Color(0.55f, 0.8f, 1f);
                case DesignerPreviewLogKind.State: return new Color(0.85f, 0.75f, 0.4f);
                case DesignerPreviewLogKind.Interaction: return new Color(0.7f, 0.85f, 0.7f);
                default: return new Color(0.75f, 0.75f, 0.78f);
            }
        }
    }
}
