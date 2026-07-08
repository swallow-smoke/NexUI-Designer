using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public abstract class DesignerInspectorBase : VisualElement
    {
        protected readonly NexUIDesignerContext Context;

        protected DesignerInspectorBase(NexUIDesignerContext context, string titleKey)
        {
            Context = context;
            AddToClassList("nexui-inspector-section");
            Add(new Label(DesignerLocalization.T(titleKey)) { name = "SectionTitle" });
        }
    }
}
