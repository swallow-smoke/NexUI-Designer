using System.Text;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Motion;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Graph
{
    /// <summary>
    /// Read-only preview for a compiled motion timeline. Compiles the preset via
    /// <see cref="MotionCompiler"/> and, as the scrub slider moves, shows each track's
    /// interpolated value at the scrubbed time plus the total duration. Editor-only math;
    /// it does not drive a live UI element.
    /// </summary>
    public sealed class MotionTimelinePreview : VisualElement
    {
        private readonly Slider _scrub;
        private readonly Label _duration;
        private readonly Label _values;
        private UIMotionPreset _preset;

        public MotionTimelinePreview()
        {
            Add(new Label("Timeline Preview") { name = "SubTitle" });
            _duration = new Label("Total Duration: -");
            _scrub = new Slider("Scrub", 0f, 1f);
            _values = new Label(string.Empty) { style = { whiteSpace = WhiteSpace.Normal } };

            Add(_duration);
            Add(_scrub);
            Add(_values);

            _scrub.RegisterValueChangedCallback(_ => UpdateReadout());
        }

        public void SetPreset(UIMotionPreset preset)
        {
            _preset = preset;
            UpdateReadout();
        }

        public void Refresh() => UpdateReadout();

        private void UpdateReadout()
        {
            if (_preset == null)
            {
                _duration.text = "Total Duration: -";
                _values.text = "No preset.";
                return;
            }

            var timeline = MotionCompiler.Compile(_preset);
            var total = timeline.TotalDuration;
            _duration.text = $"Total Duration: {total:0.###}s";

            var t = _scrub.value * total;
            var sb = new StringBuilder();
            if (timeline.Tracks != null)
            {
                foreach (var track in timeline.Tracks)
                {
                    if (track == null) continue;
                    var local = Mathf.Clamp01(Mathf.InverseLerp(track.Delay, track.Delay + track.Duration, t));
                    var from = track.Keyframes != null && track.Keyframes.Length > 0 ? track.Keyframes[0].Value : 0f;
                    var to = track.Keyframes != null && track.Keyframes.Length > 1
                        ? track.Keyframes[track.Keyframes.Length - 1].Value
                        : from;
                    var value = Mathf.Lerp(from, to, local);
                    sb.AppendLine($"{track.Property}: {value:0.###}  (t={t:0.###}s)");
                }
            }
            _values.text = sb.Length > 0 ? sb.ToString() : "No tracks.";
        }
    }
}
