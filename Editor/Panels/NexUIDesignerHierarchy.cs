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

        public NexUIDesignerHierarchy(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            Add(new Label(DesignerLocalization.T("panel.hierarchy")) { name = "PanelTitle" });
            _list = new ListView();
            _list.AddToClassList("nexui-list");
            _list.style.flexGrow = 1;
            _list.makeItem = () =>
            {
                var row = new Label();
                row.AddToClassList("nexui-hierarchy-row");
                return row;
            };
            _list.bindItem = (e, i) => ((Label)e).text = "  " + _context.CurrentBackend.GetHierarchy(_context.PreviewSurface)[i].Id;
            _list.selectionChanged += items =>
            {
                foreach (var item in items)
                    _context.Select(item as emiteat.NexUI.Abstractions.IUIElementHandle);
            };
            Add(_list);
            context.PreviewRebuilt += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            IList items = _context.CurrentBackend != null && _context.PreviewSurface != null
                ? new List<emiteat.NexUI.Abstractions.IUIElementHandle>(_context.CurrentBackend.GetHierarchy(_context.PreviewSurface))
                : new List<emiteat.NexUI.Abstractions.IUIElementHandle>();
            _list.itemsSource = items;
            _list.Rebuild();
        }
    }
}
