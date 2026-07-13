using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.ScreenFlow
{
    /// <summary>
    /// Visual editor for a <see cref="DesignerScreenFlowAsset"/> (brief §30/§33). Screens are nodes,
    /// navigation transitions are real GraphView edges; each edge maps to a
    /// <see cref="DesignerScreenFlowTransition"/> by output-port reference. Every structural edit is
    /// undo-recorded against the asset, matching the Motion Graph v2 editor's idiom.
    /// </summary>
    public sealed class ScreenFlowView : GraphView
    {
        private DesignerScreenFlowAsset _asset;

        public event Action GraphEdited;

        public ScreenFlowView()
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

        private List<DesignerScreenFlowNode> Nodes => _asset != null ? _asset.nodes : null;

        public void Populate(DesignerScreenFlowAsset asset)
        {
            DeleteElements(graphElements.ToList());
            _asset = asset;
            if (_asset == null) return;

            var views = new Dictionary<string, ScreenFlowNodeView>();
            foreach (var node in _asset.nodes)
            {
                if (node == null || string.IsNullOrEmpty(node.id)) continue;
                views[node.id] = CreateNodeView(node);
            }

            foreach (var node in _asset.nodes)
            {
                if (node == null || !views.TryGetValue(node.id ?? string.Empty, out var sourceView)) continue;
                foreach (var transition in node.transitions)
                {
                    if (transition == null || string.IsNullOrEmpty(transition.toNodeId)) continue;
                    if (!views.TryGetValue(transition.toNodeId, out var targetView) || targetView.FlowInput == null) continue;
                    var port = sourceView.TransitionPorts.FirstOrDefault(kv => kv.Value == transition).Key;
                    if (port != null)
                        AddElement(port.ConnectTo(targetView.FlowInput));
                }
            }

            GraphEdited?.Invoke();
        }

        private ScreenFlowNodeView CreateNodeView(DesignerScreenFlowNode node)
        {
            var view = new ScreenFlowNodeView(this, node);
            AddElement(view);
            return view;
        }

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

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_asset != null)
            {
                evt.menu.AppendAction("Add Screen Node", action =>
                {
                    var pos = contentViewContainer.WorldToLocal(action.eventInfo.mousePosition);
                    CreateNode(pos);
                });
                evt.menu.AppendAction("Auto Layout", _ => AutoLayout());
                evt.menu.AppendSeparator();
            }
            base.BuildContextualMenu(evt);
        }

        public void AddNodeAtCenter() => CreateNode(contentViewContainer.WorldToLocal(layout.center));

        private void CreateNode(Vector2 position)
        {
            if (_asset == null) return;
            RecordUndo("Add Screen Flow Node");
            var node = new DesignerScreenFlowNode { id = GenerateUniqueId(), position = position };
            Nodes.Add(node);
            if (string.IsNullOrEmpty(_asset.entryNodeId)) _asset.entryNodeId = node.id; // first node = entry
            CreateNodeView(node);
            MarkEdited();
        }

        // ---- Change handling ------------------------------------------------------

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            var structural = false;

            if (change.edgesToCreate != null && change.edgesToCreate.Count > 0)
            {
                RecordUndo("Connect Screen Flow");
                foreach (var edge in change.edgesToCreate)
                    ApplyEdge(edge, connect: true);
                structural = true;
            }

            if (change.elementsToRemove != null && change.elementsToRemove.Count > 0)
            {
                RecordUndo("Edit Screen Flow");
                foreach (var element in change.elementsToRemove)
                    if (element is Edge edge)
                        ApplyEdge(edge, connect: false);
                foreach (var element in change.elementsToRemove)
                    if (element is ScreenFlowNodeView nodeView)
                        RemoveNode(nodeView.Model);
                structural = true;
            }

            if (change.movedElements != null && change.movedElements.Count > 0)
            {
                RecordUndo("Move Screen Flow Node");
                SyncPositions();
                structural = true;
            }

            if (structural)
                MarkEdited();
            return change;
        }

        private void ApplyEdge(Edge edge, bool connect)
        {
            if (edge.output?.node is not ScreenFlowNodeView sourceView) return;
            if (!sourceView.TransitionPorts.TryGetValue(edge.output, out var transition)) return;
            var targetView = edge.input?.node as ScreenFlowNodeView;
            transition.toNodeId = connect && targetView != null ? targetView.Model.id : string.Empty;
        }

        private void RemoveNode(DesignerScreenFlowNode node)
        {
            if (Nodes == null) return;
            Nodes.Remove(node);
            if (string.IsNullOrEmpty(node.id)) return;

            foreach (var other in Nodes)
                foreach (var transition in other.transitions)
                    if (transition != null && transition.toNodeId == node.id)
                        transition.toNodeId = string.Empty;

            if (_asset.entryNodeId == node.id)
                _asset.entryNodeId = Nodes.FirstOrDefault()?.id ?? string.Empty;
        }

        // ---- Transition editing ---------------------------------------------------

        public void AddTransition(ScreenFlowNodeView view)
        {
            RecordUndo("Add Screen Flow Transition");
            view.Model.transitions.Add(new DesignerScreenFlowTransition(GenerateTransitionId(), ScreenFlowTransitionKind.Push));
            ReplaceNodeView(view);
            MarkEdited();
        }

        public void RemoveTransition(ScreenFlowNodeView view, DesignerScreenFlowTransition transition)
        {
            RecordUndo("Remove Screen Flow Transition");
            view.Model.transitions.Remove(transition);
            ReplaceNodeView(view);
            MarkEdited();
        }

        public void SetScreenId(DesignerScreenFlowNode node, string screenId)
        {
            RecordUndo("Edit Screen Flow Node");
            node.screenId = screenId;
            MarkEdited();
        }

        public void SetNodeKind(DesignerScreenFlowNode node, ScreenFlowNodeKind kind)
        {
            RecordUndo("Edit Screen Flow Node");
            node.kind = kind;
            MarkEdited();
        }

        public void SetTransitionKind(DesignerScreenFlowTransition transition, ScreenFlowTransitionKind kind)
        {
            RecordUndo("Edit Screen Flow Transition");
            transition.kind = kind;
            MarkEdited();
        }

        public void SetTransitionGuard(DesignerScreenFlowTransition transition, string guardKey)
        {
            RecordUndo("Edit Screen Flow Transition");
            transition.guardKey = guardKey;
            MarkEdited();
        }

        public void SetEntryNode(string nodeId)
        {
            if (_asset == null) return;
            RecordUndo("Set Screen Flow Entry");
            _asset.entryNodeId = nodeId;
            MarkEdited();
        }

        public string SelectedNodeId()
            => selection.OfType<ScreenFlowNodeView>().FirstOrDefault()?.Model.id;

        /// <summary>Rebuilds one node's view in place after its transition list changed, reconnecting
        /// both its own surviving outgoing edges and any incoming edges that still target it.</summary>
        private void ReplaceNodeView(ScreenFlowNodeView oldView)
        {
            var node = oldView.Model;

            foreach (var edge in edges.ToList().Where(e => e.output?.node == oldView || e.input?.node == oldView))
                RemoveElement(edge);
            RemoveElement(oldView);

            var newView = CreateNodeView(node);
            var allViews = graphElements.ToList().OfType<ScreenFlowNodeView>().ToList();

            // Outgoing.
            foreach (var kv in newView.TransitionPorts)
            {
                var transition = kv.Value;
                if (string.IsNullOrEmpty(transition.toNodeId)) continue;
                var targetView = allViews.FirstOrDefault(v => v.Model.id == transition.toNodeId);
                if (targetView?.FlowInput != null)
                    AddElement(kv.Key.ConnectTo(targetView.FlowInput));
            }

            // Incoming.
            foreach (var otherView in allViews)
            {
                if (otherView == newView) continue;
                foreach (var kv in otherView.TransitionPorts)
                {
                    if (kv.Value.toNodeId != node.id || newView.FlowInput == null) continue;
                    AddElement(kv.Key.ConnectTo(newView.FlowInput));
                }
            }
        }

        public void AutoLayout()
        {
            if (_asset == null || Nodes == null || Nodes.Count == 0) return;
            RecordUndo("Auto Layout Screen Flow");
            var positions = ScreenFlowLayout.LayeredLayout(Nodes, _asset.entryNodeId);
            foreach (var element in graphElements.ToList())
            {
                if (element is ScreenFlowNodeView view && positions.TryGetValue(view.Model, out var pos))
                {
                    view.Model.position = pos;
                    view.SetPosition(new Rect(pos, view.GetPosition().size));
                }
            }
            MarkEdited();
        }

        // ---- Id / persistence -----------------------------------------------------

        private bool IsIdUnique(string id)
        {
            if (Nodes == null) return true;
            foreach (var node in Nodes)
                if (node.id == id)
                    return false;
            return true;
        }

        private string GenerateUniqueId()
        {
            string id;
            do { id = "screen_" + Guid.NewGuid().ToString("N").Substring(0, 8); }
            while (!IsIdUnique(id));
            return id;
        }

        private static string GenerateTransitionId() => "t_" + Guid.NewGuid().ToString("N").Substring(0, 8);

        private void SyncPositions()
        {
            foreach (var element in graphElements.ToList())
                if (element is ScreenFlowNodeView view)
                    view.Model.position = view.GetPosition().position;
        }

        public void RecordUndo(string name)
        {
            if (_asset != null)
                Undo.RecordObject(_asset, name);
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
            SyncPositions();
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssets();
        }
    }
}
