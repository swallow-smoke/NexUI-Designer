using System;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    public interface IUIMotionClipExporter
    {
        AnimationClip Export(UIMotionClip clip);
    }

    /// <summary>Exports Motion Clip properties to standard Unity animation curves for uGUI GameObjects.</summary>
    public static class UIMotionClipExporter
    {
        public static AnimationClip Export(UIMotionClip clip)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));
            var animation = new AnimationClip { name = clip.clipName, frameRate = Mathf.Max(1, clip.fps) };
            foreach (var track in clip.tracks ?? Array.Empty<UIMotionClipTrack>())
            {
                if (track == null) continue;
                var path = track.targetElementId == "root" ? string.Empty : track.targetElementId ?? string.Empty;
                foreach (var property in track.propertyTracks ?? Array.Empty<UIMotionClipPropertyTrack>())
                    Write(animation, path, property);
            }
            var settings = AnimationUtility.GetAnimationClipSettings(animation);
            settings.loopTime = clip.loop;
            AnimationUtility.SetAnimationClipSettings(animation, settings);
            return animation;
        }

        private static void Write(AnimationClip clip, string path, UIMotionClipPropertyTrack track)
        {
            if (track?.keyframes == null || track.keyframes.Length == 0) return;
            switch (track.propertyType)
            {
                case UIMotionClipPropertyType.AnchoredPosition:
                    Set(clip, path, typeof(RectTransform), "m_AnchoredPosition.x", track, 0);
                    Set(clip, path, typeof(RectTransform), "m_AnchoredPosition.y", track, 1); break;
                case UIMotionClipPropertyType.LocalPosition:
                    Set(clip, path, typeof(Transform), "m_LocalPosition.x", track, 0);
                    Set(clip, path, typeof(Transform), "m_LocalPosition.y", track, 1);
                    Set(clip, path, typeof(Transform), "m_LocalPosition.z", track, 2); break;
                case UIMotionClipPropertyType.LocalRotationZ:
                    Set(clip, path, typeof(Transform), "localEulerAnglesRaw.z", track, 0); break;
                case UIMotionClipPropertyType.LocalScale:
                    Set(clip, path, typeof(Transform), "m_LocalScale.x", track, 0);
                    Set(clip, path, typeof(Transform), "m_LocalScale.y", track, 1);
                    Set(clip, path, typeof(Transform), "m_LocalScale.z", track, 2); break;
                case UIMotionClipPropertyType.SizeDelta:
                    Set(clip, path, typeof(RectTransform), "m_SizeDelta.x", track, 0);
                    Set(clip, path, typeof(RectTransform), "m_SizeDelta.y", track, 1); break;
                case UIMotionClipPropertyType.CanvasGroupAlpha:
                    Set(clip, path, typeof(CanvasGroup), "m_Alpha", track, 0); break;
            }
        }

        private static void Set(AnimationClip clip, string path, Type targetType, string propertyName,
            UIMotionClipPropertyTrack track, int component)
        {
            var keys = new Keyframe[track.keyframes.Length];
            for (var i = 0; i < keys.Length; i++)
                keys[i] = new Keyframe(track.keyframes[i].time, Value(track.keyframes[i].value, component));
            var curve = new AnimationCurve(keys);
            for (var i = 0; i < keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            }
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, targetType, propertyName), curve);
        }

        private static float Value(UIMotionClipValue value, int component)
        {
            switch (value.valueType)
            {
                case UIMotionClipValueType.Vector2: return component == 0 ? value.vector2Value.x : value.vector2Value.y;
                case UIMotionClipValueType.Vector3: return component == 0 ? value.vector3Value.x : component == 1 ? value.vector3Value.y : value.vector3Value.z;
                default: return value.floatValue;
            }
        }
    }
}
