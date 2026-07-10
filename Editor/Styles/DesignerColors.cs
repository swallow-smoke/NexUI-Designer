using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Styles
{
    /// <summary>
    /// IMGUI-side mirror of the dark palette defined in <c>NexUIDesigner.uss</c>. UI Toolkit
    /// windows (the main designer, the Motion Graph editor) style themselves from the USS file
    /// directly; IMGUI windows (all <see cref="Common.NexUIToolWindow"/> subclasses) have no
    /// stylesheet mechanism, so they draw with these colors instead. Keep both in sync by eye —
    /// values here are transcribed from the same hex literals used in the USS rules noted below.
    /// </summary>
    public static class DesignerColors
    {
        // .nexui-designer-root
        public static readonly Color Background = FromHex("#121317");
        // .nexui-side-panel / .nexui-panel
        public static readonly Color Panel = FromHex("#1a1d22");
        // .nexui-inspector-section
        public static readonly Color PanelAlt = FromHex("#15171b");
        // .nexui-side-panel border-color
        public static readonly Color PanelBorder = FromHex("#2a2e35");
        // .nexui-toolbar background-color
        public static readonly Color Toolbar = FromHex("#17181d");

        // .nexui-designer-root text color
        public static readonly Color TextPrimary = FromHex("#d7dbe2");
        // #PanelTitle / #BrandTitle
        public static readonly Color TextHeading = FromHex("#f5f6f8");
        // #PanelSubtitle / .nexui-viewport-hint
        public static readonly Color TextMuted = FromHex("#8a909b");
        // #SectionTitle
        public static readonly Color Accent = FromHex("#b8a9ff");
        // .nexui-design-element.is-selected / .nexui-zoom-badge text / selection accent
        public static readonly Color AccentSecondary = FromHex("#43e6c2");

        // .nexui-button-primary / .nexui-toolbar border-bottom-color (brand accent)
        public static readonly Color ButtonPrimary = FromHex("#6d5efc");
        // .nexui-button-secondary
        public static readonly Color ButtonSecondary = FromHex("#262a31");

        // .nexui-toolbar-status.is-ok
        public static readonly Color StatusOkBackground = FromHex("#0f2b21");
        public static readonly Color StatusOkText = FromHex("#4ade80");
        // .nexui-toolbar-status.is-warning
        public static readonly Color StatusWarningBackground = FromHex("#2e2410");
        public static readonly Color StatusWarningText = FromHex("#fbbf24");
        // .nexui-toolbar-status.is-muted
        public static readonly Color StatusMutedBackground = FromHex("#23262c");
        public static readonly Color StatusMutedText = FromHex("#9aa1ab");
        // not yet mirrored by a USS rule (no .is-danger class defined there) - kept for IMGUI
        // windows that need a destructive/error accent distinct from the warning color above.
        public static readonly Color StatusDangerBackground = FromHex("#2c1416");
        public static readonly Color StatusDangerText = FromHex("#f87171");

        private static Color FromHex(string hex) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.magenta;
    }
}
