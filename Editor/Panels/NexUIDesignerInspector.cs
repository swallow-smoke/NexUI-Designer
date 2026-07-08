using emiteat.NexUI.Designer.Editor.Inspectors;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerInspector : ScrollView
    {
        public NexUIDesignerInspector(NexUIDesignerContext context)
        {
            AddToClassList("nexui-inspector");
            Add(new Label(DesignerLocalization.T("panel.inspector")) { name = "PanelTitle" });
            Add(new ScreenDefinitionInspector(context));
            Add(new LayoutInspector(context));
            Add(new StyleInspector(context));
            Add(new BindingInspector(context));
            Add(new MotionInspector(context));
            Add(new ThemeInspector(context));
            Add(new PolicyInspector(context));
            Add(new CapabilityInspector(context));
            Add(new ValidationInspector(context));
        }
    }
}
