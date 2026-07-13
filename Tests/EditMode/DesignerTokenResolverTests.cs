using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Tokens;
using NUnit.Framework;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Pure resolution + validation tests for the design-token system (brief §33): references resolve
    /// through aliases to a final literal, cycles and broken references are caught, and category
    /// mismatches are warned.
    /// </summary>
    public sealed class DesignerTokenResolverTests
    {
        private static DesignerToken Color(string name, Color value)
            => new DesignerToken(name, DesignerTokenCategory.Color) { colorValue = value };

        private static DesignerToken Ref(string name, string reference, DesignerTokenCategory cat = DesignerTokenCategory.Color)
            => new DesignerToken(name, cat) { reference = reference };

        private static DesignerToken Number(string name, float value, DesignerTokenCategory cat = DesignerTokenCategory.Spacing)
            => new DesignerToken(name, cat) { numberValue = value };

        private static DesignerTokenSetAsset Set(params DesignerToken[] tokens)
        {
            var asset = ScriptableObject.CreateInstance<DesignerTokenSetAsset>();
            asset.tokens = tokens.ToList();
            return asset;
        }

        [Test]
        public void ResolvesLiteralColor()
        {
            var tokens = new List<DesignerToken> { Color("accent", UnityEngine.Color.red) };
            Assert.IsTrue(DesignerTokenResolver.TryResolveColor(tokens, "accent", out var color));
            Assert.AreEqual(UnityEngine.Color.red, color);
        }

        [Test]
        public void ResolvesThroughReferenceChain()
        {
            var tokens = new List<DesignerToken>
            {
                Color("base", UnityEngine.Color.green),
                Ref("primary", "base"),
                Ref("buttonBg", "primary")
            };

            Assert.IsTrue(DesignerTokenResolver.TryResolveColor(tokens, "buttonBg", out var color));
            Assert.AreEqual(UnityEngine.Color.green, color);
        }

        [Test]
        public void ResolvesNumber()
        {
            var tokens = new List<DesignerToken> { Number("md", 8f), Ref("gap", "md", DesignerTokenCategory.Spacing) };
            Assert.IsTrue(DesignerTokenResolver.TryResolveNumber(tokens, "gap", out var value));
            Assert.AreEqual(8f, value);
        }

        [Test]
        public void MissingToken_FailsToResolve()
        {
            Assert.IsFalse(DesignerTokenResolver.TryResolveColor(new List<DesignerToken>(), "nope", out _));
        }

        [Test]
        public void Cycle_FailsToResolve_AndIsFlagged()
        {
            var asset = Set(Ref("a", "b"), Ref("b", "a"));

            Assert.IsNull(DesignerTokenResolver.Resolve(asset.tokens, "a"));
            Assert.IsTrue(DesignerTokenResolver.Validate(asset).Any(i => i.Message.Contains("cycle")));
        }

        [Test]
        public void BrokenReference_IsError()
        {
            var asset = Set(Ref("primary", "ghost"));
            Assert.IsTrue(DesignerTokenResolver.Validate(asset)
                .Any(i => i.Level == DesignerTokenIssue.Severity.Error && i.Message.Contains("ghost")));
        }

        [Test]
        public void DuplicateName_IsError()
        {
            var asset = Set(Color("a", UnityEngine.Color.red), Color("a", UnityEngine.Color.blue));
            Assert.IsTrue(DesignerTokenResolver.Validate(asset)
                .Any(i => i.Level == DesignerTokenIssue.Severity.Error && i.Message.Contains("Duplicate")));
        }

        [Test]
        public void CategoryMismatchReference_IsWarning()
        {
            var asset = Set(Number("md", 8f), Ref("badColor", "md", DesignerTokenCategory.Color));
            Assert.IsTrue(DesignerTokenResolver.Validate(asset)
                .Any(i => i.Level == DesignerTokenIssue.Severity.Warning && i.Message.Contains("different category")));
        }

        [Test]
        public void ValidSet_HasNoErrors()
        {
            var asset = Set(Color("base", UnityEngine.Color.white), Ref("primary", "base"), Number("md", 8f));
            Assert.IsFalse(DesignerTokenResolver.Validate(asset).Any(i => i.Level == DesignerTokenIssue.Severity.Error));
        }
    }
}
