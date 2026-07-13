using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.ScreenFlow
{
    /// <summary>Left-to-right layered layout of flow nodes by transition distance from the entry node,
    /// stacking same-depth nodes vertically. Pure; a small screen-flow counterpart to
    /// <c>MotionGraphV2Layout</c>.</summary>
    public static class ScreenFlowLayout
    {
        public static Dictionary<DesignerScreenFlowNode, Vector2> LayeredLayout(
            IReadOnlyList<DesignerScreenFlowNode> nodes, string entryNodeId,
            float columnSpacing = 300f, float rowSpacing = 240f)
        {
            var positions = new Dictionary<DesignerScreenFlowNode, Vector2>();
            if (nodes == null || nodes.Count == 0) return positions;

            var byId = nodes.Where(n => n != null && !string.IsNullOrEmpty(n.id))
                            .GroupBy(n => n.id).ToDictionary(g => g.Key, g => g.First());
            var depth = new Dictionary<DesignerScreenFlowNode, int>();
            var visited = new HashSet<DesignerScreenFlowNode>();
            var queue = new Queue<DesignerScreenFlowNode>();

            if (!string.IsNullOrEmpty(entryNodeId) && byId.TryGetValue(entryNodeId, out var entry) && visited.Add(entry))
            {
                depth[entry] = 0;
                queue.Enqueue(entry);
            }

            foreach (var node in nodes)
            {
                if (node != null && visited.Add(node))
                {
                    depth[node] = 0;
                    queue.Enqueue(node);
                }
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var nextDepth = depth[node] + 1;
                foreach (var transition in node.transitions)
                {
                    if (transition == null || string.IsNullOrEmpty(transition.toNodeId)) continue;
                    if (!byId.TryGetValue(transition.toNodeId, out var target)) continue;
                    if (!depth.TryGetValue(target, out var existing) || nextDepth > existing)
                        depth[target] = nextDepth;
                }
            }

            foreach (var layer in nodes.Where(n => n != null)
                         .GroupBy(n => depth.TryGetValue(n, out var d) ? d : 0)
                         .OrderBy(g => g.Key))
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
