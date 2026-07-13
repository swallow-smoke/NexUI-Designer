using System.Linq;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Accessibility;
using emiteat.NexUI.Designer.Editor.Common;
using emiteat.NexUI.Designer.Editor.Components;

namespace emiteat.NexUI.Designer.Editor.Accessibility
{
    /// <summary>Accessibility preference preview (§19) plus a screen audit (§35/§38): WCAG contrast,
    /// touch-target size, and missing label/role over the open screen's elements.</summary>
    public sealed class AccessibilityWindow : NexUIToolWindow
    {
        [SerializeField] private bool _reduceMotion;
        [SerializeField] private bool _highContrast;
        [SerializeField] private bool _largeText;

        protected override string TitleKey => "panel.accessibility";
        protected override string TooltipKey => "tooltip.accessibility";

        public static void Open() => GetWindow<AccessibilityWindow>();

        protected override void DrawBody()
        {
            Section("panel.accessibility");
            _reduceMotion = EditorGUILayout.Toggle("reduce motion", _reduceMotion);
            _highContrast = EditorGUILayout.Toggle("high contrast", _highContrast);
            _largeText = EditorGUILayout.Toggle("large text", _largeText);

            var pref = new UIAccessibilityPreference
            {
                reduceMotion = _reduceMotion,
                highContrast = _highContrast,
                largeText = _largeText
            };

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Preview effects", EditorStyles.boldLabel);
            foreach (var effect in AccessibilityService.Effects(pref))
                EditorGUILayout.LabelField("• " + effect);

            DrawAudit();
        }

        private void DrawAudit()
        {
            Section("accessibility.audit.title");

            var context = DesignerSessions.ActiveContext;
            var metadata = context?.Metadata;
            if (metadata == null)
            {
                EditorGUILayout.HelpBox(T("accessibility.audit.noScreen"), MessageType.Info);
                return;
            }

            var issues = DesignerAccessibilityAudit.Audit(
                metadata.elements,
                type => DesignerComponentRegistry.Get(type).IsInteractive,
                AccessibilityAuditOptions.Default);

            if (issues.Count == 0)
            {
                Badge(T("accessibility.audit.passed"), BadgeKind.Ok);
                return;
            }

            foreach (var issue in issues)
                EditorGUILayout.HelpBox(issue.Message,
                    issue.Level == AccessibilityAuditIssue.Severity.Warning ? MessageType.Warning : MessageType.Info);
        }
    }
}
