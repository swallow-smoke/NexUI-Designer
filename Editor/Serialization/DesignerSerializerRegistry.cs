using emiteat.NexUI.Abstractions;

namespace emiteat.NexUI.Designer.Editor.Serialization
{
    /// <summary>Resolves the correct <see cref="IDesignerAssetSerializer"/> for a backend.</summary>
    public static class DesignerSerializerRegistry
    {
        private static readonly UIToolkitAssetSerializer UIToolkit = new UIToolkitAssetSerializer();
        private static readonly UGUIAssetSerializer UGUI = new UGUIAssetSerializer();

        public static IDesignerAssetSerializer Get(UIRenderBackend backend)
        {
            switch (backend)
            {
                case UIRenderBackend.UGUI: return UGUI;
                case UIRenderBackend.UIToolkit:
                default: return UIToolkit;
            }
        }
    }
}
