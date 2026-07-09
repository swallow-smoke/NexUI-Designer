using emiteat.NexUI.Designer.Editor.Graph;
using emiteat.NexUI.Designer.Editor.MotionClipEditor;
using emiteat.NexUI.Motion;
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
        private bool _refreshing;

        public MotionInspector(NexUIDesignerContext context) : base(context, "inspector.motion")
        {
            _preset = new ObjectField("Motion Preset") { objectType = typeof(UIMotionPreset), allowSceneObjects = false };
            _motionId = new TextField("Motion Id");
            _initial = new TextField("Initial Variant");
            _animate = new TextField("Animate Variant");
            _exit = new TextField("Exit Variant");
            _hover = new TextField("Hover Variant");
            _pressed = new TextField("Pressed Variant");
            _focus = new TextField("Focus Variant");
            _openGraph = new Button(OpenGraph) { text = "Open Motion Graph" };
            _openClipEditor = new Button(OpenClipEditor) { text = "Open Motion Clip Editor" };
            _noPresetHelp = new HelpBox("Assign a Motion Preset to edit its graph.", HelpBoxMessageType.Info);

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

            context.MetadataSelectionChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
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
            _refreshing = false;
            RefreshGraphButton();
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
