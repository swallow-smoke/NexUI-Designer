using System;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.MotionClip;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Minimal timeline visual for one <see cref="UIMotionClipPropertyTrack"/>: a ruler with
    /// draggable keyframe dots (retime by dragging) plus a per-keyframe field row (time/value/
    /// delete) below it. Deliberately simple — an MVP, not a polished graph editor.
    /// </summary>
    public sealed class MotionClipTimelineView : VisualElement
    {
        private const float RulerHeight = 28f;

        private readonly UIMotionClipPropertyTrack _track;
        private readonly Func<float> _getDuration;
        private readonly Action _onChanged;
        private readonly Action<int> _onDelete;
        private readonly VisualElement _ruler;
        private readonly VisualElement _rows;

        public MotionClipTimelineView(UIMotionClipPropertyTrack track, Func<float> getDuration, Action onChanged, Action<int> onDelete)
        {
            _track = track;
            _getDuration = getDuration;
            _onChanged = onChanged;
            _onDelete = onDelete;

            _ruler = new VisualElement { name = "Ruler" };
            _ruler.style.height = RulerHeight;
            _ruler.style.position = Position.Relative;
            _ruler.AddToClassList("nexui-list");
            Add(_ruler);

            _rows = new VisualElement { name = "KeyframeRows" };
            Add(_rows);

            Rebuild();
        }

        private void Rebuild()
        {
            _ruler.Clear();
            _rows.Clear();

            var duration = Mathf.Max(_getDuration(), 0.0001f);
            var keyframes = _track.keyframes ?? Array.Empty<UIMotionClipKeyframe>();

            for (var i = 0; i < keyframes.Length; i++)
            {
                var index = i;
                var dot = new VisualElement();
                dot.AddToClassList("nexui-resize-handle");
                dot.style.position = Position.Absolute;
                dot.style.top = 8f;
                dot.style.left = Length.Percent(Mathf.Clamp01(keyframes[index].time / duration) * 100f);
                _ruler.Add(dot);

                dot.AddManipulator(new PointerDraggable(delta =>
                {
                    var rulerWidth = _ruler.resolvedStyle.width;
                    if (rulerWidth <= 0f) return;
                    var deltaTime = delta.x / rulerWidth * duration;
                    var current = _track.keyframes[index];
                    current.time = Mathf.Clamp(current.time + deltaTime, 0f, duration);
                    _track.keyframes[index] = current;
                    dot.style.left = Length.Percent(Mathf.Clamp01(current.time / duration) * 100f);
                    _onChanged?.Invoke();
                }, () => Rebuild()));
            }

            for (var i = 0; i < keyframes.Length; i++)
                _rows.Add(BuildRow(i, keyframes[i], duration));

            if (keyframes.Length == 0)
                _rows.Add(new Label("No keyframes yet.") { name = "EmptyStateText" });
        }

        private VisualElement BuildRow(int index, UIMotionClipKeyframe keyframe, float duration)
        {
            var row = new VisualElement();
            row.AddToClassList("nexui-inline-row");

            var timeField = new FloatField("Time") { value = keyframe.time };
            timeField.style.width = 90f;
            timeField.RegisterValueChangedCallback(evt =>
            {
                var current = _track.keyframes[index];
                current.time = Mathf.Clamp(evt.newValue, 0f, duration);
                _track.keyframes[index] = current;
                _onChanged?.Invoke();
            });
            row.Add(timeField);

            row.Add(BuildValueField(index, keyframe));

            var easing = new EnumField("Easing", keyframe.easing);
            easing.style.width = 140f;
            easing.RegisterValueChangedCallback(evt =>
            {
                var current = _track.keyframes[index];
                current.easing = (UIMotionEasing)evt.newValue;
                _track.keyframes[index] = current;
                _onChanged?.Invoke();
            });
            row.Add(easing);

            var deleteButton = new Button(() => _onDelete?.Invoke(index)) { text = "Delete" };
            deleteButton.AddToClassList("nexui-toolbar-button");
            deleteButton.AddToClassList("nexui-button-secondary");
            row.Add(deleteButton);

            return row;
        }

        private VisualElement BuildValueField(int index, UIMotionClipKeyframe keyframe)
        {
            switch (_track.propertyType)
            {
                case UIMotionClipPropertyType.LocalRotationZ:
                case UIMotionClipPropertyType.CanvasGroupAlpha:
                {
                    var field = new FloatField("Value") { value = keyframe.value.floatValue };
                    field.style.width = 120f;
                    field.RegisterValueChangedCallback(evt =>
                    {
                        var current = _track.keyframes[index];
                        current.value = UIMotionClipValue.Float(evt.newValue);
                        _track.keyframes[index] = current;
                        _onChanged?.Invoke();
                    });
                    return field;
                }
                case UIMotionClipPropertyType.LocalScale:
                {
                    var field = new Vector3Field("Value") { value = keyframe.value.vector3Value };
                    field.style.width = 220f;
                    field.RegisterValueChangedCallback(evt =>
                    {
                        var current = _track.keyframes[index];
                        current.value = UIMotionClipValue.FromVector3(evt.newValue);
                        _track.keyframes[index] = current;
                        _onChanged?.Invoke();
                    });
                    return field;
                }
                default:
                {
                    var field = new Vector2Field("Value") { value = keyframe.value.vector2Value };
                    field.style.width = 180f;
                    field.RegisterValueChangedCallback(evt =>
                    {
                        var current = _track.keyframes[index];
                        current.value = UIMotionClipValue.FromVector2(evt.newValue);
                        _track.keyframes[index] = current;
                        _onChanged?.Invoke();
                    });
                    return field;
                }
            }
        }

        /// <summary>Small drag manipulator reporting per-move screen-space delta; commits via <paramref name="onUp"/> on release.</summary>
        private sealed class PointerDraggable : Manipulator
        {
            private readonly Action<Vector2> _onDrag;
            private readonly Action _onUp;
            private Vector2 _lastPosition;
            private bool _dragging;

            public PointerDraggable(Action<Vector2> onDrag, Action onUp)
            {
                _onDrag = onDrag;
                _onUp = onUp;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<PointerDownEvent>(OnPointerDown);
                target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            }

            private void OnPointerDown(PointerDownEvent evt)
            {
                _dragging = true;
                _lastPosition = evt.position;
                target.CapturePointer(evt.pointerId);
            }

            private void OnPointerMove(PointerMoveEvent evt)
            {
                if (!_dragging) return;
                var delta = (Vector2)evt.position - _lastPosition;
                _lastPosition = evt.position;
                _onDrag?.Invoke(delta);
            }

            private void OnPointerUp(PointerUpEvent evt)
            {
                if (!_dragging) return;
                _dragging = false;
                target.ReleasePointer(evt.pointerId);
                _onUp?.Invoke();
            }
        }
    }
}
