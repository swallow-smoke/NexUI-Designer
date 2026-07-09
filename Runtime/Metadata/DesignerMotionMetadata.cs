using System;
using emiteat.NexUI.Motion;

namespace emiteat.NexUI.Designer
{
    [Serializable]
    public sealed class DesignerMotionMetadata
    {
        /// <summary>
        /// Direct reference to the authoring preset asset this element animates with.
        /// The runtime metadata assembly already references emiteat.NexUI.Motion, so this
        /// is strongly typed (no UnityEditor dependency introduced).
        /// </summary>
        public UIMotionPreset motionPreset;
        public string motionId;
        public string initialVariant;
        public string animateVariant;
        public string exitVariant;
        public string hoverVariant;
        public string pressedVariant;
        public string focusVariant;
    }
}
