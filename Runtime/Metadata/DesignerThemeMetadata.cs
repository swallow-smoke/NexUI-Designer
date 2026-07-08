using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer
{
    [Serializable]
    public sealed class DesignerThemeMetadata
    {
        public string themeId;
        public List<string> classes = new List<string>();
        public List<DesignerTokenOverride> tokenOverrides = new List<DesignerTokenOverride>();
    }

    [Serializable]
    public sealed class DesignerTokenOverride
    {
        public string key;
        public string value;
    }
}
