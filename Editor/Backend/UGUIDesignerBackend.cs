using System.Collections.Generic;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Backend
{
    public sealed class UGUIDesignerBackend : INexUIDesignerBackend
    {
        public UIRenderBackend Backend => UIRenderBackend.UGUI;

        public IUISurface CreatePreviewSurface(UIScreenDefinition definition)
        {
            var prefab = definition != null ? definition.backendAsset.asset as GameObject : null;
            var root = prefab != null
                ? PrefabUtility.InstantiatePrefab(prefab) as GameObject
                : new GameObject(definition != null ? definition.ScreenId : "Preview", typeof(RectTransform));
            root.hideFlags = HideFlags.HideAndDontSave;
            var surface = new DesignerSurface(definition != null ? definition.ScreenId : "Preview", Backend, root);
            Index(root.transform, surface);
            return surface;
        }

        public void DestroyPreviewSurface(IUISurface surface) => surface?.Destroy();
        public IReadOnlyList<IUIElementHandle> GetHierarchy(IUISurface surface) => (surface as DesignerSurface)?.Handles ?? new List<IUIElementHandle>();
        public bool TrySelectElement(IUISurface surface, string elementId, out IUIElementHandle handle) { handle = surface.TryFind(elementId); return handle != null; }

        public IUIElementHandle CreateElement(IUISurface surface, string parentId, DesignerElementCreateInfo createInfo)
        {
            var ds = surface as DesignerSurface;
            var go = new GameObject(createInfo.elementId, typeof(RectTransform));
            if (surface.NativeRoot is GameObject root)
                go.transform.SetParent(root.transform, false);
            return ds?.AddHandle(createInfo.elementId, go);
        }

        public void DeleteElement(IUISurface surface, IUIElementHandle handle)
        {
            if (handle?.Native is GameObject go) Object.DestroyImmediate(go);
        }

        public IUIElementHandle DuplicateElement(IUISurface surface, IUIElementHandle handle)
        {
            var id = handle != null ? handle.Id + "Copy" : "ElementCopy";
            return CreateElement(surface, null, new DesignerElementCreateInfo { elementId = id, type = DesignerElementType.Container });
        }

        public void SetParent(IUIElementHandle child, IUIElementHandle newParent)
        {
            if (child?.Native is GameObject c && newParent?.Native is GameObject p)
                c.transform.SetParent(p.transform, false);
        }

        public void Reorder(IUIElementHandle handle, int newIndex)
        {
            if (handle?.Native is GameObject go)
                go.transform.SetSiblingIndex(Mathf.Max(0, newIndex));
        }

        public Vector2 GetPosition(IUIElementHandle handle) => handle?.Native is GameObject go && go.transform is RectTransform rt ? rt.anchoredPosition : Vector2.zero;
        public Vector2 GetSize(IUIElementHandle handle) => handle?.Native is GameObject go && go.transform is RectTransform rt ? rt.sizeDelta : Vector2.zero;
        public void SetPosition(IUIElementHandle handle, Vector2 position) { if (handle?.Native is GameObject go && go.transform is RectTransform rt) rt.anchoredPosition = position; }
        public void SetSize(IUIElementHandle handle, Vector2 size) { if (handle?.Native is GameObject go && go.transform is RectTransform rt) rt.sizeDelta = size; }
        public void SetAnchor(IUIElementHandle handle, DesignerAnchorPreset preset) { }
        public void SetVisible(IUIElementHandle handle, bool visible) { if (handle?.Native is GameObject go) go.SetActive(visible); }
        public void SetName(IUIElementHandle handle, string elementId) { (handle as DesignerElementHandle)?.Rename(elementId); if (handle?.Native is GameObject go) go.name = elementId; }
        public void AddClass(IUIElementHandle handle, string className) => handle?.As<IUIStyleCapability>()?.SetClass(className, true);
        public void RemoveClass(IUIElementHandle handle, string className) => handle?.As<IUIStyleCapability>()?.SetClass(className, false);
        public void SetBinding(IUIElementHandle handle, DesignerBindingMetadata binding) { }
        public DesignerBindingMetadata GetBinding(IUIElementHandle handle) => new DesignerBindingMetadata();
        public void Save(UIScreenDefinition definition, IUISurface surface) { if (definition != null) EditorUtility.SetDirty(definition); }

        private static void Index(Transform transform, DesignerSurface surface)
        {
            surface.AddHandle(transform.name, transform.gameObject);
            for (int i = 0; i < transform.childCount; i++)
                Index(transform.GetChild(i), surface);
        }
    }
}
