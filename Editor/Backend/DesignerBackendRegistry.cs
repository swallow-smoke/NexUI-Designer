using System.Collections.Generic;
using emiteat.NexUI.Abstractions;

namespace emiteat.NexUI.Designer.Editor.Backend
{
    public static class DesignerBackendRegistry
    {
        private static readonly Dictionary<UIRenderBackend, INexUIDesignerBackend> Backends =
            new Dictionary<UIRenderBackend, INexUIDesignerBackend>();

        public static void Register(INexUIDesignerBackend backend)
        {
            if (backend != null)
                Backends[backend.Backend] = backend;
        }

        public static bool TryGet(UIRenderBackend backend, out INexUIDesignerBackend designerBackend)
            => Backends.TryGetValue(backend, out designerBackend);

        public static void RegisterDefaults()
        {
            if (!Backends.ContainsKey(UIRenderBackend.UIToolkit))
                Register(new UIToolkitDesignerBackend());
            if (!Backends.ContainsKey(UIRenderBackend.UGUI))
                Register(new UGUIDesignerBackend());
        }
    }
}
