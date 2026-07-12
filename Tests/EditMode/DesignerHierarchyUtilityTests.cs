using emiteat.NexUI.Designer.Editor;
using NUnit.Framework;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>
    /// Pure-logic tests for the parent/child hierarchy source-of-truth
    /// (<see cref="DesignerHierarchyUtility"/>): ordering, descendants, cycle prevention,
    /// normalization and dangling-parent repair. No window/Undo required.
    /// </summary>
    public sealed class DesignerHierarchyUtilityTests
    {
        private static DesignerMetadataAsset NewAsset() => ScriptableObject.CreateInstance<DesignerMetadataAsset>();

        private static DesignerElementMetadata Add(DesignerMetadataAsset asset, string id, string parent = null, int sibling = 0)
        {
            var e = new DesignerElementMetadata { elementId = id, parentId = parent, siblingIndex = sibling };
            asset.elements.Add(e);
            return e;
        }

        [Test]
        public void GetOrderedChildren_OrdersBySiblingIndex()
        {
            var asset = NewAsset();
            Add(asset, "root");
            Add(asset, "c", "root", 2);
            Add(asset, "a", "root", 0);
            Add(asset, "b", "root", 1);

            var children = DesignerHierarchyUtility.GetOrderedChildren(asset, "root");
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual("a", children[0].elementId);
            Assert.AreEqual("b", children[1].elementId);
            Assert.AreEqual("c", children[2].elementId);
        }

        [Test]
        public void GetRootElements_ReturnsOnlyParentlessElements()
        {
            var asset = NewAsset();
            Add(asset, "r1");
            Add(asset, "r2");
            Add(asset, "child", "r1");

            var roots = DesignerHierarchyUtility.GetRootElements(asset);
            Assert.AreEqual(2, roots.Count);
            CollectionAssert.AreEquivalent(new[] { "r1", "r2" }, new[] { roots[0].elementId, roots[1].elementId });
        }

        [Test]
        public void GetDescendants_ReturnsWholeSubtreeDepthFirst()
        {
            var asset = NewAsset();
            var root = Add(asset, "root");
            Add(asset, "a", "root", 0);
            Add(asset, "a1", "a", 0);
            Add(asset, "b", "root", 1);

            var descendants = DesignerHierarchyUtility.GetDescendants(asset, root);
            Assert.AreEqual(3, descendants.Count);
            Assert.AreEqual("a", descendants[0].elementId);
            Assert.AreEqual("a1", descendants[1].elementId);
            Assert.AreEqual("b", descendants[2].elementId);
        }

        [Test]
        public void WouldCreateCycle_DetectsSelfAndDescendantTargets()
        {
            var asset = NewAsset();
            Add(asset, "root");
            Add(asset, "child", "root");
            Add(asset, "grand", "child");

            Assert.IsTrue(DesignerHierarchyUtility.WouldCreateCycle(asset, "root", "root"), "self-parent");
            Assert.IsTrue(DesignerHierarchyUtility.WouldCreateCycle(asset, "root", "child"), "parent under own child");
            Assert.IsTrue(DesignerHierarchyUtility.WouldCreateCycle(asset, "root", "grand"), "parent under own grandchild");
            Assert.IsFalse(DesignerHierarchyUtility.WouldCreateCycle(asset, "child", "root"), "already the parent - legal");
            Assert.IsFalse(DesignerHierarchyUtility.WouldCreateCycle(asset, "grand", ""), "to root - always legal");
        }

        [Test]
        public void IsDescendant_WalksAncestorChain()
        {
            var asset = NewAsset();
            Add(asset, "root");
            Add(asset, "child", "root");
            Add(asset, "grand", "child");

            Assert.IsTrue(DesignerHierarchyUtility.IsDescendant(asset, "grand", "root"));
            Assert.IsFalse(DesignerHierarchyUtility.IsDescendant(asset, "root", "grand"));
        }

        [Test]
        public void NormalizeSiblingIndices_MakesContiguousPerParent()
        {
            var asset = NewAsset();
            Add(asset, "root");
            Add(asset, "a", "root", 5);
            Add(asset, "b", "root", 5);   // duplicate index
            Add(asset, "c", "root", 40);

            Assert.IsTrue(DesignerHierarchyUtility.NormalizeSiblingIndices(asset));
            var children = DesignerHierarchyUtility.GetOrderedChildren(asset, "root");
            Assert.AreEqual(0, children[0].siblingIndex);
            Assert.AreEqual(1, children[1].siblingIndex);
            Assert.AreEqual(2, children[2].siblingIndex);
        }

        [Test]
        public void NormalizeSiblingIndices_DetachesDanglingParent()
        {
            var asset = NewAsset();
            Add(asset, "orphan", "does-not-exist");

            Assert.IsTrue(DesignerHierarchyUtility.NormalizeSiblingIndices(asset));
            Assert.IsTrue(string.IsNullOrEmpty(asset.Find("orphan").parentId));
        }

        [Test]
        public void NormalizeSiblingIndices_BreaksCycle()
        {
            var asset = NewAsset();
            var a = Add(asset, "a", "b");
            var b = Add(asset, "b", "a"); // a <-> b cycle

            Assert.IsTrue(DesignerHierarchyUtility.NormalizeSiblingIndices(asset));
            // At least one node must have been detached so no cycle remains.
            Assert.IsFalse(DesignerHierarchyUtility.IsDescendant(asset, a.elementId, b.elementId) &&
                           DesignerHierarchyUtility.IsDescendant(asset, b.elementId, a.elementId));
        }

        [Test]
        public void GetDepth_CountsAncestors()
        {
            var asset = NewAsset();
            Add(asset, "root");
            Add(asset, "child", "root");
            var grand = Add(asset, "grand", "child");

            Assert.AreEqual(2, DesignerHierarchyUtility.GetDepth(asset, grand));
            Assert.AreEqual(0, DesignerHierarchyUtility.GetDepth(asset, asset.Find("root")));
        }

        [Test]
        public void IsContainerType_LeafTypesAreNotContainers()
        {
            Assert.IsTrue(DesignerHierarchyUtility.IsContainerType("Panel"));
            Assert.IsTrue(DesignerHierarchyUtility.IsContainerType("Container"));
            Assert.IsFalse(DesignerHierarchyUtility.IsContainerType("Button"));
            Assert.IsFalse(DesignerHierarchyUtility.IsContainerType("Label"));
        }
    }
}
