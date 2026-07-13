using System;
using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Timeline visual for one <see cref="UIMotionClipPropertyTrack"/>: a ruler (second/frame ticks +
    /// playhead) with draggable, multi-selectable keyframe dots, plus a per-keyframe field row
    /// (time/value/easing/delete) below it. Deliberately simple — an MVP, not a polished dope sheet /
    /// curve editor (that's Phase 3 of Architecture-Audit.md's plan).
    /// </summary>
    public sealed class MotionClipTimelineView : VisualElement
    {
        private const float RulerHeight = 32f;

        private readonly UIMotionClipPropertyTrack _track;
        private readonly MotionClipTimelineContext _ctx;
        private readonly Action _onChanged;
        private readonly Action<IReadOnlyList<int>> _onDeleteKeyframes;
        private readonly bool _locked;
        private readonly VisualElement _ruler;
        private readonly VisualElement _rows;
        private readonly HashSet<int> _selected = new HashSet<int>();
        private int _selectionAnchor = -1;

        public MotionClipTimelineView(UIMotionClipPropertyTrack track, MotionClipTimelineContext ctx,
            Action onChanged, Action<IReadOnlyList<int>> onDeleteKeyframes, bool locked)
        {
            _track = track;
            _ctx = ctx;
            _onChanged = onChanged;
            _onDeleteKeyframes = onDeleteKeyframes;
            _locked = locked;

            focusable = true;
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            _ruler = new VisualElement { name = "Ruler" };
            _ruler.style.height = RulerHeight;
            _ruler.style.position = Position.Relative;
            _ruler.AddToClassList("nexui-list");
            Add(_ruler);

            _rows = new VisualElement { name = "KeyframeRows" };
            Add(_rows);

            Rebuild();
        }

        /// <summary>Re-renders the playhead only (called every editor-update tick during playback) without rebuilding keyframe dots/rows.</summary>
        public void RefreshPlayhead()
        {
            var playhead = _ruler.Q<VisualElement>("Playhead");
            if (playhead == null) return;
            var duration = Mathf.Max(_ctx.GetDuration(), 0.0001f);
            playhead.style.left = Length.Percent(Mathf.Clamp01(_ctx.GetPreviewTime() / duration) * 100f);
        }

        private void Rebuild()
        {
            _ruler.Clear();
            _rows.Clear();

            var duration = Mathf.Max(_ctx.GetDuration(), 0.0001f);
            var fps = Mathf.Max(_ctx.GetFps(), 1);
            var keyframes = _track.keyframes ?? Array.Empty<UIMotionClipKeyframe>();

            BuildTicks(duration);

            var playhead = new VisualElement { name = "Playhead" };
            playhead.AddToClassList("nexui-timeline-playhead");
            playhead.style.position = Position.Absolute;
            playhead.style.left = Length.Percent(Mathf.Clamp01(_ctx.GetPreviewTime() / duration) * 100f);
            playhead.pickingMode = PickingMode.Ignore;
            _ruler.Add(playhead);

            for (var i = 0; i < keyframes.Length; i++)
            {
                var index = i;
                var dot = new VisualElement();
                dot.AddToClassList("nexui-resize-handle");
                if (_selected.Contains(index)) dot.AddToClassList("nexui-timeline-keyframe-selected");
                dot.style.position = Position.Absolute;
                dot.style.top = 8f;
                dot.style.left = Length.Percent(Mathf.Clamp01(keyframes[index].time / duration) * 100f);
                DesignerTooltip.Set(dot, "tooltip.motionClip.keyframeDot");
                _ruler.Add(dot);

                dot.RegisterCallback<PointerDownEvent>(evt => OnKeyframePointerDown(evt, index));

                if (_locked) continue;

                dot.AddManipulator(new PointerDraggable(delta =>
                {
                    if (!_selected.Contains(index)) SelectSingle(index);

                    var rulerWidth = _ruler.resolvedStyle.width;
                    if (rulerWidth <= 0f) return;
                    var deltaTime = delta.x / rulerWidth * duration;

                    foreach (var selectedIndex in _selected)
                    {
                        var current = _track.keyframes[selectedIndex];
                        current.time = SnapIfEnabled(Mathf.Clamp(current.time + deltaTime, 0f, duration), fps);
                        _track.keyframes[selectedIndex] = current;
                    }
                    _onChanged?.Invoke();
                    RefreshDotPositions(duration);
                }, SortSelectedAndRebuild));

                dot.AddManipulator(new ContextualMenuManipulator(menu =>
                {
                    menu.menu.AppendAction(DesignerLocalization.T("motionClip.toolbar.deleteKeyframe"),
                        _ => DeleteSelectionOrIndex(index));
                }));
            }

            for (var i = 0; i < keyframes.Length; i++)
                _rows.Add(BuildRow(i, keyframes[i], duration));

            if (keyframes.Length == 0)
                _rows.Add(new Label(DesignerLocalization.T("motionClip.toolbar.noKeyframes")) { name = "EmptyStateText" });
        }

        private void BuildTicks(float duration)
        {
            var seconds = Mathf.CeilToInt(duration);
            var step = seconds > 20 ? 5 : (seconds > 8 ? 2 : 1);
            for (var s = 0; s <= seconds; s += step)
            {
                if (s > duration) break;
                var tick = new VisualElement();
                tick.AddToClassList("nexui-timeline-tick");
                tick.style.position = Position.Absolute;
                tick.style.left = Length.Percent(Mathf.Clamp01(s / duration) * 100f);
                _ruler.Add(tick);

                var frame = Mathf.RoundToInt(s * Mathf.Max(_ctx.GetFps(), 1));
                var label = new Label($"{s}s / {frame}f");
                label.AddToClassList("nexui-timeline-tick-label");
                label.style.position = Position.Absolute;
                label.style.left = Length.Percent(Mathf.Clamp01(s / duration) * 100f);
                _ruler.Add(label);
            }
        }

        /// <summary>Keyframes must stay time-sorted for <c>UIMotionClipEvaluator</c>'s segment scan; re-sorts after a drag and remaps the selection to the new indices.</summary>
        private void SortSelectedAndRebuild()
        {
            var keyframes = _track.keyframes;
            var order = Enumerable.Range(0, keyframes.Length).ToArray();
            Array.Sort(order, (a, b) => keyframes[a].time.CompareTo(keyframes[b].time));

            var remap = new Dictionary<int, int>();
            var sorted = new UIMotionClipKeyframe[keyframes.Length];
            for (var newIndex = 0; newIndex < order.Length; newIndex++)
            {
                sorted[newIndex] = keyframes[order[newIndex]];
                remap[order[newIndex]] = newIndex;
            }
            _track.keyframes = sorted;

            var remappedSelection = _selected.Where(remap.ContainsKey).Select(i => remap[i]).ToList();
            _selected.Clear();
            foreach (var index in remappedSelection) _selected.Add(index);

            Rebuild();
        }

        private void RefreshDotPositions(float duration)
        {
            var dots = _ruler.Children().Where(c => c.ClassListContains("nexui-resize-handle")).ToList();
            for (var i = 0; i < dots.Count && i < _track.keyframes.Length; i++)
                dots[i].style.left = Length.Percent(Mathf.Clamp01(_track.keyframes[i].time / duration) * 100f);
        }

        private float SnapIfEnabled(float time, int fps)
        {
            if (!_ctx.GetSnap()) return time;
            return Mathf.Round(time * fps) / fps;
        }

        private void OnKeyframePointerDown(PointerDownEvent evt, int index)
        {
            Focus();
            if (evt.shiftKey && _selectionAnchor >= 0)
            {
                _selected.Clear();
                var lo = Mathf.Min(_selectionAnchor, index);
                var hi = Mathf.Max(_selectionAnchor, index);
                for (var i = lo; i <= hi; i++) _selected.Add(i);
            }
            else if (evt.ctrlKey || evt.commandKey)
            {
                if (!_selected.Add(index)) _selected.Remove(index);
                _selectionAnchor = index;
            }
            else
            {
                SelectSingle(index);
            }
            RefreshSelectionHighlight();
        }

        private void SelectSingle(int index)
        {
            _selected.Clear();
            _selected.Add(index);
            _selectionAnchor = index;
        }

        private void RefreshSelectionHighlight()
        {
            var dots = _ruler.Children().Where(c => c.ClassListContains("nexui-resize-handle")).ToList();
            for (var i = 0; i < dots.Count; i++)
            {
                if (_selected.Contains(i)) dots[i].AddToClassList("nexui-timeline-keyframe-selected");
                else dots[i].RemoveFromClassList("nexui-timeline-keyframe-selected");
            }
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (_locked) return;
            if ((evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace) && _selected.Count > 0)
            {
                DeleteSelectionOrIndex(-1);
                evt.StopPropagation();
            }
        }

        private void DeleteSelectionOrIndex(int fallbackIndex)
        {
            var indices = _selected.Count > 0 ? _selected.ToList() : new List<int> { fallbackIndex };
            if (indices.Count == 0 || indices[0] < 0) return;
            _selected.Clear();
            _onDeleteKeyframes?.Invoke(indices);
        }

        private VisualElement BuildRow(int index, UIMotionClipKeyframe keyframe, float duration)
        {
            var row = new VisualElement();
            row.AddToClassList("nexui-inline-row");
            if (_selected.Contains(index)) row.AddToClassList("nexui-timeline-row-selected");

            var timeField = new FloatField("Time") { value = keyframe.time };
            timeField.style.width = 90f;
            timeField.SetEnabled(!_locked);
            timeField.RegisterValueChangedCallback(evt =>
            {
                var current = _track.keyframes[index];
                current.time = SnapIfEnabled(Mathf.Clamp(evt.newValue, 0f, duration), Mathf.Max(_ctx.GetFps(), 1));
                _track.keyframes[index] = current;
                _onChanged?.Invoke();
            });
            row.Add(timeField);

            var valueField = BuildValueField(index, keyframe);
            valueField.SetEnabled(!_locked);
            row.Add(valueField);

            row.Add(BuildEasingControl(index, keyframe));

            var deleteButton = new Button(() => _onDeleteKeyframes?.Invoke(new[] { index }))
            { text = DesignerLocalization.T("motionClip.toolbar.deleteKeyframe") };
            deleteButton.AddToClassList("nexui-toolbar-button");
            deleteButton.AddToClassList("nexui-button-secondary");
            deleteButton.SetEnabled(!_locked);
            DesignerTooltip.Set(deleteButton, "tooltip.motionClip.deleteKeyframe");
            row.Add(deleteButton);

            return row;
        }

        private VisualElement BuildEasingControl(int index, UIMotionClipKeyframe keyframe)
        {
            var row = new VisualElement();
            row.AddToClassList("nexui-inline-row");
            row.style.alignItems = Align.Center;

            var preview = new EasingCurvePreview(keyframe.easing);
            row.Add(preview);

            var isCustom = keyframe.curve != null;
            var easingButton = new Button { text = isCustom ? DesignerLocalization.T("easing.custom") : keyframe.easing.ToString() };
            easingButton.AddToClassList("nexui-toolbar-button");
            easingButton.AddToClassList("nexui-button-secondary");
            easingButton.SetEnabled(!_locked);
            DesignerTooltip.Set(easingButton, "tooltip.motionClip.easingButton");

            CurveField curveField = null;

            easingButton.clicked += () =>
            {
                var hostWindow = _ctx.GetHostWindow?.Invoke();
                var worldBound = easingButton.worldBound;
                var screenRect = hostWindow != null
                    ? new Rect(hostWindow.position.x + worldBound.x, hostWindow.position.y + worldBound.y, worldBound.width, worldBound.height)
                    : new Rect(worldBound.x, worldBound.y, worldBound.width, worldBound.height);

                var popup = new EasingBrowserPopup(
                    onSelectEasing: selected =>
                    {
                        var current = _track.keyframes[index];
                        current.easing = selected;
                        current.curve = null;
                        _track.keyframes[index] = current;
                        preview.SetEasing(selected);
                        easingButton.text = selected.ToString();
                        if (curveField != null) curveField.style.display = DisplayStyle.None;
                        _onChanged?.Invoke();
                    },
                    onSelectCustomCurve: () =>
                    {
                        var current = _track.keyframes[index];
                        if (current.curve == null) current.curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                        _track.keyframes[index] = current;
                        easingButton.text = DesignerLocalization.T("easing.custom");
                        if (curveField != null)
                        {
                            curveField.SetValueWithoutNotify(current.curve);
                            curveField.style.display = DisplayStyle.Flex;
                        }
                        _onChanged?.Invoke();
                    });
                UnityEditor.PopupWindow.Show(screenRect, popup);
            };
            row.Add(easingButton);

            curveField = new CurveField { value = keyframe.curve, style = { width = 100f, display = isCustom ? DisplayStyle.Flex : DisplayStyle.None } };
            curveField.SetEnabled(!_locked);
            DesignerTooltip.Set(curveField, "tooltip.motionClip.customCurve");
            curveField.RegisterValueChangedCallback(evt =>
            {
                var current = _track.keyframes[index];
                current.curve = evt.newValue;
                _track.keyframes[index] = current;
                _onChanged?.Invoke();
            });
            row.Add(curveField);

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
