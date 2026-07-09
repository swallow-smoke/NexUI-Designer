using emiteat.NexUI.Core;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// Screen-level inspector: edits <see cref="UIScreenDefinition.policy"/> on the open screen.
    /// Unlike the element-level inspectors this reads/writes <c>Context.CurrentScreen.policy</c>
    /// and refreshes on <c>ScreenChanged</c> / <c>CanvasChanged</c>.
    /// </summary>
    public sealed class PolicyInspector : DesignerInspectorBase
    {
        private readonly Toggle _inputBlocking;
        private readonly Toggle _pauseGameBehind;
        private readonly Toggle _closeOnBack;
        private readonly EnumField _cursorPolicy;
        private readonly EnumField _timePolicy;
        private readonly EnumField _focusPolicy;
        private readonly EnumField _conflictPolicy;
        private readonly EnumField _lifetimePolicy;
        private bool _refreshing;

        public PolicyInspector(NexUIDesignerContext context) : base(context, "inspector.policy")
        {
            _inputBlocking = new Toggle("Input Blocking");
            _pauseGameBehind = new Toggle("Pause Game Behind");
            _closeOnBack = new Toggle("Close On Back");
            _cursorPolicy = new EnumField("Cursor Policy", CursorPolicy.Unchanged);
            _timePolicy = new EnumField("Time Policy", UITimePolicy.Unchanged);
            _focusPolicy = new EnumField("Focus Policy", UIFocusPolicy.None);
            _conflictPolicy = new EnumField("Conflict Policy", UITransitionConflictPolicy.Wait);
            _lifetimePolicy = new EnumField("Lifetime Policy", UILifetimePolicy.DestroyOnClose);

            Add(_inputBlocking);
            Add(_pauseGameBehind);
            Add(_closeOnBack);
            Add(_cursorPolicy);
            Add(_timePolicy);
            Add(_focusPolicy);
            Add(_conflictPolicy);
            Add(_lifetimePolicy);

            _inputBlocking.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.blockInputBehind = evt.newValue, "Toggle NexUI Screen Input Blocking"));
            _pauseGameBehind.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.pauseGameBehind = evt.newValue, "Toggle NexUI Screen Pause Game Behind"));
            _closeOnBack.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.closeOnBack = evt.newValue, "Toggle NexUI Screen Close On Back"));
            _cursorPolicy.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.cursorPolicy = (CursorPolicy)evt.newValue, "Edit NexUI Screen Cursor Policy"));
            _timePolicy.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.timePolicy = (UITimePolicy)evt.newValue, "Edit NexUI Screen Time Policy"));
            _focusPolicy.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.focusPolicy = (UIFocusPolicy)evt.newValue, "Edit NexUI Screen Focus Policy"));
            _conflictPolicy.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.conflictPolicy = (UITransitionConflictPolicy)evt.newValue, "Edit NexUI Screen Conflict Policy"));
            _lifetimePolicy.RegisterValueChangedCallback(evt =>
                ChangePolicy((ref UIScreenPolicyConfig p) => p.lifetimePolicy = (UILifetimePolicy)evt.newValue, "Edit NexUI Screen Lifetime Policy"));

            context.ScreenChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private delegate void PolicyMutator(ref UIScreenPolicyConfig policy);

        private void ChangePolicy(PolicyMutator mutate, string undoName)
        {
            if (_refreshing) return;
            Context.UpdateScreen(s =>
            {
                var p = s.policy;
                mutate(ref p);
                s.policy = p;
            }, undoName);
        }

        private void Refresh()
        {
            _refreshing = true;
            var screen = Context.CurrentScreen;
            SetEnabled(screen != null);
            if (screen != null)
            {
                var p = screen.policy;
                _inputBlocking.SetValueWithoutNotify(p.blockInputBehind);
                _pauseGameBehind.SetValueWithoutNotify(p.pauseGameBehind);
                _closeOnBack.SetValueWithoutNotify(p.closeOnBack);
                _cursorPolicy.SetValueWithoutNotify(p.cursorPolicy);
                _timePolicy.SetValueWithoutNotify(p.timePolicy);
                _focusPolicy.SetValueWithoutNotify(p.focusPolicy);
                _conflictPolicy.SetValueWithoutNotify(p.conflictPolicy);
                _lifetimePolicy.SetValueWithoutNotify(p.lifetimePolicy);
            }
            _refreshing = false;
        }
    }
}
