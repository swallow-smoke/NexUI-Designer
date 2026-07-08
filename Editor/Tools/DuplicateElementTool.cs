namespace emiteat.NexUI.Designer.Editor.Tools
{
    public sealed class DuplicateElementTool : NexUIDesignerToolBase
    {
        public override string ToolId => "duplicateElement";
        public override string DisplayNameKey => "button.duplicate";

        public void Duplicate(NexUIDesignerContext context)
        {
            if (context.CurrentBackend == null || context.PreviewSurface == null || context.SelectedElement == null) return;
            context.Select(context.CurrentBackend.DuplicateElement(context.PreviewSurface, context.SelectedElement));
        }
    }
}
