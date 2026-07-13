using System;
using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer
{
    /// <summary>The kind of value a scenario binding overrides, matching the three preview channels
    /// the designer canvas can actually render: numeric (value), text, and boolean (visibility).</summary>
    public enum DesignerScenarioValueKind
    {
        Bool,
        Number,
        Text
    }

    /// <summary>
    /// One mock binding override in a <see cref="DesignerScenarioAsset"/>: a state/binding key plus a
    /// typed value. When applied, every element bound to <see cref="key"/> (via its
    /// <c>binding.valueKey</c>/<c>textKey</c>/<c>visibilityKey</c>) receives this value in its preview
    /// channel, so the canvas shows the same thing the running game would for that state.
    /// </summary>
    [Serializable]
    public sealed class DesignerScenarioBinding
    {
        public string key;
        public DesignerScenarioValueKind kind = DesignerScenarioValueKind.Number;
        public bool boolValue;
        public float numberValue;
        public string textValue = string.Empty;

        public DesignerScenarioBinding() { }

        public DesignerScenarioBinding(string key, DesignerScenarioValueKind kind)
        {
            this.key = key;
            this.kind = kind;
        }
    }

    /// <summary>
    /// One time-keyed value change on a scenario timeline (brief §35.2). At <see cref="time"/> the
    /// binding <see cref="key"/> takes this value. Numeric keys interpolate linearly between
    /// surrounding keyframes of the same key; bool/text keys step (hold the last value until the next).
    /// </summary>
    [Serializable]
    public sealed class DesignerScenarioTimelineKey
    {
        public float time;
        public string key;
        public DesignerScenarioValueKind kind = DesignerScenarioValueKind.Number;
        public bool boolValue;
        public float numberValue;
        public string textValue = string.Empty;

        public DesignerScenarioTimelineKey() { }

        public DesignerScenarioTimelineKey(float time, string key, DesignerScenarioValueKind kind)
        {
            this.time = time;
            this.key = key;
            this.kind = kind;
        }
    }

    /// <summary>
    /// A named preview scenario / mock-state set (brief §28, §35). A standalone asset — like a Motion
    /// Clip — so scenarios can be reused across screens and checked into source control. Applied to
    /// whichever screen is open in the Designer: its <see cref="bindings"/> drive element preview
    /// values, <see cref="forcedState"/> forces a component state, and <see cref="language"/> switches
    /// the designer language. The remaining fields (theme/inputDevice/resolution) are captured and
    /// shown for documentation but are not auto-applied yet (see <c>ScenarioService</c>).
    /// </summary>
    [CreateAssetMenu(menuName = "NexUI/Designer/Scenario", fileName = "NexUIScenario")]
    public sealed class DesignerScenarioAsset : ScriptableObject
    {
        public string scenarioName = "New Scenario";
        [TextArea] public string description = string.Empty;

        /// <summary>Screen this scenario was authored against (informational; scenarios still apply to
        /// whatever screen is open). Empty = any screen.</summary>
        public string screenId = string.Empty;

        public List<DesignerScenarioBinding> bindings = new List<DesignerScenarioBinding>();

        /// <summary>Forced component preview state name (matches <c>DesignerComponentState</c> enum
        /// member names), or empty to leave the state untouched.</summary>
        public string forcedState = string.Empty;

        /// <summary>Designer language to switch to on apply ("Korean"/"English"), or empty to leave it.</summary>
        public string language = string.Empty;

        // ---- Timeline (brief §35.2): time-keyed binding changes ---------------
        public bool useTimeline;
        public float timelineDuration = 5f;
        public bool timelineLoop;
        public List<DesignerScenarioTimelineKey> timelineKeys = new List<DesignerScenarioTimelineKey>();

        // ---- Captured context (documentation only, not auto-applied yet) -----
        public string theme = string.Empty;
        public string inputDevice = string.Empty;
        public Vector2Int resolution = new Vector2Int(1920, 1080);

        public DesignerScenarioBinding FindBinding(string bindingKey)
        {
            if (string.IsNullOrEmpty(bindingKey)) return null;
            for (int i = 0; i < bindings.Count; i++)
                if (bindings[i] != null && bindings[i].key == bindingKey)
                    return bindings[i];
            return null;
        }
    }
}
