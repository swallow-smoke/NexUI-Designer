using System;
using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Accessibility;
using emiteat.NexUI.Designer.Editor.Accessibility;
using NUnit.Framework;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Pure accessibility-audit tests (brief §38.8): WCAG contrast ratio, minimum touch target, and
    /// missing label/role on interactive elements. Interactivity is injected so the audit needs no
    /// component registry.
    /// </summary>
    public sealed class DesignerAccessibilityAuditTests
    {
        private static readonly Func<string, bool> AllInteractive = _ => true;
        private static readonly Func<string, bool> NoneInteractive = _ => false;

        private static DesignerElementMetadata El(string id, string type = "Button")
            => new DesignerElementMetadata { elementId = id, elementType = type, rect = new Rect(0, 0, 60, 60) };

        private static List<AccessibilityAuditIssue> Audit(IEnumerable<DesignerElementMetadata> els, Func<string, bool> interactive)
            => DesignerAccessibilityAudit.Audit(els.ToList(), interactive, AccessibilityAuditOptions.Default);

        [Test]
        public void ContrastRatio_BlackOnWhite_IsMax()
        {
            Assert.AreEqual(21f, DesignerAccessibilityAudit.ContrastRatio(Color.black, Color.white), 0.1f);
        }

        [Test]
        public void ContrastRatio_SameColor_IsOne()
        {
            Assert.AreEqual(1f, DesignerAccessibilityAudit.ContrastRatio(Color.gray, Color.gray), 0.001f);
        }

        [Test]
        public void LowContrastText_IsFlagged()
        {
            var el = El("lbl", "Label");
            el.text = "hi";
            el.textColor = new Color(0.5f, 0.5f, 0.5f);
            el.tint = new Color(0.55f, 0.55f, 0.55f);

            Assert.IsTrue(Audit(new[] { el }, NoneInteractive).Any(i => i.Message.Contains("Low text contrast")));
        }

        [Test]
        public void GoodContrastText_NotFlagged()
        {
            var el = El("lbl", "Label");
            el.text = "hi";
            el.textColor = Color.white;
            el.tint = Color.black;

            Assert.IsFalse(Audit(new[] { el }, NoneInteractive).Any(i => i.Message.Contains("Low text contrast")));
        }

        [Test]
        public void SmallTouchTarget_OnInteractive_IsFlagged()
        {
            var el = El("btn");
            el.rect = new Rect(0, 0, 20, 20);
            Assert.IsTrue(Audit(new[] { el }, AllInteractive).Any(i => i.Message.Contains("Touch target too small")));
        }

        [Test]
        public void SmallSize_OnNonInteractive_NotFlaggedForTouch()
        {
            var el = El("panel", "Panel");
            el.rect = new Rect(0, 0, 20, 20);
            Assert.IsFalse(Audit(new[] { el }, NoneInteractive).Any(i => i.Message.Contains("Touch target")));
        }

        [Test]
        public void InteractiveWithoutLabelOrText_IsFlagged()
        {
            var el = El("btn");
            el.rect = new Rect(0, 0, 80, 80);
            Assert.IsTrue(Audit(new[] { el }, AllInteractive).Any(i => i.Message.Contains("no accessibility label")));
        }

        [Test]
        public void InteractiveWithLabel_NotFlaggedForLabel()
        {
            var el = El("btn");
            el.rect = new Rect(0, 0, 80, 80);
            el.accessibilityLabel = "Play";
            Assert.IsFalse(Audit(new[] { el }, AllInteractive).Any(i => i.Message.Contains("no accessibility label")));
        }

        [Test]
        public void InteractiveWithoutRole_IsInfo()
        {
            var el = El("btn");
            el.rect = new Rect(0, 0, 80, 80);
            el.accessibilityLabel = "Play";
            Assert.IsTrue(Audit(new[] { el }, AllInteractive)
                .Any(i => i.Level == AccessibilityAuditIssue.Severity.Info && i.Message.Contains("no accessibility role")));
        }

        [Test]
        public void InteractiveWithRole_NoRoleIssue()
        {
            var el = El("btn");
            el.rect = new Rect(0, 0, 80, 80);
            el.accessibilityLabel = "Play";
            el.accessibilityRole = AccessibilityRole.Button;
            Assert.IsFalse(Audit(new[] { el }, AllInteractive).Any(i => i.Message.Contains("no accessibility role")));
        }
    }
}
