using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Scenario;
using NUnit.Framework;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Pure-mapping tests for the Scenario system (brief §28/§35): a scenario's mock binding values
    /// resolve to the right per-element preview-channel changes across the value/text/visibility
    /// channels, one key can drive several elements, and unmatched keys produce no change.
    /// </summary>
    public sealed class ScenarioResolverTests
    {
        private static DesignerElementMetadata Element(string id, string valueKey = null, string textKey = null, string visibilityKey = null)
        {
            return new DesignerElementMetadata
            {
                elementId = id,
                binding = new DesignerBindingMetadata
                {
                    valueKey = valueKey,
                    textKey = textKey,
                    visibilityKey = visibilityKey
                }
            };
        }

        [Test]
        public void NumberValue_DrivesPreviewValue()
        {
            var elements = new List<DesignerElementMetadata> { Element("hp", valueKey: "player.health") };
            var bindings = new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("player.health", DesignerScenarioValueKind.Number) { numberValue = 15f }
            };

            var changes = ScenarioApplyResolver.Resolve(elements, bindings);

            Assert.AreEqual(1, changes.Count);
            Assert.IsTrue(changes[0].SetPreviewValue);
            Assert.AreEqual(15f, changes[0].PreviewValue);
            Assert.IsFalse(changes[0].SetText);
            Assert.IsFalse(changes[0].SetHidden);
        }

        [Test]
        public void BoolValue_OnValueChannel_BecomesOneOrZero()
        {
            var elements = new List<DesignerElementMetadata> { Element("flag", valueKey: "quest.done") };
            var bindings = new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("quest.done", DesignerScenarioValueKind.Bool) { boolValue = true }
            };

            var changes = ScenarioApplyResolver.Resolve(elements, bindings);

            Assert.AreEqual(1f, changes.Single().PreviewValue);
        }

        [Test]
        public void TextValue_DrivesTextChannelOnly()
        {
            var elements = new List<DesignerElementMetadata> { Element("label", textKey: "hud.name") };
            var bindings = new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("hud.name", DesignerScenarioValueKind.Text) { textValue = "Hero" }
            };

            var change = ScenarioApplyResolver.Resolve(elements, bindings).Single();

            Assert.IsTrue(change.SetText);
            Assert.AreEqual("Hero", change.Text);
            Assert.IsFalse(change.SetPreviewValue);
        }

        [Test]
        public void VisibilityBool_HiddenIsInverseOfBound()
        {
            var elements = new List<DesignerElementMetadata> { Element("panel", visibilityKey: "ui.showPanel") };

            var visible = ScenarioApplyResolver.Resolve(elements, new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("ui.showPanel", DesignerScenarioValueKind.Bool) { boolValue = true }
            }).Single();
            Assert.IsTrue(visible.SetHidden);
            Assert.IsFalse(visible.Hidden);

            var hidden = ScenarioApplyResolver.Resolve(elements, new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("ui.showPanel", DesignerScenarioValueKind.Bool) { boolValue = false }
            }).Single();
            Assert.IsTrue(hidden.Hidden);
        }

        [Test]
        public void OneKey_DrivesEveryElementBoundToIt()
        {
            var elements = new List<DesignerElementMetadata>
            {
                Element("bar1", valueKey: "player.health"),
                Element("bar2", valueKey: "player.health"),
                Element("other", valueKey: "player.mana")
            };
            var bindings = new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("player.health", DesignerScenarioValueKind.Number) { numberValue = 42f }
            };

            var changes = ScenarioApplyResolver.Resolve(elements, bindings);

            Assert.AreEqual(2, changes.Count);
            Assert.IsTrue(changes.All(c => c.PreviewValue == 42f));
            Assert.IsFalse(changes.Any(c => c.ElementId == "other"));
        }

        [Test]
        public void OneElement_CanBeDrivenAcrossChannels()
        {
            var elements = new List<DesignerElementMetadata>
            {
                Element("slot", valueKey: "item.count", textKey: "item.name", visibilityKey: "item.present")
            };
            var bindings = new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("item.count", DesignerScenarioValueKind.Number) { numberValue = 3f },
                new DesignerScenarioBinding("item.name", DesignerScenarioValueKind.Text) { textValue = "Potion" },
                new DesignerScenarioBinding("item.present", DesignerScenarioValueKind.Bool) { boolValue = true }
            };

            var change = ScenarioApplyResolver.Resolve(elements, bindings).Single();

            Assert.AreEqual(3f, change.PreviewValue);
            Assert.AreEqual("Potion", change.Text);
            Assert.IsFalse(change.Hidden);
        }

        [Test]
        public void UnmatchedKey_ProducesNoChange()
        {
            var elements = new List<DesignerElementMetadata> { Element("hp", valueKey: "player.health") };
            var bindings = new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("nonexistent.key", DesignerScenarioValueKind.Number) { numberValue = 1f }
            };

            Assert.IsEmpty(ScenarioApplyResolver.Resolve(elements, bindings));
        }

        [Test]
        public void DuplicateKey_FirstOccurrenceWins()
        {
            var elements = new List<DesignerElementMetadata> { Element("hp", valueKey: "k") };
            var bindings = new List<DesignerScenarioBinding>
            {
                new DesignerScenarioBinding("k", DesignerScenarioValueKind.Number) { numberValue = 10f },
                new DesignerScenarioBinding("k", DesignerScenarioValueKind.Number) { numberValue = 99f }
            };

            Assert.AreEqual(10f, ScenarioApplyResolver.Resolve(elements, bindings).Single().PreviewValue);
        }

        [Test]
        public void EmptyOrNullInputs_ReturnNoChanges()
        {
            Assert.IsEmpty(ScenarioApplyResolver.Resolve(null, new List<DesignerScenarioBinding>()));
            Assert.IsEmpty(ScenarioApplyResolver.Resolve(new List<DesignerElementMetadata>(), null));
            Assert.IsEmpty(ScenarioApplyResolver.Resolve(
                new List<DesignerElementMetadata> { Element("x", valueKey: "k") },
                new List<DesignerScenarioBinding>()));
        }
    }
}
