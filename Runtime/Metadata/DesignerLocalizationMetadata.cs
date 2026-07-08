using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// Designer-side linkage between screen elements and game localization keys, plus
    /// an inline preview table. The authoritative runtime data lives in Core's
    /// UIGameLocalizationTable; this record drives editor preview and missing-key checks.
    /// </summary>
    [Serializable]
    public sealed class DesignerLocalizationMetadata
    {
        public string screenId;
        public List<DesignerLocalizationLink> links = new();
        public List<DesignerLocalizationRow> previewRows = new();
    }

    [Serializable]
    public sealed class DesignerLocalizationLink
    {
        public string elementId;
        public string localizationKey;
    }

    [Serializable]
    public sealed class DesignerLocalizationRow
    {
        public string key;
        public string koKR;
        public string enUS;
    }
}
