using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerCommandPanel : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly TextField _command;
        private readonly Label _result;

        public NexUIDesignerCommandPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-bottom-card");
            style.flexGrow = 1;
            Add(new Label(DesignerLocalization.T("panel.command")) { name = "PanelTitle", tooltip = DesignerLocalization.T("tooltip.command.title") });
            var row = new VisualElement();
            row.AddToClassList("nexui-inline-row");
            _command = new TextField("Command") { tooltip = DesignerLocalization.T("tooltip.command.field") };
            row.Add(_command);
            row.Add(new Button(Run) { text = "Run", tooltip = DesignerLocalization.T("tooltip.command.run") });
            _result = new Label();
            _result.AddToClassList("nexui-bottom-text");
            Add(_result);

            var subscriptions = new ContextBoundSubscriptions(this);
            subscriptions.Add<DesignerElementMetadata>(h => context.MetadataSelectionChanged += h, h => context.MetadataSelectionChanged -= h, _ => Refresh());
            subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, Refresh);
            Refresh();
        }

        private void Refresh()
        {
            var selected = _context.SelectedMetadata;
            _command.SetValueWithoutNotify(selected != null ? selected.binding.commandKey : string.Empty);
            _result.text = selected == null ? "No command" : (string.IsNullOrEmpty(selected.binding.commandKey) ? selected.elementId + ": no command" : selected.binding.commandKey);
        }

        private void Run()
        {
            var command = _command.value;
            if (string.IsNullOrEmpty(command))
            {
                _result.text = "No command key to run.";
                return;
            }
            _result.text = "Preview command: " + command;
            if (_context.SelectedMetadata != null)
                _context.UpdateSelectedElement(e => e.binding.commandKey = command, "Edit NexUI Command Key");
        }
    }
}
