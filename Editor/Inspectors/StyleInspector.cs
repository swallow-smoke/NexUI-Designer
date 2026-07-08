using emiteat.NexUI.Designer.Editor.Backend;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class StyleInspector : DesignerInspectorBase
    {
        private readonly TextField _id;
        private readonly TextField _displayName;
        private readonly EnumField _type;
        private readonly TextField _text;
        private readonly TextField _classes;
        private readonly ColorField _tint;
        private readonly ColorField _textColor;
        private readonly IntegerField _fontSize;
        private readonly Toggle _hidden;
        private bool _refreshing;

        public StyleInspector(NexUIDesignerContext context) : base(context, "inspector.style")
        {
            _id = new TextField("Element Id");
            _displayName = new TextField("Name");
            _type = new EnumField("Type", DesignerElementType.Panel);
            _text = new TextField("Text");
            _classes = new TextField("Classes");
            _tint = new ColorField("Tint");
            _textColor = new ColorField("Text Color");
            _fontSize = new IntegerField("Font Size");
            _hidden = new Toggle("Hidden");

            Add(_id);
            Add(_displayName);
            Add(_type);
            Add(_text);
            Add(_classes);
            Add(_tint);
            Add(_textColor);
            Add(_fontSize);
            Add(_hidden);

            _id.RegisterValueChangedCallback(evt => Change(e => e.elementId = evt.newValue, "Rename NexUI Element"));
            _displayName.RegisterValueChangedCallback(evt => Change(e => e.displayName = evt.newValue, "Rename NexUI Element Display"));
            _type.RegisterValueChangedCallback(evt => Change(e => e.elementType = evt.newValue.ToString(), "Change NexUI Element Type"));
            _text.RegisterValueChangedCallback(evt => Change(e => e.text = evt.newValue, "Edit NexUI Element Text"));
            _classes.RegisterValueChangedCallback(evt => Change(e =>
            {
                e.classes.Clear();
                foreach (var token in evt.newValue.Split(' '))
                    if (!string.IsNullOrWhiteSpace(token))
                        e.classes.Add(token.Trim());
            }, "Edit NexUI Element Classes"));
            _tint.RegisterValueChangedCallback(evt => Change(e => e.tint = evt.newValue, "Edit NexUI Element Tint"));
            _textColor.RegisterValueChangedCallback(evt => Change(e => e.textColor = evt.newValue, "Edit NexUI Element Text Color"));
            _fontSize.RegisterValueChangedCallback(evt => Change(e => e.fontSize = Mathf.Clamp(evt.newValue, 8, 96), "Edit NexUI Element Font Size"));
            _hidden.RegisterValueChangedCallback(evt => Change(e => e.hiddenInDesigner = evt.newValue, "Toggle NexUI Element Hidden"));

            context.MetadataSelectionChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private void Change(System.Action<DesignerElementMetadata> change, string undoName)
        {
            if (_refreshing) return;
            Context.UpdateSelectedElement(change, undoName);
        }

        private void Refresh()
        {
            _refreshing = true;
            var selected = Context.SelectedMetadata;
            SetEnabled(selected != null);
            if (selected != null)
            {
                _id.SetValueWithoutNotify(selected.elementId);
                _displayName.SetValueWithoutNotify(selected.displayName);
                if (System.Enum.TryParse(selected.elementType, out DesignerElementType parsed))
                    _type.SetValueWithoutNotify(parsed);
                _text.SetValueWithoutNotify(selected.text);
                _classes.SetValueWithoutNotify(string.Join(" ", selected.classes));
                _tint.SetValueWithoutNotify(selected.tint);
                _textColor.SetValueWithoutNotify(selected.textColor);
                _fontSize.SetValueWithoutNotify(selected.fontSize);
                _hidden.SetValueWithoutNotify(selected.hiddenInDesigner);
            }
            _refreshing = false;
        }
    }
}
