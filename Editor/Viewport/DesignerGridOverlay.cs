using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    public sealed class DesignerGridOverlay : VisualElement
    {
        public DesignerGridOverlay()
        {
            name = "DesignerGridOverlay";
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            style.left = 0;
            style.right = 0;
            style.top = 0;
            style.bottom = 0;
            style.borderBottomColor = new Color(0.2f, 0.2f, 0.2f);
        }
    }
}
