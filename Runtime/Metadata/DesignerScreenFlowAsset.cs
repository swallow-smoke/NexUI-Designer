using System;
using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer
{
    /// <summary>Role a screen node plays in a flow (brief §33.1).</summary>
    public enum ScreenFlowNodeKind
    {
        Screen,
        Overlay,
        Modal,
        PersistentHUD,
        Subflow
    }

    /// <summary>How a transition moves between screens (brief §33.2).</summary>
    public enum ScreenFlowTransitionKind
    {
        Push,
        Pop,
        Replace,
        Overlay,
        Modal,
        Return
    }

    /// <summary>
    /// One outgoing transition from a flow node to another (brief §33.2/§33.3). <see cref="toNodeId"/>
    /// empty = dangling (authored but not yet connected). <see cref="guardKey"/> is an optional binding
    /// bool that must be true for the transition to be taken.
    /// </summary>
    [Serializable]
    public sealed class DesignerScreenFlowTransition
    {
        public string id;
        public ScreenFlowTransitionKind kind = ScreenFlowTransitionKind.Push;
        public string toNodeId = string.Empty;
        public string guardKey = string.Empty;
        public string label = string.Empty;

        public DesignerScreenFlowTransition() { }

        public DesignerScreenFlowTransition(string id, ScreenFlowTransitionKind kind)
        {
            this.id = id;
            this.kind = kind;
        }
    }

    /// <summary>One node in a screen flow: a screen (or overlay/modal/HUD/subflow) plus its outgoing
    /// transitions. <see cref="screenId"/> references a <c>UIScreenDefinition.screenId</c>.</summary>
    [Serializable]
    public sealed class DesignerScreenFlowNode
    {
        public string id;
        public string screenId = string.Empty;
        public ScreenFlowNodeKind kind = ScreenFlowNodeKind.Screen;
        public Vector2 position;
        public List<DesignerScreenFlowTransition> transitions = new List<DesignerScreenFlowTransition>();

        public DesignerScreenFlowTransition FindTransition(string transitionId)
        {
            if (string.IsNullOrEmpty(transitionId)) return null;
            for (int i = 0; i < transitions.Count; i++)
                if (transitions[i] != null && transitions[i].id == transitionId)
                    return transitions[i];
            return null;
        }
    }

    /// <summary>
    /// A visual screen-flow graph (brief §30/§33): screens as nodes, navigation as typed transitions
    /// (Push/Pop/Replace/Overlay/Modal/Return). A standalone authoring + validation asset — like a
    /// Motion Graph — independent of the runtime UIManager stack, referencing screens by id so the
    /// runtime package needs no dependency on it.
    /// </summary>
    [CreateAssetMenu(menuName = "NexUI/Designer/Screen Flow", fileName = "NexUIScreenFlow")]
    public sealed class DesignerScreenFlowAsset : ScriptableObject
    {
        public string flowName = "New Screen Flow";
        public string entryNodeId = string.Empty;
        public List<DesignerScreenFlowNode> nodes = new List<DesignerScreenFlowNode>();

        public DesignerScreenFlowNode FindNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId)) return null;
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i] != null && nodes[i].id == nodeId)
                    return nodes[i];
            return null;
        }
    }
}
