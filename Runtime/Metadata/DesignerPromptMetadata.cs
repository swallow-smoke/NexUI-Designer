using System;
using System.Collections.Generic;
using emiteat.NexUI.Prompt;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// Designer-side authoring record for input prompt actions and their per-device
    /// glyph coverage. Drives the prompt glyph editor and missing-glyph validation.
    /// </summary>
    [Serializable]
    public sealed class DesignerPromptMetadata
    {
        public List<DesignerPromptActionMetadata> actions = new();
    }

    [Serializable]
    public sealed class DesignerPromptActionMetadata
    {
        public string actionId;
        public string displayName;
        public List<DesignerPromptDeviceMetadata> devices = new();
    }

    [Serializable]
    public sealed class DesignerPromptDeviceMetadata
    {
        public UIPromptDevice device;
        public bool hasGlyph;
        public string textFallback;
    }
}
