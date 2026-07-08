using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer
{
    [CreateAssetMenu(menuName = "NexUI/Designer/Metadata", fileName = "NexUIDesignerMetadata")]
    public sealed class DesignerMetadataAsset : ScriptableObject
    {
        public string screenId;
        public List<DesignerElementMetadata> elements = new List<DesignerElementMetadata>();

        // ---- Advanced Extension Pack metadata --------------------------------
        public List<DesignerVariantMetadata> variants = new List<DesignerVariantMetadata>();
        public List<DesignerResponsiveMetadata> responsiveRules = new List<DesignerResponsiveMetadata>();
        public DesignerContractMetadata contract = new DesignerContractMetadata();
        public DesignerSnapshotMetadata snapshots = new DesignerSnapshotMetadata();
        public DesignerLocalizationMetadata localization = new DesignerLocalizationMetadata();
        public DesignerPromptMetadata prompts = new DesignerPromptMetadata();
        public List<DesignerRecipeMetadata> recipes = new List<DesignerRecipeMetadata>();

        public DesignerElementMetadata Find(string elementId)
        {
            for (int i = 0; i < elements.Count; i++)
                if (elements[i] != null && elements[i].elementId == elementId)
                    return elements[i];
            return null;
        }
    }
}
