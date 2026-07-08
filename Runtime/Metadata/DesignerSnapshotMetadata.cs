using System;
using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// A captured snapshot of a screen's element state, used for snapshot testing and
    /// diffing. Serializable so it can be stored as a JSON/asset baseline.
    /// </summary>
    [Serializable]
    public sealed class UISnapshot
    {
        public string snapshotId;
        public string screenId;
        public string variantId;
        public string themeId;
        public List<UISnapshotElement> elements = new();

        public UISnapshotElement Find(string elementId)
        {
            for (int i = 0; i < elements.Count; i++)
                if (elements[i] != null && elements[i].elementId == elementId)
                    return elements[i];
            return null;
        }
    }

    [Serializable]
    public sealed class UISnapshotElement
    {
        public string elementId;
        public bool visible;
        public bool interactable;
        public string text;
        public float value;
        public Vector2 position;
        public Vector2 size;
        public List<string> classes = new();
    }

    /// <summary>Designer wrapper holding baseline snapshots for a screen.</summary>
    [Serializable]
    public sealed class DesignerSnapshotMetadata
    {
        public string screenId;
        public List<UISnapshot> baselines = new();
    }
}
