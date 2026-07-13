using System;
using emiteat.NexUI.Designer.Editor.MotionClipEditor;
using emiteat.NexUI.MotionClip;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Tests.EditMode
{
    /// <summary>Motion Editor UX Phase 1: every animatable property must resolve to exactly one track-hierarchy category, with no silent fallback gaps.</summary>
    public sealed class MotionClipPropertyCategoryTests
    {
        [Test]
        public void EveryPropertyTypeHasACategory()
        {
            foreach (UIMotionClipPropertyType propertyType in Enum.GetValues(typeof(UIMotionClipPropertyType)))
            {
                Assert.DoesNotThrow(() => MotionClipPropertyCategoryUtility.CategoryOf(propertyType));
            }
        }

        [Test]
        public void CanvasGroupAlphaIsVisualCategory()
        {
            Assert.AreEqual(MotionClipPropertyCategory.Visual,
                MotionClipPropertyCategoryUtility.CategoryOf(UIMotionClipPropertyType.CanvasGroupAlpha));
        }

        [TestCase(UIMotionClipPropertyType.AnchoredPosition)]
        [TestCase(UIMotionClipPropertyType.LocalPosition)]
        [TestCase(UIMotionClipPropertyType.LocalRotationZ)]
        [TestCase(UIMotionClipPropertyType.LocalScale)]
        [TestCase(UIMotionClipPropertyType.SizeDelta)]
        public void TransformPropertiesAreTransformCategory(UIMotionClipPropertyType propertyType)
        {
            Assert.AreEqual(MotionClipPropertyCategory.Transform, MotionClipPropertyCategoryUtility.CategoryOf(propertyType));
        }

        [Test]
        public void LocalizationKeyIsNeverEmpty()
        {
            foreach (MotionClipPropertyCategory category in Enum.GetValues(typeof(MotionClipPropertyCategory)))
            {
                Assert.IsFalse(string.IsNullOrEmpty(MotionClipPropertyCategoryUtility.LocalizationKey(category)));
            }
        }

        [Test]
        public void AnimationClip_ImportExport_RoundTripsSupportedRectTransformCurves()
        {
            var source = new AnimationClip { name = "Open", frameRate = 30f };
            AnimationUtility.SetEditorCurve(source,
                EditorCurveBinding.FloatCurve("Panel", typeof(RectTransform), "m_AnchoredPosition.x"),
                AnimationCurve.Linear(0f, 10f, 1f, 30f));
            AnimationUtility.SetEditorCurve(source,
                EditorCurveBinding.FloatCurve("Panel", typeof(RectTransform), "m_AnchoredPosition.y"),
                AnimationCurve.Linear(0f, 20f, 1f, 40f));

            var motion = UIMotionClipImporter.Import(source);
            try
            {
                Assert.AreEqual("Open", motion.clipName);
                Assert.AreEqual(1, motion.tracks.Length);
                Assert.AreEqual("Panel", motion.tracks[0].targetElementId);
                var property = motion.tracks[0].propertyTracks[0];
                Assert.AreEqual(UIMotionClipPropertyType.AnchoredPosition, property.propertyType);
                Assert.AreEqual(new Vector2(10f, 20f), property.keyframes[0].value.vector2Value);
                Assert.AreEqual(new Vector2(30f, 40f), property.keyframes[1].value.vector2Value);

                var exported = UIMotionClipExporter.Export(motion);
                try
                {
                    var x = AnimationUtility.GetEditorCurve(exported,
                        EditorCurveBinding.FloatCurve("Panel", typeof(RectTransform), "m_AnchoredPosition.x"));
                    var y = AnimationUtility.GetEditorCurve(exported,
                        EditorCurveBinding.FloatCurve("Panel", typeof(RectTransform), "m_AnchoredPosition.y"));
                    Assert.NotNull(x);
                    Assert.NotNull(y);
                    Assert.AreEqual(30f, x.Evaluate(1f), .001f);
                    Assert.AreEqual(40f, y.Evaluate(1f), .001f);
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(exported);
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(motion);
                UnityEngine.Object.DestroyImmediate(source);
            }
        }
    }
}
