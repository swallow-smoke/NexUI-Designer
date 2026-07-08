using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerStatePanel : VisualElement
    {
        public NexUIDesignerStatePanel(NexUIDesignerContext context)
        {
            AddToClassList("nexui-panel");
            AddToClassList("nexui-bottom-card");
            style.flexGrow = 1;
            Add(new Label(DesignerLocalization.T("panel.state")) { name = "PanelTitle" });
            Add(new Label("Mock keys and live binding values") { name = "PanelSubtitle" });
            var key = new TextField("Key");
            key.AddToClassList("nexui-compact-field");
            Add(key);
        }
    }
}
