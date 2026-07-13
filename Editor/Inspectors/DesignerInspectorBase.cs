using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public abstract class DesignerInspectorBase : VisualElement
    {
        protected readonly NexUIDesignerContext Context;
        protected readonly ContextBoundSubscriptions Subscriptions;

        protected DesignerInspectorBase(NexUIDesignerContext context, string titleKey)
        {
            Context = context;
            Subscriptions = new ContextBoundSubscriptions(this);
            AddToClassList("nexui-inspector-section");
            Add(new Label(DesignerLocalization.T(titleKey)) { name = "SectionTitle" });
        }
    }
}
