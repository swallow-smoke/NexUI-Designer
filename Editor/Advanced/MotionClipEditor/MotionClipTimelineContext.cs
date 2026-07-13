using System;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>Shared read-only state a <see cref="MotionClipTimelineView"/> needs from its owning window, grouped so the view's constructor doesn't grow a parameter per feature.</summary>
    public sealed class MotionClipTimelineContext
    {
        public Func<float> GetDuration;
        public Func<int> GetFps;
        public Func<bool> GetSnap;
        public Func<float> GetPreviewTime;

        /// <summary>Owning window, used to convert a control's <c>worldBound</c> into a screen-space rect for anchoring <see cref="EasingBrowserPopup"/>.</summary>
        public Func<EditorWindow> GetHostWindow;
    }
}
