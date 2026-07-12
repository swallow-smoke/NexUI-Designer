using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Hierarchy-editing surface of <see cref="NexUIDesignerContext"/>: reparenting, sibling
    /// reordering, wrap-in-container and hierarchy-aware delete. Kept in its own partial file so
    /// the parenting concern is separable from the (already large) core context. Every mutation
    /// here is a single Undo group and re-validates/redraws through the existing dirty pipeline.
    /// </summary>
    public sealed partial class NexUIDesignerContext
    {
        /// <summary>Ordered direct children of <paramref name="element"/> (siblingIndex order).</summary>
        public List<DesignerElementMetadata> GetOrderedChildren(DesignerElementMetadata element)
            => DesignerHierarchyUtility.GetOrderedChildren(Metadata, element);

        public List<DesignerElementMetadata> GetRootElements()
            => DesignerHierarchyUtility.GetRootElements(Metadata);

        public List<DesignerElementMetadata> GetDescendants(DesignerElementMetadata element)
            => DesignerHierarchyUtility.GetDescendants(Metadata, element);

        /// <summary>Whether reparenting <paramref name="element"/> under <paramref name="newParent"/> is legal (no self/cycle).</summary>
        public bool CanReparent(DesignerElementMetadata element, DesignerElementMetadata newParent)
        {
            if (Metadata == null || element == null) return false;
            var newParentId = newParent != null ? newParent.elementId : string.Empty;
            return !DesignerHierarchyUtility.WouldCreateCycle(Metadata, element.elementId, newParentId);
        }

        /// <summary>
        /// Reparents a single element, appending it as the last child of <paramref name="newParent"/>
        /// (or a root when null). By default the element keeps its on-screen (canvas) position;
        /// pass <paramref name="keepCanvasPosition"/> = false to instead preserve its local offset
        /// (so it snaps relative to the new parent's content origin).
        /// </summary>
        public void ReparentElement(DesignerElementMetadata element, DesignerElementMetadata newParent, bool keepCanvasPosition = true)
        {
            if (element == null) return;
            ReparentElements(new[] { element }, newParent, -1, keepCanvasPosition);
        }

        /// <summary>
        /// Reparents a set of elements under <paramref name="newParent"/> (null ⇒ root) at
        /// <paramref name="insertIndex"/> (&lt;0 ⇒ append) as one Undo group. Multi-select rules:
        /// only the top-most selected nodes move (a selected child of another selected node is
        /// skipped so subtrees stay intact), their relative order is preserved, and any move that
        /// would create a cycle is dropped. Canvas positions are preserved by default.
        /// </summary>
        public void ReparentElements(IReadOnlyList<DesignerElementMetadata> elements, DesignerElementMetadata newParent, int insertIndex = -1, bool keepCanvasPosition = true)
        {
            if (Metadata == null || elements == null || elements.Count == 0) return;
            var newParentId = newParent != null ? newParent.elementId : string.Empty;

            // Keep only the top-most nodes of the selection (drop any node that is a descendant of
            // another node being moved) and reject illegal targets, preserving selection order.
            var movers = new List<DesignerElementMetadata>();
            foreach (var e in elements)
            {
                if (e == null) continue;
                if (DesignerHierarchyUtility.WouldCreateCycle(Metadata, e.elementId, newParentId)) continue;
                var isNestedUnderAnotherMover = false;
                foreach (var other in elements)
                {
                    if (other == null || other == e) continue;
                    if (DesignerHierarchyUtility.IsDescendant(Metadata, e.elementId, other.elementId)) { isNestedUnderAnotherMover = true; break; }
                }
                if (!isNestedUnderAnotherMover && !movers.Contains(e))
                    movers.Add(e);
            }
            if (movers.Count == 0) return;

            RecordMetadata("Reparent NexUI Element" + (movers.Count > 1 ? "s" : string.Empty));

            // Preserve canvas position: capture before changing parents, reapply after.
            Dictionary<DesignerElementMetadata, Vector2> canvasPositions = null;
            if (keepCanvasPosition)
            {
                canvasPositions = new Dictionary<DesignerElementMetadata, Vector2>();
                foreach (var m in movers)
                    canvasPositions[m] = m.rect.position;
            }

            // Existing children of the target parent (excluding the movers) define the insertion order.
            var siblings = DesignerHierarchyUtility.GetOrderedChildren(Metadata, newParentId);
            siblings.RemoveAll(movers.Contains);
            var at = insertIndex < 0 || insertIndex > siblings.Count ? siblings.Count : insertIndex;
            siblings.InsertRange(at, movers);

            foreach (var m in movers)
                m.parentId = newParentId;
            for (int i = 0; i < siblings.Count; i++)
                siblings[i].siblingIndex = i;

            DesignerHierarchyUtility.NormalizeSiblingIndices(Metadata);

            // Reapply canvas positions (delta the whole subtree so children keep their offsets).
            if (canvasPositions != null)
            {
                foreach (var pair in canvasPositions)
                {
                    var delta = pair.Value - pair.Key.rect.position;
                    if (delta == Vector2.zero) continue;
                    pair.Key.rect = new Rect(pair.Value, pair.Key.rect.size);
                    foreach (var d in DesignerHierarchyUtility.GetDescendants(Metadata, pair.Key))
                        d.rect = new Rect(d.rect.position + delta, d.rect.size);
                }
            }

            MarkMetadataDirty();
            RaiseSelectionChanged();
        }

        /// <summary>Detaches an element to the canvas root, keeping its on-screen position.</summary>
        public void MoveToRoot(DesignerElementMetadata element) => ReparentElement(element, null);

        public void MoveSelectionToRoot()
        {
            if (_selection.Count > 0)
                ReparentElements(new List<DesignerElementMetadata>(_selection), null);
        }

        /// <summary>Sets an element's sibling index within its current parent (clamped), re-normalizing the group.</summary>
        public void SetSiblingIndex(DesignerElementMetadata element, int index)
        {
            if (Metadata == null || element == null) return;
            var siblings = DesignerHierarchyUtility.GetOrderedChildren(Metadata, element.parentId);
            if (!siblings.Contains(element)) return;
            index = Mathf.Clamp(index, 0, siblings.Count - 1);
            if (siblings.IndexOf(element) == index) return;

            RecordMetadata("Reorder NexUI Sibling");
            siblings.Remove(element);
            siblings.Insert(index, element);
            for (int i = 0; i < siblings.Count; i++)
                siblings[i].siblingIndex = i;
            MarkMetadataDirty();
            RaiseSelectionChanged();
        }

        public void MoveSiblingBy(DesignerElementMetadata element, int delta)
        {
            if (Metadata == null || element == null || delta == 0) return;
            var siblings = DesignerHierarchyUtility.GetOrderedChildren(Metadata, element.parentId);
            var current = siblings.IndexOf(element);
            if (current < 0) return;
            SetSiblingIndex(element, current + delta);
        }

        /// <summary>
        /// Wraps the current selection in a new container element sized to their canvas bounding
        /// box (+ optional padding). The container is parented to the selection's common parent,
        /// the selected elements become its children (canvas positions preserved), and the new
        /// container is selected. One Undo group.
        /// </summary>
        public DesignerElementMetadata WrapSelectionInContainer(float padding = 0f, string containerType = "Container")
        {
            if (Metadata == null || _selection.Count == 0) return null;
            RecordMetadata("Wrap NexUI Elements In Container");

            var members = new List<DesignerElementMetadata>(_selection);
            var bounds = DesignerCoordinateUtility.GetCanvasBounds(members);
            bounds = new Rect(bounds.x - padding, bounds.y - padding, bounds.width + padding * 2f, bounds.height + padding * 2f);

            // Common parent = shared parentId if all members share one, else root.
            string commonParent = members[0].parentId ?? string.Empty;
            for (int i = 1; i < members.Count; i++)
                if ((members[i].parentId ?? string.Empty) != commonParent) { commonParent = string.Empty; break; }

            var container = new DesignerElementMetadata
            {
                elementId = UniqueElementId("container" + _groupCounter++),
                displayName = "Container",
                elementType = containerType,
                rect = bounds,
                parentId = commonParent,
                tint = new Color(0f, 0f, 0f, 0f),
                clipChildren = false
            };
            Metadata.elements.Add(container);

            // Reparent members into the container (append order = current sibling order), keep pos.
            ReparentElementsInternal(members, container.elementId, true);

            DesignerHierarchyUtility.NormalizeSiblingIndices(Metadata);
            MarkMetadataDirty();
            SelectMetadata(container);
            return container;
        }

        /// <summary>Undo-less core of a reparent (caller owns the Undo record); preserves canvas position when asked.</summary>
        private void ReparentElementsInternal(IReadOnlyList<DesignerElementMetadata> members, string newParentId, bool keepCanvasPosition)
        {
            var siblings = DesignerHierarchyUtility.GetOrderedChildren(Metadata, newParentId);
            var memberSet = new HashSet<DesignerElementMetadata>(members);
            siblings.RemoveAll(memberSet.Contains);
            foreach (var m in members)
            {
                if (m == null) continue;
                m.parentId = newParentId;
                siblings.Add(m);
            }
            for (int i = 0; i < siblings.Count; i++)
                siblings[i].siblingIndex = i;
        }

        /// <summary>
        /// Deletes an element. When <paramref name="withChildren"/> (default) the whole subtree is
        /// removed; otherwise the direct children are re-parented to the deleted element's parent
        /// (keeping their canvas positions) before it is removed. One Undo group.
        /// </summary>
        public void DeleteElementHierarchical(DesignerElementMetadata element, bool withChildren = true)
        {
            if (Metadata == null || element == null) return;
            RecordMetadata(withChildren ? "Delete NexUI Element (with children)" : "Delete NexUI Element (keep children)");

            if (withChildren)
            {
                var subtree = DesignerHierarchyUtility.GetDescendants(Metadata, element);
                foreach (var d in subtree)
                    Metadata.elements.Remove(d);
            }
            else
            {
                var children = DesignerHierarchyUtility.GetOrderedChildren(Metadata, element);
                ReparentElementsInternal(children, element.parentId ?? string.Empty, true);
            }

            Metadata.elements.Remove(element);
            _selection.Remove(element);
            DesignerHierarchyUtility.NormalizeSiblingIndices(Metadata);
            MarkMetadataDirty();
            RaiseSelectionChanged();
        }
    }
}
