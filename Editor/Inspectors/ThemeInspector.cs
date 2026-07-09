using emiteat.NexUI.Theme;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// Element-level inspector for <see cref="DesignerThemeMetadata"/>: a theme asset picker
    /// (which also seeds the resolved <c>themeId</c>), a raw themeId override, space-separated
    /// style classes, and a dynamic list of key/value token overrides.
    /// </summary>
    public sealed class ThemeInspector : DesignerInspectorBase
    {
        private readonly ObjectField _themeAsset;
        private readonly TextField _themeId;
        private readonly TextField _classes;
        private readonly VisualElement _tokenList;
        private readonly Button _addToken;
        private bool _refreshing;

        public ThemeInspector(NexUIDesignerContext context) : base(context, "inspector.theme")
        {
            _themeAsset = new ObjectField("Theme") { objectType = typeof(UITheme), allowSceneObjects = false };
            _themeId = new TextField("Theme Id");
            _classes = new TextField("Classes");
            _tokenList = new VisualElement();
            _addToken = new Button(AddTokenOverride) { text = "Add Token Override" };

            Add(_themeAsset);
            Add(_themeId);
            Add(_classes);
            Add(new Label("Token Overrides") { name = "SubTitle" });
            Add(_tokenList);
            Add(_addToken);

            _themeAsset.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                var theme = evt.newValue as UITheme;
                Context.UpdateSelectedElement(e =>
                {
                    e.theme.themeRef = theme;
                    if (theme != null && !string.IsNullOrEmpty(theme.themeId))
                        e.theme.themeId = theme.themeId;
                }, "Assign NexUI Element Theme");
            });
            _themeId.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                Context.UpdateSelectedElement(e => e.theme.themeId = evt.newValue, "Edit NexUI Element Theme Id");
            });
            _classes.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                Context.UpdateSelectedElement(e =>
                {
                    e.theme.classes.Clear();
                    foreach (var token in evt.newValue.Split(' '))
                        if (!string.IsNullOrWhiteSpace(token))
                            e.theme.classes.Add(token.Trim());
                }, "Edit NexUI Element Theme Classes");
            });

            context.MetadataSelectionChanged += _ => Refresh();
            context.CanvasChanged += Refresh;
            Refresh();
        }

        private void AddTokenOverride()
        {
            if (Context.SelectedMetadata == null) return;
            Context.UpdateSelectedElement(
                e => e.theme.tokenOverrides.Add(new DesignerTokenOverride()),
                "Add NexUI Theme Token Override");
            Refresh();
        }

        private void Refresh()
        {
            _refreshing = true;
            var selected = Context.SelectedMetadata;
            SetEnabled(selected != null);
            _tokenList.Clear();

            if (selected != null)
            {
                _themeAsset.SetValueWithoutNotify(selected.theme.themeRef);
                _themeId.SetValueWithoutNotify(selected.theme.themeId);
                _classes.SetValueWithoutNotify(string.Join(" ", selected.theme.classes));

                var overrides = selected.theme.tokenOverrides;
                for (int i = 0; i < overrides.Count; i++)
                    _tokenList.Add(BuildTokenRow(i));
            }
            _refreshing = false;
        }

        private VisualElement BuildTokenRow(int index)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;

            var keyField = new TextField { value = Context.SelectedMetadata.theme.tokenOverrides[index].key };
            keyField.style.flexGrow = 1f;
            keyField.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                Context.UpdateSelectedElement(e =>
                {
                    if (index < e.theme.tokenOverrides.Count)
                        e.theme.tokenOverrides[index].key = evt.newValue;
                }, "Edit NexUI Theme Token Key");
            });

            var valueField = new TextField { value = Context.SelectedMetadata.theme.tokenOverrides[index].value };
            valueField.style.flexGrow = 1f;
            valueField.RegisterValueChangedCallback(evt =>
            {
                if (_refreshing) return;
                Context.UpdateSelectedElement(e =>
                {
                    if (index < e.theme.tokenOverrides.Count)
                        e.theme.tokenOverrides[index].value = evt.newValue;
                }, "Edit NexUI Theme Token Value");
            });

            var remove = new Button(() =>
            {
                Context.UpdateSelectedElement(e =>
                {
                    if (index < e.theme.tokenOverrides.Count)
                        e.theme.tokenOverrides.RemoveAt(index);
                }, "Remove NexUI Theme Token Override");
                Refresh();
            })
            { text = "X" };

            row.Add(keyField);
            row.Add(valueField);
            row.Add(remove);
            return row;
        }
    }
}
