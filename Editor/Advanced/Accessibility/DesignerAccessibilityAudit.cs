using System;
using System.Collections.Generic;
using emiteat.NexUI.Accessibility;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Accessibility
{
    /// <summary>An accessibility audit finding (brief §38.8).</summary>
    public struct AccessibilityAuditIssue
    {
        public enum Severity { Info, Warning }
        public Severity Level;
        public string ElementId;
        public string Message;
    }

    /// <summary>Thresholds for the audit (brief §35/§38). Defaults follow WCAG AA (4.5:1 text
    /// contrast) and the common 44px minimum touch target.</summary>
    public struct AccessibilityAuditOptions
    {
        public float MinContrast;
        public float MinTouchTarget;

        public static AccessibilityAuditOptions Default => new AccessibilityAuditOptions
        {
            MinContrast = 4.5f,
            MinTouchTarget = 44f
        };
    }

    /// <summary>
    /// Pure accessibility audit of a screen's elements (brief §38.8): WCAG text contrast, minimum
    /// touch-target size, and missing accessibility label/role on interactive elements. Interactivity
    /// is supplied as a predicate so the core has no <c>DesignerComponentRegistry</c> dependency and is
    /// fully unit-testable. Contrast compares an element's text color against its own background tint
    /// (a documented approximation — text drawn on its own element background, e.g. a Button).
    /// </summary>
    public static class DesignerAccessibilityAudit
    {
        public static List<AccessibilityAuditIssue> Audit(
            IReadOnlyList<DesignerElementMetadata> elements,
            Func<string, bool> isInteractive,
            AccessibilityAuditOptions options)
        {
            var issues = new List<AccessibilityAuditIssue>();
            if (elements == null) return issues;
            isInteractive ??= _ => false;

            foreach (var element in elements)
            {
                if (element == null || string.IsNullOrEmpty(element.elementId)) continue;
                var interactive = isInteractive(element.elementType);

                if (!string.IsNullOrEmpty(element.text))
                {
                    var ratio = ContrastRatio(element.textColor, element.tint);
                    if (ratio < options.MinContrast)
                        issues.Add(Warn(element.elementId,
                            $"Low text contrast on '{element.elementId}': {ratio:0.0}:1 (needs {options.MinContrast:0.0}:1)."));
                }

                if (interactive)
                {
                    if (element.rect.width < options.MinTouchTarget || element.rect.height < options.MinTouchTarget)
                        issues.Add(Warn(element.elementId,
                            $"Touch target too small on '{element.elementId}': {element.rect.width:0}x{element.rect.height:0} (min {options.MinTouchTarget:0})."));

                    if (string.IsNullOrEmpty(element.accessibilityLabel) && string.IsNullOrEmpty(element.text))
                        issues.Add(Warn(element.elementId, $"Interactive element '{element.elementId}' has no accessibility label or text."));

                    if (element.accessibilityRole == AccessibilityRole.None)
                        issues.Add(Info(element.elementId, $"Interactive element '{element.elementId}' has no accessibility role."));
                }
            }

            return issues;
        }

        // ---- WCAG contrast ------------------------------------------------------

        /// <summary>WCAG 2.x contrast ratio between two colors, from 1:1 to 21:1.</summary>
        public static float ContrastRatio(Color a, Color b)
        {
            var la = RelativeLuminance(a);
            var lb = RelativeLuminance(b);
            var lighter = Mathf.Max(la, lb);
            var darker = Mathf.Min(la, lb);
            return (lighter + 0.05f) / (darker + 0.05f);
        }

        private static float RelativeLuminance(Color c)
            => 0.2126f * Linearize(c.r) + 0.7152f * Linearize(c.g) + 0.0722f * Linearize(c.b);

        private static float Linearize(float channel)
            => channel <= 0.03928f ? channel / 12.92f : Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);

        private static AccessibilityAuditIssue Info(string id, string message) => new AccessibilityAuditIssue { Level = AccessibilityAuditIssue.Severity.Info, ElementId = id, Message = message };
        private static AccessibilityAuditIssue Warn(string id, string message) => new AccessibilityAuditIssue { Level = AccessibilityAuditIssue.Severity.Warning, ElementId = id, Message = message };
    }
}
