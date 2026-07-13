using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.FocusNav
{
    /// <summary>Selected-element directional navigation links (brief §29). Sits alongside Binding/State/Command in the Prototype tab - same granularity (per-selected-element authoring), plus an "Auto Generate All" that runs <see cref="FocusNavigationAutoLayout"/> over every element on the screen.</summary>
    public sealed class FocusNavigationPanel : VisualElement
    {
        private const string NoneChoice = "(None)";

        private readonly NexUIDesignerContext _context;
        private readonly VisualElement _body;

        public FocusNavigationPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-inspector-section");
            Add(new Label(DesignerLocalization.T("focusNav.panel.title")) { name = "SectionTitle" });

            _body = new VisualElement();
            Add(_body);

            var autoGenerateButton = new Button(AutoGenerateAll) { text = DesignerLocalization.T("focusNav.panel.autoGenerate") };
            autoGenerateButton.AddToClassList("nexui-toolbar-button");
            autoGenerateButton.AddToClassList("nexui-button-secondary");
            DesignerTooltip.Set(autoGenerateButton, "tooltip.focusNav.autoGenerate");
            Add(autoGenerateButton);

            var subscriptions = new ContextBoundSubscriptions(this);
            subscriptions.Add<DesignerElementMetadata>(h => context.MetadataSelectionChanged += h, h => context.MetadataSelectionChanged -= h, _ => Refresh());
            subscriptions.Add<DesignerMetadataAsset>(h => context.MetadataChanged += h, h => context.MetadataChanged -= h, _ => Refresh());
            subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, Refresh);
            Refresh();
        }

        private void Refresh()
        {
            _body.Clear();
            var selected = _context.SelectedMetadata;
            if (selected == null || _context.Metadata == null)
            {
                _body.Add(new Label(DesignerLocalization.T("focusNav.panel.noSelection")));
                return;
            }

            var choices = new List<string> { NoneChoice };
            foreach (var element in _context.Metadata.elements)
                if (element != null && element != selected)
                    choices.Add(element.elementId);

            _body.Add(BuildDirectionField(DesignerLocalization.T("focusNav.panel.up"), choices, selected.focus.upElementId,
                value => _context.UpdateSelectedElement(e => e.focus.upElementId = value, "Edit Focus Navigation")));
            _body.Add(BuildDirectionField(DesignerLocalization.T("focusNav.panel.down"), choices, selected.focus.downElementId,
                value => _context.UpdateSelectedElement(e => e.focus.downElementId = value, "Edit Focus Navigation")));
            _body.Add(BuildDirectionField(DesignerLocalization.T("focusNav.panel.left"), choices, selected.focus.leftElementId,
                value => _context.UpdateSelectedElement(e => e.focus.leftElementId = value, "Edit Focus Navigation")));
            _body.Add(BuildDirectionField(DesignerLocalization.T("focusNav.panel.right"), choices, selected.focus.rightElementId,
                value => _context.UpdateSelectedElement(e => e.focus.rightElementId = value, "Edit Focus Navigation")));

            var defaultFocusToggle = new Toggle(DesignerLocalization.T("focusNav.panel.defaultFocus")) { value = selected.focus.isDefaultFocus };
            DesignerTooltip.Set(defaultFocusToggle, "tooltip.focusNav.defaultFocus");
            defaultFocusToggle.RegisterValueChangedCallback(evt =>
                _context.UpdateSelectedElement(e => e.focus.isDefaultFocus = evt.newValue, "Edit Focus Navigation"));
            _body.Add(defaultFocusToggle);
        }

        private static VisualElement BuildDirectionField(string label, List<string> choices, string currentValue,
            System.Action<string> onChanged)
        {
            var index = string.IsNullOrEmpty(currentValue) ? 0 : choices.IndexOf(currentValue);
            if (index < 0) index = 0;
            var field = new PopupField<string>(label, choices, index);
            field.RegisterValueChangedCallback(evt => onChanged(evt.newValue == NoneChoice ? string.Empty : evt.newValue));
            return field;
        }

        private void AutoGenerateAll()
        {
            if (_context.Metadata == null || _context.Metadata.elements.Count == 0) return;

            var bounds = _context.Metadata.elements
                .Where(e => e != null)
                .Select(e => new FocusNavigationAutoLayout.ElementBounds(e.elementId, e.rect.center))
                .ToList();
            var links = FocusNavigationAutoLayout.GenerateNearest(bounds);

            NexUIDesignerUndo.Group("Auto Generate Focus Navigation", () =>
            {
                foreach (var element in _context.Metadata.elements)
                {
                    if (element == null || !links.TryGetValue(element.elementId, out var link)) continue;
                    _context.UpdateElement(element, e =>
                    {
                        e.focus.upElementId = link.UpElementId ?? string.Empty;
                        e.focus.downElementId = link.DownElementId ?? string.Empty;
                        e.focus.leftElementId = link.LeftElementId ?? string.Empty;
                        e.focus.rightElementId = link.RightElementId ?? string.Empty;
                    }, "Auto Generate Focus Navigation");
                }
            });

            Refresh();
        }
    }
}
