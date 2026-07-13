using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.MotionGraph;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.GraphV2
{
    /// <summary>Arranges nodes left-to-right by flow distance from the nearest entry point, stacking same-depth nodes vertically. A new, small equivalent of <c>GraphLayoutUtility.LayeredLayout</c> for <see cref="UIGraphNode"/>'s named-flow-output model rather than adapting that dependency-array-based helper.</summary>
    public static class MotionGraphV2Layout
    {
        public static Dictionary<UIGraphNode, Vector2> LayeredLayout(
            IReadOnlyList<UIGraphNode> nodes, IReadOnlyList<UIGraphEntryPoint> entryPoints,
            float columnSpacing = 280f, float rowSpacing = 220f)
        {
            var positions = new Dictionary<UIGraphNode, Vector2>();
            if (nodes == null || nodes.Count == 0) return positions;

            var byId = nodes.Where(n => !string.IsNullOrEmpty(n.id)).ToDictionary(n => n.id, n => n);
            var depth = new Dictionary<UIGraphNode, int>();
            var visited = new HashSet<UIGraphNode>();
            var queue = new Queue<UIGraphNode>();

            foreach (var entry in entryPoints ?? System.Array.Empty<UIGraphEntryPoint>())
            {
                if (byId.TryGetValue(entry.nodeId, out var node) && visited.Add(node))
                {
                    depth[node] = 0;
                    queue.Enqueue(node);
                }
            }

            // Nodes unreachable from any entry point still need a starting depth so they lay out too.
            foreach (var node in nodes)
            {
                if (visited.Add(node))
                {
                    depth[node] = 0;
                    queue.Enqueue(node);
                }
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var nextDepth = depth[node] + 1;
                foreach (var output in node.flowOutputs)
                {
                    if (string.IsNullOrEmpty(output.targetNodeId) || !byId.TryGetValue(output.targetNodeId, out var target))
                        continue;
                    if (!depth.TryGetValue(target, out var existing) || nextDepth > existing)
                        depth[target] = nextDepth;
                    if (visited.Add(target))
                        queue.Enqueue(target);
                }
            }

            foreach (var layer in nodes.GroupBy(n => depth.TryGetValue(n, out var d) ? d : 0).OrderBy(g => g.Key))
            {
                var row = 0;
                foreach (var node in layer)
                {
                    positions[node] = new Vector2(layer.Key * columnSpacing, row * rowSpacing);
                    row++;
                }
            }

            return positions;
        }
    }
}
