using emiteat.NexUI.Designer.Editor.Graph;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.MotionClipEditor;
using emiteat.NexUI.Designer.Editor;
using emiteat.NexUI.Motion;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// Element-level inspector for <see cref="DesignerMotionMetadata"/>: the preset asset,
    /// its motionId, the six per-state variant names, and an entry point that opens the
    /// visual Motion Graph editor for the assigned preset.
    /// </summary>
    public sealed class MotionInspector : DesignerInspectorBase
    {
        private readonly ObjectField _preset;
        private readonly TextField _motionId;
        private readonly TextField _initial;
        private readonly TextField _animate;
        private readonly TextField _exit;
        private readonly TextField _hover;
        private readonly TextField _pressed;
        private readonly TextField _focus;
        private readonly Button _openGraph;
        private readonly Button _openClipEditor;
        private readonly HelpBox _noPresetHelp;
        private readonly ObjectField _entryClip;
        private readonly ObjectField _exitClip;
        private readonly Foldout _bindings;
        private readonly Button _addBinding;
        private bool _refreshing;

        public MotionInspector(NexUIDesignerContext context) : base(context, "inspector.motion")
        {
            _preset = new ObjectField("Motion Preset") { objectType = typeof(UIMotionPreset), allowSceneObjects = false, tooltip = DesignerLocalization.T("tooltip.motion.preset") };
            _motionId = new TextField("Motion Id") { tooltip = DesignerLocalization.T("tooltip.motion.motionId") };
            _initial = new TextField("Initial Variant") { tooltip = DesignerLocalization.T("tooltip.motion.initial") };
            _animate = new TextField("Animate Variant") { tooltip = DesignerLocalization.T("tooltip.motion.animate") };
            _exit = new TextField("Exit Variant") { tooltip = DesignerLocalization.T("tooltip.motion.exit") };
            _hover = new TextField("Hover Variant") { tooltip = DesignerLocalization.T("tooltip.motion.hover") };
            _pressed = new TextField("Pressed Variant") { tooltip = DesignerLocalization.T("tooltip.motion.pressed") };
            _focus = new TextField("Focus Variant") { tooltip = DesignerLocalization.T("tooltip.motion.focus") };
            _openGraph = new Button(OpenGraph) { text = DesignerLocalization.T("button.openMotionGraph"), tooltip = DesignerLocalization.T("tooltip.motion.openGraph") };
            _openClipEditor = new Button(OpenClipEditor) { text = DesignerLocalization.T("button.openMotionClipEditor"), tooltip = DesignerLocalization.T("tooltip.motion.openClipEditor") };
            _noPresetHelp = new HelpBox("Assign a Motion Preset to edit its graph.", HelpBoxMessageType.Info);
            _entryClip = new ObjectField(DesignerLocalization.T("motion.entryClip")) { objectType = typeof(UIMotionClip), allowSceneObjects = false };
            _exitClip = new ObjectField(DesignerLocalization.T("motion.exitClip")) { objectType = typeof(UIMotionClip), allowSceneObjects = false };
            _bindings = new Foldout { text = DesignerLocalization.T("motion.bindings"), value = true };
            _addBinding = new Button(() =>
            {
                Context.AddMotionBinding(DesignerMotionTrigger.Click);
                RefreshBindings();
            }) { text = DesignerLocalization.T("motion.addBinding") };

            Add(_preset);
            Add(_motionId);
            Add(_initial);
            Add(_animate);
            Add(_exit);
            Add(_hover);
            Add(_pressed);
            Add(_focus);
            Add(_openGraph);
            Add(_noPresetHelp);
            Add(_openClipEditor);
            Add(_entryClip);
            Add(_exitClip);
            Add(_bindings);
            Add(_addBinding);

            _preset.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                var preset = evt.newValue as UIMotionPreset;
                Context.UpdateSelectedElement(e =>
                {
                    e.motion.motionPreset = preset;
                    if (preset != null && !string.IsNullOrEmpty(preset.motionId))
                        e.motion.motionId = preset.motionId;
                }, "Assign NexUI Element Motion Preset");
                RefreshGraphButton();
            });
            _motionId.RegisterValueChangedCallback(evt => Change(e => e.motion.motionId = evt.newValue, "Edit NexUI Motion Id"));
            _initial.RegisterValueChangedCallback(evt => Change(e => e.motion.initialVariant = evt.newValue, "Edit NexUI Initial Variant"));
            _animate.RegisterValueChangedCallback(evt => Change(e => e.motion.animateVariant = evt.newValue, "Edit NexUI Animate Variant"));
            _exit.RegisterValueChangedCallback(evt => Change(e => e.motion.exitVariant = evt.newValue, "Edit NexUI Exit Variant"));
            _hover.RegisterValueChangedCallback(evt => Change(e => e.motion.hoverVariant = evt.newValue, "Edit NexUI Hover Variant"));
            _pressed.RegisterValueChangedCallback(evt => Change(e => e.motion.pressedVariant = evt.newValue, "Edit NexUI Pressed Variant"));
            _focus.RegisterValueChangedCallback(evt => Change(e => e.motion.focusVariant = evt.newValue, "Edit NexUI Focus Variant"));
            _entryClip.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                Context.UpdateScreenMotion(m => m.entryClip = evt.newValue as UIMotionClip, "Assign NexUI Screen Enter Clip");
            });
            _exitClip.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                Context.UpdateScreenMotion(m => m.exitClip = evt.newValue as UIMotionClip, "Assign NexUI Screen Exit Clip");
            });

            var subscriptions = new ContextBoundSubscriptions(this);
            subscriptions.Add<DesignerElementMetadata>(h => context.MetadataSelectionChanged += h, h => context.MetadataSelectionChanged -= h, _ => Refresh());
            subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, Refresh);
            Refresh();
        }

        private void Change(System.Action<DesignerElementMetadata> change, string undoName)
        {
            if (_refreshing) return;
            Context.UpdateSelectedElement(change, undoName);
        }

        private void OpenGraph()
        {
            var preset = Context.SelectedMetadata?.motion.motionPreset;
            if (preset != null)
                MotionGraphWindow.Open(preset);
        }

        private void OpenClipEditor()
        {
            var elementId = Context.SelectedMetadata?.elementId;
            if (string.IsNullOrEmpty(elementId)) return;
            MotionClipEditorWindow.Open(Context.PreviewSurface, elementId);
        }

        private void Refresh()
        {
            _refreshing = true;
            var selected = Context.SelectedMetadata;
            SetEnabled(selected != null);
            if (selected != null)
            {
                var m = selected.motion;
                _preset.SetValueWithoutNotify(m.motionPreset);
                _motionId.SetValueWithoutNotify(m.motionId);
                _initial.SetValueWithoutNotify(m.initialVariant);
                _animate.SetValueWithoutNotify(m.animateVariant);
                _exit.SetValueWithoutNotify(m.exitVariant);
                _hover.SetValueWithoutNotify(m.hoverVariant);
                _pressed.SetValueWithoutNotify(m.pressedVariant);
                _focus.SetValueWithoutNotify(m.focusVariant);
            }
            var screenMotion = Context.Metadata?.screenMotion;
            _entryClip.SetValueWithoutNotify(screenMotion?.entryClip);
            _exitClip.SetValueWithoutNotify(screenMotion?.exitClip);
            _refreshing = false;
            RefreshBindings();
            RefreshGraphButton();
        }

        private void RefreshBindings()
        {
            _bindings.Clear();
            var motion = Context.Metadata?.screenMotion;
            var selectedId = Context.SelectedMetadata?.elementId;
            if (motion?.bindings == null) return;

            foreach (var binding in motion.bindings)
            {
                if (binding == null || (!string.IsNullOrEmpty(binding.targetElementId) && binding.targetElementId != selectedId))
                    continue;

                var captured = binding;
                var row = new VisualElement();
                row.AddToClassList("nexui-motion-binding");
                var trigger = new EnumField(captured.trigger);
                var clip = new ObjectField(DesignerLocalization.T("motion.clip")) { objectType = typeof(UIMotionClip), allowSceneObjects = false, value = captured.clip };
                var reduced = new ObjectField(DesignerLocalization.T("motion.reducedClip")) { objectType = typeof(UIMotionClip), allowSceneObjects = false, value = captured.reducedMotionClip };
                var state = new TextField(DesignerLocalization.T("motion.stateId")) { value = captured.stateId };
                var command = new TextField(DesignerLocalization.T("motion.commandId")) { value = captured.commandId };
                var remove = new Button(() => { Context.RemoveMotionBinding(captured); RefreshBindings(); }) { text = DesignerLocalization.T("motion.removeBinding") };
                trigger.RegisterValueChangedCallback(evt => Context.UpdateMotionBinding(captured, b => b.trigger = (DesignerMotionTrigger)evt.newValue));
                clip.RegisterValueChangedCallback(evt => Context.UpdateMotionBinding(captured, b => b.clip = evt.newValue as UIMotionClip));
                reduced.RegisterValueChangedCallback(evt => Context.UpdateMotionBinding(captured, b => b.reducedMotionClip = evt.newValue as UIMotionClip));
                state.RegisterValueChangedCallback(evt => Context.UpdateMotionBinding(captured, b => b.stateId = evt.newValue));
                command.RegisterValueChangedCallback(evt => Context.UpdateMotionBinding(captured, b => b.commandId = evt.newValue));
                row.Add(trigger);
                row.Add(clip);
                row.Add(reduced);
                row.Add(state);
                row.Add(command);
                row.Add(remove);
                _bindings.Add(row);
            }
        }

        private void RefreshGraphButton()
        {
            var hasPreset = Context.SelectedMetadata?.motion.motionPreset != null;
            _openGraph.SetEnabled(hasPreset);
            _openGraph.style.display = hasPreset ? DisplayStyle.Flex : DisplayStyle.None;
            _noPresetHelp.style.display = hasPreset ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
