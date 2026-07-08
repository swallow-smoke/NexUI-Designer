using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class StyleInspector : DesignerInspectorBase
    {
        public StyleInspector(NexUIDesignerContext context) : base(context, "inspector.style")
        {
            Add(new TextField("Class"));
        }
    }
}
