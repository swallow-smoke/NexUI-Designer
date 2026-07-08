using System;
using System.Collections.Generic;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Motion;
using emiteat.NexUI.State;
using emiteat.NexUI.Theme;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed class NexUIDesignerContext : IDisposable
    {
        private readonly List<string> _validationMessages = new List<string>();

        public UIScreenDefinition CurrentScreen { get; private set; }
        public IUISurface PreviewSurface { get; private set; }
        public IUIElementHandle SelectedElement { get; private set; }
        public UIRenderBackend Backend { get; private set; }
        public UIStateStore PreviewStateStore { get; private set; }
        public IUIMotionPlayer PreviewMotionPlayer { get; private set; }
        public ThemeRegistry PreviewThemeRegistry { get; private set; }
        public INexUIDesignerBackend CurrentBackend { get; private set; }
        public Vector2Int Resolution { get; private set; }
        public IReadOnlyList<string> ValidationMessages => _validationMessages;

        public event Action<UIScreenDefinition> ScreenChanged;
        public event Action<IUIElementHandle> SelectionChanged;
        public event Action PreviewRebuilt;
        public event Action ValidationChanged;

        public NexUIDesignerContext()
        {
            PreviewStateStore = new UIStateStore();
            PreviewMotionPlayer = new BuiltInMotionPlayer();
            PreviewThemeRegistry = new ThemeRegistry();
            Resolution = new Vector2Int(1920, 1080);
            DesignerBackendRegistry.RegisterDefaults();
        }

        public void Open(UIScreenDefinition definition)
        {
            CurrentScreen = definition;
            Backend = definition != null ? definition.backendAsset.backend : UIRenderBackend.UIToolkit;
            ScreenChanged?.Invoke(definition);
            RebuildPreview();
        }

        public void Select(IUIElementHandle handle)
        {
            SelectedElement = handle;
            SelectionChanged?.Invoke(handle);
        }

        public void ClearSelection() => Select(null);

        public void RebuildPreview()
        {
            if (PreviewSurface != null && CurrentBackend != null)
                CurrentBackend.DestroyPreviewSurface(PreviewSurface);

            PreviewSurface = null;
            CurrentBackend = null;

            if (CurrentScreen != null && DesignerBackendRegistry.TryGet(CurrentScreen.backendAsset.backend, out var backend))
            {
                CurrentBackend = backend;
                PreviewSurface = backend.CreatePreviewSurface(CurrentScreen);
            }

            ClearSelection();
            PreviewRebuilt?.Invoke();
            Validate();
        }

        public void Save()
        {
            if (CurrentScreen != null && CurrentBackend != null && PreviewSurface != null)
            {
                CurrentBackend.Save(CurrentScreen, PreviewSurface);
                EditorUtility.SetDirty(CurrentScreen);
            }
            AssetDatabase.SaveAssets();
        }

        public void Validate()
        {
            _validationMessages.Clear();
            if (CurrentScreen == null)
                _validationMessages.Add("message.noScreenSelected");
            else
            {
                if (string.IsNullOrEmpty(CurrentScreen.ScreenId))
                    _validationMessages.Add("validation.error: screenId");
                if (CurrentScreen.backendAsset.asset == null)
                    _validationMessages.Add("validation.warning: backend asset");
                if (!DesignerBackendRegistry.TryGet(CurrentScreen.backendAsset.backend, out _))
                    _validationMessages.Add("validation.error: backend");
            }
            ValidationChanged?.Invoke();
        }

        public void SetResolution(Vector2Int resolution)
        {
            Resolution = resolution;
            PreviewRebuilt?.Invoke();
        }

        public void SetTheme(string themeId)
        {
            if (!string.IsNullOrEmpty(themeId))
                NexUITheme.Use(themeId);
            PreviewRebuilt?.Invoke();
        }

        public bool TryFindElement(string elementId, out IUIElementHandle handle)
        {
            handle = PreviewSurface?.TryFind(elementId);
            return handle != null;
        }

        public void Dispose()
        {
            if (PreviewSurface != null && CurrentBackend != null)
                CurrentBackend.DestroyPreviewSurface(PreviewSurface);
        }
    }
}
