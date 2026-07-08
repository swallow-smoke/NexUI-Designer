using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class ThemeInspector : DesignerInspectorBase
    {
        public ThemeInspector(NexUIDesignerContext context) : base(context, "inspector.theme")
        {
            Add(new ObjectField("Theme"));
            Add(new TextField("Token Override"));
        }
    }
}
