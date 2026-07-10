using emiteat.NexUI.Motion;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Graph
{
    /// <summary>
    /// Composite editor surface: the <see cref="MotionGraphView"/> stretched to fill, with a
    /// <see cref="MotionTimelinePreview"/> docked below. Hosted by <see cref="MotionGraphWindow"/>
    /// (and reusable anywhere a graph + preview pair is needed). Graph edits refresh the preview.
    /// </summary>
    public sealed class MotionGraphPanel : VisualElement
    {
        public MotionGraphView GraphView { get; }
        private readonly MotionTimelinePreview _preview;

        public MotionGraphPanel(UIMotionPreset preset)
        {
            style.flexGrow = 1f;

            GraphView = new MotionGraphView();
            GraphView.style.flexGrow = 1f;

            _preview = new MotionTimelinePreview();
            _preview.style.minHeight = 140f;

            GraphView.GraphEdited += _preview.Refresh;

            Add(GraphView);
            Add(_preview);

            SetPreset(preset);
        }

        public void SetPreset(UIMotionPreset preset)
        {
            GraphView.Populate(preset);
            _preview.SetPreset(preset);
        }

        public void Save() => GraphView.SaveNow();
    }
}
