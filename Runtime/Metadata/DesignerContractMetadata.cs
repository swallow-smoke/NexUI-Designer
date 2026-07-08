using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer
{
    /// <summary>
    /// Designer-side authoring record for a screen's UI contract: the elements and
    /// capabilities the screen must provide. Mirrors Core's UIScreenContract.
    /// </summary>
    [Serializable]
    public sealed class DesignerContractMetadata
    {
        public string contractId;
        public string screenId;
        public List<DesignerContractElementMetadata> requiredElements = new();
    }

    [Serializable]
    public sealed class DesignerContractElementMetadata
    {
        public string elementId;
        public List<string> requiredCapabilities = new();
        public bool required = true;
    }
}
