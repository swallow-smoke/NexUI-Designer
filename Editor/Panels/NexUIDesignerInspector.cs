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

            // C3 (Simple/Advanced mode): essential, always visible.
            Add(new MultiSelectionInspector(context));
            Add(new LayoutInspector(context));
            Add(new AutoLayoutInspector(context));
            Add(new StyleInspector(context));
            Add(new BindingInspector(context));
            Add(new ValidationInspector(context));

            // Advanced-only: raw screen/theme/motion/policy/capability editing that a
            // non-programmer doing a first layout pass doesn't need to see.
            AddAdvancedOnly(new ConstraintsInspector(context));
            AddAdvancedOnly(new ScreenDefinitionInspector(context));
            AddAdvancedOnly(new MotionInspector(context));
            AddAdvancedOnly(new ThemeInspector(context));
            AddAdvancedOnly(new AccessibilityInspector(context));
            AddAdvancedOnly(new PolicyInspector(context));
            AddAdvancedOnly(new CapabilityInspector(context));
        }

        private void AddAdvancedOnly(VisualElement section)
        {
            Add(section);
            void Refresh() => section.style.display = DesignerEditMode.IsAdvanced ? DisplayStyle.Flex : DisplayStyle.None;
            Refresh();
            DesignerEditMode.Changed += _ => Refresh();
        }
    }
}
