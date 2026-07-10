using System.Collections;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    /// <summary>
    /// C2: read-only log of recent edits (newest first), so a user can see what just changed
    /// instead of guessing before hitting Ctrl+Z. See <see cref="NexUIDesignerContext.RecentActions"/>
    /// for why this is a log rather than a true steppable/jump-to-any-point history browser.
    /// </summary>
    public sealed class NexUIDesignerHistoryPanel : VisualElement
    {
        private readonly NexUIDesignerContext _context;
        private readonly ListView _list;
        private List<string> _items = new List<string>();

        public NexUIDesignerHistoryPanel(NexUIDesignerContext context)
        {
            _context = context;
            AddToClassList("nexui-panel");
            AddToClassList("nexui-history-panel");
            Add(new Label(DesignerLocalization.T("panel.history")) { name = "PanelTitle" });

            _list = new ListView();
            _list.AddToClassList("nexui-list");
            _list.style.flexGrow = 1;
            _list.makeItem = () => new Label();
            _list.bindItem = (e, i) => ((Label)e).text = _items[i];
            Add(_list);

            context.RecentActionsChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            _items = new List<string>(_context.RecentActions);
            _list.itemsSource = (IList)_items;
            _list.Rebuild();
        }
    }
}
