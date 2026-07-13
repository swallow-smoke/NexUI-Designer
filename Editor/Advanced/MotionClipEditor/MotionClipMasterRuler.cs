using System;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Clip-level ruler shown once above the per-property-track timelines: the Work Area band (if
    /// enabled) and every named <see cref="UIMotionClipMarker"/>, plus a playhead. Markers: click to
    /// jump the scrub time there, right-click for Rename/Delete. Work Area bounds themselves are
    /// edited via the toolbar fields in <see cref="MotionClipEditorWindow"/>, not here.
    /// </summary>
    public sealed class MotionClipMasterRuler : VisualElement
    {
        private const float Height = 22f;

        private readonly Func<UIMotionClip> _getClip;
        private readonly Func<float> _getPreviewTime;
        private readonly Action<float> _onJumpToTime;
        private readonly Action _onMarkersChanged;

        private readonly VisualElement _workAreaBand;
        private readonly VisualElement _markerLayer;
        private readonly VisualElement _playhead;

        public MotionClipMasterRuler(Func<UIMotionClip> getClip, Func<float> getPreviewTime,
            Action<float> onJumpToTime, Action onMarkersChanged)
        {
            _getClip = getClip;
            _getPreviewTime = getPreviewTime;
            _onJumpToTime = onJumpToTime;
            _onMarkersChanged = onMarkersChanged;

            name = "MasterRuler";
            style.height = Height;
            style.position = Position.Relative;
            AddToClassList("nexui-list");

            _workAreaBand = new VisualElement { name = "WorkAreaBand" };
            _workAreaBand.AddToClassList("nexui-timeline-workarea");
            _workAreaBand.style.position = Position.Absolute;
            _workAreaBand.pickingMode = PickingMode.Ignore;
            Add(_workAreaBand);

            _markerLayer = new VisualElement { name = "MarkerLayer" };
            _markerLayer.style.position = Position.Absolute;
            _markerLayer.style.left = 0;
            _markerLayer.style.right = 0;
            _markerLayer.style.top = 0;
            _markerLayer.style.bottom = 0;
            Add(_markerLayer);

            _playhead = new VisualElement { name = "Playhead" };
            _playhead.AddToClassList("nexui-timeline-playhead");
            _playhead.style.position = Position.Absolute;
            _playhead.pickingMode = PickingMode.Ignore;
            Add(_playhead);

            RegisterCallback<PointerDownEvent>(OnRulerPointerDown);

            Rebuild();
        }

        public void RefreshPlayhead()
        {
            var clip = _getClip();
            if (clip == null) return;
            var duration = Mathf.Max(clip.duration, 0.0001f);
            _playhead.style.left = Length.Percent(Mathf.Clamp01(_getPreviewTime() / duration) * 100f);
        }

        public void Rebuild()
        {
            _markerLayer.Clear();
            var clip = _getClip();
            if (clip == null)
            {
                _workAreaBand.style.display = DisplayStyle.None;
                return;
            }

            var duration = Mathf.Max(clip.duration, 0.0001f);

            if (clip.useWorkArea)
            {
                var start = Mathf.Clamp01(clip.workAreaStart / duration);
                var end = Mathf.Clamp01(clip.workAreaEnd / duration);
                _workAreaBand.style.display = DisplayStyle.Flex;
                _workAreaBand.style.left = Length.Percent(Mathf.Min(start, end) * 100f);
                _workAreaBand.style.width = Length.Percent(Mathf.Abs(end - start) * 100f);
            }
            else
            {
                _workAreaBand.style.display = DisplayStyle.None;
            }

            for (var i = 0; i < clip.markers.Length; i++)
            {
                var index = i;
                var marker = clip.markers[i];
                var flag = new VisualElement();
                flag.AddToClassList("nexui-timeline-marker");
                flag.style.position = Position.Absolute;
                flag.style.left = Length.Percent(Mathf.Clamp01(marker.time / duration) * 100f);
                DesignerTooltip.Set(flag, "tooltip.motionClip.markerFlag");
                _markerLayer.Add(flag);

                flag.RegisterCallback<PointerDownEvent>(evt =>
                {
                    if (evt.button == 0) _onJumpToTime?.Invoke(clip.markers[index].time);
                    evt.StopPropagation();
                });

                flag.AddManipulator(new ContextualMenuManipulator(menu =>
                {
                    menu.menu.AppendAction(DesignerLocalization.T("motionClip.toolbar.renameMarker"), _ => BeginRename(clip, index));
                    menu.menu.AppendAction(DesignerLocalization.T("motionClip.toolbar.deleteMarker"), _ => DeleteMarker(clip, index));
                }));
            }

            RefreshPlayhead();
        }

        private void BeginRename(UIMotionClip clip, int index)
        {
            var field = new TextField { value = clip.markers[index].name };
            field.style.position = Position.Absolute;
            field.style.left = Length.Percent(Mathf.Clamp01(clip.markers[index].time / Mathf.Max(clip.duration, 0.0001f)) * 100f);
            field.style.width = 100f;
            Add(field);
            field.Focus();

            var committed = false;
            void Commit()
            {
                if (committed) return;
                committed = true;

                var marker = clip.markers[index];
                marker.name = string.IsNullOrWhiteSpace(field.value) ? marker.name : field.value.Trim();
                clip.markers[index] = marker;
                Remove(field);
                Undo.RecordObject(clip, "Rename Motion Clip Marker");
                EditorUtility.SetDirty(clip);
                _onMarkersChanged?.Invoke();
            }

            field.RegisterCallback<FocusOutEvent>(_ => Commit());
            field.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) Commit();
            });
        }

        private void DeleteMarker(UIMotionClip clip, int index)
        {
            Undo.RecordObject(clip, "Delete Motion Clip Marker");
            var markers = new System.Collections.Generic.List<UIMotionClipMarker>(clip.markers);
            markers.RemoveAt(index);
            clip.markers = markers.ToArray();
            EditorUtility.SetDirty(clip);
            _onMarkersChanged?.Invoke();
        }

        private void OnRulerPointerDown(PointerDownEvent evt)
        {
            var clip = _getClip();
            if (clip == null) return;
            var width = resolvedStyle.width;
            if (width <= 0f) return;
            var t = Mathf.Clamp01(evt.localPosition.x / width) * clip.duration;
            _onJumpToTime?.Invoke(t);
        }
    }
}
