using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerStatePanel : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly Label _summary;
        private readonly TextField _filter;
        private readonly Label _keys;

        public NexUIDesignerStatePanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-bottom-card");
            style.flexGrow = 1;
            Add(new Label(DesignerLocalization.T("panel.state")) { name = "PanelTitle", tooltip = DesignerLocalization.T("tooltip.state.title") });
            _summary = new Label();
            _summary.name = "PanelSubtitle";
            Add(_summary);
            _filter = new TextField("Filter") { tooltip = DesignerLocalization.T("tooltip.state.filter") };
            _filter.AddToClassList("nexui-compact-field");
            _filter.RegisterValueChangedCallback(_ => Refresh());
            _keys = new Label { tooltip = DesignerLocalization.T("tooltip.state.keys") };
            _keys.AddToClassList("nexui-bottom-text");
            Add(_keys);

            var subscriptions = new ContextBoundSubscriptions(this);
            subscriptions.Add<DesignerMetadataAsset>(h => context.MetadataChanged += h, h => context.MetadataChanged -= h, _ => Refresh());
            subscriptions.Add<DesignerElementMetadata>(h => context.MetadataSelectionChanged += h, h => context.MetadataSelectionChanged -= h, _ => Refresh());
            subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, Refresh);
            Refresh();
        }

        private void Refresh()
        {
            var selected = _context.SelectedMetadata;
            _summary.text = selected == null ? "Bindings" : selected.elementId;
            if (_context.Metadata == null)
            {
                _keys.text = "No Metadata";
                return;
            }

            var filter = _filter.value ?? string.Empty;
            var text = string.Empty;
            foreach (var e in _context.Metadata.elements)
            {
                if (e == null || (selected != null && e != selected)) continue;
                AppendKey(ref text, e.elementId, "text", e.binding.textKey, filter);
                AppendKey(ref text, e.elementId, "value", e.binding.valueKey, filter);
                AppendKey(ref text, e.elementId, "visible", e.binding.visibilityKey, filter);
                AppendKey(ref text, e.elementId, "class", e.binding.classKey, filter);
                AppendKey(ref text, e.elementId, "interactable", e.binding.interactableKey, filter);
            }
            _keys.text = string.IsNullOrEmpty(text) ? "No binding keys" : text.TrimEnd().Replace("\n", " | ");
        }

        private static void AppendKey(ref string output, string elementId, string kind, string key, string filter)
        {
            if (string.IsNullOrEmpty(key)) return;
            var line = elementId + " / " + kind + " = " + key;
            if (!string.IsNullOrEmpty(filter) && !line.ToLowerInvariant().Contains(filter.ToLowerInvariant())) return;
            output += line + "\n";
        }
    }
}
