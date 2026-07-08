using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Serialization;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed class NexUIDesignerSerializer
    {
        public void Save(UIScreenDefinition definition)
        {
            if (definition == null) return;
            DesignerMetadataUtility.MarkDirty(definition);
        }
    }
}
