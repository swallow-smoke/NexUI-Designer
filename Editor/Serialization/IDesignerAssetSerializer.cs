using emiteat.NexUI.Core;

namespace emiteat.NexUI.Designer.Editor.Serialization
{
    /// <summary>
    /// Persists Designer metadata (and, where safe, the backend asset itself) for one screen.
    /// Implementations return a <see cref="DesignerSaveReport"/> describing exactly what was
    /// written vs. skipped so the caller can report the truth to the user.
    /// </summary>
    public interface IDesignerAssetSerializer
    {
        DesignerSaveReport Save(UIScreenDefinition definition, DesignerMetadataAsset metadata);
    }
}
