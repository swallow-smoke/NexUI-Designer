using System.Collections.Generic;
using emiteat.NexUI.MotionClip;
using emiteat.NexUI.MotionGraph;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.GraphV2
{
    /// <summary>
    /// One GraphView node bound to a <see cref="UIGraphNode"/>. Ports are built generically from
    /// <see cref="MotionGraphNodeRegistry"/>'s descriptor rather than per-node-type view classes.
    /// Data inputs render as inline constant-value fields, not GraphView data ports/wires - even
    /// though <c>Data.Expression</c> (Phase 6) now produces a real "Result" output, wiring it into
    /// another node's input still goes through <see cref="UIGraphPortSourceKind.NodeOutput"/> set
    /// by hand rather than a drag-connected wire; a proper wireable-data-port UI is future editor
    /// polish, not a Runtime engine gap.
    /// </summary>
    public sealed class MotionGraphV2NodeView : Node
    {
        public UIGraphNode Model { get; }
        public Port FlowInput { get; private set; }
        public List<Port> FlowOutputs { get; } = new List<Port>();

        private readonly MotionGraphV2View _graphView;
        private readonly GraphNodeDescriptor _descriptor;
        private VisualElement _stepsContainer;
        private int _stepCounter;

        public MotionGraphV2NodeView(MotionGraphV2View graphView, UIGraphNode model)
        {
            _graphView = graphView;
            Model = model;
            _descriptor = MotionGraphNodeRegistry.Get(model.nodeType);
            title = $"{_descriptor.DisplayName} ({model.id})";

            if (_descriptor.HasFlowInput)
            {
                FlowInput = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(UIGraphFlowSignal));
                FlowInput.portName = "In";
                inputContainer.Add(FlowInput);
            }

            if (model.nodeType == "Event")
                BuildEventFields();

            if (_descriptor.SupportsDynamicFlowOutputs)
            {
                BuildDynamicFlowOutputs();
                foreach (var outputName in _descriptor.FixedFlowOutputsWithDynamic)
                    AddFlowOutput(outputName);
            }
            else
            {
                foreach (var outputName in _descriptor.FlowOutputs)
                    AddFlowOutput(outputName);
            }

            foreach (var input in _descriptor.DataInputs)
                BuildDataInputField(input);

            RefreshExpandedState();
            RefreshPorts();
            SetPosition(new Rect(model.position, new Vector2(220f, 160f)));
        }

        private void BuildEventFields()
        {
            var eventName = _graphView.GetEventNameForNode(Model.id);
            var field = new TextField("Event Name") { value = eventName };
            field.RegisterValueChangedCallback(evt => _graphView.SetEventNameForNode(Model.id, evt.newValue));
            extensionContainer.Add(field);
        }

        private Port AddFlowOutput(string outputName)
        {
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(UIGraphFlowSignal));
            port.portName = outputName;
            outputContainer.Add(port);
            FlowOutputs.Add(port);
            return port;
        }

        private void BuildDynamicFlowOutputs()
        {
            _stepsContainer = new VisualElement();
            extensionContainer.Add(_stepsContainer);

            foreach (var output in Model.flowOutputs)
                AddStepRow(output.name);

            var addButton = new Button(() => _graphView.AddStep(this)) { text = "+ Step" };
            addButton.AddToClassList("nexui-toolbar-button");
            addButton.AddToClassList("nexui-button-secondary");
            extensionContainer.Add(addButton);
        }

        private void AddStepRow(string outputName)
        {
            _stepCounter++;
            var port = AddFlowOutput(outputName);

            var row = new VisualElement();
            row.AddToClassList("nexui-inline-row");
            var label = new Label(outputName);
            row.Add(label);
            var removeButton = new Button(() => _graphView.RemoveStep(this, outputName)) { text = "x" };
            removeButton.AddToClassList("nexui-toolbar-button");
            removeButton.AddToClassList("nexui-button-secondary");
            row.Add(removeButton);
            _stepsContainer.Add(row);
        }

        public string NextStepName() => $"Step{_stepCounter}";

        private void BuildDataInputField(GraphNodeDataInput input)
        {
            var source = Model.FindInput(input.PortName);
            switch (input.ValueType)
            {
                case UIGraphValueType.Float:
                {
                    var field = new FloatField(input.PortName) { value = source?.constant.floatValue ?? 0f };
                    field.RegisterValueChangedCallback(evt =>
                        _graphView.SetConstantInput(Model, input, UIGraphValue.Float(evt.newValue)));
                    extensionContainer.Add(field);
                    break;
                }
                case UIGraphValueType.Int:
                {
                    var field = new IntegerField(input.PortName) { value = source?.constant.intValue ?? 0 };
                    field.RegisterValueChangedCallback(evt =>
                        _graphView.SetConstantInput(Model, input, UIGraphValue.Int(evt.newValue)));
                    extensionContainer.Add(field);
                    break;
                }
                case UIGraphValueType.Bool:
                {
                    var field = new Toggle(input.PortName) { value = source?.constant.boolValue ?? false };
                    field.RegisterValueChangedCallback(evt =>
                        _graphView.SetConstantInput(Model, input, UIGraphValue.Bool(evt.newValue)));
                    extensionContainer.Add(field);
                    break;
                }
                case UIGraphValueType.String:
                {
                    var field = new TextField(input.PortName) { value = source?.constant.stringValue ?? string.Empty };
                    field.RegisterValueChangedCallback(evt =>
                        _graphView.SetConstantInput(Model, input, UIGraphValue.String(evt.newValue)));
                    extensionContainer.Add(field);
                    break;
                }
                case UIGraphValueType.MotionClip:
                {
                    var field = new ObjectField(input.PortName)
                    { objectType = typeof(UIMotionClip), allowSceneObjects = false, value = source?.constant.motionClipValue };
                    field.RegisterValueChangedCallback(evt =>
                        _graphView.SetConstantInput(Model, input, UIGraphValue.Clip(evt.newValue as UIMotionClip)));
                    extensionContainer.Add(field);
                    break;
                }
                case UIGraphValueType.MotionGraph:
                {
                    var field = new ObjectField(input.PortName)
                    { objectType = typeof(UIMotionGraphAsset), allowSceneObjects = false, value = source?.constant.motionGraphValue };
                    field.RegisterValueChangedCallback(evt =>
                        _graphView.SetConstantInput(Model, input, UIGraphValue.Graph(evt.newValue as UIMotionGraphAsset)));
                    extensionContainer.Add(field);
                    break;
                }
                default:
                {
                    extensionContainer.Add(new Label($"{input.PortName} ({input.ValueType})"));
                    break;
                }
            }
        }
    }
}
