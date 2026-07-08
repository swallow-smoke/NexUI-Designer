using emiteat.NexUI.Core;

namespace emiteat.NexUI.Designer.Editor.Serialization
{
    public interface IDesignerAssetSerializer
    {
        void Save(UIScreenDefinition definition, DesignerMetadataAsset metadata);
    }
}
