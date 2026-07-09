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
        public static readonly Color Background = FromHex("#171a1f");
        // .nexui-side-panel / .nexui-panel
        public static readonly Color Panel = FromHex("#202731");
        // .nexui-inspector-section
        public static readonly Color PanelAlt = FromHex("#1b222b");
        // .nexui-side-panel border-color
        public static readonly Color PanelBorder = FromHex("#2d3744");
        // .nexui-toolbar background-color
        public static readonly Color Toolbar = FromHex("#1f2630");

        // .nexui-designer-root text color
        public static readonly Color TextPrimary = FromHex("#d9e2ec");
        // #PanelTitle / #BrandTitle
        public static readonly Color TextHeading = FromHex("#f3f7fb");
        // #PanelSubtitle / .nexui-viewport-hint
        public static readonly Color TextMuted = FromHex("#8ca0b6");
        // #SectionTitle
        public static readonly Color Accent = FromHex("#b7d7ff");
        // .nexui-design-element.is-selected / .nexui-zoom-badge text
        public static readonly Color AccentSecondary = FromHex("#7dd3fc");

        // .nexui-button-primary
        public static readonly Color ButtonPrimary = FromHex("#2563eb");
        // .nexui-button-secondary
        public static readonly Color ButtonSecondary = FromHex("#2d3642");

        // .nexui-toolbar-status.is-ok
        public static readonly Color StatusOkBackground = FromHex("#123626");
        public static readonly Color StatusOkText = FromHex("#8df2bd");
        // .nexui-toolbar-status.is-warning
        public static readonly Color StatusWarningBackground = FromHex("#3b2b12");
        public static readonly Color StatusWarningText = FromHex("#ffd27a");
        // .nexui-toolbar-status.is-muted
        public static readonly Color StatusMutedBackground = FromHex("#2a3038");
        public static readonly Color StatusMutedText = FromHex("#93a4b7");

        private static Color FromHex(string hex) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.magenta;
    }
}
