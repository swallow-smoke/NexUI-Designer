using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Components;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Pure (no Unity-Editor state, no side effects) parent/child hierarchy computations over a
    /// <see cref="DesignerMetadataAsset"/>. The <b>source of truth</b> for the tree is each
    /// element's <see cref="DesignerElementMetadata.parentId"/> + <see cref="DesignerElementMetadata.siblingIndex"/>;
    /// no redundant children list is stored. Ordering falls back to the stable
    /// <see cref="DesignerMetadataAsset.elements"/> index so pre-hierarchy assets (all
    /// siblingIndex == 0) still list deterministically.
    ///
    /// Kept deliberately allocation-light and framework-free so it is unit-testable in EditMode
    /// without opening a window. Undo-aware mutation lives in the context; this only computes.
    /// </summary>
    public static class DesignerHierarchyUtility
    {
        /// <summary>Element types that are containers by design intent, per the component registry (single source of truth).</summary>
        public static bool IsContainerType(string elementType)
            => DesignerComponentRegistry.IsContainer(elementType);

        /// <summary>
        /// Whether <paramref name="element"/> may hold authored children (declares at least one
        /// slot), per its registry descriptor. Leaf types (Label/Image/ProgressBar...) return
        /// false and raise a Validation warning if given children.
        /// </summary>
        public static bool CanHaveChildren(DesignerElementMetadata element)
            => element != null && DesignerComponentRegistry.CanHaveChildren(element.elementType);

        public static bool IsRoot(DesignerElementMetadata element)
            => element != null && string.IsNullOrEmpty(element.parentId);

        /// <summary>Direct children of <paramref name="parentId"/> (null/empty ⇒ roots), ordered by siblingIndex then list order.</summary>
        public static List<DesignerElementMetadata> GetOrderedChildren(DesignerMetadataAsset asset, string parentId)
        {
            var result = new List<DesignerElementMetadata>();
            if (asset == null) return result;
            var wantRoot = string.IsNullOrEmpty(parentId);
            for (int i = 0; i < asset.elements.Count; i++)
            {
                var e = asset.elements[i];
                if (e == null) continue;
                var eParent = e.parentId ?? string.Empty;
                if (wantRoot ? eParent.Length == 0 : eParent == parentId)
                    result.Add(e);
            }
            SortBySibling(asset, result);
            return result;
        }

        public static List<DesignerElementMetadata> GetOrderedChildren(DesignerMetadataAsset asset, DesignerElementMetadata parent)
            => GetOrderedChildren(asset, parent != null ? parent.elementId : null);

        public static List<DesignerElementMetadata> GetRootElements(DesignerMetadataAsset asset)
            => GetOrderedChildren(asset, (string)null);

        /// <summary>Stable ordering: primary key siblingIndex, tie-broken by original list index so equal indices keep authoring order.</summary>
        private static void SortBySibling(DesignerMetadataAsset asset, List<DesignerElementMetadata> items)
        {
            items.Sort((a, b) =>
            {
                if (a.siblingIndex != b.siblingIndex) return a.siblingIndex.CompareTo(b.siblingIndex);
                return asset.elements.IndexOf(a).CompareTo(asset.elements.IndexOf(b));
            });
        }

        /// <summary>All descendants of <paramref name="element"/> (depth-first, ordered), excluding the element itself.</summary>
        public static List<DesignerElementMetadata> GetDescendants(DesignerMetadataAsset asset, DesignerElementMetadata element)
        {
            var result = new List<DesignerElementMetadata>();
            if (asset == null || element == null) return result;
            CollectDescendants(asset, element.elementId, result, new HashSet<string>());
            return result;
        }

        private static void CollectDescendants(DesignerMetadataAsset asset, string parentId, List<DesignerElementMetadata> into, HashSet<string> guard)
        {
            if (!guard.Add(parentId)) return; // cycle guard - never recurse into an already-visited node
            foreach (var child in GetOrderedChildren(asset, parentId))
            {
                into.Add(child);
                CollectDescendants(asset, child.elementId, into, guard);
            }
        }

        /// <summary>True when <paramref name="candidate"/> is <paramref name="ancestor"/> itself or lies below it in the tree.</summary>
        public static bool IsSelfOrDescendant(DesignerMetadataAsset asset, DesignerElementMetadata candidate, DesignerElementMetadata ancestor)
        {
            if (candidate == null || ancestor == null) return false;
            if (candidate == ancestor || candidate.elementId == ancestor.elementId) return true;
            return IsDescendant(asset, candidate.elementId, ancestor.elementId);
        }

        /// <summary>True when <paramref name="descendantId"/> is somewhere below <paramref name="ancestorId"/>.</summary>
        public static bool IsDescendant(DesignerMetadataAsset asset, string descendantId, string ancestorId)
        {
            if (asset == null || string.IsNullOrEmpty(descendantId) || string.IsNullOrEmpty(ancestorId)) return false;
            var current = asset.Find(descendantId);
            var guard = new HashSet<string>();
            while (current != null && !string.IsNullOrEmpty(current.parentId))
            {
                if (!guard.Add(current.elementId)) return false; // corrupt cycle - stop
                if (current.parentId == ancestorId) return true;
                current = asset.Find(current.parentId);
            }
            return false;
        }

        /// <summary>
        /// Whether reparenting <paramref name="elementId"/> under <paramref name="newParentId"/>
        /// would be illegal: self-parenting, or making an element a child of its own descendant
        /// (which would form a cycle). Reparenting to root (null/empty) is always allowed.
        /// </summary>
        public static bool WouldCreateCycle(DesignerMetadataAsset asset, string elementId, string newParentId)
        {
            if (asset == null || string.IsNullOrEmpty(elementId)) return false;
            if (string.IsNullOrEmpty(newParentId)) return false;       // → root, safe
            if (elementId == newParentId) return true;                 // self-parent
            // Illegal iff the intended parent is the element itself or one of its descendants.
            return newParentId == elementId || IsDescendant(asset, newParentId, elementId);
        }

        /// <summary>Depth from root (root elements = 0).</summary>
        public static int GetDepth(DesignerMetadataAsset asset, DesignerElementMetadata element)
        {
            if (asset == null || element == null) return 0;
            var depth = 0;
            var current = element;
            var guard = new HashSet<string>();
            while (current != null && !string.IsNullOrEmpty(current.parentId) && guard.Add(current.elementId))
            {
                depth++;
                current = asset.Find(current.parentId);
            }
            return depth;
        }

        public static int CountChildren(DesignerMetadataAsset asset, DesignerElementMetadata element)
        {
            if (asset == null || element == null) return 0;
            var count = 0;
            foreach (var e in asset.elements)
                if (e != null && e.parentId == element.elementId) count++;
            return count;
        }

        /// <summary>
        /// Rewrites every element's <see cref="DesignerElementMetadata.siblingIndex"/> to a
        /// contiguous 0..n-1 per parent, preserving current visual order (siblingIndex then list
        /// order). Also detaches parentIds that reference a missing element (→ root) so the tree
        /// never has dangling links. Returns true when any value actually changed (so callers can
        /// skip a needless dirty/undo record).
        /// </summary>
        public static bool NormalizeSiblingIndices(DesignerMetadataAsset asset)
        {
            if (asset == null) return false;
            var changed = false;

            // 1) Detach dangling parentIds first so those elements normalize as roots.
            foreach (var e in asset.elements)
            {
                if (e == null || string.IsNullOrEmpty(e.parentId)) continue;
                if (asset.Find(e.parentId) == null)
                {
                    e.parentId = string.Empty;
                    changed = true;
                }
            }

            // 2) Break any residual cycles by detaching the offending node to root.
            foreach (var e in asset.elements)
            {
                if (e == null || string.IsNullOrEmpty(e.parentId)) continue;
                if (IsDescendant(asset, e.parentId, e.elementId) || e.parentId == e.elementId)
                {
                    e.parentId = string.Empty;
                    changed = true;
                }
            }

            // 3) Contiguous, gap-free sibling indices per parent group.
            var groups = new Dictionary<string, List<DesignerElementMetadata>>();
            foreach (var e in asset.elements)
            {
                if (e == null) continue;
                var key = e.parentId ?? string.Empty;
                if (!groups.TryGetValue(key, out var list))
                    groups[key] = list = new List<DesignerElementMetadata>();
                list.Add(e);
            }
            foreach (var pair in groups)
            {
                SortBySibling(asset, pair.Value);
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    if (pair.Value[i].siblingIndex != i)
                    {
                        pair.Value[i].siblingIndex = i;
                        changed = true;
                    }
                }
            }
            return changed;
        }
    }
}
