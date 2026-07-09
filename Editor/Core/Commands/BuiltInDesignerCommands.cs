using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Commands
{
    public sealed class SelectAllCommand : IUIDesignerCommand
    {
        public string Id => "selectAll";
        public string DisplayName => "Select All";
        public bool CanExecute(NexUIDesignerContext context) => context.Metadata != null && context.Metadata.elements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.SelectAll();
    }

    public sealed class ClearSelectionCommand : IUIDesignerCommand
    {
        public string Id => "clearSelection";
        public string DisplayName => "Clear Selection";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.ClearSelection();
    }

    public sealed class DeleteSelectionCommand : IUIDesignerCommand
    {
        public string Id => "deleteSelection";
        public string DisplayName => "Delete";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.DeleteSelection();
    }

    public sealed class DuplicateSelectionCommand : IUIDesignerCommand
    {
        public string Id => "duplicateSelection";
        public string DisplayName => "Duplicate";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.DuplicateSelection();
    }

    public sealed class CopySelectionCommand : IUIDesignerCommand
    {
        public string Id => "copySelection";
        public string DisplayName => "Copy";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.CopySelection();
    }

    public sealed class PasteSelectionCommand : IUIDesignerCommand
    {
        public string Id => "pasteSelection";
        public string DisplayName => "Paste";
        public bool CanExecute(NexUIDesignerContext context) => context.HasClipboard;
        public void Execute(NexUIDesignerContext context) => context.PasteSelection();
    }

    public sealed class GroupCommand : IUIDesignerCommand
    {
        public string Id => "group";
        public string DisplayName => "Group";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count >= 2;
        public void Execute(NexUIDesignerContext context) => context.GroupSelection();
    }

    public sealed class UngroupCommand : IUIDesignerCommand
    {
        public string Id => "ungroup";
        public string DisplayName => "Ungroup";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedMetadata != null;
        public void Execute(NexUIDesignerContext context) => context.UngroupSelection();
    }

    public sealed class BringForwardCommand : IUIDesignerCommand
    {
        public string Id => "bringForward";
        public string DisplayName => "Bring Forward";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.BringSelectionForward();
    }

    public sealed class SendBackwardCommand : IUIDesignerCommand
    {
        public string Id => "sendBackward";
        public string DisplayName => "Send Backward";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.SendSelectionBackward();
    }

    public sealed class BringToFrontCommand : IUIDesignerCommand
    {
        public string Id => "bringToFront";
        public string DisplayName => "Bring To Front";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.BringSelectionToFront();
    }

    public sealed class SendToBackCommand : IUIDesignerCommand
    {
        public string Id => "sendToBack";
        public string DisplayName => "Send To Back";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.SendSelectionToBack();
    }

    public sealed class AlignLeftCommand : IUIDesignerCommand
    {
        public string Id => "alignLeft";
        public string DisplayName => "Align Left";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.AlignSelection("left");
    }

    public sealed class AlignCenterXCommand : IUIDesignerCommand
    {
        public string Id => "alignCenterX";
        public string DisplayName => "Align Center X";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.AlignSelection("centerX");
    }

    public sealed class AlignRightCommand : IUIDesignerCommand
    {
        public string Id => "alignRight";
        public string DisplayName => "Align Right";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.AlignSelection("right");
    }

    public sealed class AlignTopCommand : IUIDesignerCommand
    {
        public string Id => "alignTop";
        public string DisplayName => "Align Top";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.AlignSelection("top");
    }

    public sealed class AlignCenterYCommand : IUIDesignerCommand
    {
        public string Id => "alignCenterY";
        public string DisplayName => "Align Center Y";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.AlignSelection("centerY");
    }

    public sealed class AlignBottomCommand : IUIDesignerCommand
    {
        public string Id => "alignBottom";
        public string DisplayName => "Align Bottom";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.AlignSelection("bottom");
    }

    public sealed class DistributeHorizontalCommand : IUIDesignerCommand
    {
        public string Id => "distributeHorizontal";
        public string DisplayName => "Distribute Horizontal";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count >= 3;
        public void Execute(NexUIDesignerContext context) => context.DistributeSelectionHorizontal();
    }

    public sealed class DistributeVerticalCommand : IUIDesignerCommand
    {
        public string Id => "distributeVertical";
        public string DisplayName => "Distribute Vertical";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count >= 3;
        public void Execute(NexUIDesignerContext context) => context.DistributeSelectionVertical();
    }

    public sealed class MoveSelectionLeftCommand : IUIDesignerCommand
    {
        public string Id => "moveLeft";
        public string DisplayName => "Move Left 1px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(-1f, 0f));
    }

    public sealed class MoveSelectionRightCommand : IUIDesignerCommand
    {
        public string Id => "moveRight";
        public string DisplayName => "Move Right 1px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(1f, 0f));
    }

    public sealed class MoveSelectionUpCommand : IUIDesignerCommand
    {
        public string Id => "moveUp";
        public string DisplayName => "Move Up 1px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(0f, -1f));
    }

    public sealed class MoveSelectionDownCommand : IUIDesignerCommand
    {
        public string Id => "moveDown";
        public string DisplayName => "Move Down 1px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(0f, 1f));
    }

    public sealed class MoveSelectionLeftFastCommand : IUIDesignerCommand
    {
        public string Id => "moveLeftFast";
        public string DisplayName => "Move Left 10px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(-10f, 0f));
    }

    public sealed class MoveSelectionRightFastCommand : IUIDesignerCommand
    {
        public string Id => "moveRightFast";
        public string DisplayName => "Move Right 10px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(10f, 0f));
    }

    public sealed class MoveSelectionUpFastCommand : IUIDesignerCommand
    {
        public string Id => "moveUpFast";
        public string DisplayName => "Move Up 10px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(0f, -10f));
    }

    public sealed class MoveSelectionDownFastCommand : IUIDesignerCommand
    {
        public string Id => "moveDownFast";
        public string DisplayName => "Move Down 10px";
        public bool CanExecute(NexUIDesignerContext context) => context.SelectedElements.Count > 0;
        public void Execute(NexUIDesignerContext context) => context.MoveSelection(new Vector2(0f, 10f));
    }
}
