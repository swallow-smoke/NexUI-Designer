using System;
using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    public interface IUIMotionClipImporter
    {
        UIMotionClip Import(AnimationClip source);
    }

    /// <summary>Converts supported Transform, RectTransform and CanvasGroup curves into a backend-neutral Motion Clip.</summary>
    public static class UIMotionClipImporter
    {
        private sealed class Curves
        {
            public readonly Dictionary<int, AnimationCurve> Components = new Dictionary<int, AnimationCurve>();
        }

        public static UIMotionClip Import(AnimationClip source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var grouped = new Dictionary<(string path, UIMotionClipPropertyType type), Curves>();
            foreach (var binding in AnimationUtility.GetCurveBindings(source))
            {
                if (!TryMap(binding, out var type, out var component)) continue;
                var key = (binding.path ?? string.Empty, type);
                if (!grouped.TryGetValue(key, out var curves)) grouped[key] = curves = new Curves();
                curves.Components[component] = AnimationUtility.GetEditorCurve(source, binding);
            }

            var tracks = new List<UIMotionClipTrack>();
            foreach (var targetGroup in grouped.GroupBy(x => x.Key.path))
            {
                var properties = new List<UIMotionClipPropertyTrack>();
                foreach (var entry in targetGroup)
                {
                    var times = entry.Value.Components.Values.SelectMany(x => x.keys.Select(k => k.time)).Distinct().OrderBy(x => x).ToArray();
                    if (times.Length == 0) continue;
                    var keys = times.Select(time => new UIMotionClipKeyframe(time,
                        Evaluate(entry.Key.type, entry.Value.Components, time))).ToArray();
                    properties.Add(new UIMotionClipPropertyTrack { propertyType = entry.Key.type, keyframes = keys });
                }
                if (properties.Count == 0) continue;
                var id = string.IsNullOrEmpty(targetGroup.Key) ? "root" : targetGroup.Key.Replace('\\', '/').Split('/').Last();
                tracks.Add(new UIMotionClipTrack { targetElementId = id, propertyTracks = properties.ToArray() });
            }

            var clip = ScriptableObject.CreateInstance<UIMotionClip>();
            clip.name = source.name + " Motion";
            clip.clipName = source.name;
            clip.duration = Mathf.Max(.01f, source.length);
            clip.fps = Mathf.Max(1, Mathf.RoundToInt(source.frameRate));
            clip.loop = AnimationUtility.GetAnimationClipSettings(source).loopTime;
            clip.workAreaEnd = clip.duration;
            clip.tracks = tracks.ToArray();
            return clip;
        }

        private static UIMotionClipValue Evaluate(UIMotionClipPropertyType type, Dictionary<int, AnimationCurve> curves, float time)
        {
            float V(int component, float fallback) => curves.TryGetValue(component, out var curve) ? curve.Evaluate(time) : fallback;
            switch (type)
            {
                case UIMotionClipPropertyType.AnchoredPosition:
                case UIMotionClipPropertyType.SizeDelta: return UIMotionClipValue.FromVector2(new Vector2(V(0, 0f), V(1, 0f)));
                case UIMotionClipPropertyType.LocalPosition: return UIMotionClipValue.FromVector3(new Vector3(V(0, 0f), V(1, 0f), V(2, 0f)));
                case UIMotionClipPropertyType.LocalScale: return UIMotionClipValue.FromVector3(new Vector3(V(0, 1f), V(1, 1f), V(2, 1f)));
                case UIMotionClipPropertyType.LocalRotationZ:
                case UIMotionClipPropertyType.CanvasGroupAlpha: return UIMotionClipValue.Float(V(0, type == UIMotionClipPropertyType.CanvasGroupAlpha ? 1f : 0f));
                default: return UIMotionClipValue.Float(0f);
            }
        }

        private static bool TryMap(EditorCurveBinding binding, out UIMotionClipPropertyType type, out int component)
        {
            type = default;
            component = Component(binding.propertyName);
            var p = binding.propertyName ?? string.Empty;
            if (p.StartsWith("m_AnchoredPosition.")) type = UIMotionClipPropertyType.AnchoredPosition;
            else if (p.StartsWith("m_LocalPosition.")) type = UIMotionClipPropertyType.LocalPosition;
            else if (p.StartsWith("m_LocalScale.")) type = UIMotionClipPropertyType.LocalScale;
            else if (p.StartsWith("m_SizeDelta.")) type = UIMotionClipPropertyType.SizeDelta;
            else if (p == "m_Alpha") { type = UIMotionClipPropertyType.CanvasGroupAlpha; component = 0; }
            else if (p.EndsWith("EulerAnglesRaw.z") || p.EndsWith("EulerAnglesBaked.z")) { type = UIMotionClipPropertyType.LocalRotationZ; component = 0; }
            else return false;
            return component >= 0;
        }

        private static int Component(string property)
        {
            if (property.EndsWith(".x")) return 0;
            if (property.EndsWith(".y")) return 1;
            if (property.EndsWith(".z")) return 2;
            return -1;
        }
    }
}
