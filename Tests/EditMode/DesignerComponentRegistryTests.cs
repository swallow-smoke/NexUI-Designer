using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Designer.Editor.Components;
using NUnit.Framework;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Registry-completeness tests (spec §34): every runtime component type has a valid descriptor,
    /// ids are unique, defaults are sane, and unknown types resolve to a safe Generic descriptor.
    /// </summary>
    public sealed class DesignerComponentRegistryTests
    {
        [Test]
        public void EveryElementTypeHasADescriptor()
        {
            foreach (DesignerElementType type in Enum.GetValues(typeof(DesignerElementType)))
            {
                var d = DesignerComponentRegistry.Get(type);
                Assert.IsNotNull(d, $"No descriptor for {type}");
                Assert.AreEqual(type.ToString(), d.TypeId, $"Descriptor TypeId mismatch for {type}");
                Assert.IsFalse(string.IsNullOrEmpty(d.DisplayName), $"{type} has no DisplayName");
                Assert.IsFalse(string.IsNullOrEmpty(d.LocalizationKey), $"{type} has no LocalizationKey");
            }
        }

        [Test]
        public void NoDuplicateTypeIds()
        {
            var seen = new HashSet<string>();
            foreach (var d in DesignerComponentRegistry.All)
                Assert.IsTrue(seen.Add(d.TypeId), $"Duplicate descriptor TypeId '{d.TypeId}'");
        }

        [Test]
        public void DefaultSizesAreValidAndAtLeastMinimum()
        {
            foreach (var d in DesignerComponentRegistry.All)
            {
                Assert.Greater(d.DefaultSize.x, 0f, $"{d.TypeId} default width");
                Assert.Greater(d.DefaultSize.y, 0f, $"{d.TypeId} default height");
                Assert.GreaterOrEqual(d.DefaultSize.x, d.MinimumSize.x, $"{d.TypeId} width < min");
                Assert.GreaterOrEqual(d.DefaultSize.y, d.MinimumSize.y, $"{d.TypeId} height < min");
            }
        }

        [Test]
        public void ContainersDeclareAtLeastOneSlot()
        {
            foreach (var d in DesignerComponentRegistry.All)
                if (d.CanHaveChildren)
                    Assert.Greater(d.Slots.Count, 0, $"{d.TypeId} can have children but declares no slots");
        }

        [Test]
        public void UnknownTypeResolvesToGenericKeepingItsId()
        {
            var d = DesignerComponentRegistry.Get("MyCustomWidget");
            Assert.IsTrue(d.IsGeneric);
            Assert.AreEqual("MyCustomWidget", d.TypeId);
            Assert.IsTrue(d.CanHaveChildren, "Generic must be permissive so custom screens aren't blocked");
        }

        [Test]
        public void LeafTypesCannotHaveChildren()
        {
            Assert.IsFalse(DesignerComponentRegistry.CanHaveChildren("Label"));
            Assert.IsFalse(DesignerComponentRegistry.CanHaveChildren("Image"));
            Assert.IsFalse(DesignerComponentRegistry.CanHaveChildren("ProgressBar"));
            Assert.IsTrue(DesignerComponentRegistry.CanHaveChildren("Panel"));
            Assert.IsTrue(DesignerComponentRegistry.CanHaveChildren("Button"), "Button has icon/content slots");
        }

        [Test]
        public void ContainerFlagMatchesExpectations()
        {
            Assert.IsTrue(DesignerComponentRegistry.IsContainer("Panel"));
            Assert.IsTrue(DesignerComponentRegistry.IsContainer("Modal"));
            Assert.IsFalse(DesignerComponentRegistry.IsContainer("Label"));
            Assert.IsFalse(DesignerComponentRegistry.IsContainer("Button"), "Button holds slot children but is not a layout container");
        }

        [Test]
        public void SupportedBindingsAreDeclaredPerType()
        {
            var button = DesignerComponentRegistry.Get("Button");
            Assert.IsTrue(button.SupportsBinding(DesignerBindingChannel.Command));
            Assert.IsTrue(button.SupportsBinding(DesignerBindingChannel.Text));

            var progress = DesignerComponentRegistry.Get("ProgressBar");
            Assert.IsTrue(progress.SupportsBinding(DesignerBindingChannel.Value));
            Assert.IsFalse(progress.SupportsBinding(DesignerBindingChannel.Command), "ProgressBar is not command-driven");
        }

        [Test]
        public void TemplateSlotsAreMarkedOnCollectionTypes()
        {
            foreach (var typeId in new[] { "List", "Grid", "ChoiceList", "Hotbar" })
            {
                var d = DesignerComponentRegistry.Get(typeId);
                var hasTemplate = false;
                foreach (var s in d.Slots) if (s.IsTemplateSlot) hasTemplate = true;
                Assert.IsTrue(hasTemplate, $"{typeId} should declare a template slot");
            }
        }

        [Test]
        public void ChannelForKeyName_MapsSerializedKeys()
        {
            Assert.AreEqual(DesignerBindingChannel.Command, DesignerComponentDescriptor.ChannelForKeyName("commandKey"));
            Assert.AreEqual(DesignerBindingChannel.Value, DesignerComponentDescriptor.ChannelForKeyName("valueKey"));
            Assert.AreEqual(DesignerBindingChannel.None, DesignerComponentDescriptor.ChannelForKeyName("nope"));
        }

        [Test]
        public void DefaultSlotIdPrefersContent()
        {
            Assert.AreEqual("content", DesignerComponentRegistry.Get("Panel").DefaultSlotId);
            Assert.AreEqual("content", DesignerComponentRegistry.Get("Modal").DefaultSlotId);
        }
    }
}
