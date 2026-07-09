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

            var topRow = new VisualElement();
            topRow.AddToClassList("nexui-toolbar-row");
            var bottomRow = new VisualElement();
            bottomRow.AddToClassList("nexui-toolbar-row");
            bottomRow.AddToClassList("nexui-toolbar-row-secondary");
            Add(topRow);
            Add(bottomRow);

            var brand = new VisualElement();
            brand.AddToClassList("nexui-toolbar-brand");
            brand.Add(new Label("NexUI") { name = "BrandTitle" });
            brand.Add(new Label("Designer") { name = "BrandSubtitle" });
            topRow.Add(brand);

            var objectField = new ObjectField
            {
                objectType = typeof(UIScreenDefinition),
                allowSceneObjects = false,
                label = "Screen"
            };
            objectField.AddToClassList("nexui-screen-field");
            objectField.RegisterValueChangedCallback(evt => context.Open(evt.newValue as UIScreenDefinition));
            topRow.Add(objectField);

            var metadataField = new ObjectField
            {
                objectType = typeof(DesignerMetadataAsset),
                allowSceneObjects = false,
                label = "Metadata"
            };
            metadataField.AddToClassList("nexui-metadata-field");
            metadataField.RegisterValueChangedCallback(evt => context.SetMetadata(evt.newValue as DesignerMetadataAsset));
            topRow.Add(metadataField);
            context.MetadataChanged += metadata => metadataField.SetValueWithoutNotify(metadata);
            topRow.Add(MakeButton(() => context.CreateMetadataAsset(), "New", "nexui-button-secondary"));

            _status = new Label();
            _status.AddToClassList("nexui-toolbar-status");
            topRow.Add(_status);

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
            bottomRow.Add(resolution);

            var state = new PopupField<string>("State", new System.Collections.Generic.List<string> { "Normal", "Hover", "Pressed", "Disabled", "Focused" }, context.PreviewState);
            state.AddToClassList("nexui-compact-popup");
            state.RegisterValueChangedCallback(evt => context.SetPreviewState(evt.newValue));
            bottomRow.Add(state);

            var input = new PopupField<string>("Input", new System.Collections.Generic.List<string> { "Keyboard", "Gamepad", "Touch", "SteamDeck" }, context.InputMode);
            input.AddToClassList("nexui-compact-popup");
            input.RegisterValueChangedCallback(evt => context.SetInputMode(evt.newValue));
            bottomRow.Add(input);

            var snap = new Toggle("Snap") { value = context.SnapEnabled };
            snap.AddToClassList("nexui-toolbar-toggle");
            snap.RegisterValueChangedCallback(evt => context.SetSnap(evt.newValue));
            bottomRow.Add(snap);

            var zoomOut = MakeButton(() => context.ZoomBy(-0.1f), "-", "nexui-button-secondary");
            var zoomIn = MakeButton(() => context.ZoomBy(0.1f), "+", "nexui-button-secondary");
            bottomRow.Add(zoomOut);
            bottomRow.Add(zoomIn);

            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            bottomRow.Add(spacer);

            bottomRow.Add(MakeButton(context.RebuildPreview, "Rebuild", "nexui-button-secondary"));
            bottomRow.Add(MakeButton(() => context.Save(), DesignerLocalization.T("toolbar.save"), "nexui-button-primary"));
            bottomRow.Add(MakeButton(context.Validate, DesignerLocalization.T("toolbar.validate"), "nexui-button-secondary"));

            context.ScreenChanged += _ => RefreshStatus(context);
            context.ValidationChanged += () => RefreshStatus(context);
            context.SaveCompleted += report =>
            {
                _status.text = report.Summary();
                _status.RemoveFromClassList("is-muted");
                _status.RemoveFromClassList("is-ok");
                _status.RemoveFromClassList("is-warning");
                _status.AddToClassList(report.HasErrors ? "is-warning" : report.HasWarnings ? "is-warning" : "is-ok");
            };
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

            _status.text = context.Backend + " | " + context.CurrentScreen.ScreenId;
            _status.RemoveFromClassList("is-muted");
            _status.AddToClassList(context.ValidationMessages.Count == 0 ? "is-ok" : "is-warning");
        }
    }
}
