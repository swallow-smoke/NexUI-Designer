using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Reorders elements within <see cref="DesignerMetadataAsset.elements"/>. List order is the
    /// single source of truth for sibling/z-order for both the UGUI and UI Toolkit backends
    /// (both serializers iterate <c>metadata.elements</c> in list order when building the saved
    /// hierarchy), and the viewport renders later-in-list elements on top. So reordering the
    /// list is sufficient - no separate per-backend adapter is needed.
    /// </summary>
    public static class UIElementLayerOrder
    {
        public static void BringForward(DesignerMetadataAsset metadata, IReadOnlyList<DesignerElementMetadata> selection)
        {
            if (metadata == null || selection == null || selection.Count == 0) return;
            var list = metadata.elements;
            // Walk from the back of the list to the front so an element never leapfrogs
            // another selected element it was already behind (keeps relative order stable).
            for (int i = list.Count - 2; i >= 0; i--)
            {
                if (!Contains(selection, list[i])) continue;
                if (Contains(selection, list[i + 1])) continue;
                Swap(list, i, i + 1);
            }
        }

        public static void SendBackward(DesignerMetadataAsset metadata, IReadOnlyList<DesignerElementMetadata> selection)
        {
            if (metadata == null || selection == null || selection.Count == 0) return;
            var list = metadata.elements;
            for (int i = 1; i < list.Count; i++)
            {
                if (!Contains(selection, list[i])) continue;
                if (Contains(selection, list[i - 1])) continue;
                Swap(list, i, i - 1);
            }
        }

        public static void BringToFront(DesignerMetadataAsset metadata, IReadOnlyList<DesignerElementMetadata> selection)
        {
            if (metadata == null || selection == null || selection.Count == 0) return;
            var list = metadata.elements;
            var moved = new List<DesignerElementMetadata>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!Contains(selection, list[i])) continue;
                moved.Insert(0, list[i]);
                list.RemoveAt(i);
            }
            list.AddRange(moved);
        }

        public static void SendToBack(DesignerMetadataAsset metadata, IReadOnlyList<DesignerElementMetadata> selection)
        {
            if (metadata == null || selection == null || selection.Count == 0) return;
            var list = metadata.elements;
            var moved = new List<DesignerElementMetadata>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!Contains(selection, list[i])) continue;
                moved.Insert(0, list[i]);
                list.RemoveAt(i);
            }
            list.InsertRange(0, moved);
        }

        private static bool Contains(IReadOnlyList<DesignerElementMetadata> selection, DesignerElementMetadata element)
        {
            for (int i = 0; i < selection.Count; i++)
                if (ReferenceEquals(selection[i], element))
                    return true;
            return false;
        }

        private static void Swap(List<DesignerElementMetadata> list, int a, int b)
        {
            (list[a], list[b]) = (list[b], list[a]);
        }
    }
}
