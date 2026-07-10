using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class ScreenDefinitionInspector : DesignerInspectorBase
    {
        public ScreenDefinitionInspector(NexUIDesignerContext context) : base(context, "designer.title")
        {
            Add(new ObjectField("Screen") { value = context.CurrentScreen, tooltip = DesignerLocalization.T("tooltip.screenDefinition.screen") });
        }
    }
}
