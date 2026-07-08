using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class LayoutInspector : DesignerInspectorBase
    {
        public LayoutInspector(NexUIDesignerContext context) : base(context, "inspector.layout")
        {
            Add(new Vector2Field("Position"));
            Add(new Vector2Field("Size"));
        }
    }
}
