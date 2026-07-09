using System;
using emiteat.NexUI.MotionClip;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// TODO: convert a Unity <see cref="AnimationClip"/> into a <see cref="UIMotionClip"/>.
    /// Deferred per the Motion Clip Editor's scoped priorities — <see cref="UnityAnimationClipAdapter"/>
    /// (preview-only) ships in this pass, full import does not. Interface + stub only.
    /// </summary>
    public interface IUIMotionClipImporter
    {
        UIMotionClip Import(AnimationClip source);
    }

    public static class UIMotionClipImporter
    {
        public static UIMotionClip Import(AnimationClip source)
            => throw new NotImplementedException(
                "TODO: AnimationClip -> UIMotionClip import is not implemented yet. " +
                "Use UnityAnimationClipAdapter to preview/play an AnimationClip directly in the meantime.");
    }
}
