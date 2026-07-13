using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.ScreenFlow
{
    /// <summary>
    /// One GraphView node bound to a <see cref="DesignerScreenFlowNode"/>. A single flow input; one
    /// output port per outgoing transition, mapped by <see cref="TransitionPorts"/> so the view can
    /// resolve which transition an edge belongs to by port reference (kinds can repeat, so names are
    /// not unique). Screen id and node/transition kinds are edited inline. Mirrors the port/step-row
    /// idiom of <c>MotionGraphV2NodeView</c>.
    /// </summary>
    public sealed class ScreenFlowNodeView : Node
    {
        public DesignerScreenFlowNode Model { get; }
        public Port FlowInput { get; private set; }
        public Dictionary<Port, DesignerScreenFlowTransition> TransitionPorts { get; } = new Dictionary<Port, DesignerScreenFlowTransition>();

        private readonly ScreenFlowView _view;

        public ScreenFlowNodeView(ScreenFlowView view, DesignerScreenFlowNode model)
        {
            _view = view;
            Model = model;
            AddToClassList("nexui-graph-node");
            RefreshTitle();

            FlowInput = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(ScreenFlowSignal));
            FlowInput.portName = "In";
            inputContainer.Add(FlowInput);

            var screenField = new TextField("Screen") { value = model.screenId };
            screenField.RegisterValueChangedCallback(evt => { _view.SetScreenId(Model, evt.newValue); RefreshTitle(); });
            extensionContainer.Add(screenField);

            var kindField = new EnumField("Kind", model.kind);
            kindField.RegisterValueChangedCallback(evt => { _view.SetNodeKind(Model, (ScreenFlowNodeKind)evt.newValue); RefreshTitle(); });
            extensionContainer.Add(kindField);

            foreach (var transition in model.transitions)
                if (transition != null)
                    AddTransitionRow(transition);

            var addButton = new Button(() => _view.AddTransition(this)) { text = "+ Transition" };
            addButton.AddToClassList("nexui-toolbar-button");
            addButton.AddToClassList("nexui-button-secondary");
            extensionContainer.Add(addButton);

            RefreshExpandedState();
            RefreshPorts();
            SetPosition(new Rect(model.position, new Vector2(250f, 220f)));
        }

        public void RefreshTitle()
            => title = $"{Model.kind}: {(string.IsNullOrEmpty(Model.screenId) ? "(no screen)" : Model.screenId)}";

        private void AddTransitionRow(DesignerScreenFlowTransition transition)
        {
            var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ScreenFlowSignal));
            port.portName = transition.kind.ToString();
            outputContainer.Add(port);
            TransitionPorts[port] = transition;

            var row = new VisualElement();
            row.AddToClassList("nexui-inline-row");

            var kindPopup = new EnumField(transition.kind);
            kindPopup.RegisterValueChangedCallback(evt =>
            {
                _view.SetTransitionKind(transition, (ScreenFlowTransitionKind)evt.newValue);
                port.portName = evt.newValue.ToString();
            });
            row.Add(kindPopup);

            var guardField = new TextField { value = transition.guardKey };
            guardField.tooltip = "Guard binding key (optional): the transition is only taken when this bool is true.";
            guardField.RegisterValueChangedCallback(evt => _view.SetTransitionGuard(transition, evt.newValue));
            row.Add(guardField);

            var removeButton = new Button(() => _view.RemoveTransition(this, transition)) { text = "x" };
            removeButton.AddToClassList("nexui-toolbar-button");
            removeButton.AddToClassList("nexui-button-secondary");
            row.Add(removeButton);

            extensionContainer.Add(row);
        }
    }
}
