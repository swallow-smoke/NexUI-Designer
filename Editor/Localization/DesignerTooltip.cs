using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Localization
{
    /// <summary>
    /// Applies a localized tooltip (from <see cref="DesignerLocalization"/>) to a control, optionally
    /// appending a shortcut hint and/or a "why is this disabled" reason. Uses UI Toolkit's native
    /// <see cref="VisualElement.tooltip"/> (multi-line, OS-rendered) rather than a custom popup —
    /// deliberately simple; a rich custom tooltip surface (images/rich text) is future work, not a stub.
    /// </summary>
    public static class DesignerTooltip
    {
        /// <param name="element">Control to receive the tooltip.</param>
        /// <param name="tooltipKey">Localization key resolved via <see cref="DesignerLocalization.T(string)"/>.</param>
        /// <param name="shortcut">Optional literal shortcut text (not localized — key names are language-agnostic), e.g. "K".</param>
        /// <param name="disabledReason">Optional localized reason shown when the control is currently disabled.</param>
        public static void Set(VisualElement element, string tooltipKey, string shortcut = null, string disabledReason = null)
        {
            var text = DesignerLocalization.T(tooltipKey);

            if (!string.IsNullOrEmpty(shortcut))
                text += $"\n\n{DesignerLocalization.T("tooltip.common.shortcutPrefix")}: {shortcut}";

            if (!string.IsNullOrEmpty(disabledReason))
                text += $"\n\n{DesignerLocalization.T("tooltip.common.disabledPrefix")}: {disabledReason}";

            element.tooltip = text;
        }
    }
}
