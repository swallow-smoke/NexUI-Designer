using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public sealed class DesignerLayerOverlay : VisualElement
    {
        public DesignerLayerOverlay(NexUIDesignerContext context)
        {
            name = "DesignerLayerOverlay";
            pickingMode = PickingMode.Ignore;
        }
    }
}
