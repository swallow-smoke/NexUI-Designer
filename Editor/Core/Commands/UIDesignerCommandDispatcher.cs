using System.Collections.Generic;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Commands
{
    /// <summary>Matches a key event against <see cref="UIDesignerShortcutRegistry.Current"/> and runs the bound command.</summary>
    public static class UIDesignerCommandDispatcher
    {
        private static readonly Dictionary<string, IUIDesignerCommand> _commands = BuildRegistry();

        public static IReadOnlyDictionary<string, IUIDesignerCommand> Commands => _commands;

        /// <summary>Returns true (and stops propagation upstream) if a bound, executable command handled the key.</summary>
        public static bool TryDispatch(KeyDownEvent evt, NexUIDesignerContext context)
        {
            var shortcuts = UIDesignerShortcutRegistry.Current;
            for (int i = 0; i < shortcuts.Count; i++)
            {
                var shortcut = shortcuts[i];
                if (!shortcut.Matches(evt)) continue;
                if (!_commands.TryGetValue(shortcut.commandId, out var command)) continue;
                if (!command.CanExecute(context)) continue;
                command.Execute(context);
                return true;
            }
            return false;
        }

        private static Dictionary<string, IUIDesignerCommand> BuildRegistry()
        {
            var commands = new IUIDesignerCommand[]
            {
                new SelectAllCommand(),
                new ClearSelectionCommand(),
                new DeleteSelectionCommand(),
                new DuplicateSelectionCommand(),
                new CopySelectionCommand(),
                new PasteSelectionCommand(),
                new GroupCommand(),
                new UngroupCommand(),
                new BringForwardCommand(),
                new SendBackwardCommand(),
                new BringToFrontCommand(),
                new SendToBackCommand(),
                new AlignLeftCommand(),
                new AlignCenterXCommand(),
                new AlignRightCommand(),
                new AlignTopCommand(),
                new AlignCenterYCommand(),
                new AlignBottomCommand(),
                new DistributeHorizontalCommand(),
                new DistributeVerticalCommand(),
                new MoveSelectionLeftCommand(),
                new MoveSelectionRightCommand(),
                new MoveSelectionUpCommand(),
                new MoveSelectionDownCommand(),
                new MoveSelectionLeftFastCommand(),
                new MoveSelectionRightFastCommand(),
                new MoveSelectionUpFastCommand(),
                new MoveSelectionDownFastCommand(),
            };

            var dict = new Dictionary<string, IUIDesignerCommand>();
            foreach (var command in commands)
                dict[command.Id] = command;
            return dict;
        }
    }
}
