using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.Viewport;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Panels
{
    public sealed class NexUIDesignerToolbar : Toolbar
    {
        private readonly Label _status;

        public NexUIDesignerToolbar(NexUIDesignerContext context)
        {
            AddToClassList("nexui-toolbar");

            var brand = new VisualElement();
            brand.AddToClassList("nexui-toolbar-brand");
            brand.Add(new Label("NexUI") { name = "BrandTitle" });
            brand.Add(new Label("Designer") { name = "BrandSubtitle" });
            Add(brand);

            var objectField = new ObjectField
            {
                objectType = typeof(UIScreenDefinition),
                allowSceneObjects = false,
                label = "Screen"
            };
            objectField.AddToClassList("nexui-screen-field");
            objectField.RegisterValueChangedCallback(evt => context.Open(evt.newValue as UIScreenDefinition));
            Add(objectField);

            var resolution = new PopupField<string>("Preview");
            foreach (var preset in DesignerResolutionPreset.Defaults)
                resolution.choices.Add(preset.Name);
            resolution.value = "1920x1080";
            resolution.AddToClassList("nexui-resolution-field");
            resolution.RegisterValueChangedCallback(evt =>
            {
                foreach (var preset in DesignerResolutionPreset.Defaults)
                    if (preset.Name == evt.newValue)
                        context.SetResolution(preset.Resolution);
            });
            Add(resolution);

            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            Add(spacer);

            _status = new Label();
            _status.AddToClassList("nexui-toolbar-status");
            Add(_status);

            Add(MakeButton(context.RebuildPreview, DesignerLocalization.T("toolbar.rebuildPreview"), "nexui-button-secondary"));
            Add(MakeButton(context.Save, DesignerLocalization.T("toolbar.save"), "nexui-button-primary"));
            Add(MakeButton(context.Validate, DesignerLocalization.T("toolbar.validate"), "nexui-button-secondary"));

            context.ScreenChanged += _ => RefreshStatus(context);
            context.ValidationChanged += () => RefreshStatus(context);
            RefreshStatus(context);
        }

        private static ToolbarButton MakeButton(System.Action action, string text, string className)
        {
            var button = new ToolbarButton(action) { text = text };
            button.AddToClassList("nexui-toolbar-button");
            button.AddToClassList(className);
            return button;
        }

        private void RefreshStatus(NexUIDesignerContext context)
        {
            if (context.CurrentScreen == null)
            {
                _status.text = "No screen";
                _status.RemoveFromClassList("is-ok");
                _status.AddToClassList("is-muted");
                return;
            }

            _status.text = context.Backend + "  |  " + context.CurrentScreen.ScreenId;
            _status.RemoveFromClassList("is-muted");
            _status.AddToClassList(context.ValidationMessages.Count == 0 ? "is-ok" : "is-warning");
        }
    }
}
