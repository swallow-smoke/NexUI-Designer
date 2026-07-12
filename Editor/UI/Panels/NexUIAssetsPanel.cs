using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.UI.Panels
{
    public sealed class NexUIAssetsPanel : VisualElement
    {
        public NexUIAssetsPanel()
        {
            AddToClassList("nexui-assets-panel");

            var search = new ToolbarSearchField { tooltip = "Search images, fonts, themes, motion clips, and presets." };
            Add(search);

            var filters = new VisualElement();
            filters.AddToClassList("nexui-asset-filters");
            foreach (var label in new[] { "Images", "Fonts", "Themes", "Motion", "Templates" })
            {
                var chip = new Button { text = label, tooltip = "Filter " + label };
                chip.AddToClassList("nexui-filter-chip");
                filters.Add(chip);
            }
            Add(filters);

            var empty = new Label("Asset browser placeholder. Images, sprites, fonts, themes, motion presets, motion clips, templates, and component presets will appear here.");
            empty.AddToClassList("nexui-empty-note");
            Add(empty);
        }
    }
}
