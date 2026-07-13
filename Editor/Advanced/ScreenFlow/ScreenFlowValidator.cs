using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.ScreenFlow
{
    /// <summary>A screen-flow validation finding (brief §42.x navigation category).</summary>
    public struct ScreenFlowIssue
    {
        public enum Severity { Info, Warning, Error }
        public Severity Level;
        public string NodeId;
        public string Message;
    }

    /// <summary>
    /// Pure validation of a <see cref="DesignerScreenFlowAsset"/>: broken/dangling transitions, missing
    /// entry, empty screen references, and nodes unreachable from the entry node. No Unity dependency,
    /// so the whole ruleset is unit-tested in EditMode.
    /// </summary>
    public static class ScreenFlowValidator
    {
        public static List<ScreenFlowIssue> Validate(DesignerScreenFlowAsset asset)
        {
            var issues = new List<ScreenFlowIssue>();
            if (asset == null) return issues;

            var byId = new Dictionary<string, DesignerScreenFlowNode>();
            foreach (var node in asset.nodes)
            {
                if (node == null || string.IsNullOrEmpty(node.id)) continue;
                if (byId.ContainsKey(node.id))
                    issues.Add(Error(node.id, $"Duplicate node id '{node.id}'."));
                else
                    byId[node.id] = node;
            }

            // Entry.
            if (string.IsNullOrEmpty(asset.entryNodeId))
                issues.Add(Warning(null, "No entry node set."));
            else if (!byId.ContainsKey(asset.entryNodeId))
                issues.Add(Error(null, $"Entry node '{asset.entryNodeId}' does not exist."));

            // Per-node checks.
            foreach (var node in asset.nodes)
            {
                if (node == null || string.IsNullOrEmpty(node.id)) continue;
                if (string.IsNullOrEmpty(node.screenId) && node.kind != ScreenFlowNodeKind.Subflow)
                    issues.Add(Warning(node.id, $"Node '{node.id}' has no screen assigned."));

                foreach (var transition in node.transitions)
                {
                    if (transition == null) continue;
                    if (string.IsNullOrEmpty(transition.toNodeId))
                        issues.Add(Warning(node.id, $"Transition '{transition.kind}' from '{node.id}' is not connected."));
                    else if (!byId.ContainsKey(transition.toNodeId))
                        issues.Add(Error(node.id, $"Transition from '{node.id}' targets missing node '{transition.toNodeId}'."));
                }
            }

            // Reachability from entry.
            if (!string.IsNullOrEmpty(asset.entryNodeId) && byId.ContainsKey(asset.entryNodeId))
            {
                var reachable = ComputeReachable(asset.entryNodeId, byId);
                foreach (var node in asset.nodes)
                {
                    if (node == null || string.IsNullOrEmpty(node.id)) continue;
                    // Persistent HUDs are typically always-on, not reached via a transition - don't flag them.
                    if (node.kind == ScreenFlowNodeKind.PersistentHUD) continue;
                    if (!reachable.Contains(node.id))
                        issues.Add(Warning(node.id, $"Node '{node.id}' is unreachable from the entry node."));
                }
            }

            return issues;
        }

        public static HashSet<string> ComputeReachable(string entryNodeId, IReadOnlyDictionary<string, DesignerScreenFlowNode> byId)
        {
            var reachable = new HashSet<string>();
            if (string.IsNullOrEmpty(entryNodeId) || byId == null || !byId.ContainsKey(entryNodeId))
                return reachable;

            var stack = new Stack<string>();
            stack.Push(entryNodeId);
            while (stack.Count > 0)
            {
                var id = stack.Pop();
                if (!reachable.Add(id)) continue;
                if (!byId.TryGetValue(id, out var node)) continue;
                foreach (var transition in node.transitions)
                {
                    if (transition == null || string.IsNullOrEmpty(transition.toNodeId)) continue;
                    if (!reachable.Contains(transition.toNodeId))
                        stack.Push(transition.toNodeId);
                }
            }
            return reachable;
        }

        private static ScreenFlowIssue Info(string nodeId, string message) => new ScreenFlowIssue { Level = ScreenFlowIssue.Severity.Info, NodeId = nodeId, Message = message };
        private static ScreenFlowIssue Warning(string nodeId, string message) => new ScreenFlowIssue { Level = ScreenFlowIssue.Severity.Warning, NodeId = nodeId, Message = message };
        private static ScreenFlowIssue Error(string nodeId, string message) => new ScreenFlowIssue { Level = ScreenFlowIssue.Severity.Error, NodeId = nodeId, Message = message };
    }
}
