using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class ValidationInspector : DesignerInspectorBase
    {
        public ValidationInspector(NexUIDesignerContext context) : base(context, "panel.validation")
        {
            Add(new Label(DesignerLocalization.T("inspector.validation.text")) { tooltip = DesignerLocalization.T("tooltip.validation.text") });
        }
    }
}
