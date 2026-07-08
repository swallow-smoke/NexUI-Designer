using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class BindingInspector : DesignerInspectorBase
    {
        private readonly TextField _textKey;
        private readonly TextField _valueKey;
        private readonly TextField _visibilityKey;
        private readonly TextField _classKey;
        private readonly TextField _commandKey;
        private readonly TextField _interactableKey;
        private bool _refreshing;

        public BindingInspector(NexUIDesignerContext context) : base(context, "inspector.binding")
        {
            _textKey = new TextField("Text Key");
            _valueKey = new TextField("Value Key");
            _visibilityKey = new TextField("Visibility Key");
            _classKey = new TextField("Class Key");
            _commandKey = new TextField("Command Key");
            _interactableKey = new TextField("Interactable Key");
            Add(_textKey);
            Add(_valueKey);
            Add(_visibilityKey);
            Add(_classKey);
            Add(_commandKey);
            Add(_interactableKey);

            _textKey.RegisterValueChangedCallback(evt => Change(e => e.binding.textKey = evt.newValue));
            _valueKey.RegisterValueChangedCallback(evt => Change(e => e.binding.valueKey = evt.newValue));
            _visibilityKey.RegisterValueChangedCallback(evt => Change(e => e.binding.visibilityKey = evt.newValue));
            _classKey.RegisterValueChangedCallback(evt => Change(e => e.binding.classKey = evt.newValue));
            _commandKey.RegisterValueChangedCallback(evt => Change(e => e.binding.commandKey = evt.newValue));
            _interactableKey.RegisterValueChangedCallback(evt => Change(e => e.binding.interactableKey = evt.newValue));

            context.MetadataSelectionChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private void Change(System.Action<DesignerElementMetadata> change)
        {
            if (_refreshing) return;
            Context.UpdateSelectedElement(change, "Edit NexUI Element Binding");
        }

        private void Refresh()
        {
            _refreshing = true;
            var selected = Context.SelectedMetadata;
            SetEnabled(selected != null);
            if (selected != null)
            {
                _textKey.SetValueWithoutNotify(selected.binding.textKey);
                _valueKey.SetValueWithoutNotify(selected.binding.valueKey);
                _visibilityKey.SetValueWithoutNotify(selected.binding.visibilityKey);
                _classKey.SetValueWithoutNotify(selected.binding.classKey);
                _commandKey.SetValueWithoutNotify(selected.binding.commandKey);
                _interactableKey.SetValueWithoutNotify(selected.binding.interactableKey);
            }
            _refreshing = false;
        }
    }
}
