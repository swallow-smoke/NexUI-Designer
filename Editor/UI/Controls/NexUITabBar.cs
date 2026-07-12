using System;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.UI.Controls
{
    public sealed class NexUITabBar<T> : VisualElement where T : Enum
    {
        private readonly Action<T> _onChanged;

        public NexUITabBar(T current, Action<T> onChanged, params (T value, string label, string tooltip)[] tabs)
        {
            _onChanged = onChanged;
            AddToClassList("nexui-tabbar");

            foreach (var tab in tabs)
            {
                var button = new Button(() => _onChanged(tab.value))
                {
                    text = tab.label,
                    tooltip = tab.tooltip
                };
                button.AddToClassList("nexui-tab");
                button.userData = tab.value;
                Add(button);
            }

            SetCurrent(current);
        }

        public void SetCurrent(T current)
        {
            foreach (var child in Children())
            {
                if (child is Button button && button.userData is T value)
                    button.EnableInClassList("is-active", value.Equals(current));
            }
        }
    }
}
