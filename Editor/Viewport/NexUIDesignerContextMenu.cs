using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Designer.Editor.MotionClipEditor;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Viewport
{
    /// <summary>
    /// Right-click menu for the Designer canvas. Built fresh on every right-click (no cached
    /// state) so it always reflects the current selection. When multiple elements overlap the
    /// click point, a flat "Select Element/&lt;name&gt;" list is prefixed to let the user pick
    /// exactly which one they meant, Photoshop/Figma style - <see cref="GenericMenu"/> has no
    /// support for a richer hover-preview list, so this is the pragmatic MVP.
    /// </summary>
    public static class NexUIDesignerContextMenu
    {
        public static void Show(NexUIDesignerContext context, Vector2 canvasPoint, Action<DesignerElementMetadata> requestRename)
        {
            var hits = HitTest(context, canvasPoint);
            var menu = new GenericMenu();

            if (hits.Count == 0)
            {
                BuildEmptyCanvasMenu(menu, context, canvasPoint);
            }
            else
            {
                if (hits.Count > 1)
                {
                    foreach (var hit in hits)
                    {
                        var captured = hit;
                        menu.AddItem(new GUIContent("Select Element/" + Label(captured)), false, () => context.SelectMetadata(captured));
                    }
                }

                var primary = hits[0];
                if (!context.IsSelected(primary))
                    context.SelectMetadata(primary);

                BuildElementMenu(menu, context, primary, requestRename);
            }

            menu.ShowAsContext();
        }

        private static List<DesignerElementMetadata> HitTest(NexUIDesignerContext context, Vector2 point)
        {
            var result = new List<DesignerElementMetadata>();
            if (context.Metadata == null) return result;
            // Later-in-list elements render on top, so walk back-to-front for a front-to-back hit order.
            for (int i = context.Metadata.elements.Count - 1; i >= 0; i--)
            {
                var element = context.Metadata.elements[i];
                if (element == null || element.hiddenInDesigner) continue;
                if (element.rect.Contains(point))
                    result.Add(element);
            }
            return result;
        }

        private static string Label(DesignerElementMetadata element)
            => string.IsNullOrEmpty(element.displayName) ? element.elementId : element.displayName;

        private static void BuildEmptyCanvasMenu(GenericMenu menu, NexUIDesignerContext context, Vector2 canvasPoint)
        {
            menu.AddItem(new GUIContent("Create Panel"), false, () => CreateAt(context, DesignerElementType.Panel, canvasPoint));
            menu.AddItem(new GUIContent("Create Button"), false, () => CreateAt(context, DesignerElementType.Button, canvasPoint));
            menu.AddItem(new GUIContent("Create Text"), false, () => CreateAt(context, DesignerElementType.Label, canvasPoint));
            menu.AddSeparator("");

            if (context.HasClipboard)
                menu.AddItem(new GUIContent("Paste"), false, () => context.PasteSelection());
            else
                menu.AddDisabledItem(new GUIContent("Paste"));

            menu.AddSeparator("");

            if (context.Metadata != null && context.Metadata.elements.Count > 0)
                menu.AddItem(new GUIContent("Select All"), false, context.SelectAll);
            else
                menu.AddDisabledItem(new GUIContent("Select All"));

            if (context.SelectedElements.Count > 0)
                menu.AddItem(new GUIContent("Clear Selection"), false, context.ClearSelection);
            else
                menu.AddDisabledItem(new GUIContent("Clear Selection"));
        }

        private static void BuildElementMenu(GenericMenu menu, NexUIDesignerContext context, DesignerElementMetadata primary, Action<DesignerElementMetadata> requestRename)
        {
            menu.AddItem(new GUIContent("Select"), false, () => context.SelectMetadata(primary));
            menu.AddItem(new GUIContent("Add to Selection"), false, () => context.AddToSelection(primary));

            var children = context.GetChildren(primary);
            if (children.Count > 0)
                menu.AddItem(new GUIContent("Select Children"), false, () => context.SelectChildren(primary));
            else
                menu.AddDisabledItem(new GUIContent("Select Children"));

            if (!string.IsNullOrEmpty(primary.parentId))
                menu.AddItem(new GUIContent("Select Parent"), false, () => context.SelectParent(primary));
            else
                menu.AddDisabledItem(new GUIContent("Select Parent"));

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Duplicate"), false, () => context.DuplicateSelection());
            menu.AddItem(new GUIContent("Delete"), false, () => context.DeleteSelection());
            menu.AddItem(new GUIContent("Rename"), false, () => requestRename?.Invoke(primary));

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Bring Forward"), false, context.BringSelectionForward);
            menu.AddItem(new GUIContent("Send Backward"), false, context.SendSelectionBackward);
            menu.AddItem(new GUIContent("Bring To Front"), false, context.BringSelectionToFront);
            menu.AddItem(new GUIContent("Send To Back"), false, context.SendSelectionToBack);

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Align/Left"), false, () => context.AlignSelection("left"));
            menu.AddItem(new GUIContent("Align/Center X"), false, () => context.AlignSelection("centerX"));
            menu.AddItem(new GUIContent("Align/Right"), false, () => context.AlignSelection("right"));
            menu.AddItem(new GUIContent("Align/Top"), false, () => context.AlignSelection("top"));
            menu.AddItem(new GUIContent("Align/Center Y"), false, () => context.AlignSelection("centerY"));
            menu.AddItem(new GUIContent("Align/Bottom"), false, () => context.AlignSelection("bottom"));

            if (context.SelectedElements.Count >= 3)
            {
                menu.AddItem(new GUIContent("Distribute/Horizontal"), false, context.DistributeSelectionHorizontal);
                menu.AddItem(new GUIContent("Distribute/Vertical"), false, context.DistributeSelectionVertical);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Distribute/Horizontal"));
                menu.AddDisabledItem(new GUIContent("Distribute/Vertical"));
            }

            menu.AddSeparator("");
            if (context.SelectedElements.Count >= 2)
                menu.AddItem(new GUIContent("Group"), false, () => context.GroupSelection());
            else
                menu.AddDisabledItem(new GUIContent("Group"));

            if (children.Count > 0)
                menu.AddItem(new GUIContent("Ungroup"), false, () => context.UngroupSelection());
            else
                menu.AddDisabledItem(new GUIContent("Ungroup"));

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Create Motion Clip From Selection"), false, () => OpenMotionClipEditor(context, primary));
        }

        private static void CreateAt(NexUIDesignerContext context, DesignerElementType type, Vector2 canvasPoint)
        {
            var element = context.CreateMetadataElement(type);
            if (element == null) return;
            var r = element.rect;
            r.position = canvasPoint;
            context.UpdateSelectedRect(r);
        }

        private static void OpenMotionClipEditor(NexUIDesignerContext context, DesignerElementMetadata primary)
        {
            MotionClipEditorWindow.Open(context.PreviewSurface, primary.elementId);
        }
    }
}
