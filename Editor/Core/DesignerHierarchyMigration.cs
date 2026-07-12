using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Forward-only migration of a <see cref="DesignerMetadataAsset"/> to the current schema.
    ///
    /// v0 → v1 (hierarchy): pre-hierarchy assets have no explicit sibling indices. Because element
    /// rects were - and still are - stored in absolute canvas space, no <i>position</i> conversion
    /// is needed: the migration only assigns contiguous sibling indices derived from the existing
    /// <see cref="DesignerMetadataAsset.elements"/> order (the order the viewport already drew in),
    /// so an existing screen is visually identical before and after. The schemaVersion stamp makes
    /// the migration idempotent - it never runs twice on the same asset.
    /// </summary>
    public static class DesignerHierarchyMigration
    {
        /// <summary>
        /// Migrates <paramref name="asset"/> in place if needed. When <paramref name="recordUndo"/>
        /// is true the change is Undo-tracked and the asset marked dirty (interactive open path);
        /// pass false from pure/test contexts. Returns true if the asset was changed.
        /// </summary>
        public static bool Migrate(DesignerMetadataAsset asset, bool recordUndo = true)
        {
            if (asset == null) return false;
            if (asset.schemaVersion >= DesignerMetadataAsset.CurrentSchemaVersion)
            {
                // Already current: still normalize defensively (cheap, only records if it changed).
                return NormalizeOnly(asset, recordUndo);
            }

            if (recordUndo) Undo.RecordObject(asset, "Migrate NexUI Metadata");

            // v0 → v1: seed sibling indices from current list order within each parent group,
            // then normalize (contiguous, cycle/dangling-safe).
            var perParentCounter = new System.Collections.Generic.Dictionary<string, int>();
            foreach (var e in asset.elements)
            {
                if (e == null) continue;
                var key = e.parentId ?? string.Empty;
                perParentCounter.TryGetValue(key, out var next);
                e.siblingIndex = next;
                perParentCounter[key] = next + 1;
            }
            DesignerHierarchyUtility.NormalizeSiblingIndices(asset);

            asset.schemaVersion = DesignerMetadataAsset.CurrentSchemaVersion;
            if (recordUndo) EditorUtility.SetDirty(asset);
            return true;
        }

        private static bool NormalizeOnly(DesignerMetadataAsset asset, bool recordUndo)
        {
            var changed = DesignerHierarchyUtility.NormalizeSiblingIndices(asset);
            if (changed && recordUndo)
            {
                Undo.RecordObject(asset, "Normalize NexUI Hierarchy");
                EditorUtility.SetDirty(asset);
            }
            return changed;
        }
    }
}
