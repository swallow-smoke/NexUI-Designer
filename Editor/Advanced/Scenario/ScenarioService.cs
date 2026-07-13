using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Components;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Scenario
{
    /// <summary>Result of applying a scenario, surfaced to the editor window as a status readout.</summary>
    public struct ScenarioApplyReport
    {
        public int ElementsChanged;
        public List<string> Messages;

        public static ScenarioApplyReport Empty => new ScenarioApplyReport { ElementsChanged = 0, Messages = new List<string>() };
    }

    /// <summary>A validation finding for a scenario (mirrors the Info/Warning severity used elsewhere).</summary>
    public struct ScenarioIssue
    {
        public bool IsWarning;
        public string Message;
    }

    /// <summary>
    /// Backend-independent logic for creating, applying, capturing and validating
    /// <see cref="DesignerScenarioAsset"/>s. Apply genuinely mutates the open screen's element preview
    /// channels (undoable, single collapsed step) and the session forced-state/language — it never
    /// runs game commands. Capture reads the current preview state back into a scenario.
    /// </summary>
    public static class ScenarioService
    {
        public static DesignerScenarioAsset CreateAsset()
        {
            var asset = ScriptableObject.CreateInstance<DesignerScenarioAsset>();
            asset.scenarioName = "New Scenario";
            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewScenario.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(asset);
            return asset;
        }

        /// <summary>
        /// Applies a scenario to the open screen: pushes each mock binding value into the matching
        /// element preview channels, forces the component preview state, and switches designer
        /// language. Element edits are collapsed into one Undo step so a single Ctrl+Z reverts the
        /// whole scenario.
        /// </summary>
        public static ScenarioApplyReport Apply(DesignerScenarioAsset scenario, NexUIDesignerContext context)
        {
            var report = ScenarioApplyReport.Empty;
            if (scenario == null || context == null || context.Metadata == null)
            {
                report.Messages.Add("No scenario or open screen.");
                return report;
            }

            var changes = ScenarioApplyResolver.Resolve(context.Metadata.elements, scenario.bindings);
            var changeById = new Dictionary<string, ScenarioElementChange>(changes.Count);
            foreach (var c in changes) changeById[c.ElementId] = c;

            if (changes.Count > 0)
            {
                NexUIDesignerUndo.Group("Apply Scenario", () =>
                {
                    foreach (var element in context.Metadata.elements)
                    {
                        if (element == null || !changeById.TryGetValue(element.elementId, out var change)) continue;
                        context.UpdateElement(element, e =>
                        {
                            if (change.SetPreviewValue) e.previewValue = change.PreviewValue;
                            if (change.SetText) e.text = change.Text;
                            if (change.SetHidden) e.hiddenInDesigner = change.Hidden;
                        }, "Apply Scenario");
                    }
                });
                report.ElementsChanged = changes.Count;
            }

            if (!string.IsNullOrEmpty(scenario.forcedState) &&
                Enum.TryParse<DesignerComponentState>(scenario.forcedState, out var state))
            {
                context.SetForcedPreviewState(state);
                report.Messages.Add($"Forced state → {state}");
            }

            if (!string.IsNullOrEmpty(scenario.language) &&
                Enum.TryParse<DesignerLanguage>(scenario.language, out var language))
            {
                DesignerLocalization.SetLanguage(language);
                report.Messages.Add($"Language → {language}");
            }

            report.Messages.Insert(0, $"{report.ElementsChanged} element(s) updated.");
            return report;
        }

        /// <summary>
        /// Applies the scenario's timeline evaluated at <paramref name="time"/> to the open screen.
        /// Used for timeline scrubbing/playback: the evaluated binding set flows through the same
        /// resolver + element-update path as <see cref="Apply"/>. Element edits are NOT collapsed under
        /// Undo here (playback would flood the Undo stack); scrubbing is a transient preview, and the
        /// caller restores the authored state on stop via a normal <see cref="Apply"/> or Undo.
        /// </summary>
        public static int ApplyAtTime(DesignerScenarioAsset scenario, NexUIDesignerContext context, float time)
        {
            if (scenario == null || context == null || context.Metadata == null) return 0;

            var bindings = ScenarioTimelineEvaluator.EvaluateAt(scenario.timelineKeys, time);
            var changes = ScenarioApplyResolver.Resolve(context.Metadata.elements, bindings);
            if (changes.Count == 0) return 0;

            var changeById = new Dictionary<string, ScenarioElementChange>(changes.Count);
            foreach (var c in changes) changeById[c.ElementId] = c;

            foreach (var element in context.Metadata.elements)
            {
                if (element == null || !changeById.TryGetValue(element.elementId, out var change)) continue;
                // Direct mutation + a single ElementChanged notify per element; no per-scrub Undo step.
                if (change.SetPreviewValue) element.previewValue = change.PreviewValue;
                if (change.SetText) element.text = change.Text;
                if (change.SetHidden) element.hiddenInDesigner = change.Hidden;
            }
            context.NotifyPreviewValuesChanged();
            return changes.Count;
        }

        /// <summary>
        /// Fills <paramref name="scenario"/> from the current preview state of the open screen: one
        /// binding entry per element binding key (value/text/visibility), plus the current forced
        /// state and language. First element to declare a key wins, so duplicate bindings collapse.
        /// </summary>
        public static void Capture(DesignerScenarioAsset scenario, NexUIDesignerContext context)
        {
            if (scenario == null || context == null || context.Metadata == null) return;

            Undo.RecordObject(scenario, "Capture Scenario");
            scenario.bindings.Clear();
            scenario.screenId = context.Metadata.screenId ?? string.Empty;

            var seen = new HashSet<string>();
            foreach (var element in context.Metadata.elements)
            {
                if (element == null || element.binding == null) continue;
                var b = element.binding;

                if (!string.IsNullOrEmpty(b.valueKey) && seen.Add(b.valueKey))
                    scenario.bindings.Add(new DesignerScenarioBinding(b.valueKey, DesignerScenarioValueKind.Number)
                    { numberValue = element.previewValue });

                if (!string.IsNullOrEmpty(b.textKey) && seen.Add(b.textKey))
                    scenario.bindings.Add(new DesignerScenarioBinding(b.textKey, DesignerScenarioValueKind.Text)
                    { textValue = element.text ?? string.Empty });

                if (!string.IsNullOrEmpty(b.visibilityKey) && seen.Add(b.visibilityKey))
                    scenario.bindings.Add(new DesignerScenarioBinding(b.visibilityKey, DesignerScenarioValueKind.Bool)
                    { boolValue = !element.hiddenInDesigner });
            }

            scenario.forcedState = context.ForcedPreviewState == DesignerComponentState.Normal
                ? string.Empty : context.ForcedPreviewState.ToString();
            scenario.language = DesignerLocalization.CurrentLanguage.ToString();

            EditorUtility.SetDirty(scenario);
        }

        /// <summary>Validates a scenario against the open screen: empty keys, and keys that no element
        /// on the current screen actually binds (so an apply would silently do nothing for them).</summary>
        public static List<ScenarioIssue> Validate(DesignerScenarioAsset scenario, NexUIDesignerContext context)
        {
            var issues = new List<ScenarioIssue>();
            if (scenario == null) return issues;

            var boundKeys = CollectBoundKeys(context);
            var seenKeys = new HashSet<string>();

            foreach (var binding in scenario.bindings)
            {
                if (binding == null) continue;
                if (string.IsNullOrEmpty(binding.key))
                {
                    issues.Add(new ScenarioIssue { IsWarning = true, Message = "Empty binding key." });
                    continue;
                }
                if (!seenKeys.Add(binding.key))
                    issues.Add(new ScenarioIssue { IsWarning = false, Message = $"Duplicate key '{binding.key}' (first wins)." });
                if (boundKeys != null && !boundKeys.Contains(binding.key))
                    issues.Add(new ScenarioIssue { IsWarning = true, Message = $"Key '{binding.key}' is not bound by any element on this screen." });
            }

            return issues;
        }

        private static HashSet<string> CollectBoundKeys(NexUIDesignerContext context)
        {
            if (context == null || context.Metadata == null) return null;
            var keys = new HashSet<string>();
            foreach (var element in context.Metadata.elements)
            {
                if (element?.binding == null) continue;
                AddKey(keys, element.binding.valueKey);
                AddKey(keys, element.binding.textKey);
                AddKey(keys, element.binding.visibilityKey);
            }
            return keys;
        }

        private static void AddKey(HashSet<string> keys, string key)
        {
            if (!string.IsNullOrEmpty(key)) keys.Add(key);
        }
    }
}
