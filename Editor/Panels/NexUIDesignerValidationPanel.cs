using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerValidationPanel : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly Label _content;

        public NexUIDesignerValidationPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-validation-panel");
            Add(new Label(DesignerLocalization.T("panel.validation")) { name = "PanelTitle" });
            _content = new Label();
            _content.AddToClassList("nexui-validation-content");
            Add(_content);
            context.ValidationChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            _content.text = _context.ValidationMessages.Count == 0
                ? DesignerLocalization.T("message.validationPassed")
                : string.Join("\n", _context.ValidationMessages);
            EnableInClassList("is-valid", _context.ValidationMessages.Count == 0);
            EnableInClassList("has-issues", _context.ValidationMessages.Count != 0);
        }
    }
}
