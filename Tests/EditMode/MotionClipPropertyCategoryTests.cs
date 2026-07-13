using System;
using emiteat.NexUI.Designer.Editor.MotionClipEditor;
using emiteat.NexUI.MotionClip;
using NUnit.Framework;

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
    }
}
