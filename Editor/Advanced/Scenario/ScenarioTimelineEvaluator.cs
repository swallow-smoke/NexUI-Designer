using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Scenario
{
    /// <summary>
    /// Pure evaluation of a scenario timeline at a point in time (brief §35.2). For each distinct
    /// binding key, finds the value active at <c>time</c>: numeric keys interpolate linearly between
    /// the two surrounding keyframes, bool/text keys step (hold the previous keyframe's value). The
    /// result is a set of <see cref="DesignerScenarioBinding"/> that feeds the same
    /// <see cref="ScenarioApplyResolver"/> a static scenario uses — so timeline playback drives the
    /// canvas through exactly one, already-tested apply path. No Unity Editor dependency.
    /// </summary>
    public static class ScenarioTimelineEvaluator
    {
        /// <summary>Builds the active binding values at <paramref name="time"/>. Keys with no keyframe
        /// at or before the time (i.e. their first keyframe is later) are omitted until they start.</summary>
        public static List<DesignerScenarioBinding> EvaluateAt(
            IReadOnlyList<DesignerScenarioTimelineKey> keys, float time)
        {
            var result = new List<DesignerScenarioBinding>();
            if (keys == null || keys.Count == 0) return result;

            // Group keyframes by binding key, preserving first-seen order for stable output.
            var order = new List<string>();
            var groups = new Dictionary<string, List<DesignerScenarioTimelineKey>>();
            foreach (var k in keys)
            {
                if (k == null || string.IsNullOrEmpty(k.key)) continue;
                if (!groups.TryGetValue(k.key, out var list))
                {
                    list = new List<DesignerScenarioTimelineKey>();
                    groups[k.key] = list;
                    order.Add(k.key);
                }
                list.Add(k);
            }

            foreach (var key in order)
            {
                var list = groups[key];
                list.Sort((a, b) => a.time.CompareTo(b.time));

                // Before the first keyframe: the key hasn't started yet - skip it.
                if (time < list[0].time) continue;

                var binding = EvaluateKey(list, time);
                if (binding != null) result.Add(binding);
            }

            return result;
        }

        private static DesignerScenarioBinding EvaluateKey(List<DesignerScenarioTimelineKey> sorted, float time)
        {
            // Find the last keyframe at or before `time` (prev) and the next one after it (next).
            int prevIndex = 0;
            for (int i = 0; i < sorted.Count; i++)
            {
                if (sorted[i].time <= time) prevIndex = i;
                else break;
            }

            var prev = sorted[prevIndex];
            var binding = new DesignerScenarioBinding(prev.key, prev.kind)
            {
                boolValue = prev.boolValue,
                numberValue = prev.numberValue,
                textValue = prev.textValue
            };

            // Numeric interpolation between prev and the next keyframe of the same kind.
            if (prev.kind == DesignerScenarioValueKind.Number && prevIndex + 1 < sorted.Count)
            {
                var next = sorted[prevIndex + 1];
                if (next.kind == DesignerScenarioValueKind.Number)
                {
                    var span = next.time - prev.time;
                    if (span > Mathf.Epsilon)
                    {
                        var t = Mathf.Clamp01((time - prev.time) / span);
                        binding.numberValue = Mathf.Lerp(prev.numberValue, next.numberValue, t);
                    }
                }
            }

            return binding;
        }
    }
}
