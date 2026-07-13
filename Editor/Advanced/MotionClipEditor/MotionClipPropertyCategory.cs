using emiteat.NexUI.MotionClip;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Groups the properties <see cref="UIMotionClip"/> can currently animate into the track-hierarchy
    /// categories described by the Motion Editor UX plan (Transform / Visual / ...). Only the categories
    /// that have at least one backing <see cref="UIMotionClipPropertyType"/> today are populated; Layout/
    /// Text/Effects/Game UI/Event categories require new runtime capabilities (see Architecture-Audit.md
    /// section 5) and are intentionally not listed here yet — showing them empty would be a fake affordance.
    /// </summary>
    public enum MotionClipPropertyCategory
    {
        Transform,
        Visual
    }

    public static class MotionClipPropertyCategoryUtility
    {
        public static MotionClipPropertyCategory CategoryOf(UIMotionClipPropertyType propertyType)
        {
            switch (propertyType)
            {
                case UIMotionClipPropertyType.CanvasGroupAlpha:
                    return MotionClipPropertyCategory.Visual;
                default:
                    return MotionClipPropertyCategory.Transform;
            }
        }

        public static string LocalizationKey(MotionClipPropertyCategory category)
        {
            switch (category)
            {
                case MotionClipPropertyCategory.Visual:
                    return "motionClip.category.visual";
                default:
                    return "motionClip.category.transform";
            }
        }
    }
}
