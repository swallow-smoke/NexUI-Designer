using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerPalette : VisualElement
    {
        // Every DesignerElementType except Custom (which has no sensible one-click default and
        // is created via other flows, e.g. "Sync Metadata From Backend"), grouped into
        // folder-like categories (UI Toolkit Foldout) instead of one long flat list.
        private static readonly (string category, (DesignerElementType type, string label)[] items)[] Categories =
        {
            ("Containers", new[]
            {
                (DesignerElementType.Panel, "Panel"),
                (DesignerElementType.Card, "Card"),
                (DesignerElementType.Container, "Container"),
                (DesignerElementType.Modal, "Modal"),
            }),
            ("Text & Media", new[]
            {
                (DesignerElementType.Label, "Text"),
                (DesignerElementType.Image, "Image"),
            }),
            ("Actions & Input", new[]
            {
                (DesignerElementType.Button, "Button"),
                (DesignerElementType.IconButton, "Icon Button"),
                (DesignerElementType.ChoiceList, "Choice List"),
            }),
            ("Feedback & Status", new[]
            {
                (DesignerElementType.Toast, "Toast"),
                (DesignerElementType.Tooltip, "Tooltip"),
                (DesignerElementType.Popover, "Popover"),
                (DesignerElementType.ProgressBar, "Progress Bar"),
                (DesignerElementType.StatBar, "Stat Bar"),
                (DesignerElementType.RadialFill, "Radial Fill"),
                (DesignerElementType.Spinner, "Spinner"),
                (DesignerElementType.Skeleton, "Skeleton"),
            }),
            ("Data & Lists", new[]
            {
                (DesignerElementType.List, "List"),
                (DesignerElementType.Grid, "Grid"),
                (DesignerElementType.Slot, "Slot"),
            }),
        };

        private readonly VisualElement _grid;
        private readonly Dictionary<Button, string> _buttonLabels = new();
        private readonly List<Foldout> _categoryFoldouts = new();

        public NexUIDesignerPalette(NexUIDesignerContext context)
        {
            AddToClassList("nexui-panel");
            AddToClassList("nexui-palette");
            Add(new Label("Components") { name = "PanelTitle" });

            var search = new ToolbarSearchField { tooltip = DesignerLocalization.T("tooltip.palette.search") };
            search.RegisterValueChangedCallback(evt => ApplyFilter(evt.newValue));
            Add(search);

            _grid = new VisualElement();
            _grid.AddToClassList("nexui-palette-grid");
            Add(_grid);

            foreach (var (category, items) in Categories)
            {
                var prefsKey = "NexUI.Designer.Palette.Category." + category;
                var foldout = new Foldout { text = category, value = EditorPrefs.GetBool(prefsKey, true) };
                foldout.AddToClassList("nexui-palette-category");
                foldout.RegisterValueChangedCallback(evt => EditorPrefs.SetBool(prefsKey, evt.newValue));
                _grid.Add(foldout);
                _categoryFoldouts.Add(foldout);

                foreach (var (type, label) in items)
                {
                    var button = AddButton(foldout, context, type, label);
                    _buttonLabels[button] = label;
                }
            }

            Add(new Label("Selection actions") { name = "PanelSubtitle" });
            var align = new VisualElement();
            align.AddToClassList("nexui-align-grid");
            Add(align);
            AddActionRow(align,
                "Left", () => context.AlignSelected("left"), DesignerLocalization.T("tooltip.palette.alignLeft"),
                "Center", () => context.AlignSelected("centerX"), DesignerLocalization.T("tooltip.palette.alignCenterX"));
            AddActionRow(align,
                "Right", () => context.AlignSelected("right"), DesignerLocalization.T("tooltip.palette.alignRight"),
                "Top", () => context.AlignSelected("top"), DesignerLocalization.T("tooltip.palette.alignTop"));
            AddActionRow(align,
                "Middle", () => context.AlignSelected("centerY"), DesignerLocalization.T("tooltip.palette.alignCenterY"),
                "Bottom", () => context.AlignSelected("bottom"), DesignerLocalization.T("tooltip.palette.alignBottom"));
            AddActionRow(align,
                "Fill", () => context.AlignSelected("fill"), DesignerLocalization.T("tooltip.palette.fill"),
                "Copy", () => context.DuplicateSelectedMetadata(), DesignerLocalization.T("tooltip.palette.copy"));
            AddActionRow(align,
                "Delete", () => context.DeleteSelectedMetadata(), DesignerLocalization.T("tooltip.palette.delete"),
                null, null, null);
        }

        private void ApplyFilter(string filter)
        {
            var hasFilter = !string.IsNullOrEmpty(filter);
            foreach (var foldout in _categoryFoldouts)
            {
                var anyVisible = false;
                foreach (var child in foldout.Children())
                {
                    if (child is not Button button || !_buttonLabels.TryGetValue(button, out var label)) continue;
                    var visible = !hasFilter || label.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
                    button.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                    if (visible) anyVisible = true;
                }

                foldout.style.display = anyVisible ? DisplayStyle.Flex : DisplayStyle.None;
                if (hasFilter && anyVisible) foldout.value = true; // auto-expand so matches are visible
            }
        }

        private static Button AddButton(VisualElement parent, NexUIDesignerContext context, DesignerElementType type, string label)
        {
            var button = new Button(() => context.CreateMetadataElement(type))
            {
                text = label,
                tooltip = string.Format(DesignerLocalization.T("tooltip.palette.addComponent"), label)
            };
            button.AddToClassList("nexui-palette-button");
            parent.Add(button);
            return button;
        }

        private static void AddActionRow(VisualElement parent,
            string leftLabel, System.Action leftAction, string leftTooltip,
            string rightLabel, System.Action rightAction, string rightTooltip)
        {
            var row = new VisualElement();
            row.AddToClassList("nexui-palette-row");
            parent.Add(row);
            AddAction(row, leftLabel, leftAction, leftTooltip);
            if (!string.IsNullOrEmpty(rightLabel))
                AddAction(row, rightLabel, rightAction, rightTooltip);
        }

        private static void AddAction(VisualElement parent, string label, System.Action action, string tooltip)
        {
            var button = new Button(action) { text = label, tooltip = tooltip };
            button.AddToClassList("nexui-align-button");
            parent.Add(button);
        }
    }
}
