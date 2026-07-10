using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    /// <summary>
    /// Screen summary strip. When an element is selected, doubles as the C1 "command/binding
    /// link view": shows which commandKey/textKey/valueKey/etc. that specific element is wired
    /// to, so clicking an element answers "what does this do?" without opening the Binding
    /// inspector section separately.
    /// </summary>
    public sealed class NexUIDesignerScreenGraphPanel : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly Label _graph;

        public NexUIDesignerScreenGraphPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-bottom-card");
            style.flexGrow = 1;
            Add(new Label(DesignerLocalization.T("panel.screenGraph")) { name = "PanelTitle", tooltip = DesignerLocalization.T("tooltip.screenGraph.title") });
            _graph = new Label();
            _graph.AddToClassList("nexui-bottom-text");
            Add(_graph);

            context.ScreenChanged += _ => Refresh();
            context.MetadataChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            context.MetadataSelectionChanged += _ => Refresh();
            Refresh();
        }

        private void Refresh()
        {
            if (_context.CurrentScreen == null)
            {
                _graph.text = "No Screen";
                return;
            }

            var selected = _context.SelectedMetadata;
            if (selected != null)
            {
                _graph.text = selected.elementId + "  ->  " + DescribeBindings(selected);
                return;
            }

            var variants = _context.CurrentScreen.variants != null ? _context.CurrentScreen.variants.Length : 0;
            var responsive = _context.CurrentScreen.responsiveRules != null ? _context.CurrentScreen.responsiveRules.Length : 0;
            var elements = _context.Metadata != null ? _context.Metadata.elements.Count : 0;
            _graph.text = _context.CurrentScreen.ScreenId + " | " + _context.Backend + " | V " + variants + " | R " + responsive + " | E " + elements;
        }

        private static string DescribeBindings(DesignerElementMetadata element)
        {
            var b = element.binding;
            if (b == null) return "(no bindings)";

            var parts = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(b.commandKey)) parts.Add("command: " + b.commandKey);
            if (!string.IsNullOrEmpty(b.textKey)) parts.Add("text: " + b.textKey);
            if (!string.IsNullOrEmpty(b.valueKey)) parts.Add("value: " + b.valueKey);
            if (!string.IsNullOrEmpty(b.visibilityKey)) parts.Add("visibility: " + b.visibilityKey);
            if (!string.IsNullOrEmpty(b.interactableKey)) parts.Add("interactable: " + b.interactableKey);
            if (!string.IsNullOrEmpty(b.classKey)) parts.Add("class: " + b.classKey);

            return parts.Count == 0 ? "(no bindings)" : string.Join("  |  ", parts);
        }
    }
}
