using emiteat.NexUI.Core;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor.Serialization
{
    public sealed class UIToolkitAssetSerializer : IDesignerAssetSerializer
    {
        public void Save(UIScreenDefinition definition, DesignerMetadataAsset metadata)
        {
            DesignerMetadataUtility.MarkDirty(definition);
            DesignerMetadataUtility.MarkDirty(metadata);
            AssetDatabase.SaveAssets();
        }
    }
}
