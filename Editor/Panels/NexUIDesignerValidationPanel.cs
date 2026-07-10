using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.Validation;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerValidationPanel : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly Label _summary;
        private readonly ScrollView _list;

        public NexUIDesignerValidationPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-validation-panel");
            Add(new Label(DesignerLocalization.T("panel.validation")) { name = "PanelTitle", tooltip = DesignerLocalization.T("tooltip.validationPanel.title") });

            _summary = new Label();
            _summary.AddToClassList("nexui-validation-summary");
            Add(_summary);

            _list = new ScrollView { name = "ValidationList" };
            _list.AddToClassList("nexui-validation-content");
            Add(_list);

            context.ValidationChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            _list.Clear();
            var issues = _context.ValidationIssues;

            if (issues.Count == 0)
            {
                _summary.text = DesignerLocalization.T("message.validationPassed");
                EnableInClassList("is-valid", true);
                EnableInClassList("has-issues", false);
                return;
            }

            _summary.text = $"{_context.ErrorCount} error(s), {_context.WarningCount} warning(s)";
            EnableInClassList("is-valid", false);
            EnableInClassList("has-issues", true);

            foreach (var issue in issues)
            {
                var row = new Label(issue.ToString());
                row.AddToClassList("nexui-validation-item");
                row.AddToClassList("severity-" + issue.Severity.ToString().ToLowerInvariant());
                if (!string.IsNullOrEmpty(issue.ElementId))
                {
                    var id = issue.ElementId;
                    row.tooltip = string.Format(DesignerLocalization.T("tooltip.validationPanel.item"), id);
                    row.RegisterCallback<ClickEvent>(_ => _context.SelectMetadata(id));
                }
                _list.Add(row);
            }
        }
    }
}
