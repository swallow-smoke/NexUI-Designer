using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class CapabilityInspector : DesignerInspectorBase
    {
        private readonly Label _capabilities;

        public CapabilityInspector(NexUIDesignerContext context) : base(context, "inspector.capability")
        {
            _capabilities = new Label { tooltip = DesignerLocalization.T("tooltip.capability.list") };
            Add(_capabilities);
            context.SelectionChanged += _ => Refresh();
            Refresh();
        }

        private void Refresh()
        {
            _capabilities.text = Context.SelectedElement == null
                ? DesignerLocalization.T("message.noElementSelected")
                : "IUITextCapability\nIUIValueCapability\nIUIVisibilityCapability\nIUIStyleCapability\nIUITransformCapability";
        }
    }
}
