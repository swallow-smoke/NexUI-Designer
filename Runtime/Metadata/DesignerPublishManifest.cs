using System;
using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer
{
    /// <summary>Baseline recorded for one screen at its last publish: the content hashes that were
    /// written to disk, so a later publish can tell whether the Designer, the file, or both changed.</summary>
    [Serializable]
    public sealed class DesignerPublishEntry
    {
        public string screenId;
        public string uxmlHash;
        public string ussHash;
    }

    /// <summary>
    /// Records the last-published content hashes per screen (brief §32/§39.3). This is the "base" of
    /// the three-way sync comparison: it distinguishes a Designer edit (file still matches this) from a
    /// hand-edit of a generated file (file no longer matches this) from a conflict (both diverged). One
    /// shared asset per project.
    /// </summary>
    [CreateAssetMenu(menuName = "NexUI/Designer/Publish Manifest", fileName = "NexUIPublishManifest")]
    public sealed class DesignerPublishManifest : ScriptableObject
    {
        public List<DesignerPublishEntry> entries = new List<DesignerPublishEntry>();

        public DesignerPublishEntry Find(string screenId)
        {
            if (string.IsNullOrEmpty(screenId)) return null;
            for (int i = 0; i < entries.Count; i++)
                if (entries[i] != null && entries[i].screenId == screenId)
                    return entries[i];
            return null;
        }

        public DesignerPublishEntry GetOrCreate(string screenId)
        {
            var entry = Find(screenId);
            if (entry == null)
            {
                entry = new DesignerPublishEntry { screenId = screenId };
                entries.Add(entry);
            }
            return entry;
        }
    }
}
