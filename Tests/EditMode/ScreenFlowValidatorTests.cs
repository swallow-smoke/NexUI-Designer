using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.ScreenFlow;
using NUnit.Framework;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Pure validation tests for the Screen Flow editor (brief §30/§33/§42): broken and dangling
    /// transitions, unreachable nodes, missing entry, and empty screen references are each flagged at
    /// the right severity.
    /// </summary>
    public sealed class ScreenFlowValidatorTests
    {
        private static DesignerScreenFlowNode Node(string id, string screenId, params DesignerScreenFlowTransition[] transitions)
        {
            return new DesignerScreenFlowNode
            {
                id = id,
                screenId = screenId,
                transitions = transitions.ToList()
            };
        }

        private static DesignerScreenFlowTransition To(string toNodeId, ScreenFlowTransitionKind kind = ScreenFlowTransitionKind.Push)
            => new DesignerScreenFlowTransition("t_" + toNodeId, kind) { toNodeId = toNodeId };

        private static DesignerScreenFlowAsset Asset(string entry, params DesignerScreenFlowNode[] nodes)
        {
            var asset = ScriptableObject.CreateInstance<DesignerScreenFlowAsset>();
            asset.entryNodeId = entry;
            asset.nodes = nodes.ToList();
            return asset;
        }

        [Test]
        public void ValidGraph_HasNoErrors()
        {
            var asset = Asset("menu",
                Node("menu", "MainMenu", To("play")),
                Node("play", "Gameplay"));

            var issues = ScreenFlowValidator.Validate(asset);

            Assert.IsFalse(issues.Any(i => i.Level == ScreenFlowIssue.Severity.Error));
            Assert.IsFalse(issues.Any(i => i.Message.Contains("unreachable")));
        }

        [Test]
        public void BrokenTransition_IsError()
        {
            var asset = Asset("menu", Node("menu", "MainMenu", To("ghost")));

            var issues = ScreenFlowValidator.Validate(asset);

            Assert.IsTrue(issues.Any(i => i.Level == ScreenFlowIssue.Severity.Error && i.Message.Contains("ghost")));
        }

        [Test]
        public void DanglingTransition_IsWarning()
        {
            var asset = Asset("menu", Node("menu", "MainMenu", new DesignerScreenFlowTransition("t1", ScreenFlowTransitionKind.Push)));

            var issues = ScreenFlowValidator.Validate(asset);

            Assert.IsTrue(issues.Any(i => i.Level == ScreenFlowIssue.Severity.Warning && i.Message.Contains("not connected")));
        }

        [Test]
        public void UnreachableNode_IsWarning()
        {
            var asset = Asset("menu",
                Node("menu", "MainMenu"),
                Node("orphan", "Orphan"));

            var issues = ScreenFlowValidator.Validate(asset);

            Assert.IsTrue(issues.Any(i => i.NodeId == "orphan" && i.Message.Contains("unreachable")));
        }

        [Test]
        public void PersistentHud_NotFlaggedUnreachable()
        {
            var hud = Node("hud", "HUD");
            hud.kind = ScreenFlowNodeKind.PersistentHUD;
            var asset = Asset("menu", Node("menu", "MainMenu"), hud);

            var issues = ScreenFlowValidator.Validate(asset);

            Assert.IsFalse(issues.Any(i => i.NodeId == "hud" && i.Message.Contains("unreachable")));
        }

        [Test]
        public void MissingEntry_IsWarning_UnknownEntry_IsError()
        {
            Assert.IsTrue(ScreenFlowValidator.Validate(Asset("", Node("a", "A")))
                .Any(i => i.Level == ScreenFlowIssue.Severity.Warning && i.Message.Contains("entry")));

            Assert.IsTrue(ScreenFlowValidator.Validate(Asset("missing", Node("a", "A")))
                .Any(i => i.Level == ScreenFlowIssue.Severity.Error && i.Message.Contains("does not exist")));
        }

        [Test]
        public void EmptyScreenId_IsWarning_ExceptSubflow()
        {
            var subflow = Node("sub", "");
            subflow.kind = ScreenFlowNodeKind.Subflow;
            var asset = Asset("a", Node("a", ""), subflow);
            asset.nodes[0].transitions.Add(To("sub"));

            var issues = ScreenFlowValidator.Validate(asset);

            Assert.IsTrue(issues.Any(i => i.NodeId == "a" && i.Message.Contains("no screen")));
            Assert.IsFalse(issues.Any(i => i.NodeId == "sub" && i.Message.Contains("no screen")));
        }

        [Test]
        public void Reachability_FollowsTransitionChain()
        {
            var byId = new Dictionary<string, DesignerScreenFlowNode>
            {
                ["a"] = Node("a", "A", To("b")),
                ["b"] = Node("b", "B", To("c")),
                ["c"] = Node("c", "C"),
                ["x"] = Node("x", "X")
            };

            var reachable = ScreenFlowValidator.ComputeReachable("a", byId);

            Assert.IsTrue(reachable.SetEquals(new[] { "a", "b", "c" }));
        }

        [Test]
        public void DuplicateNodeId_IsError()
        {
            var asset = Asset("a", Node("a", "A"), Node("a", "B"));
            Assert.IsTrue(ScreenFlowValidator.Validate(asset).Any(i => i.Level == ScreenFlowIssue.Severity.Error && i.Message.Contains("Duplicate")));
        }
    }
}
