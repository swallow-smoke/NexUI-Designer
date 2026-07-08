using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public class NexUIDesignerOverlay : VisualElement
    {
        protected readonly NexUIDesignerContext Context;
        public NexUIDesignerOverlay(NexUIDesignerContext context)
        {
            Context = context;
            pickingMode = PickingMode.Ignore;
        }
    }
}
