namespace emiteat.NexUI.Designer.Editor.Tools
{
    public sealed class DeleteElementTool : NexUIDesignerToolBase
    {
        public override string ToolId => "deleteElement";
        public override string DisplayNameKey => "button.delete";

        public void Delete(NexUIDesignerContext context)
        {
            if (context.CurrentBackend == null || context.PreviewSurface == null || context.SelectedElement == null) return;
            context.CurrentBackend.DeleteElement(context.PreviewSurface, context.SelectedElement);
            context.ClearSelection();
        }
    }
}
