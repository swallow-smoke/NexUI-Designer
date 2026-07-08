using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// A point-in-time measurement of NexUI runtime cost. Collected by the profiler
    /// service from the live UIManager and displayed / exported by the profiler panel.
    /// </summary>
    [Serializable]
    public sealed class UIProfilerSnapshot
    {
        public int openedScreenCount;
        public int elementCount;
        public int bindingCount;
        public int activeMotionCount;

        public float lastScreenOpenMs;
        public float lastScreenCloseMs;
        public float lastThemeApplyMs;
        public float lastBindingUpdateMs;
    }

    /// <summary>A recorded series of profiler snapshots for a session.</summary>
    [Serializable]
    public sealed class DesignerProfilerMetadata
    {
        public List<UIProfilerSnapshot> records = new();
    }
}
