using System;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// Explicit gamepad/keyboard directional navigation links for one element (brief §29). Empty
    /// string means "no link authored in that direction" - never null, so metadata authored before
    /// this field existed deserializes to "no navigation links" rather than throwing.
    /// </summary>
    [Serializable]
    public sealed class DesignerFocusMetadata
    {
        public string upElementId = string.Empty;
        public string downElementId = string.Empty;
        public string leftElementId = string.Empty;
        public string rightElementId = string.Empty;

        /// <summary>Screen-level default: at most one element per screen should have this set (not enforced here - see Focus Navigation validation).</summary>
        public bool isDefaultFocus;
    }
}
