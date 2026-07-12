using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.UI.Panels
{
    public sealed class NexUIComponentsPanel : VisualElement
    {
        private static readonly (string category, (DesignerElementType type, string label)[] items)[] Categories =
        {
            ("Containers", new[] { (DesignerElementType.Panel, "Panel"), (DesignerElementType.Card, "Card"), (DesignerElementType.Container, "Container"), (DesignerElementType.Modal, "Modal") }),
            ("Text & Media", new[] { (DesignerElementType.Label, "Text"), (DesignerElementType.Image, "Image") }),
            ("Input", new[] { (DesignerElementType.Button, "Button"), (DesignerElementType.IconButton, "Icon Button"), (DesignerElementType.ChoiceList, "Choice List") }),
            ("Feedback", new[] { (DesignerElementType.Toast, "Toast"), (DesignerElementType.Tooltip, "Tooltip"), (DesignerElementType.ProgressBar, "Progress"), (DesignerElementType.Spinner, "Spinner") }),
            ("Data", new[] { (DesignerElementType.List, "List"), (DesignerElementType.Grid, "Grid"), (DesignerElementType.Slot, "Slot"), (DesignerElementType.Skeleton, "Skeleton") }),
        };

        private readonly NexUIDesignerContext _context;
        private readonly VisualElement _content;
        private readonly List<Button> _cards = new();
        private string _filter = "";

        public NexUIComponentsPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-components-panel");

            var search = new ToolbarSearchField { tooltip = DesignerLocalization.T("tooltip.palette.search") };
            search.RegisterValueChangedCallback(evt =>
            {
                _filter = evt.newValue ?? "";
                RefreshFilter();
            });
            Add(search);

            _content = new ScrollView();
            _content.AddToClassList("nexui-sidebar-scroll");
            Add(_content);

            BuildRecent();
            foreach (var category in Categories)
                BuildCategory(category.category, category.items);
        }

        private void BuildRecent()
        {
            var foldout = new Foldout { text = "Recent", value = true };
            foldout.AddToClassList("nexui-sidebar-foldout");
            var grid = new VisualElement();
            grid.AddToClassList("nexui-component-grid");
            foldout.Add(grid);

            foreach (DesignerElementType type in new[] { DesignerElementType.Panel, DesignerElementType.Button, DesignerElementType.Label, DesignerElementType.Image })
                grid.Add(CreateCard(type, type == DesignerElementType.Label ? "Text" : type.ToString()));

            _content.Add(foldout);
        }

        private void BuildCategory(string title, IReadOnlyList<(DesignerElementType type, string label)> items)
        {
            var foldout = new Foldout { text = title, value = EditorPrefs.GetBool("NexUI.Designer.Components." + title, true) };
            foldout.AddToClassList("nexui-sidebar-foldout");
            foldout.RegisterValueChangedCallback(evt => EditorPrefs.SetBool("NexUI.Designer.Components." + title, evt.newValue));

            var grid = new VisualElement();
            grid.AddToClassList("nexui-component-grid");
            foldout.Add(grid);

            foreach (var item in items)
                grid.Add(CreateCard(item.type, item.label));

            _content.Add(foldout);
        }

        private Button CreateCard(DesignerElementType type, string label)
        {
            var button = new Button(() => _context.CreateMetadataElement(type))
            {
                text = IconFor(type) + " " + label,
                tooltip = string.Format(DesignerLocalization.T("tooltip.palette.addComponent"), label)
            };
            button.AddToClassList("nexui-component-card");
            button.userData = label;
            _cards.Add(button);
            return button;
        }

        private void RefreshFilter()
        {
            foreach (var card in _cards)
            {
                var label = card.userData as string ?? "";
                card.style.display = string.IsNullOrEmpty(_filter) || label.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
        }

        private static string IconFor(DesignerElementType type)
        {
            return type switch
            {
                DesignerElementType.Button => "[B]",
                DesignerElementType.IconButton => "[I]",
                DesignerElementType.Label => "[T]",
                DesignerElementType.Image => "[M]",
                DesignerElementType.List => "[L]",
                DesignerElementType.Grid => "[G]",
                DesignerElementType.Modal => "[O]",
                _ => "[ ]"
            };
        }
    }
}
