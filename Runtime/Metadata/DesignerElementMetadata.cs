using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer
{
    [Serializable]
    public sealed class DesignerElementMetadata
    {
        public string elementId;
        public string parentId;
        public List<string> classes = new List<string>();
        public DesignerBindingMetadata binding = new DesignerBindingMetadata();
        public DesignerMotionMetadata motion = new DesignerMotionMetadata();
        public DesignerThemeMetadata theme = new DesignerThemeMetadata();
        public bool locked;
        public bool hiddenInDesigner;
    }
}
