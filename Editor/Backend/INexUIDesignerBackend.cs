using System.Collections.Generic;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Backend
{
    // DesignerAnchorPreset now lives in the runtime metadata assembly
    // (emiteat.NexUI.Designer namespace) so DesignerElementMetadata can persist it without a
    // UnityEditor dependency. It resolves here via enclosing-namespace lookup.
    public enum DesignerElementType
    {
        Panel,
        Card,
        Label,
        Button,
        IconButton,
        ProgressBar,
        StatBar,
        Modal,
        Toast,
        Tooltip,
        Popover,
        List,
        Grid,
        Slot,
        ChoiceList,
        Spinner,
        Skeleton,
        RadialFill,
        Image,
        Container,
        Custom
    }

    public sealed class DesignerElementCreateInfo
    {
        public DesignerElementType type;
        public string elementId;
        public string displayName;
    }

    public interface INexUIDesignerBackend
    {
        UIRenderBackend Backend { get; }
        IUISurface CreatePreviewSurface(UIScreenDefinition definition);
        void DestroyPreviewSurface(IUISurface surface);
        IReadOnlyList<IUIElementHandle> GetHierarchy(IUISurface surface);
        bool TrySelectElement(IUISurface surface, string elementId, out IUIElementHandle handle);
        IUIElementHandle CreateElement(IUISurface surface, string parentId, DesignerElementCreateInfo createInfo);
        void DeleteElement(IUISurface surface, IUIElementHandle handle);
        IUIElementHandle DuplicateElement(IUISurface surface, IUIElementHandle handle);
        void SetParent(IUIElementHandle child, IUIElementHandle newParent);
        void Reorder(IUIElementHandle handle, int newIndex);
        Vector2 GetPosition(IUIElementHandle handle);
        Vector2 GetSize(IUIElementHandle handle);
        void SetPosition(IUIElementHandle handle, Vector2 position);
        void SetSize(IUIElementHandle handle, Vector2 size);
        void SetAnchor(IUIElementHandle handle, DesignerAnchorPreset preset);
        void SetVisible(IUIElementHandle handle, bool visible);
        void SetName(IUIElementHandle handle, string elementId);
        void AddClass(IUIElementHandle handle, string className);
        void RemoveClass(IUIElementHandle handle, string className);
        void SetBinding(IUIElementHandle handle, DesignerBindingMetadata binding);
        DesignerBindingMetadata GetBinding(IUIElementHandle handle);
        void Save(UIScreenDefinition definition, IUISurface surface);
    }
}
