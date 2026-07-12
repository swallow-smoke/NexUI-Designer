using System;
using emiteat.NexUI.Designer.Editor.Components;
using UnityEditor;

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

        public bool IsInteractive => InteractionMode == DesignerInteractionMode.Interactive;

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
