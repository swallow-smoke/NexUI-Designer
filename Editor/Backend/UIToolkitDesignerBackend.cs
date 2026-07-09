using System.Collections.Generic;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Backend
{
    public sealed class UIToolkitDesignerBackend : INexUIDesignerBackend
    {
        public UIRenderBackend Backend => UIRenderBackend.UIToolkit;

        public IUISurface CreatePreviewSurface(UIScreenDefinition definition)
        {
            VisualElement root = null;
            var vta = definition != null ? definition.backendAsset.asset as VisualTreeAsset : null;
            root = vta != null ? vta.CloneTree() : new VisualElement { name = definition != null ? definition.ScreenId : "Preview" };
            var surface = new DesignerSurface(definition != null ? definition.ScreenId : "Preview", Backend, root);
            Index(root, surface);
            return surface;
        }

        public void DestroyPreviewSurface(IUISurface surface) => surface?.Destroy();
        public IReadOnlyList<IUIElementHandle> GetHierarchy(IUISurface surface) => (surface as DesignerSurface)?.Handles ?? new List<IUIElementHandle>();
        public bool TrySelectElement(IUISurface surface, string elementId, out IUIElementHandle handle) { handle = surface.TryFind(elementId); return handle != null; }

        public IUIElementHandle CreateElement(IUISurface surface, string parentId, DesignerElementCreateInfo createInfo)
        {
            var ds = surface as DesignerSurface;
            var element = new VisualElement { name = createInfo.elementId };
            if (surface.NativeRoot is VisualElement root)
                root.Add(element);
            return ds?.AddHandle(createInfo.elementId, element);
        }

        public void DeleteElement(IUISurface surface, IUIElementHandle handle)
        {
            if (handle?.Native is VisualElement ve) ve.RemoveFromHierarchy();
        }

        public IUIElementHandle DuplicateElement(IUISurface surface, IUIElementHandle handle)
        {
            var id = handle != null ? handle.Id + "Copy" : "ElementCopy";
            return CreateElement(surface, null, new DesignerElementCreateInfo { elementId = id, type = DesignerElementType.Container });
        }

        public void SetParent(IUIElementHandle child, IUIElementHandle newParent)
        {
            if (child?.Native is VisualElement childVe && newParent?.Native is VisualElement parentVe)
                parentVe.Add(childVe);
        }

        public void Reorder(IUIElementHandle handle, int newIndex)
        {
            if (handle?.Native is VisualElement ve) ve.PlaceInFront(null);
        }

        public Vector2 GetPosition(IUIElementHandle handle) => handle?.As<IUITransformCapability>()?.Position ?? Vector2.zero;
        public Vector2 GetSize(IUIElementHandle handle) => handle?.Native is VisualElement ve ? new Vector2(ve.resolvedStyle.width, ve.resolvedStyle.height) : Vector2.zero;
        public void SetPosition(IUIElementHandle handle, Vector2 position) { var cap = handle?.As<IUITransformCapability>(); if (cap != null) cap.Position = position; }
        public void SetSize(IUIElementHandle handle, Vector2 size) { if (handle?.Native is VisualElement ve) { ve.style.width = size.x; ve.style.height = size.y; } }
        // UI Toolkit layout is USS / flex driven, so absolute anchor presets do not map
        // 1:1 to a VisualElement. We translate the preset into an absolute-position hint on
        // the inline style so the preview reflects intent; USS authored in UI Builder wins at
        // runtime. Validation explains this to the user.
        public void SetAnchor(IUIElementHandle handle, DesignerAnchorPreset preset)
        {
            if (!(handle?.Native is VisualElement ve)) return;
            ve.style.position = Position.Absolute;
            switch (preset)
            {
                case DesignerAnchorPreset.TopLeft: ve.style.left = 0; ve.style.top = 0; break;
                case DesignerAnchorPreset.TopRight: ve.style.right = 0; ve.style.top = 0; break;
                case DesignerAnchorPreset.BottomLeft: ve.style.left = 0; ve.style.bottom = 0; break;
                case DesignerAnchorPreset.BottomRight: ve.style.right = 0; ve.style.bottom = 0; break;
                case DesignerAnchorPreset.Stretch:
                    ve.style.left = 0; ve.style.top = 0; ve.style.right = 0; ve.style.bottom = 0; break;
            }
        }
        public void SetVisible(IUIElementHandle handle, bool visible) { var cap = handle?.As<IUIVisibilityCapability>(); if (cap != null) cap.Visible = visible; }
        public void SetName(IUIElementHandle handle, string elementId) { (handle as DesignerElementHandle)?.Rename(elementId); if (handle?.Native is VisualElement ve) ve.name = elementId; }
        public void AddClass(IUIElementHandle handle, string className) { if (handle?.Native is VisualElement ve) ve.AddToClassList(className); handle?.As<IUIStyleCapability>()?.SetClass(className, true); }
        public void RemoveClass(IUIElementHandle handle, string className) { if (handle?.Native is VisualElement ve) ve.RemoveFromClassList(className); handle?.As<IUIStyleCapability>()?.SetClass(className, false); }
        public void SetBinding(IUIElementHandle handle, DesignerBindingMetadata binding)
        {
            if (!(handle is DesignerElementHandle h) || binding == null) return;
            h.Binding = binding;
            // Mirror the runtime-facing keys onto the VisualElement's viewDataKey so the
            // preview element carries a stable, inspectable trace of its binding intent.
            if (h.Native is VisualElement ve && !string.IsNullOrEmpty(binding.valueKey))
                ve.viewDataKey = binding.valueKey;
        }
        public DesignerBindingMetadata GetBinding(IUIElementHandle handle) => (handle as DesignerElementHandle)?.Binding ?? new DesignerBindingMetadata();
        public void Save(UIScreenDefinition definition, IUISurface surface) { if (definition != null) EditorUtility.SetDirty(definition); }

        private static void Index(VisualElement element, DesignerSurface surface)
        {
            if (!string.IsNullOrEmpty(element.name))
                surface.AddHandle(element.name, element);
            foreach (var child in element.Children())
                Index(child, surface);
        }
    }
}
