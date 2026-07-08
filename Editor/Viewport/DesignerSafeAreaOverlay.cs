using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public sealed class DesignerSafeAreaOverlay : VisualElement
    {
        public DesignerSafeAreaOverlay()
        {
            name = "DesignerSafeAreaOverlay";
            pickingMode = PickingMode.Ignore;
        }
    }
}
