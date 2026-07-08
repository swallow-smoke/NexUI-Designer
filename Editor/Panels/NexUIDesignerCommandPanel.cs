using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerCommandPanel : VisualElement
    {
        public NexUIDesignerCommandPanel(NexUIDesignerContext context)
        {
            AddToClassList("nexui-panel");
            AddToClassList("nexui-bottom-card");
            style.flexGrow = 1;
            Add(new Label(DesignerLocalization.T("panel.command")) { name = "PanelTitle" });
            Add(new Label("Run command previews without entering play mode") { name = "PanelSubtitle" });
            var row = new VisualElement();
            row.AddToClassList("nexui-inline-row");
            row.Add(new TextField("Command"));
            row.Add(new Button(() => { }) { text = "Run" });
            Add(row);
        }
    }
}
