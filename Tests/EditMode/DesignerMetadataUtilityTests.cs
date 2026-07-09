using emiteat.NexUI.Designer.Editor.Serialization;
using NUnit.Framework;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    public sealed class DesignerMetadataUtilityTests
    {
        private static DesignerMetadataAsset NewAsset()
            => ScriptableObject.CreateInstance<DesignerMetadataAsset>();

        [Test]
        public void MakeUniqueId_ReturnsBaseWhenFree()
        {
            var asset = NewAsset();
            Assert.AreEqual("panel", DesignerMetadataUtility.MakeUniqueId(asset, "panel"));
        }

        [Test]
        public void MakeUniqueId_AppendsNumberWhenTaken()
        {
            var asset = NewAsset();
            asset.elements.Add(new DesignerElementMetadata { elementId = "panel" });
            Assert.AreEqual("panel1", DesignerMetadataUtility.MakeUniqueId(asset, "panel"));
        }

        [Test]
        public void Rename_RepointsChildParentIds()
        {
            var asset = NewAsset();
            var parent = DesignerMetadataUtility.Create(asset, new DesignerElementMetadata { elementId = "parent" });
            DesignerMetadataUtility.Create(asset, new DesignerElementMetadata { elementId = "child", parentId = "parent" });

            Assert.IsTrue(DesignerMetadataUtility.Rename(asset, parent, "root"));
            Assert.AreEqual("root", asset.Find("child").parentId);
        }

        [Test]
        public void Rename_RejectsCollision()
        {
            var asset = NewAsset();
            var a = DesignerMetadataUtility.Create(asset, new DesignerElementMetadata { elementId = "a" });
            DesignerMetadataUtility.Create(asset, new DesignerElementMetadata { elementId = "b" });
            Assert.IsFalse(DesignerMetadataUtility.Rename(asset, a, "b"));
        }

        [Test]
        public void Duplicate_ProducesUniqueDeepCopy()
        {
            var asset = NewAsset();
            var src = DesignerMetadataUtility.Create(asset, new DesignerElementMetadata { elementId = "btn", text = "Hi" });
            src.classes.Add("primary");

            var copy = DesignerMetadataUtility.Duplicate(asset, src);
            Assert.AreNotEqual(src.elementId, copy.elementId);
            Assert.AreEqual("Hi", copy.text);
            Assert.Contains("primary", copy.classes);
            copy.classes.Add("mutated");
            Assert.AreEqual(1, src.classes.Count, "clone must not share the classes list");
        }

        [Test]
        public void AnchorPreset_DefaultsToTopLeft()
        {
            var element = new DesignerElementMetadata();
            Assert.AreEqual(DesignerAnchorPreset.TopLeft, element.anchorPreset,
                "Default (0) must be TopLeft so pre-existing metadata deserializes to the historical anchor.");
        }

        [Test]
        public void Duplicate_PreservesAnchorPreset()
        {
            var asset = NewAsset();
            var src = DesignerMetadataUtility.Create(asset,
                new DesignerElementMetadata { elementId = "panel", anchorPreset = DesignerAnchorPreset.BottomRight });

            var copy = DesignerMetadataUtility.Duplicate(asset, src);
            Assert.AreEqual(DesignerAnchorPreset.BottomRight, copy.anchorPreset);
        }

        [Test]
        public void FindDuplicateIds_DetectsRepeats()
        {
            var asset = NewAsset();
            asset.elements.Add(new DesignerElementMetadata { elementId = "dup" });
            asset.elements.Add(new DesignerElementMetadata { elementId = "dup" });
            asset.elements.Add(new DesignerElementMetadata { elementId = "unique" });
            var dupes = DesignerMetadataUtility.FindDuplicateIds(asset);
            Assert.AreEqual(1, dupes.Count);
            Assert.AreEqual("dup", dupes[0]);
        }

        [Test]
        public void IsValidElementId_RejectsBadIds()
        {
            Assert.IsTrue(DesignerMetadataUtility.IsValidElementId("login_button-1"));
            Assert.IsFalse(DesignerMetadataUtility.IsValidElementId("1button"));
            Assert.IsFalse(DesignerMetadataUtility.IsValidElementId("has space"));
            Assert.IsFalse(DesignerMetadataUtility.IsValidElementId(""));
        }
    }
}
