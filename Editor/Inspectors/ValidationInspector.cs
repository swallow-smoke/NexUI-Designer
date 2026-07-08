using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class ValidationInspector : DesignerInspectorBase
    {
        public ValidationInspector(NexUIDesignerContext context) : base(context, "panel.validation")
        {
            Add(new Label("Screen / binding / motion / theme validation"));
        }
    }
}
