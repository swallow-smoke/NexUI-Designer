using System.Collections.Generic;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor.Scenario
{
    /// <summary>
    /// Records live Designer edits into a scenario timeline (brief §35.3). While recording, every
    /// element change whose bound value/text/visibility channel differs from the last recorded value
    /// appends a timeline key at the elapsed time. The result is a normal
    /// <see cref="DesignerScenarioAsset"/> timeline that replays through the already-tested
    /// <see cref="ScenarioTimelineEvaluator"/> path — record once, replay/scrub forever. It only
    /// records what the timeline can replay (binding value channels); forced-state/language are left
    /// to explicit Capture.
    /// </summary>
    public sealed class ScenarioRecorder
    {
        private NexUIDesignerContext _context;
        private DesignerScenarioAsset _target;
        private double _startTime;
        private readonly Dictionary<string, DesignerScenarioBinding> _lastByKey = new Dictionary<string, DesignerScenarioBinding>();

        public bool IsRecording { get; private set; }
        public int RecordedKeyCount => _target != null ? _target.timelineKeys.Count : 0;
        public float ElapsedTime => IsRecording ? (float)(EditorApplication.timeSinceStartup - _startTime) : 0f;

        public void Start(DesignerScenarioAsset target, NexUIDesignerContext context)
        {
            if (IsRecording || target == null || context?.Metadata == null) return;

            _target = target;
            _context = context;
            _startTime = EditorApplication.timeSinceStartup;
            _lastByKey.Clear();

            Undo.RecordObject(target, "Record Scenario");
            target.useTimeline = true;
            target.timelineKeys.Clear();
            target.screenId = context.Metadata.screenId ?? string.Empty;

            // Baseline at t=0 so replay starts from the exact state recording began in.
            foreach (var element in context.Metadata.elements)
                Append(0f, element);

            IsRecording = true;
            _context.ElementChanged += OnElementChanged;
            EditorUtility.SetDirty(target);
        }

        public void Stop()
        {
            if (!IsRecording) return;
            if (_context != null) _context.ElementChanged -= OnElementChanged;
            IsRecording = false;

            if (_target != null)
            {
                var maxTime = 0f;
                foreach (var key in _target.timelineKeys)
                    if (key != null && key.time > maxTime) maxTime = key.time;
                _target.timelineDuration = System.Math.Max(_target.timelineDuration, maxTime + 0.5f);
                _target.timelineKeys.Sort((a, b) => a.time.CompareTo(b.time));
                EditorUtility.SetDirty(_target);
            }

            _target = null;
            _context = null;
        }

        private void OnElementChanged(DesignerElementMetadata element)
        {
            if (!IsRecording) return;
            Append(ElapsedTime, element);
        }

        private void Append(float time, DesignerElementMetadata element)
        {
            if (element == null) return;
            foreach (var binding in ScenarioBindingExtractor.FromElement(element))
            {
                // Skip a key that repeats the value already active for this binding key (dedupe no-op edits).
                if (_lastByKey.TryGetValue(binding.key, out var last) && ScenarioBindingExtractor.SameValue(last, binding))
                    continue;
                _lastByKey[binding.key] = binding;

                _target.timelineKeys.Add(new DesignerScenarioTimelineKey(time, binding.key, binding.kind)
                {
                    boolValue = binding.boolValue,
                    numberValue = binding.numberValue,
                    textValue = binding.textValue
                });
            }
        }
    }
}
