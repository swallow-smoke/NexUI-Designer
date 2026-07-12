using emiteat.NexUI.Designer.Editor;
using emiteat.NexUI.Designer.Editor.Viewport;
using NUnit.Framework;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    public sealed class DesignerUIStateTests
    {
        [Test]
        public void UIState_ChangesPersistOnContext()
        {
            var context = new NexUIDesignerContext();

            context.SetSidebarTab(DesignerSidebarTab.Components);
            context.SetInspectorTab(DesignerInspectorTab.Motion);
            context.SetBottomTab(DesignerBottomTab.History, true);
            context.SetTool(DesignerTool.Hand);

            Assert.AreEqual(DesignerSidebarTab.Components, context.SidebarTab);
            Assert.AreEqual(DesignerInspectorTab.Motion, context.InspectorTab);
            Assert.AreEqual(DesignerBottomTab.History, context.BottomTab);
            Assert.AreEqual(DesignerTool.Hand, context.CurrentTool);
            Assert.IsTrue(context.BottomDrawerOpen);
        }

        [Test]
        public void KeyObject_BecomesAlignmentBounds()
        {
            var context = new NexUIDesignerContext();
            var asset = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            var left = new DesignerElementMetadata { elementId = "left", rect = new Rect(10, 0, 20, 20) };
            var key = new DesignerElementMetadata { elementId = "key", rect = new Rect(100, 0, 50, 20) };
            asset.elements.Add(left);
            asset.elements.Add(key);
            context.SetMetadata(asset);
            context.SelectMany(new[] { left, key });
            context.SetKeyObject(key);

            context.AlignSelection("right");

            Assert.AreEqual(130, left.rect.x);
            Assert.AreEqual(150, key.rect.xMax);
        }

        [Test]
        public void AltDragDuplicateEntryPoint_DuplicatesSelection()
        {
            var context = new NexUIDesignerContext();
            var asset = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            var element = new DesignerElementMetadata { elementId = "button", rect = new Rect(0, 0, 20, 20) };
            asset.elements.Add(element);
            context.SetMetadata(asset);
            context.SelectMetadata(element);

            var copies = context.DuplicateSelectionAtDragStart();

            Assert.AreEqual(1, copies.Count);
            Assert.AreEqual(2, asset.elements.Count);
            Assert.AreEqual(copies[0], context.SelectedMetadata);
        }

        [Test]
        public void LayerOrder_MoveElementChangesMetadataOrder()
        {
            var context = new NexUIDesignerContext();
            var asset = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            var a = new DesignerElementMetadata { elementId = "a" };
            var b = new DesignerElementMetadata { elementId = "b" };
            asset.elements.Add(a);
            asset.elements.Add(b);
            context.SetMetadata(asset);

            context.MoveElementInLayerOrder(b, -1);

            Assert.AreEqual(b, asset.elements[0]);
            Assert.AreEqual(a, asset.elements[1]);
        }

        [Test]
        public void SmartGuide_SnapsToNearbyElementEdge()
        {
            var moving = new DesignerElementMetadata { elementId = "moving", rect = new Rect(96, 0, 20, 20) };
            var target = new DesignerElementMetadata { elementId = "target", rect = new Rect(120, 0, 30, 20) };

            var result = NexUISmartGuideUtility.Snap(moving.rect, new[] { moving, target }, moving, 8f);

            Assert.AreEqual(100, result.Rect.x);
            Assert.AreEqual(120, result.VerticalGuide.Value);
        }
    }
}
