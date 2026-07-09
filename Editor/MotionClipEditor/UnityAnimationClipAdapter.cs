using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Lets an existing Unity <see cref="AnimationClip"/> be sampled/previewed the same way a
    /// <see cref="MotionClip.UIMotionClip"/> is, without converting it. UGUI/GameObject targets
    /// only — <see cref="AnimationClip.SampleAnimation"/> has no UI Toolkit equivalent (no
    /// Animator on a VisualElement), which is why full clip conversion (see
    /// <see cref="UIMotionClipImporter"/>/<see cref="UIMotionClipExporter"/>) is left as future
    /// work rather than attempted here.
    /// </summary>
    public static class UnityAnimationClipAdapter
    {
        public static void Evaluate(AnimationClip clip, GameObject target, float time)
        {
            if (clip == null || target == null) return;
            clip.SampleAnimation(target, time);
        }
    }
}
