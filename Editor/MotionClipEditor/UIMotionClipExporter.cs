using System;
using emiteat.NexUI.MotionClip;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// TODO: convert a <see cref="UIMotionClip"/> into a Unity <see cref="AnimationClip"/>.
    /// Deferred per the Motion Clip Editor's scoped priorities. Interface + stub only.
    /// </summary>
    public interface IUIMotionClipExporter
    {
        AnimationClip Export(UIMotionClip clip);
    }

    public static class UIMotionClipExporter
    {
        public static AnimationClip Export(UIMotionClip clip)
            => throw new NotImplementedException("TODO: UIMotionClip -> AnimationClip export is not implemented yet.");
    }
}
