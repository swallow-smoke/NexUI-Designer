using System.Collections.Generic;
using emiteat.NexUI.Abstractions;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>Groups every <see cref="UIMotionEasing"/> value into the categories the Easing Browser lists (brief §5.7), with a localized per-category usage hint shown in each entry's tooltip.</summary>
    public enum EasingCategory
    {
        Basic,
        Quadratic,
        Cubic,
        Quartic,
        Quintic,
        Sine,
        Exponential,
        Circular,
        Back,
        Elastic,
        Bounce
    }

    public static class EasingCatalog
    {
        private static readonly Dictionary<UIMotionEasing, EasingCategory> Categories = new Dictionary<UIMotionEasing, EasingCategory>
        {
            [UIMotionEasing.Linear] = EasingCategory.Basic,
            [UIMotionEasing.EaseInOut] = EasingCategory.Basic,

            [UIMotionEasing.EaseInQuad] = EasingCategory.Quadratic,
            [UIMotionEasing.EaseOutQuad] = EasingCategory.Quadratic,
            [UIMotionEasing.EaseInOutQuad] = EasingCategory.Quadratic,

            [UIMotionEasing.EaseInCubic] = EasingCategory.Cubic,
            [UIMotionEasing.EaseOutCubic] = EasingCategory.Cubic,
            [UIMotionEasing.EaseInOutCubic] = EasingCategory.Cubic,

            [UIMotionEasing.EaseInQuart] = EasingCategory.Quartic,
            [UIMotionEasing.EaseOutQuart] = EasingCategory.Quartic,
            [UIMotionEasing.EaseInOutQuart] = EasingCategory.Quartic,

            [UIMotionEasing.EaseInQuint] = EasingCategory.Quintic,
            [UIMotionEasing.EaseOutQuint] = EasingCategory.Quintic,
            [UIMotionEasing.EaseInOutQuint] = EasingCategory.Quintic,

            [UIMotionEasing.EaseInSine] = EasingCategory.Sine,
            [UIMotionEasing.EaseOutSine] = EasingCategory.Sine,
            [UIMotionEasing.EaseInOutSine] = EasingCategory.Sine,

            [UIMotionEasing.EaseInExpo] = EasingCategory.Exponential,
            [UIMotionEasing.EaseOutExpo] = EasingCategory.Exponential,
            [UIMotionEasing.EaseInOutExpo] = EasingCategory.Exponential,

            [UIMotionEasing.EaseInCirc] = EasingCategory.Circular,
            [UIMotionEasing.EaseOutCirc] = EasingCategory.Circular,
            [UIMotionEasing.EaseInOutCirc] = EasingCategory.Circular,

            [UIMotionEasing.EaseInBack] = EasingCategory.Back,
            [UIMotionEasing.EaseOutBack] = EasingCategory.Back,
            [UIMotionEasing.EaseInOutBack] = EasingCategory.Back,

            [UIMotionEasing.EaseInElastic] = EasingCategory.Elastic,
            [UIMotionEasing.EaseOutElastic] = EasingCategory.Elastic,
            [UIMotionEasing.EaseInOutElastic] = EasingCategory.Elastic,

            [UIMotionEasing.EaseInBounce] = EasingCategory.Bounce,
            [UIMotionEasing.EaseOutBounce] = EasingCategory.Bounce,
            [UIMotionEasing.EaseInOutBounce] = EasingCategory.Bounce,
        };

        public static EasingCategory CategoryOf(UIMotionEasing easing)
            => Categories.TryGetValue(easing, out var category) ? category : EasingCategory.Basic;

        public static string CategoryLocalizationKey(EasingCategory category) => $"easing.category.{category}";

        /// <summary>Localization key for the one-line "when to use this" hint shown in each entry's tooltip (shared across a category - see Architecture-Audit.md Phase 3 scope note on per-easing descriptions).</summary>
        public static string CategoryHintKey(EasingCategory category) => $"easing.hint.{category}";

        public static IEnumerable<UIMotionEasing> All()
        {
            foreach (var pair in Categories) yield return pair.Key;
        }
    }
}
