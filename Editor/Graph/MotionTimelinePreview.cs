using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Graph
{
    public sealed class MotionTimelinePreview : VisualElement
    {
        public MotionTimelinePreview()
        {
            Add(new Label("Timeline Preview"));
            Add(new Slider("Scrub", 0f, 1f));
        }
    }
}
