using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerScreenGraphPanel : VisualElement
    {
        public NexUIDesignerScreenGraphPanel(NexUIDesignerContext context)
        {
            AddToClassList("nexui-panel");
            AddToClassList("nexui-bottom-card");
            style.flexGrow = 1;
            Add(new Label(DesignerLocalization.T("panel.screenGraph")) { name = "PanelTitle" });
            Add(new Label("Routes / back stack / modal relations") { name = "PanelSubtitle" });
            Add(new Label("No graph edges yet") { name = "EmptyStateText" });
        }
    }
}
