using emiteat.NexUI.Accessibility;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class AccessibilityInspector : DesignerInspectorBase
    {
        private readonly TextField _label;
        private readonly EnumField _role;
        private bool _refreshing;

        public AccessibilityInspector(NexUIDesignerContext context) : base(context, "inspector.accessibility")
        {
            _label = new TextField("Accessibility Label") { tooltip = DesignerLocalization.T("tooltip.accessibility.label") };
            _role = new EnumField("Role", AccessibilityRole.None) { tooltip = DesignerLocalization.T("tooltip.accessibility.role") };
            Add(_label);
            Add(_role);

            _label.RegisterValueChangedCallback(evt =>
                Change(e => e.accessibilityLabel = evt.newValue));
            _role.RegisterValueChangedCallback(evt =>
                Change(e => e.accessibilityRole = (AccessibilityRole)evt.newValue));

            context.MetadataSelectionChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private void Change(System.Action<DesignerElementMetadata> change)
        {
            if (_refreshing) return;
            Context.UpdateSelectedElement(change, "Edit NexUI Element Accessibility");
        }

        private void Refresh()
        {
            _refreshing = true;
            var selected = Context.SelectedMetadata;
            SetEnabled(selected != null);
            if (selected != null)
            {
                _label.SetValueWithoutNotify(selected.accessibilityLabel);
                _role.SetValueWithoutNotify(selected.accessibilityRole);
            }
            _refreshing = false;
        }
    }
}
