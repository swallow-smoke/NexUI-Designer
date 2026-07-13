using System.Linq;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Validation;
using emiteat.NexUI.Designer.Editor;
using emiteat.NexUI.MotionClip;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    public sealed class DesignerMotionPersistenceTests
    {
        private const string TempFolder = "Assets/NexUIDesignerMotionTests";

        [SetUp]
        public void SetUp()
        {
            if (!AssetDatabase.IsValidFolder(TempFolder)) AssetDatabase.CreateFolder("Assets", "NexUIDesignerMotionTests");
            Undo.ClearAll();
        }

        [TearDown]
        public void TearDown()
        {
            AssetDatabase.DeleteAsset(TempFolder);
            Undo.ClearAll();
        }

        [Test]
        public void MotionBinding_SurvivesAssetSaveAndReload()
        {
            var clip = ScriptableObject.CreateInstance<UIMotionClip>();
            AssetDatabase.CreateAsset(clip, TempFolder + "/Hover.asset");
            var metadata = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            metadata.screenId = "inventory";
            metadata.elements.Add(new DesignerElementMetadata { elementId = "slot1" });
            metadata.screenMotion.entryClip = clip;
            metadata.screenMotion.exitClip = clip;
            metadata.screenMotion.bindings.Add(new DesignerMotionBinding
            {
                bindingId = "hover-slot1", targetElementId = "slot1",
                trigger = DesignerMotionTrigger.HoverEnter, clip = clip, reducedMotionClip = clip
            });
            var path = TempFolder + "/Inventory.Metadata.asset";
            AssetDatabase.CreateAsset(metadata, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            var loaded = AssetDatabase.LoadAssetAtPath<DesignerMetadataAsset>(path);
            Assert.That(loaded.screenMotion.entryClip, Is.SameAs(clip));
            Assert.That(loaded.screenMotion.exitClip, Is.SameAs(clip));
            Assert.That(loaded.screenMotion.bindings.Single().targetElementId, Is.EqualTo("slot1"));
            Assert.That(loaded.screenMotion.bindings.Single().reducedMotionClip, Is.SameAs(clip));
        }

        [Test]
        public void RenameElement_UpdatesMotionTarget_AndUndoRestoresBoth()
        {
            var metadata = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            var element = new DesignerElementMetadata { elementId = "oldId" };
            metadata.elements.Add(element);
            metadata.screenMotion.bindings.Add(new DesignerMotionBinding { bindingId = "b", targetElementId = "oldId" });
            var context = new NexUIDesignerContext();
            context.SetMetadata(metadata);
            context.RenameElementId(element, "newId");
            Assert.That(element.elementId, Is.EqualTo("newId"));
            Assert.That(metadata.screenMotion.bindings[0].targetElementId, Is.EqualTo("newId"));
            Undo.PerformUndo();
            Assert.That(element.elementId, Is.EqualTo("oldId"));
            Assert.That(metadata.screenMotion.bindings[0].targetElementId, Is.EqualTo("oldId"));
            context.Dispose();
            Object.DestroyImmediate(metadata);
        }

        [Test]
        public void DeletedTargetAndMissingClip_AreValidationErrors()
        {
            var screen = ScriptableObject.CreateInstance<UIScreenDefinition>();
            screen.identity = new UIScreenIdentity { screenId = "inventory" };
            screen.backendAsset = new UIScreenBackendAsset { backend = UIRenderBackend.UIToolkit };
            var metadata = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            metadata.screenId = "inventory";
            metadata.screenMotion.bindings.Add(new DesignerMotionBinding
            {
                bindingId = "broken", targetElementId = "deletedSlot", trigger = DesignerMotionTrigger.Click
            });
            var issues = DesignerValidationService.Validate(screen, metadata);
            Assert.That(issues.Any(i => i.Code == "motion-target-missing"), Is.True);
            Assert.That(issues.Any(i => i.Code == "motion-clip-missing"), Is.True);
            Object.DestroyImmediate(metadata);
            Object.DestroyImmediate(screen);
        }
    }
}
