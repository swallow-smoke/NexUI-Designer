using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.FlowSimulator
{
    public enum InteractionFlowStepKind { Click, Command, ScreenOpen, ScreenClose, Focus, Info }

    public sealed class InteractionFlowStep
    {
        public int order;
        public InteractionFlowStepKind kind;
        public string description;
    }

    /// <summary>
    /// Model-based simulation of an interaction: what happens when an element is clicked,
    /// derived from its binding command and motion metadata. Command prefixes
    /// "open:" / "close:" and "ui.back"/"back" are interpreted as screen transitions.
    /// </summary>
    public static class FlowSimulatorService
    {
        public static List<InteractionFlowStep> Simulate(DesignerMetadataAsset asset, string elementId)
        {
            var steps = new List<InteractionFlowStep>();
            var e = asset != null ? asset.Find(elementId) : null;
            if (e == null)
            {
                steps.Add(new InteractionFlowStep { order = 0, kind = InteractionFlowStepKind.Info, description = "element not found" });
                return steps;
            }

            int i = 0;
            steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.Click, description = $"click '{elementId}'" });
            if (e.locked)
                steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.Info, description = "element is locked (not interactable)" });

            string cmd = e.binding != null ? e.binding.commandKey : null;
            if (!string.IsNullOrEmpty(cmd))
            {
                steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.Command, description = $"dispatch command '{cmd}'" });
                if (cmd.StartsWith("open:"))
                    steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.ScreenOpen, description = $"open screen '{cmd.Substring(5)}'" });
                else if (cmd.StartsWith("close:"))
                    steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.ScreenClose, description = $"close screen '{cmd.Substring(6)}'" });
                else if (cmd == "ui.back" || cmd == "back")
                    steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.ScreenClose, description = "close top screen (back)" });
            }
            else
            {
                steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.Info, description = "no command bound" });
            }

            if (e.motion != null && !string.IsNullOrEmpty(e.motion.focusVariant))
                steps.Add(new InteractionFlowStep { order = i++, kind = InteractionFlowStepKind.Focus, description = "focus motion plays" });

            return steps;
        }

        /// <summary>Element ids that participate in interaction (command or interactable binding).</summary>
        public static List<string> InteractiveElementIds(DesignerMetadataAsset asset)
        {
            var ids = new List<string>();
            if (asset == null) return ids;
            foreach (var e in asset.elements)
            {
                if (e == null || e.binding == null) continue;
                if (!string.IsNullOrEmpty(e.binding.commandKey) || !string.IsNullOrEmpty(e.binding.interactableKey))
                    ids.Add(e.elementId);
            }
            return ids;
        }
    }
}
