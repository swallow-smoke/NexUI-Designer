using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// Designer-side authoring record describing a UI recipe instance placed into a
    /// screen (which recipe, and the id prefix used for the generated elements).
    /// Mirrors Core's UIRecipe / UIRecipeElement for editor generation.
    /// </summary>
    [Serializable]
    public sealed class DesignerRecipeMetadata
    {
        public string recipeId;
        public string displayName;
        public string idPrefix;
        public List<DesignerRecipeElementMetadata> elements = new();
    }

    [Serializable]
    public sealed class DesignerRecipeElementMetadata
    {
        public string elementId;
        public string elementType;
        public string parentElementId;
        public List<string> capabilities = new();
        public List<string> bindingCandidates = new();
    }
}
