using emiteat.NexUI.Designer.Editor.Backend;

namespace emiteat.NexUI.Designer.Editor.Tools
{
    public sealed class CreateElementTool : NexUIDesignerToolBase
    {
        public override string ToolId => "createElement";
        public override string DisplayNameKey => "tool.createElement";

        public void Create(NexUIDesignerContext context, DesignerElementType type, string elementId)
        {
            if (context.CurrentBackend == null || context.PreviewSurface == null) return;
            var handle = context.CurrentBackend.CreateElement(context.PreviewSurface, null, new DesignerElementCreateInfo
            {
                type = type,
                elementId = elementId,
                displayName = elementId
            });
            context.Select(handle);
        }
    }
}
