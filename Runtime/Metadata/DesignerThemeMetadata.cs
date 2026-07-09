using System;
using System.Collections.Generic;
using emiteat.NexUI.Theme;

namespace emiteat.NexUI.Designer
{
    [Serializable]
    public sealed class DesignerThemeMetadata
    {
        /// <summary>
        /// Optional direct reference to the theme asset (drives the inspector's ObjectField
        /// picker). Strongly typed because the runtime metadata assembly already references
        /// emiteat.NexUI.Theme; introduces no UnityEditor dependency.
        /// </summary>
        public UITheme themeRef;
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
