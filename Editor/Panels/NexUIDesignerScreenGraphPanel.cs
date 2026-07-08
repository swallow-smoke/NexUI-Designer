using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
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
            Add(new Label(DesignerLocalization.T("panel.screenGraph")) { name = "PanelTitle" });
            _graph = new Label();
            _graph.AddToClassList("nexui-bottom-text");
            Add(_graph);

            context.ScreenChanged += _ => Refresh();
            context.MetadataChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            if (_context.CurrentScreen == null)
            {
                _graph.text = "No Screen";
                return;
            }

            var variants = _context.CurrentScreen.variants != null ? _context.CurrentScreen.variants.Length : 0;
            var responsive = _context.CurrentScreen.responsiveRules != null ? _context.CurrentScreen.responsiveRules.Length : 0;
            var elements = _context.Metadata != null ? _context.Metadata.elements.Count : 0;
            _graph.text = _context.CurrentScreen.ScreenId + " | " + _context.Backend + " | V " + variants + " | R " + responsive + " | E " + elements;
        }
    }
}
