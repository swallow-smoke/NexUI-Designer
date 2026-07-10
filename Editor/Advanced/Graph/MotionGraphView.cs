using System;
using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Motion;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Graph
{
    /// <summary>
    /// Visual editor for a <see cref="UIMotionPreset"/>'s <see cref="UIMotionGraph"/>. Each
    /// <see cref="UIMotionGraph.Node"/> becomes a <see cref="MotionGraphNodeView"/>; connecting
    /// an output port to an input port records a dependency (target depends on source).
    /// Every mutation is undo-recorded against the preset asset (matching the metadata inspectors'
    /// per-edit <c>Undo.RecordObject</c> idiom); an explicit "Save Now" forces the asset to disk.
    /// </summary>
    public sealed class MotionGraphView : GraphView
    {
        private readonly List<UIMotionGraph.Node> _model = new List<UIMotionGraph.Node>();
        private UIMotionPreset _preset;

        /// <summary>Raised after any edit so hosts (e.g. the timeline preview) can refresh.</summary>
        public event Action GraphEdited;

        public MotionGraphView()
        {
            style.flexGrow = 1f;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            graphViewChanged = OnGraphViewChanged;
        }

        public void Populate(UIMotionPreset preset)
        {
            // Clear existing view elements.
            DeleteElements(graphElements.ToList());
            _model.Clear();
            _preset = preset;
            if (_preset == null) return;

            if (_preset.graph == null)
                _preset.graph = new UIMotionGraph();
            if (_preset.graph.nodes != null)
            {
                foreach (var node in _preset.graph.nodes)
                {
                    if (node == null) continue;
                    if (node.dependencies == null) node.dependencies = Array.Empty<string>();
                    _model.Add(node);
                }
            }

            if (_model.Count == 0)
                CreateDefaultStartEndNodes();

            var views = new Dictionary<string, MotionGraphNodeView>();
            foreach (var node in _model)
            {
                var view = CreateNodeView(node);
                if (!string.IsNullOrEmpty(node.id))
                    views[node.id] = view;
            }

            // Rebuild edges from dependencies: dep -> node means node depends on dep.
            foreach (var node in _model)
            {
                if (node.dependencies == null) continue;
                if (!views.TryGetValue(node.id ?? string.Empty, out var targetView)) continue;
                foreach (var depId in node.dependencies)
                {
                    if (string.IsNullOrEmpty(depId)) continue;
                    if (!views.TryGetValue(depId, out var sourceView)) continue;
                    var edge = sourceView.Output.ConnectTo(targetView.Input);
                    AddElement(edge);
                }
            }

            GraphEdited?.Invoke();
        }

        private MotionGraphNodeView CreateNodeView(UIMotionGraph.Node node)
        {
            var view = new MotionGraphNodeView(this, node);
            AddElement(view);
            return view;
        }

        /// <summary>
        /// Seeds a brand-new (empty) graph with a "start" node and an "end" node depending on
        /// it, so every motion graph opens with a visible beginning and end to build between.
        /// Both use a zero-duration, no-op step (opacity 1 -> 1) so they don't affect playback
        /// until the user repurposes or removes them.
        /// </summary>
        private void CreateDefaultStartEndNodes()
        {
            UIMotionStep NoOpStep() => new UIMotionStep
            {
                property = UIMotionProperty.Opacity,
                from = 1f,
                to = 1f,
                duration = 0f,
                delay = 0f,
                easing = UIMotionEasing.Linear
            };

            var start = new UIMotionGraph.Node
            {
                id = "start",
                dependencies = Array.Empty<string>(),
                editorPosition = new Vector2(0f, 0f),
                step = NoOpStep()
            };
            var end = new UIMotionGraph.Node
            {
                id = "end",
                dependencies = new[] { "start" },
                editorPosition = new Vector2(320f, 0f),
                step = NoOpStep()
            };

            _model.Add(start);
            _model.Add(end);
            _preset.graph.nodes = _model.ToArray();
            EditorUtility.SetDirty(_preset);
        }

        // ---- Port compatibility ------------------------------------------------

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatible = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort == port) return;
                if (startPort.node == port.node) return;          // no self connection
                if (startPort.direction == port.direction) return; // only input<->output
                compatible.Add(port);
            });
            return compatible;
        }

        // ---- Contextual menu ---------------------------------------------------

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (_preset != null)
            {
                evt.menu.AppendAction("Add Motion Node", action =>
                {
                    var pos = contentViewContainer.WorldToLocal(action.eventInfo.mousePosition);
                    CreateNode(pos);
                });

                if (evt.target is MotionGraphNodeView nodeView)
                    evt.menu.AppendAction("Duplicate Node", _ => DuplicateNode(nodeView.Model));

                evt.menu.AppendAction("Auto Layout", _ => AutoLayout());
                evt.menu.AppendSeparator();
            }
            base.BuildContextualMenu(evt);
        }

        public void AddNodeAtCenter()
        {
            var center = contentViewContainer.WorldToLocal(layout.center);
            CreateNode(center);
        }

        /// <summary>Duplicates a node's step (fresh id, no dependencies) near the source node.</summary>
        public void DuplicateNode(UIMotionGraph.Node source)
        {
            if (_preset == null || source == null) return;
            RecordUndo("Duplicate Motion Node");
            var node = new UIMotionGraph.Node
            {
                id = GenerateUniqueId(),
                dependencies = Array.Empty<string>(),
                editorPosition = source.editorPosition + new Vector2(40f, 40f),
                step = new UIMotionStep
                {
                    property = source.step.property,
                    from = source.step.from,
                    to = source.step.to,
                    duration = source.step.duration,
                    delay = source.step.delay,
                    easing = source.step.easing
                }
            };
            _model.Add(node);
            CommitStructure();
            CreateNodeView(node);
            MarkEdited();
        }

        /// <summary>Re-arranges all nodes by dependency depth (see <see cref="GraphLayoutUtility.LayeredLayout"/>).</summary>
        public void AutoLayout()
        {
            if (_preset == null || _model.Count == 0) return;
            RecordUndo("Auto Layout Motion Graph");
            var positions = GraphLayoutUtility.LayeredLayout(_model);
            foreach (var element in graphElements.ToList())
            {
                if (element is MotionGraphNodeView view && positions.TryGetValue(view.Model, out var pos))
                {
                    view.Model.editorPosition = pos;
                    var size = view.GetPosition().size;
                    view.SetPosition(new Rect(pos, size));
                }
            }
            CommitStructure();
            MarkEdited();
        }

        private void CreateNode(Vector2 position)
        {
            if (_preset == null) return;
            RecordUndo("Add Motion Node");
            var node = new UIMotionGraph.Node
            {
                id = GenerateUniqueId(),
                dependencies = Array.Empty<string>(),
                editorPosition = position,
                step = new UIMotionStep
                {
                    property = UIMotionProperty.Opacity,
                    from = 0f,
                    to = 1f,
                    duration = 0.3f,
                    delay = 0f,
                    easing = UIMotionEasing.EaseInOut
                }
            };
            _model.Add(node);
            CommitStructure();
            CreateNodeView(node);
            MarkEdited();
        }

        // ---- Change handling ---------------------------------------------------

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            var structural = false;

            if (change.edgesToCreate != null && change.edgesToCreate.Count > 0)
            {
                RecordUndo("Connect Motion Nodes");
                foreach (var edge in change.edgesToCreate)
                    ApplyEdge(edge, connect: true);
                structural = true;
            }

            if (change.elementsToRemove != null && change.elementsToRemove.Count > 0)
            {
                RecordUndo("Edit Motion Graph");
                // Remove edges first (unlink dependencies), then nodes.
                foreach (var element in change.elementsToRemove)
                    if (element is Edge edge)
                        ApplyEdge(edge, connect: false);
                foreach (var element in change.elementsToRemove)
                    if (element is MotionGraphNodeView nodeView)
                        RemoveNode(nodeView.Model);
                structural = true;
            }

            if (change.movedElements != null && change.movedElements.Count > 0)
            {
                RecordUndo("Move Motion Node");
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
            var source = edge.output?.node as MotionGraphNodeView; // upstream
            var target = edge.input?.node as MotionGraphNodeView;  // depends on source
            if (source == null || target == null) return;
            if (connect) AddDependency(target.Model, source.Model.id);
            else RemoveDependency(target.Model, source.Model.id);
        }

        private void RemoveNode(UIMotionGraph.Node node)
        {
            _model.Remove(node);
            if (string.IsNullOrEmpty(node.id)) return;
            foreach (var other in _model)
                RemoveDependency(other, node.id);
        }

        // ---- Model helpers -----------------------------------------------------

        public void RenameNode(UIMotionGraph.Node node, string newId)
        {
            var oldId = node.id;
            RecordUndo("Rename Motion Node");
            if (!string.IsNullOrEmpty(oldId))
            {
                foreach (var other in _model)
                {
                    if (other.dependencies == null) continue;
                    for (int i = 0; i < other.dependencies.Length; i++)
                        if (other.dependencies[i] == oldId)
                            other.dependencies[i] = newId;
                }
            }
            node.id = newId;
            CommitStructure();
            MarkEdited();
        }

        public bool IsIdUnique(string id, UIMotionGraph.Node except)
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

        private static void AddDependency(UIMotionGraph.Node node, string depId)
        {
            if (string.IsNullOrEmpty(depId)) return;
            var deps = node.dependencies != null ? new List<string>(node.dependencies) : new List<string>();
            if (!deps.Contains(depId)) deps.Add(depId);
            node.dependencies = deps.ToArray();
        }

        private static void RemoveDependency(UIMotionGraph.Node node, string depId)
        {
            if (node.dependencies == null || node.dependencies.Length == 0) return;
            var deps = new List<string>(node.dependencies);
            if (deps.Remove(depId))
                node.dependencies = deps.ToArray();
        }

        private void SyncPositions()
        {
            foreach (var element in graphElements.ToList())
                if (element is MotionGraphNodeView view)
                    view.Model.editorPosition = view.GetPosition().position;
        }

        // ---- Persistence -------------------------------------------------------

        public void RecordUndo(string name)
        {
            if (_preset != null)
                Undo.RecordObject(_preset, name);
        }

        /// <summary>Writes the working node list back into the preset's serialized graph.</summary>
        private void CommitStructure()
        {
            if (_preset == null) return;
            if (_preset.graph == null) _preset.graph = new UIMotionGraph();
            SyncPositions();
            _preset.graph.nodes = _model.ToArray();
        }

        public void MarkEdited()
        {
            if (_preset != null)
                EditorUtility.SetDirty(_preset);
            GraphEdited?.Invoke();
        }

        /// <summary>Commits the current state and forces the asset to disk.</summary>
        public void SaveNow()
        {
            if (_preset == null) return;
            CommitStructure();
            EditorUtility.SetDirty(_preset);
            AssetDatabase.SaveAssets();
        }
    }
}
