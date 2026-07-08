using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class MotionInspector : DesignerInspectorBase
    {
        public MotionInspector(NexUIDesignerContext context) : base(context, "inspector.motion")
        {
            Add(new ObjectField("Motion Preset"));
            Add(new TextField("Variant"));
        }
    }
}
