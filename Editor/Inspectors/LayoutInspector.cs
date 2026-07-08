using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    public sealed class LayoutInspector : DesignerInspectorBase
    {
        private readonly Vector2Field _position;
        private readonly Vector2Field _size;
        private readonly Toggle _locked;
        private bool _refreshing;

        public LayoutInspector(NexUIDesignerContext context) : base(context, "inspector.layout")
        {
            _position = new Vector2Field("Position");
            _size = new Vector2Field("Size");
            _locked = new Toggle("Locked");
            Add(_position);
            Add(_size);
            Add(_locked);

            _position.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing || Context.SelectedMetadata == null) return;
                var r = Context.SelectedMetadata.rect;
                r.position = evt.newValue;
                Context.UpdateSelectedRect(r);
            });
            _size.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing || Context.SelectedMetadata == null) return;
                var r = Context.SelectedMetadata.rect;
                r.size = new Vector2(Mathf.Max(24f, evt.newValue.x), Mathf.Max(24f, evt.newValue.y));
                Context.UpdateSelectedRect(r);
            });
            _locked.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                Context.UpdateSelectedElement(e => e.locked = evt.newValue, "Toggle NexUI Element Lock");
            });

            context.MetadataSelectionChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            _refreshing = true;
            var selected = Context.SelectedMetadata;
            SetEnabled(selected != null);
            if (selected != null)
            {
                _position.SetValueWithoutNotify(selected.rect.position);
                _size.SetValueWithoutNotify(selected.rect.size);
                _locked.SetValueWithoutNotify(selected.locked);
            }
            _refreshing = false;
        }
    }
}
