using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Motion;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Graph
{
    /// <summary>
    /// One GraphView node bound to a single <see cref="UIMotionGraph.Node"/>. Exposes an input
    /// port ("depends on") and an output port ("feeds"), and inline controls for the underlying
    /// <see cref="UIMotionStep"/>. All edits route back through the owning <see cref="MotionGraphView"/>
    /// so they are undo-tracked against the preset asset.
    /// </summary>
    public sealed class MotionGraphNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public UIMotionGraph.Node Model { get; }
        public Port Input { get; }
        public Port Output { get; }

        private readonly MotionGraphView _graphView;
        private readonly TextField _idField;
        private bool _refreshing;

        public MotionGraphNodeView(MotionGraphView graphView, UIMotionGraph.Node model)
        {
            _graphView = graphView;
            Model = model;
            title = string.IsNullOrEmpty(model.id) ? "node" : model.id;

            Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            Input.portName = "Depends On";
            inputContainer.Add(Input);

            Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            Output.portName = "Feeds";
            outputContainer.Add(Output);

            _idField = new TextField("Id") { value = model.id };
            _idField.RegisterValueChangedCallback(OnIdChanged);
            extensionContainer.Add(_idField);

            var property = new EnumField("Property", model.step.property);
            property.RegisterValueChangedCallback(evt =>
                Edit(() => Model.step.property = (UIMotionProperty)evt.newValue, "Edit Motion Node Property"));
            extensionContainer.Add(property);

            var from = new FloatField("From") { value = model.step.from };
            from.RegisterValueChangedCallback(evt =>
                Edit(() => Model.step.from = evt.newValue, "Edit Motion Node From"));
            extensionContainer.Add(from);

            var to = new FloatField("To") { value = model.step.to };
            to.RegisterValueChangedCallback(evt =>
                Edit(() => Model.step.to = evt.newValue, "Edit Motion Node To"));
            extensionContainer.Add(to);

            var duration = new FloatField("Duration") { value = model.step.duration };
            duration.RegisterValueChangedCallback(evt =>
                Edit(() => Model.step.duration = Mathf.Max(0f, evt.newValue), "Edit Motion Node Duration"));
            extensionContainer.Add(duration);

            var delay = new FloatField("Delay") { value = model.step.delay };
            delay.RegisterValueChangedCallback(evt =>
                Edit(() => Model.step.delay = Mathf.Max(0f, evt.newValue), "Edit Motion Node Delay"));
            extensionContainer.Add(delay);

            var easing = new EnumField("Easing", model.step.easing);
            easing.RegisterValueChangedCallback(evt =>
                Edit(() => Model.step.easing = (UIMotionEasing)evt.newValue, "Edit Motion Node Easing"));
            extensionContainer.Add(easing);

            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(model.editorPosition, new Vector2(220f, 240f)));
        }

        private void OnIdChanged(ChangeEvent<string> evt)
        {
            if (_refreshing) return;
            var newId = evt.newValue;
            if (string.IsNullOrWhiteSpace(newId) || !_graphView.IsIdUnique(newId, Model))
            {
                Debug.LogWarning($"[NexUI Motion Graph] Node id '{newId}' is empty or a duplicate; reverting.");
                _refreshing = true;
                _idField.SetValueWithoutNotify(Model.id);
                _refreshing = false;
                return;
            }
            _graphView.RenameNode(Model, newId);
            title = newId;
        }

        private void Edit(System.Action apply, string undoName)
        {
            if (_refreshing) return;
            _graphView.RecordUndo(undoName);
            apply();
            _graphView.MarkEdited();
        }
    }
}
