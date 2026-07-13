using System.Collections.Generic;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.MotionClip;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Captures each target element's pre-preview transform/size once, before the Motion Clip
    /// Editor first mutates the shared Designer preview surface, and can restore it afterwards.
    /// Guards against scrubbed/played poses being left on the canvas after Stop/Close — the saved
    /// Metadata/prefab is unaffected either way (uGUI/UI Toolkit serialization writes from
    /// <c>DesignerElementMetadata.rect</c>, not from live transform state), but the canvas should
    /// still look like the authored screen, not a frozen mid-animation frame, once preview ends.
    /// </summary>
    public sealed class MotionClipPreviewSnapshot
    {
        private struct ElementState
        {
            public bool HasTransform;
            public Vector2 Position;
            public Vector3 Scale;
            public float Rotation;
            public float Opacity;
            public bool HasSize;
            public Vector2 SizeDelta;
        }

        private readonly Dictionary<string, ElementState> _states = new Dictionary<string, ElementState>();

        public bool IsCaptured { get; private set; }

        /// <summary>No-op if already captured for this preview session; call <see cref="Clear"/> first to force a re-capture.</summary>
        public void CaptureIfNeeded(IUISurface surface, UIMotionClip clip)
        {
            if (IsCaptured || surface == null || clip?.tracks == null) return;

            foreach (var track in clip.tracks)
            {
                if (track == null || string.IsNullOrEmpty(track.targetElementId) || _states.ContainsKey(track.targetElementId))
                    continue;

                var target = UIMotionClipTargetResolver.Resolve(surface, track.targetElementId);
                if (target == null) continue;

                var state = new ElementState();
                var transformCap = target.As<IUITransformCapability>();
                if (transformCap != null)
                {
                    state.HasTransform = true;
                    state.Position = transformCap.Position;
                    state.Scale = transformCap.Scale;
                    state.Rotation = transformCap.Rotation;
                    state.Opacity = transformCap.Opacity;
                }

                var sizeCap = target.As<IUISizeCapability>();
                if (sizeCap != null)
                {
                    state.HasSize = true;
                    state.SizeDelta = sizeCap.SizeDelta;
                }

                _states[track.targetElementId] = state;
            }

            IsCaptured = true;
        }

        public void Restore(IUISurface surface)
        {
            if (!IsCaptured || surface == null) return;

            foreach (var pair in _states)
            {
                var target = UIMotionClipTargetResolver.Resolve(surface, pair.Key);
                if (target == null) continue;

                var state = pair.Value;
                if (state.HasTransform)
                {
                    var cap = target.As<IUITransformCapability>();
                    if (cap != null)
                    {
                        cap.Position = state.Position;
                        cap.Scale = state.Scale;
                        cap.Rotation = state.Rotation;
                        cap.Opacity = state.Opacity;
                    }
                }

                if (state.HasSize)
                {
                    var cap = target.As<IUISizeCapability>();
                    if (cap != null) cap.SizeDelta = state.SizeDelta;
                }
            }

            Clear();
        }

        public void Clear()
        {
            _states.Clear();
            IsCaptured = false;
        }
    }
}
