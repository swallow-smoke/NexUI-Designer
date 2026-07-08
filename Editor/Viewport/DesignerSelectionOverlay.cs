using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public sealed class DesignerSelectionOverlay : VisualElement
    {
        private readonly Label _label;
        private readonly NexUIDesignerContext _context;

        public DesignerSelectionOverlay(NexUIDesignerContext context)
        {
            _context = context;
            _label = new Label();
            Add(_label);
            context.SelectionChanged += _ => Refresh();
            Refresh();
        }

        private void Refresh()
            => _label.text = _context.SelectedElement == null ? DesignerLocalization.T("message.noElementSelected") : _context.SelectedElement.Id;
    }
}
