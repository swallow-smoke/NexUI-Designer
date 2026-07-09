using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Motion;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Graph
{
    public static class GraphLayoutUtility
    {
        public static Vector2 PlaceOnCircle(int index, int count, float radius)
        {
            if (count <= 0) return Vector2.zero;
            var angle = Mathf.PI * 2f * index / count;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        /// <summary>
        /// Arranges nodes left-to-right by dependency depth (longest chain from a root node),
        /// stacking nodes that share a depth vertically. Nodes participating in a dependency
        /// cycle fall back to depth 0 rather than looping forever.
        /// </summary>
        public static Dictionary<UIMotionGraph.Node, Vector2> LayeredLayout(
            IReadOnlyList<UIMotionGraph.Node> nodes, float columnSpacing = 260f, float rowSpacing = 280f)
        {
            var positions = new Dictionary<UIMotionGraph.Node, Vector2>();
            if (nodes == null || nodes.Count == 0) return positions;

            var byId = nodes.Where(n => !string.IsNullOrEmpty(n.id)).ToDictionary(n => n.id, n => n);
            var depth = new Dictionary<UIMotionGraph.Node, int>();

            int DepthOf(UIMotionGraph.Node node, HashSet<UIMotionGraph.Node> visiting)
            {
                if (depth.TryGetValue(node, out var cached)) return cached;
                if (!visiting.Add(node)) return 0; // cycle guard
                var best = 0;
                if (node.dependencies != null)
                {
                    foreach (var depId in node.dependencies)
                    {
                        if (string.IsNullOrEmpty(depId)) continue;
                        if (!byId.TryGetValue(depId, out var dep) || dep == node) continue;
                        best = Mathf.Max(best, DepthOf(dep, visiting) + 1);
                    }
                }
                visiting.Remove(node);
                depth[node] = best;
                return best;
            }

            foreach (var node in nodes)
                DepthOf(node, new HashSet<UIMotionGraph.Node>());

            var byDepth = nodes.GroupBy(n => depth[n]).OrderBy(g => g.Key);
            foreach (var layer in byDepth)
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
