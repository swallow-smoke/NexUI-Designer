using System.Collections.Generic;
using System;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Productivity;
using emiteat.NexUI.Designer.Editor.Scenario;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Designer.Editor.Validation;
using emiteat.NexUI.Designer.Editor;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    public sealed class ProductivityServiceTests
    {
        private const string TempFolder = "Assets/__NexUIProductivityTests";

        [TearDown]
        public void TearDown()
        {
            if (AssetDatabase.IsValidFolder(TempFolder)) AssetDatabase.DeleteAsset(TempFolder);
        }

        [Test]
        public void ScreenWizard_RejectsInvalidScreenId()
        {
            var request = new DesignerScreenCreationRequest { ScreenId = "123 invalid", Folder = "Assets/Temp" };
            Assert.IsFalse(DesignerScreenCreationService.Validate(request, out var error));
            Assert.IsNotEmpty(error);
        }

        [TestCase(UIRenderBackend.UGUI, DesignerScreenTemplate.FullScreen)]
        [TestCase(UIRenderBackend.UGUI, DesignerScreenTemplate.Popup)]
        [TestCase(UIRenderBackend.UGUI, DesignerScreenTemplate.Modal)]
        [TestCase(UIRenderBackend.UIToolkit, DesignerScreenTemplate.FullScreen)]
        public void ScreenWizard_CreatesConnectedAssets(UIRenderBackend backend, DesignerScreenTemplate template)
        {
            var request = new DesignerScreenCreationRequest
            {
                ScreenName = "Test Screen",
                ScreenId = "Test" + Guid.NewGuid().ToString("N"),
                Backend = backend,
                Template = template,
                Folder = TempFolder + "/" + backend + template,
                CreateTransition = false
            };
            var result = DesignerScreenCreationService.Create(request);
            Assert.IsTrue(result.Success, result.Error);
            Assert.NotNull(result.Screen);
            Assert.NotNull(result.Metadata);
            Assert.AreEqual(request.ScreenId, result.Screen.ScreenId);
            Assert.AreEqual(request.ScreenId, result.Metadata.screenId);
            Assert.IsTrue(result.Metadata.elements.Count > 0);
            Assert.NotNull(result.Screen.backendAsset.asset);
            if (backend == UIRenderBackend.UIToolkit)
                Assert.IsTrue(result.CreatedPaths.Any(x => x.EndsWith(".uss")));
        }

        [Test]
        public void LayoutAnalysis_DetectsVerticalListAndSpacing()
        {
            var items = new List<DesignerElementMetadata>
            {
                Element("a", new Rect(20, 10, 100, 30)),
                Element("b", new Rect(20, 52, 100, 30)),
                Element("c", new Rect(20, 94, 100, 30))
            };
            var analysis = DesignerLayoutAnalysisService.Analyze(items);
            Assert.AreEqual(DesignerDetectedLayout.Vertical, analysis.Layout);
            Assert.AreEqual(12f, analysis.Spacing, .001f);
            Assert.IsTrue(analysis.SameSize);
        }

        [Test]
        public void AnchorRecommendation_PreservesSemanticPosition()
        {
            Assert.AreEqual(DesignerAnchorPreset.TopLeft,
                DesignerAnchorRecommendationService.Recommend(new Rect(10, 10, 100, 50), new Vector2(1920, 1080)));
            Assert.AreEqual(DesignerAnchorPreset.Center,
                DesignerAnchorRecommendationService.Recommend(new Rect(860, 490, 200, 100), new Vector2(1920, 1080)));
            Assert.AreEqual(DesignerAnchorPreset.Stretch,
                DesignerAnchorRecommendationService.Recommend(new Rect(0, 0, 1900, 1060), new Vector2(1920, 1080)));
        }

        [Test]
        public void TransitionPreset_BuildsStaggerAndReverse()
        {
            var metadata = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            metadata.elements.Add(Element("first", new Rect(0, 0, 100, 40)));
            metadata.elements.Add(Element("second", new Rect(0, 50, 100, 40)));
            var settings = new DesignerTransitionSettings { Duration = .2f, StaggerInterval = .1f };
            var open = DesignerTransitionPresetService.Build(metadata, metadata.elements, DesignerTransitionPreset.StaggerList, settings, false);
            var close = DesignerTransitionPresetService.Reverse(open);
            Assert.AreEqual(2, open.tracks.Length);
            Assert.AreEqual(0f, open.tracks[0].propertyTracks[0].keyframes[0].time, .001f);
            Assert.AreEqual(.1f, open.tracks[1].propertyTracks[0].keyframes[0].time, .001f);
            Assert.AreEqual(open.duration, close.tracks[0].propertyTracks[0].keyframes.Last().time, .001f);
            UnityEngine.Object.DestroyImmediate(open);
            UnityEngine.Object.DestroyImmediate(close);
            UnityEngine.Object.DestroyImmediate(metadata);
        }

        [Test]
        public void Scenario_ListBinding_DrivesCollectionPreviewCount()
        {
            var element = Element("inventory", new Rect(0, 0, 100, 100));
            element.binding.valueKey = "inventory.items";
            var binding = new DesignerScenarioBinding("inventory.items", DesignerScenarioValueKind.List)
            {
                listValue = new List<string> { "Sword", "Potion", "Key" }
            };
            var change = ScenarioApplyResolver.Resolve(new[] { element }, new[] { binding }).Single();
            Assert.IsTrue(change.SetPreviewItemCount);
            Assert.AreEqual(3, change.PreviewItemCount);
            Assert.IsFalse(change.SetPreviewValue);
        }

        [Test]
        public void AutoFix_DuplicateId_RenamesSecondElement()
        {
            var metadata = ScriptableObject.CreateInstance<DesignerMetadataAsset>();
            metadata.elements.Add(Element("duplicate", new Rect(0, 0, 100, 40)));
            metadata.elements.Add(Element("duplicate", new Rect(0, 50, 100, 40)));
            var context = new NexUIDesignerContext();
            context.SetMetadata(metadata);
            var issue = new DesignerValidationIssue(DesignerValidationSeverity.Error, "duplicate-element-id", "duplicate", "rename", "screen", "duplicate");
            var fix = DesignerAutoFixService.GetFix(context, issue);
            Assert.NotNull(fix);
            Assert.IsTrue(fix.IsSafe);
            fix.Apply();
            Assert.AreEqual(2, metadata.elements.Select(x => x.elementId).Distinct().Count());
            context.Dispose();
            UnityEngine.Object.DestroyImmediate(metadata);
        }

        private static DesignerElementMetadata Element(string id, Rect rect)
            => new DesignerElementMetadata { elementId = id, rect = rect, binding = new DesignerBindingMetadata() };
    }
}
