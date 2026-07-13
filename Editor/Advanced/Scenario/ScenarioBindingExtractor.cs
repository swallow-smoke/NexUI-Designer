using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.Scenario
{
    /// <summary>
    /// Pure extraction of an element's current preview values as scenario bindings — the inverse of
    /// <see cref="ScenarioApplyResolver"/>. For each bound channel (value/text/visibility) it emits a
    /// <see cref="DesignerScenarioBinding"/> carrying the element's current preview value. Shared by
    /// scenario Capture and the interaction Recorder so both agree on the mapping, and unit-tested
    /// without Unity.
    /// </summary>
    public static class ScenarioBindingExtractor
    {
        public static List<DesignerScenarioBinding> FromElement(DesignerElementMetadata element)
        {
            var result = new List<DesignerScenarioBinding>();
            if (element?.binding == null) return result;
            var b = element.binding;

            if (!string.IsNullOrEmpty(b.valueKey))
                result.Add(new DesignerScenarioBinding(b.valueKey, DesignerScenarioValueKind.Number)
                { numberValue = element.previewValue });

            if (!string.IsNullOrEmpty(b.textKey))
                result.Add(new DesignerScenarioBinding(b.textKey, DesignerScenarioValueKind.Text)
                { textValue = element.text ?? string.Empty });

            if (!string.IsNullOrEmpty(b.visibilityKey))
                result.Add(new DesignerScenarioBinding(b.visibilityKey, DesignerScenarioValueKind.Bool)
                { boolValue = !element.hiddenInDesigner });

            return result;
        }

        /// <summary>Whether two bindings carry the same value (used to skip redundant timeline keys
        /// when recording repeated no-op element edits).</summary>
        public static bool SameValue(DesignerScenarioBinding a, DesignerScenarioBinding b)
        {
            if (a == null || b == null || a.kind != b.kind) return false;
            switch (a.kind)
            {
                case DesignerScenarioValueKind.Bool: return a.boolValue == b.boolValue;
                case DesignerScenarioValueKind.Number: return a.numberValue == b.numberValue;
                case DesignerScenarioValueKind.Text: return (a.textValue ?? string.Empty) == (b.textValue ?? string.Empty);
                default: return false;
            }
        }
    }
}
