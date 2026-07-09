namespace emiteat.NexUI.Designer.Editor.Commands
{
    /// <summary>A single undoable Designer action, invocable from a shortcut, a menu item, or a toolbar button.</summary>
    public interface IUIDesignerCommand
    {
        string Id { get; }
        string DisplayName { get; }
        bool CanExecute(NexUIDesignerContext context);
        void Execute(NexUIDesignerContext context);
    }
}
