using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// C2 (second slice, pairs with C1's breakpoint preview): pin behavior for this element
    /// when its parent is resized - Figma's Constraints model (Start/End/Center/Scale per
    /// axis). Advanced-only per C3 (a first layout pass rarely needs this; it matters once a
    /// screen has to survive multiple breakpoints). Metadata-only for now: the responsive
    /// preview does not yet re-run these constraints when swapping resolution presets - that
    /// wiring is a follow-up once the canvas itself understands parent/child nesting (see C2's
    /// Auto Layout panel note on the same limitation).
    /// </summary>
    public sealed class ConstraintsInspector : DesignerInspectorBase
    {
        private readonly EnumField _horizontal;
        private readonly EnumField _vertical;
        private bool _refreshing;

        public ConstraintsInspector(NexUIDesignerContext context) : base(context, "inspector.constraints")
        {
            _horizontal = new EnumField("Horizontal", DesignerConstraintMode.Start) { tooltip = DesignerLocalization.T("tooltip.constraints.horizontal") };
            _vertical = new EnumField("Vertical", DesignerConstraintMode.Start) { tooltip = DesignerLocalization.T("tooltip.constraints.vertical") };

            Add(_horizontal);
            Add(_vertical);

            _horizontal.RegisterValueChangedCallback(evt => Change(e => e.constraint.horizontal = (DesignerConstraintMode)evt.newValue, "Change NexUI Element Horizontal Constraint"));
            _vertical.RegisterValueChangedCallback(evt => Change(e => e.constraint.vertical = (DesignerConstraintMode)evt.newValue, "Change NexUI Element Vertical Constraint"));

            context.MetadataSelectionChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private void Change(System.Action<DesignerElementMetadata> change, string undoName)
        {
            if (_refreshing) return;
            Context.UpdateSelectedElement(change, undoName);
        }

        private void Refresh()
        {
            _refreshing = true;
            var selected = Context.SelectedMetadata;
            SetEnabled(selected != null);
            if (selected != null)
            {
                _horizontal.SetValueWithoutNotify(selected.constraint.horizontal);
                _vertical.SetValueWithoutNotify(selected.constraint.vertical);
            }
            _refreshing = false;
        }
    }
}
