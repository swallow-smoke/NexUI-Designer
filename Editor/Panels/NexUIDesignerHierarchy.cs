using emiteat.NexUI.Designer.Editor.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerHierarchy : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly ListView _list;
        private readonly Label _empty;
        private readonly ToolbarSearchField _search;
        private string _filter = "";
        private List<DesignerElementMetadata> _currentItems = new();

        public NexUIDesignerHierarchy(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-hierarchy-panel");
            Add(new Label(DesignerLocalization.T("panel.hierarchy")) { name = "PanelTitle" });

            _search = new ToolbarSearchField { tooltip = DesignerLocalization.T("tooltip.hierarchy.search") };
            _search.RegisterValueChangedCallback(evt =>
            {
                _filter = evt.newValue ?? "";
                Refresh();
            });
            Add(_search);

            _empty = new Label("Select a Metadata asset to show elements.");
            _empty.AddToClassList("nexui-empty-note");
            Add(_empty);

            _list = new ListView();
            _list.AddToClassList("nexui-list");
            _list.style.flexGrow = 1;
            _list.selectionType = SelectionType.Multiple;
            _list.makeItem = () =>
            {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                row.AddToClassList("nexui-hierarchy-row");

                var visibility = new Button { text = "◉" }; // filled circle: on = visible
                visibility.tooltip = DesignerLocalization.T("tooltip.hierarchy.visibility");
                visibility.style.width = 20;
                row.Add(visibility);

                var lockToggle = new Button { text = "✓" }; // check: on = locked
                lockToggle.tooltip = DesignerLocalization.T("tooltip.hierarchy.lock");
                lockToggle.style.width = 20;
                row.Add(lockToggle);

                var label = new Label { style = { flexGrow = 1 } };
                row.Add(label);
                return row;
            };
            _list.bindItem = (e, i) =>
            {
                var item = (DesignerElementMetadata)_list.itemsSource[i];
                var row = (VisualElement)e;
                var visibility = (Button)row[0];
                var lockToggle = (Button)row[1];
                var label = (Label)row[2];

                label.text = string.IsNullOrEmpty(item.displayName)
                    ? item.elementId
                    : item.displayName + "  [" + item.elementId + "]";

                visibility.style.opacity = item.hiddenInDesigner ? 0.35f : 1f;
                visibility.clicked -= visibility.userData as System.Action;
                System.Action toggleVisibility = () => _context.UpdateElement(item, el => el.hiddenInDesigner = !el.hiddenInDesigner, "Toggle NexUI Element Hidden");
                visibility.userData = toggleVisibility;
                visibility.clicked += toggleVisibility;

                lockToggle.style.opacity = item.locked ? 1f : 0.35f;
                lockToggle.clicked -= lockToggle.userData as System.Action;
                System.Action toggleLock = () => _context.UpdateElement(item, el => el.locked = !el.locked, "Toggle NexUI Element Lock");
                lockToggle.userData = toggleLock;
                lockToggle.clicked += toggleLock;
            };
            _list.selectionChanged += OnSelectionChanged;
            Add(_list);

            var subscriptions = new ContextBoundSubscriptions(this);
            subscriptions.Add<DesignerMetadataAsset>(h => context.MetadataChanged += h, h => context.MetadataChanged -= h, _ => Refresh());
            subscriptions.Add(h => context.CanvasChanged += h, h => context.CanvasChanged -= h, Refresh);
            subscriptions.Add<System.Collections.Generic.IReadOnlyList<DesignerElementMetadata>>(h => context.MultiSelectionChanged += h, h => context.MultiSelectionChanged -= h, SyncListSelection);
            Refresh();
        }

        private void OnSelectionChanged(IEnumerable<object> items)
        {
            var selected = new List<DesignerElementMetadata>();
            foreach (var item in items)
                if (item is DesignerElementMetadata element)
                    selected.Add(element);

            if (_syncingFromContext) return;
            _context.SelectMany(selected);
        }

        private bool _syncingFromContext;

        private void SyncListSelection(IReadOnlyList<DesignerElementMetadata> selection)
        {
            _syncingFromContext = true;
            if (selection == null || selection.Count == 0)
            {
                _list.ClearSelection();
            }
            else
            {
                var indices = new List<int>();
                foreach (var element in selection)
                {
                    var index = _currentItems.IndexOf(element);
                    if (index >= 0) indices.Add(index);
                }
                _list.SetSelectionWithoutNotify(indices);
            }
            _syncingFromContext = false;
        }

        private void Refresh()
        {
            _currentItems = new List<DesignerElementMetadata>();
            if (_context.Metadata != null)
            {
                foreach (var e in _context.Metadata.elements)
                {
                    if (e == null) continue;
                    if (!Matches(e, _filter)) continue;
                    _currentItems.Add(e);
                }
            }

            _empty.style.display = _currentItems.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _empty.text = string.IsNullOrEmpty(_filter)
                ? "Select a Metadata asset to show elements."
                : "No elements match the filter.";
            _list.itemsSource = (IList)_currentItems;
            _list.Rebuild();
        }

        private static bool Matches(DesignerElementMetadata e, string filter)
        {
            if (string.IsNullOrEmpty(filter)) return true;
            return (!string.IsNullOrEmpty(e.elementId) && e.elementId.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0)
                || (!string.IsNullOrEmpty(e.displayName) && e.displayName.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
