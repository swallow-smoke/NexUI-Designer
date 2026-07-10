using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// C2 (first slice): Figma-style Auto Layout panel. <see cref="DesignerAutoLayoutMetadata.enabled"/>
    /// turns the selected element into an Auto Layout container that arranges its own direct
    /// children; the Hug/Fill/Fixed fields below describe how the *selected* element itself
    /// sizes when its parent is an Auto Layout container. This panel only edits metadata - it
    /// reuses UI Toolkit's existing USS flex-direction/padding/flex-grow properties rather than
    /// introducing a new layout engine, so no new runtime concept is added.
    /// </summary>
    public sealed class AutoLayoutInspector : DesignerInspectorBase
    {
        private readonly Toggle _enabled;
        private readonly EnumField _direction;
        private readonly FloatField _spacing;
        private readonly FloatField _paddingLeft;
        private readonly FloatField _paddingTop;
        private readonly FloatField _paddingRight;
        private readonly FloatField _paddingBottom;
        private readonly EnumField _widthSizing;
        private readonly EnumField _heightSizing;
        private bool _refreshing;

        public AutoLayoutInspector(NexUIDesignerContext context) : base(context, "inspector.autoLayout")
        {
            _enabled = new Toggle("Auto Layout") { tooltip = DesignerLocalization.T("tooltip.autoLayout.enabled") };
            _direction = new EnumField("Direction", DesignerAutoLayoutDirection.Column) { tooltip = DesignerLocalization.T("tooltip.autoLayout.direction") };
            _spacing = new FloatField("Spacing") { tooltip = DesignerLocalization.T("tooltip.autoLayout.spacing") };

            _paddingLeft = new FloatField("Left") { tooltip = DesignerLocalization.T("tooltip.autoLayout.paddingLeft") };
            _paddingTop = new FloatField("Top") { tooltip = DesignerLocalization.T("tooltip.autoLayout.paddingTop") };
            _paddingRight = new FloatField("Right") { tooltip = DesignerLocalization.T("tooltip.autoLayout.paddingRight") };
            _paddingBottom = new FloatField("Bottom") { tooltip = DesignerLocalization.T("tooltip.autoLayout.paddingBottom") };

            _widthSizing = new EnumField("Width", DesignerAutoLayoutSizing.Fixed) { tooltip = DesignerLocalization.T("tooltip.autoLayout.widthSizing") };
            _heightSizing = new EnumField("Height", DesignerAutoLayoutSizing.Fixed) { tooltip = DesignerLocalization.T("tooltip.autoLayout.heightSizing") };

            Add(_enabled);
            Add(_direction);
            Add(_spacing);

            var paddingRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            paddingRow.Add(_paddingLeft);
            paddingRow.Add(_paddingTop);
            paddingRow.Add(_paddingRight);
            paddingRow.Add(_paddingBottom);
            Add(new Label("Padding"));
            Add(paddingRow);

            var sizingRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            sizingRow.Add(_widthSizing);
            sizingRow.Add(_heightSizing);
            Add(new Label("Self sizing (as a child of an Auto Layout parent)"));
            Add(sizingRow);

            _enabled.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.enabled = evt.newValue, "Toggle NexUI Auto Layout"));
            _direction.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.direction = (DesignerAutoLayoutDirection)evt.newValue, "Change NexUI Auto Layout Direction"));
            _spacing.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.spacing = evt.newValue, "Edit NexUI Auto Layout Spacing"));
            _paddingLeft.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.paddingLeft = evt.newValue, "Edit NexUI Auto Layout Padding"));
            _paddingTop.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.paddingTop = evt.newValue, "Edit NexUI Auto Layout Padding"));
            _paddingRight.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.paddingRight = evt.newValue, "Edit NexUI Auto Layout Padding"));
            _paddingBottom.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.paddingBottom = evt.newValue, "Edit NexUI Auto Layout Padding"));
            _widthSizing.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.widthSizing = (DesignerAutoLayoutSizing)evt.newValue, "Change NexUI Auto Layout Width Sizing"));
            _heightSizing.RegisterValueChangedCallback(evt => Change(e => e.autoLayout.heightSizing = (DesignerAutoLayoutSizing)evt.newValue, "Change NexUI Auto Layout Height Sizing"));

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
                var layout = selected.autoLayout;
                _enabled.SetValueWithoutNotify(layout.enabled);
                _direction.SetValueWithoutNotify(layout.direction);
                _spacing.SetValueWithoutNotify(layout.spacing);
                _paddingLeft.SetValueWithoutNotify(layout.paddingLeft);
                _paddingTop.SetValueWithoutNotify(layout.paddingTop);
                _paddingRight.SetValueWithoutNotify(layout.paddingRight);
                _paddingBottom.SetValueWithoutNotify(layout.paddingBottom);
                _widthSizing.SetValueWithoutNotify(layout.widthSizing);
                _heightSizing.SetValueWithoutNotify(layout.heightSizing);

                var showChildLayoutFields = layout.enabled;
                _direction.style.display = showChildLayoutFields ? DisplayStyle.Flex : DisplayStyle.None;
                _spacing.style.display = showChildLayoutFields ? DisplayStyle.Flex : DisplayStyle.None;
                _paddingLeft.parent.style.display = showChildLayoutFields ? DisplayStyle.Flex : DisplayStyle.None;
            }
            _refreshing = false;
        }
    }
}
