using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Serialization
{
    public static class DesignerMetadataUtility
    {
        public static void MarkDirty(Object asset)
        {
            if (asset != null)
                EditorUtility.SetDirty(asset);
        }
    }
}
