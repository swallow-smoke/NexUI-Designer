using System;

namespace emiteat.NexUI.Designer
{
    [Serializable]
    public sealed class DesignerMotionMetadata
    {
        public string motionId;
        public string initialVariant;
        public string animateVariant;
        public string exitVariant;
        public string hoverVariant;
        public string pressedVariant;
        public string focusVariant;
    }
}
