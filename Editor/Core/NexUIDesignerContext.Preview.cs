using System;
using emiteat.NexUI.Designer.Editor.Components;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Interactive-preview surface of the context: the forced preview state (a per-screen preview
    /// setting, never written to element data), the Design/Interactive interaction mode, and the
    /// Preview Log. Command "execution" here is always a safe simulation that only writes to the log.
    /// </summary>
    public sealed partial class NexUIDesignerContext
    {
        private const string PreviewPrefPrefix = "NexUI.Designer.Preview.";

        /// <summary>The state the canvas renders selected/all components in. Not persisted on elements.</summary>
        public DesignerComponentState ForcedPreviewState { get; private set; } = DesignerComponentState.Normal;
        public DesignerInteractionMode InteractionMode { get; private set; } = DesignerInteractionMode.Design;
        public DesignerPreviewLog PreviewLog { get; } = new DesignerPreviewLog();

        public event Action PreviewSettingsChanged;

        /// <summary>
        /// The clip currently open in <c>MotionClipEditorWindow</c> (if any) and its scrub time, kept
        /// here so <c>Viewport.MotionPathOverlay</c>/<c>OnionSkinOverlay</c> can draw without the
        /// Viewport needing a direct reference to whichever Motion Clip Editor window is open.
        /// </summary>
        public UIMotionClip ActiveMotionClip { get; private set; }
        public float ActiveMotionClipTime { get; private set; }
        public bool ShowMotionPath { get; private set; } = true;
        public bool ShowOnionSkin { get; private set; }
        public bool ShowFocusNav { get; private set; }

        public event Action ActiveMotionClipChanged;

        public void SetActiveMotionClip(UIMotionClip clip, float time)
        {
            ActiveMotionClip = clip;
            ActiveMotionClipTime = time;
            ActiveMotionClipChanged?.Invoke();
        }

        public void SetActiveMotionClipTime(float time)
        {
            if (Mathf.Approximately(ActiveMotionClipTime, time)) return;
            ActiveMotionClipTime = time;
            ActiveMotionClipChanged?.Invoke();
        }

        public void SetShowMotionPath(bool show)
        {
            if (ShowMotionPath == show) return;
            ShowMotionPath = show;
            PreviewSettingsChanged?.Invoke();
        }

        public void SetShowOnionSkin(bool show)
        {
            if (ShowOnionSkin == show) return;
            ShowOnionSkin = show;
            PreviewSettingsChanged?.Invoke();
        }

        public void SetShowFocusNav(bool show)
        {
            if (ShowFocusNav == show) return;
            ShowFocusNav = show;
            PreviewSettingsChanged?.Invoke();
        }

        public bool IsInteractive => InteractionMode == DesignerInteractionMode.Interactive;

        /// <summary>
        /// Requests a canvas repaint after an external tool has changed element preview values in
        /// place (e.g. Scenario timeline scrubbing) without going through Undo/dirty. Deliberately
        /// does not mark metadata dirty or record Undo — transient preview mutation only. Callers that
        /// mutate preview values this way are responsible for restoring the authored values afterward.
        /// </summary>
        public void NotifyPreviewValuesChanged() => CanvasChanged?.Invoke();

        public void SetForcedPreviewState(DesignerComponentState state)
        {
            if (ForcedPreviewState == state) return;
            ForcedPreviewState = state;
            PreviewLog.Log(DesignerPreviewLogKind.State, null, "Forced preview state → " + state);
            PreviewSettingsChanged?.Invoke();
            CanvasChanged?.Invoke();
        }

        public void SetInteractionMode(DesignerInteractionMode mode)
        {
            if (InteractionMode == mode) return;
            InteractionMode = mode;
            if (mode == DesignerInteractionMode.Design)
                ForcedPreviewState = DesignerComponentState.Normal; // leave interactive states behind
            PreviewLog.Log(DesignerPreviewLogKind.Info, null, "Interaction mode → " + mode);
            PreviewSettingsChanged?.Invoke();
            UIStateChanged?.Invoke();
            CanvasChanged?.Invoke();
        }

        public void ToggleInteractionMode()
            => SetInteractionMode(IsInteractive ? DesignerInteractionMode.Design : DesignerInteractionMode.Interactive);

        /// <summary>
        /// Simulates the primary interaction for an element in Interactive mode: resolves its
        /// command key (or a synthetic "activate") and its supported event, and records both to the
        /// Preview Log. It never dispatches a real <c>UIStateStore</c>/runtime command - Editor
        /// preview must not run destructive game logic. Returns false if the element isn't
        /// interactive per its descriptor.
        /// </summary>
        public bool SimulatePrimaryInteraction(DesignerElementMetadata element)
        {
            if (element == null) return false;
            var d = DesignerComponentRegistry.Get(element.elementType);
            if (!d.IsInteractive) return false;

            var commandKey = element.binding != null ? element.binding.commandKey : null;
            var evt = d.SupportedEvents.Count > 0 ? d.SupportedEvents[0] : "Activate";
            var payload = BuildSimPayload(element, d);

            if (!string.IsNullOrEmpty(commandKey))
                PreviewLog.Log(DesignerPreviewLogKind.Command, element.elementId,
                    $"{evt} → command '{commandKey}' (simulated)", payload);
            else
                PreviewLog.Log(DesignerPreviewLogKind.Command, element.elementId,
                    $"{evt} (no command key bound; simulated)", payload);
            return true;
        }

        public void LogPreviewInteraction(DesignerElementMetadata element, string message)
            => PreviewLog.Log(DesignerPreviewLogKind.Interaction, element != null ? element.elementId : null, message);

        private static string BuildSimPayload(DesignerElementMetadata element, DesignerComponentDescriptor d)
        {
            switch (element.elementType)
            {
                case "Slot":
                    return $"{{ \"slotId\": \"{element.elementId}\", \"itemKey\": \"{Safe(element.binding?.valueKey)}\" }}";
                case "ProgressBar":
                case "StatBar":
                case "RadialFill":
                    return $"{{ \"value\": {element.previewValue:0.##} }}";
                default:
                    return string.IsNullOrEmpty(element.binding?.valueKey) ? null
                        : $"{{ \"value\": \"{element.binding.valueKey}\" }}";
            }
        }

        private static string Safe(string s) => string.IsNullOrEmpty(s) ? "" : s;
    }
}
