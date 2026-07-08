using emiteat.NexUI.Designer.Editor.Backend;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerPalette : VisualElement
    {
        public NexUIDesignerPalette(NexUIDesignerContext context)
        {
            AddToClassList("nexui-panel");
            AddToClassList("nexui-palette");
            Add(new Label("Components") { name = "PanelTitle" });

            var grid = new VisualElement();
            grid.AddToClassList("nexui-palette-grid");
            Add(grid);

            AddButtonRow(grid, context, DesignerElementType.Panel, "Panel", DesignerElementType.Button, "Button");
            AddButtonRow(grid, context, DesignerElementType.Label, "Text", DesignerElementType.Image, "Image");
            AddButtonRow(grid, context, DesignerElementType.Card, "Card", DesignerElementType.Modal, "Modal");
            AddButtonRow(grid, context, DesignerElementType.List, "List", DesignerElementType.Slot, "Slot");

            Add(new Label("Selection actions") { name = "PanelSubtitle" });
            var align = new VisualElement();
            align.AddToClassList("nexui-align-grid");
            Add(align);
            AddActionRow(align, "Left", () => context.AlignSelected("left"), "Center", () => context.AlignSelected("centerX"));
            AddActionRow(align, "Right", () => context.AlignSelected("right"), "Top", () => context.AlignSelected("top"));
            AddActionRow(align, "Middle", () => context.AlignSelected("centerY"), "Bottom", () => context.AlignSelected("bottom"));
            AddActionRow(align, "Fill", () => context.AlignSelected("fill"), "Copy", () => context.DuplicateSelectedMetadata());
            AddActionRow(align, "Delete", () => context.DeleteSelectedMetadata(), null, null);
        }

        private static void AddButtonRow(VisualElement parent, NexUIDesignerContext context, DesignerElementType leftType, string leftLabel, DesignerElementType rightType, string rightLabel)
        {
            var row = new VisualElement();
            row.AddToClassList("nexui-palette-row");
            parent.Add(row);
            AddButton(row, context, leftType, leftLabel);
            AddButton(row, context, rightType, rightLabel);
        }

        private static void AddButton(VisualElement parent, NexUIDesignerContext context, DesignerElementType type, string label)
        {
            var button = new Button(() => context.CreateMetadataElement(type)) { text = label };
            button.AddToClassList("nexui-palette-button");
            parent.Add(button);
        }

        private static void AddActionRow(VisualElement parent, string leftLabel, System.Action leftAction, string rightLabel, System.Action rightAction)
        {
            var row = new VisualElement();
            row.AddToClassList("nexui-palette-row");
            parent.Add(row);
            AddAction(row, leftLabel, leftAction);
            if (!string.IsNullOrEmpty(rightLabel))
                AddAction(row, rightLabel, rightAction);
        }

        private static void AddAction(VisualElement parent, string label, System.Action action)
        {
            var button = new Button(action) { text = label };
            button.AddToClassList("nexui-align-button");
            parent.Add(button);
        }
    }
}
