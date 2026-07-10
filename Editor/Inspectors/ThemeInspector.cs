using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Theme;
using UnityEditor.UIElements;
using UnityEngine;
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
        private readonly ColorField _eyedropperColor;
        private readonly Button _addEyedropperToken;
        private bool _refreshing;

        public ThemeInspector(NexUIDesignerContext context) : base(context, "inspector.theme")
        {
            _themeAsset = new ObjectField("Theme") { objectType = typeof(UITheme), allowSceneObjects = false, tooltip = DesignerLocalization.T("tooltip.theme.themeAsset") };
            _themeId = new TextField("Theme Id") { tooltip = DesignerLocalization.T("tooltip.theme.themeId") };
            _classes = new TextField("Classes") { tooltip = DesignerLocalization.T("tooltip.theme.classes") };
            _tokenList = new VisualElement();
            _addToken = new Button(AddTokenOverride) { text = DesignerLocalization.T("button.addTokenOverride"), tooltip = DesignerLocalization.T("tooltip.theme.addToken") };

            _eyedropperColor = new ColorField { value = Color.white, tooltip = DesignerLocalization.T("tooltip.theme.eyedropperColor") };
            _eyedropperColor.style.flexGrow = 1;
            _addEyedropperToken = new Button(AddTokenFromEyedropperColor) { text = DesignerLocalization.T("button.addAsToken"), tooltip = DesignerLocalization.T("tooltip.theme.addEyedropperToken") };
            var eyedropperRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            eyedropperRow.Add(_eyedropperColor);
            eyedropperRow.Add(_addEyedropperToken);

            Add(_themeAsset);
            Add(_themeId);
            Add(_classes);
            Add(new Label("Token Overrides") { name = "SubTitle" });
            Add(_tokenList);
            Add(_addToken);
            Add(eyedropperRow);

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

        /// <summary>
        /// C2: adds the current <see cref="_eyedropperColor"/> value as a new token override.
        /// <see cref="UnityEditor.EyeDropper"/> is internal, so this leans on the ColorField's
        /// own picker popup (public API), which already has a built-in eyedropper button for
        /// sampling any pixel on screen - no reflection into Unity internals needed.
        /// </summary>
        private void AddTokenFromEyedropperColor()
        {
            if (Context.SelectedMetadata == null) return;
            var hex = "#" + ColorUtility.ToHtmlStringRGBA(_eyedropperColor.value);
            Context.UpdateSelectedElement(
                e => e.theme.tokenOverrides.Add(new DesignerTokenOverride { value = hex }),
                "Add NexUI Theme Token Override (Eyedropper)");
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

            var keyField = new TextField { value = Context.SelectedMetadata.theme.tokenOverrides[index].key, tooltip = DesignerLocalization.T("tooltip.theme.tokenKey") };
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

            var valueField = new TextField { value = Context.SelectedMetadata.theme.tokenOverrides[index].value, tooltip = DesignerLocalization.T("tooltip.theme.tokenValue") };
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
            { text = "X", tooltip = DesignerLocalization.T("tooltip.theme.removeToken") };

            row.Add(keyField);
            row.Add(valueField);
            row.Add(remove);
            return row;
        }
    }
}
