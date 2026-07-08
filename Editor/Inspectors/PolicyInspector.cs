using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class PolicyInspector : DesignerInspectorBase
    {
        public PolicyInspector(NexUIDesignerContext context) : base(context, "inspector.policy")
        {
            Add(new Toggle("Input Blocking"));
            Add(new Toggle("Close On Back"));
        }
    }
}
