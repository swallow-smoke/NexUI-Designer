using System;
using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.MotionGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.GraphV2
{
    /// <summary>
    /// Visual editor for a <see cref="UIMotionGraphAsset"/> (the new typed data-flow/event engine -
    /// see Architecture-Audit.md Phase 5). Flow connections are real GraphView edges; data inputs
    /// are inline constant fields on each node (see <see cref="MotionGraphV2NodeView"/> for why).
    /// Every structural edit is undo-recorded against the asset, matching <c>MotionGraphView</c>'s
    /// established per-edit <c>Undo.RecordObject</c> idiom.
    /// </summary>
    public sealed class MotionGraphV2View : GraphView
    {
        private readonly List<UIGraphNode> _model = new List<UIGraphNode>();
        private UIMotionGraphAsset _asset;

        public event Action GraphEdited;

        public MotionGraphV2View()
        {
            AddToClassList("nexui-graph-surface");

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            graphViewChanged = OnGraphViewChanged;
        }

        public void Populate(UIMotionGraphAsset asset)
        {
            DeleteElements(graphElements.ToList());
            _model.Clear();
            _asset = asset;
            if (_asset == null) return;

            if (_asset.nodes != null)
                foreach (var node in _asset.nodes)
                    if (node != null)
                        _model.Add(node);

            if (_model.Count == 0)
                CreateDefaultEventNode();

            var views = new Dictionary<string, MotionGraphV2NodeView>();
            foreach (var node in _model)
            {
                var view = CreateNodeView(node);
                if (!string.IsNullOrEmpty(node.id))
                    views[node.id] = view;
            }

            foreach (var node in _model)
            {
                if (!views.TryGetValue(node.id ?? string.Empty, out var sourceView)) continue;
                foreach (var output in node.flowOutputs)
                {
                    if (string.IsNullOrEmpty(output.targetNodeId)) continue;
                    if (!views.TryGetValue(output.targetNodeId, out var targetView)) continue;
                    var outputPort = sourceView.FlowOutputs.FirstOrDefault(p => p.portName == output.name);
                    if (outputPort == null || targetView.FlowInput == null) continue;
                    AddElement(outputPort.ConnectTo(targetView.FlowInput));
                }
            }

            GraphEdited?.Invoke();
        }

        private void CreateDefaultEventNode()
        {
            var node = new UIGraphNode { id = GenerateUniqueId(), nodeType = "Event", position = Vector2.zero };
            _model.Add(node);
            _asset.nodes = _model.ToArray();
            _asset.entryPoints = new[] { new UIGraphEntryPoint { eventName = "OnClick", nodeId = node.id } };
            EditorUtility.SetDirty(_asset);
        }

        private MotionGraphV2NodeView CreateNodeView(UIGraphNode node)
        {
            var view = new MotionGraphV2NodeView(this, node);
            AddElement(view);
            return view;
        }

        // ---- Port compatibility ------------------------------------------------

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatible = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if (startPort.node == port.node) return;
                if (startPort.direction == port.direction) return;
                compatible.Add(port);
            });
            return compatible;
        }

        // ---- Contextual menu ----------------------------------------------------

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_asset != null)
            {
                foreach (var descriptor in MotionGraphNodeRegistry.All.Values)
                {
                    var nodeType = descriptor.NodeType;
                    evt.menu.AppendAction($"Add Node/{descriptor.DisplayName}", action =>
                    {
                        var pos = contentViewContainer.WorldToLocal(action.eventInfo.mousePosition);
                        CreateNode(nodeType, pos);
                    });
                }
                evt.menu.AppendAction("Auto Layout", _ => AutoLayout());
                evt.menu.AppendSeparator();
            }
            base.BuildContextualMenu(evt);
        }

        public void AddNodeAtCenter(string nodeType)
        {
            var center = contentViewContainer.WorldToLocal(layout.center);
            CreateNode(nodeType, center);
        }

        private void CreateNode(string nodeType, Vector2 position)
        {
            if (_asset == null) return;
            RecordUndo("Add Motion Graph Node");
            var node = new UIGraphNode { id = GenerateUniqueId(), nodeType = nodeType, position = position };
            _model.Add(node);
            CommitStructure();
            CreateNodeView(node);
            MarkEdited();
        }

        public void AutoLayout()
        {
            if (_asset == null || _model.Count == 0) return;
            RecordUndo("Auto Layout Motion Graph");
            var positions = MotionGraphV2Layout.LayeredLayout(_model, _asset.entryPoints);
            foreach (var element in graphElements.ToList())
            {
                if (element is MotionGraphV2NodeView view && positions.TryGetValue(view.Model, out var pos))
                {
                    view.Model.position = pos;
                    view.SetPosition(new Rect(pos, view.GetPosition().size));
                }
            }
            CommitStructure();
            MarkEdited();
        }

        // ---- Change handling ------------------------------------------------------

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            var structural = false;

            if (change.edgesToCreate != null && change.edgesToCreate.Count > 0)
            {
                RecordUndo("Connect Motion Graph Nodes");
                foreach (var edge in change.edgesToCreate)
                    ApplyEdge(edge, connect: true);
                structural = true;
            }

            if (change.elementsToRemove != null && change.elementsToRemove.Count > 0)
            {
                RecordUndo("Edit Motion Graph");
                foreach (var element in change.elementsToRemove)
                    if (element is Edge edge)
                        ApplyEdge(edge, connect: false);
                foreach (var element in change.elementsToRemove)
                    if (element is MotionGraphV2NodeView nodeView)
                        RemoveNode(nodeView.Model);
                structural = true;
            }

            if (change.movedElements != null && change.movedElements.Count > 0)
            {
                RecordUndo("Move Motion Graph Node");
                SyncPositions();
                structural = true;
            }

            if (structural)
            {
                CommitStructure();
                MarkEdited();
            }
            return change;
        }

        private void ApplyEdge(Edge edge, bool connect)
        {
            var sourceView = edge.output?.node as MotionGraphV2NodeView;
            var targetView = edge.input?.node as MotionGraphV2NodeView;
            if (sourceView == null || targetView == null || edge.output == null) return;
            SetFlowTarget(sourceView.Model, edge.output.portName, connect ? targetView.Model.id : null);
        }

        private static void SetFlowTarget(UIGraphNode node, string outputName, string targetNodeId)
        {
            for (var i = 0; i < node.flowOutputs.Length; i++)
            {
                if (node.flowOutputs[i].name != outputName) continue;
                var output = node.flowOutputs[i];
                output.targetNodeId = targetNodeId;
                node.flowOutputs[i] = output;
                return;
            }
        }

        private void RemoveNode(UIGraphNode node)
        {
            _model.Remove(node);
            if (string.IsNullOrEmpty(node.id)) return;
            foreach (var other in _model)
                for (var i = 0; i < other.flowOutputs.Length; i++)
                    if (other.flowOutputs[i].targetNodeId == node.id)
                    {
                        var output = other.flowOutputs[i];
                        output.targetNodeId = null;
                        other.flowOutputs[i] = output;
                    }

            if (_asset.entryPoints != null)
                _asset.entryPoints = _asset.entryPoints.Where(e => e.nodeId != node.id).ToArray();
        }

        // ---- Node-view callbacks (Sequence/Parallel steps, Event names, constants) -------------

        public void AddStep(MotionGraphV2NodeView view)
        {
            RecordUndo("Add Motion Graph Step");
            var outputs = new List<UIGraphFlowOutput>(view.Model.flowOutputs)
            {
                new UIGraphFlowOutput(view.NextStepName(), null)
            };
            view.Model.flowOutputs = outputs.ToArray();
            CommitStructure();
            ReplaceNodeView(view);
            MarkEdited();
        }

        public void RemoveStep(MotionGraphV2NodeView view, string outputName)
        {
            RecordUndo("Remove Motion Graph Step");
            view.Model.flowOutputs = view.Model.flowOutputs.Where(o => o.name != outputName).ToArray();
            CommitStructure();
            ReplaceNodeView(view);
            MarkEdited();
        }

        /// <summary>Rebuilds one node's view in place (removing its old ports/edges first) after a structural change to its port list.</summary>
        public void ReplaceNodeView(MotionGraphV2NodeView oldView)
        {
            var node = oldView.Model;

            // Rebuild wholesale rather than patching ports in place: remove this node's view edges
            // (the model's flowOutputs already reflect the add/remove that triggered this call, so
            // reconnecting from the model below recreates only the edges that should still exist).
            foreach (var edge in edges.ToList().Where(e => e.output?.node == oldView || e.input?.node == oldView))
                RemoveElement(edge);
            RemoveElement(oldView);

            var newView = CreateNodeView(node);
            var allViews = graphElements.ToList().OfType<MotionGraphV2NodeView>().ToList();

            // Outgoing: reconnect this node's own surviving flow outputs to their targets.
            foreach (var output in node.flowOutputs)
            {
                if (string.IsNullOrEmpty(output.targetNodeId)) continue;
                var targetView = allViews.FirstOrDefault(v => v.Model.id == output.targetNodeId);
                if (targetView == null || targetView.FlowInput == null) continue;
                var outputPort = newView.FlowOutputs.FirstOrDefault(p => p.portName == output.name);
                if (outputPort == null) continue;
                AddElement(outputPort.ConnectTo(targetView.FlowInput));
            }

            // Incoming: other nodes' outputs still target this node's id in the model even though
            // rebuilding this view just dropped the visual edge - reconnect those too, or an
            // Add/Remove Step on this node would silently sever every upstream connection into it.
            foreach (var otherView in allViews)
            {
                if (otherView == newView) continue;
                foreach (var output in otherView.Model.flowOutputs)
                {
                    if (output.targetNodeId != node.id) continue;
                    var outputPort = otherView.FlowOutputs.FirstOrDefault(p => p.portName == output.name);
                    if (outputPort == null || newView.FlowInput == null) continue;
                    AddElement(outputPort.ConnectTo(newView.FlowInput));
                }
            }
        }

        public string GetEventNameForNode(string nodeId)
        {
            if (_asset?.entryPoints == null) return string.Empty;
            foreach (var entry in _asset.entryPoints)
                if (entry.nodeId == nodeId)
                    return entry.eventName ?? string.Empty;
            return string.Empty;
        }

        public void SetEventNameForNode(string nodeId, string eventName)
        {
            RecordUndo("Rename Motion Graph Event");
            var entryPoints = new List<UIGraphEntryPoint>(_asset.entryPoints ?? Array.Empty<UIGraphEntryPoint>());
            var index = entryPoints.FindIndex(e => e.nodeId == nodeId);
            if (index >= 0) entryPoints[index] = new UIGraphEntryPoint { eventName = eventName, nodeId = nodeId };
            else entryPoints.Add(new UIGraphEntryPoint { eventName = eventName, nodeId = nodeId });
            _asset.entryPoints = entryPoints.ToArray();
            MarkEdited();
        }

        public void SetConstantInput(UIGraphNode node, GraphNodeDataInput input, UIGraphValue value)
        {
            RecordUndo("Edit Motion Graph Node Input");
            var source = node.FindInput(input.PortName);
            if (source == null)
            {
                var inputs = new List<UIGraphPortSource>(node.dataInputs)
                {
                    new UIGraphPortSource { portName = input.PortName, kind = UIGraphPortSourceKind.Constant, constant = value }
                };
                node.dataInputs = inputs.ToArray();
            }
            else
            {
                source.kind = UIGraphPortSourceKind.Constant;
                source.constant = value;
            }
            MarkEdited();
        }

        // ---- Id / persistence -----------------------------------------------------

        public bool IsIdUnique(string id, UIGraphNode except)
        {
            foreach (var node in _model)
                if (node != except && node.id == id)
                    return false;
            return true;
        }

        private string GenerateUniqueId()
        {
            string id;
            do { id = "node_" + Guid.NewGuid().ToString("N").Substring(0, 8); }
            while (!IsIdUnique(id, null));
            return id;
        }

        private void SyncPositions()
        {
            foreach (var element in graphElements.ToList())
                if (element is MotionGraphV2NodeView view)
                    view.Model.position = view.GetPosition().position;
        }

        public void RecordUndo(string name)
        {
            if (_asset != null)
                Undo.RecordObject(_asset, name);
        }

        private void CommitStructure()
        {
            if (_asset == null) return;
            SyncPositions();
            _asset.nodes = _model.ToArray();
        }

        public void MarkEdited()
        {
            if (_asset != null)
                EditorUtility.SetDirty(_asset);
            GraphEdited?.Invoke();
        }

        public void SaveNow()
        {
            if (_asset == null) return;
            CommitStructure();
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssets();
        }
    }
}
