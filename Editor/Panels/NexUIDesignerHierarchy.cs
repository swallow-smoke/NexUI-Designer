using emiteat.NexUI.Designer.Editor.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerHierarchy : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly ListView _list;
        private readonly Label _empty;

        public NexUIDesignerHierarchy(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-hierarchy-panel");
            Add(new Label(DesignerLocalization.T("panel.hierarchy")) { name = "PanelTitle" });
            _empty = new Label("Select a Metadata asset to show elements.");
            _empty.AddToClassList("nexui-empty-note");
            Add(_empty);

            _list = new ListView();
            _list.AddToClassList("nexui-list");
            _list.style.flexGrow = 1;
            _list.makeItem = () =>
            {
                var row = new Label();
                row.AddToClassList("nexui-hierarchy-row");
                return row;
            };
            _list.bindItem = (e, i) =>
            {
                var item = (DesignerElementMetadata)_list.itemsSource[i];
                ((Label)e).text = string.IsNullOrEmpty(item.displayName)
                    ? item.elementId
                    : item.displayName + "  [" + item.elementId + "]";
            };
            _list.selectionChanged += OnSelectionChanged;
            Add(_list);

            context.MetadataChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            context.MetadataSelectionChanged += SelectInList;
            Refresh();
        }

        private void OnSelectionChanged(IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                _context.SelectMetadata(item as DesignerElementMetadata);
                break;
            }
        }

        private void SelectInList(DesignerElementMetadata element)
        {
            if (element == null)
            {
                _list.ClearSelection();
                return;
            }

            var index = _context.Metadata != null ? _context.Metadata.elements.IndexOf(element) : -1;
            if (index >= 0 && _list.selectedIndex != index)
                _list.SetSelection(index);
        }

        private void Refresh()
        {
            IList items = _context.Metadata != null
                ? new List<DesignerElementMetadata>(_context.Metadata.elements)
                : new List<DesignerElementMetadata>();
            _empty.style.display = items.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            _list.itemsSource = items;
            _list.Rebuild();
        }
    }
}
