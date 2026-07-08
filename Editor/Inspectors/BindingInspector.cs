using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class BindingInspector : DesignerInspectorBase
    {
        public BindingInspector(NexUIDesignerContext context) : base(context, "inspector.binding")
        {
            Add(new TextField("Text Key"));
            Add(new TextField("Value Key"));
            Add(new TextField("Command Key"));
        }
    }
}
