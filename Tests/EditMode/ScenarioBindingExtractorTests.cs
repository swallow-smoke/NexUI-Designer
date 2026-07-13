using System.Linq;
using emiteat.NexUI.Designer.Editor.Scenario;
using NUnit.Framework;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Tests for the inverse of the apply resolver (brief §35.3 recording / §35.1 capture): an
    /// element's bound channels are extracted as scenario bindings carrying its current preview values.
    /// </summary>
    public sealed class ScenarioBindingExtractorTests
    {
        private static DesignerElementMetadata Element(string valueKey = null, string textKey = null, string visibilityKey = null)
        {
            return new DesignerElementMetadata
            {
                elementId = "e",
                previewValue = 42f,
                text = "hello",
                hiddenInDesigner = false,
                binding = new DesignerBindingMetadata { valueKey = valueKey, textKey = textKey, visibilityKey = visibilityKey }
            };
        }

        [Test]
        public void ExtractsEachBoundChannel()
        {
            var bindings = ScenarioBindingExtractor.FromElement(Element("v", "t", "vis"));

            Assert.AreEqual(3, bindings.Count);
            Assert.AreEqual(42f, bindings.Single(b => b.key == "v").numberValue);
            Assert.AreEqual("hello", bindings.Single(b => b.key == "t").textValue);
            Assert.IsTrue(bindings.Single(b => b.key == "vis").boolValue); // visible => bound bool true
        }

        [Test]
        public void UnboundChannels_AreSkipped()
        {
            Assert.AreEqual(1, ScenarioBindingExtractor.FromElement(Element(valueKey: "v")).Count);
            Assert.IsEmpty(ScenarioBindingExtractor.FromElement(Element()));
        }

        [Test]
        public void HiddenElement_VisibilityBoolIsFalse()
        {
            var element = Element(visibilityKey: "vis");
            element.hiddenInDesigner = true;
            Assert.IsFalse(ScenarioBindingExtractor.FromElement(element).Single().boolValue);
        }

        [Test]
        public void SameValue_ComparesByKind()
        {
            var a = new DesignerScenarioBinding("k", DesignerScenarioValueKind.Number) { numberValue = 1f };
            var b = new DesignerScenarioBinding("k", DesignerScenarioValueKind.Number) { numberValue = 1f };
            var c = new DesignerScenarioBinding("k", DesignerScenarioValueKind.Number) { numberValue = 2f };

            Assert.IsTrue(ScenarioBindingExtractor.SameValue(a, b));
            Assert.IsFalse(ScenarioBindingExtractor.SameValue(a, c));
            Assert.IsFalse(ScenarioBindingExtractor.SameValue(a, null));
        }

        [Test]
        public void RoundTrip_ExtractThenResolve_PreservesValue()
        {
            var element = Element(valueKey: "hp");
            var bindings = ScenarioBindingExtractor.FromElement(element);
            var changes = ScenarioApplyResolver.Resolve(new[] { element }, bindings);

            Assert.AreEqual(element.previewValue, changes.Single().PreviewValue);
        }
    }
}
