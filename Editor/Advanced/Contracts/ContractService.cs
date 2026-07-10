using System.Collections.Generic;
using emiteat.NexUI.Core;

namespace emiteat.NexUI.Designer.Editor.Contracts
{
    /// <summary>
    /// Authoring + checking logic for UI contracts. Capabilities are inferred from which
    /// binding keys an element declares (commandKey → click, textKey → text, ...), since
    /// the Designer element model expresses capabilities through bindings.
    /// </summary>
    public static class ContractService
    {
        /// <summary>Capability interface names a contract can require.</summary>
        public static readonly string[] KnownCapabilities =
        {
            "IUITextCapability", "IUIValueCapability", "IUIVisibilityCapability",
            "IUIInteractableCapability", "IUIClickCapability", "IUIStyleCapability",
            "IUITransformCapability"
        };

        public static DesignerContractElementMetadata AddRequirement(DesignerContractMetadata contract, string elementId)
        {
            var e = new DesignerContractElementMetadata { elementId = elementId };
            contract.requiredElements.Add(e);
            return e;
        }

        public static void RemoveRequirement(DesignerContractMetadata contract, DesignerContractElementMetadata e)
            => contract.requiredElements.Remove(e);

        public static bool HasCapability(DesignerElementMetadata e, string capability)
        {
            if (e == null) return false;
            var b = e.binding;
            switch (capability)
            {
                case "IUITextCapability": return !string.IsNullOrEmpty(b.textKey);
                case "IUIValueCapability": return !string.IsNullOrEmpty(b.valueKey);
                case "IUIVisibilityCapability": return !string.IsNullOrEmpty(b.visibilityKey);
                case "IUIInteractableCapability": return !string.IsNullOrEmpty(b.interactableKey);
                case "IUIClickCapability": return !string.IsNullOrEmpty(b.commandKey);
                case "IUIStyleCapability": return !string.IsNullOrEmpty(b.classKey) || e.classes.Count > 0;
                case "IUITransformCapability": return true;
                default: return true; // unknown capability: don't fail the check
            }
        }

        /// <summary>Returns unmet-requirement messages; empty means the contract is satisfied.</summary>
        public static List<string> CheckSatisfaction(DesignerMetadataAsset asset)
        {
            var messages = new List<string>();
            var contract = asset.contract;
            if (contract == null) return messages;

            foreach (var req in contract.requiredElements)
            {
                var element = asset.Find(req.elementId);
                if (element == null)
                {
                    if (req.required)
                        messages.Add($"• missing required element: {req.elementId}");
                    continue;
                }
                foreach (var cap in req.requiredCapabilities)
                    if (!HasCapability(element, cap))
                        messages.Add($"• {req.elementId} missing capability: {cap}");
            }
            return messages;
        }

        public static List<string> Validate(DesignerMetadataAsset asset)
        {
            var messages = new List<string>();
            var contract = asset.contract;
            if (contract == null) return messages;

            if (!string.IsNullOrEmpty(contract.screenId) && !string.IsNullOrEmpty(asset.screenId)
                && contract.screenId != asset.screenId)
                messages.Add($"• contract.screenId '{contract.screenId}' != screen '{asset.screenId}'");

            foreach (var req in contract.requiredElements)
                if (string.IsNullOrEmpty(req.elementId))
                    messages.Add("• requirement with empty elementId");

            return messages;
        }

        /// <summary>Populates a runtime <see cref="UIScreenContract"/> from designer metadata.</summary>
        public static void Populate(UIScreenContract target, DesignerContractMetadata contract)
        {
            target.screenId = contract.screenId;
            target.requiredElements = new List<UIElementContract>();
            foreach (var req in contract.requiredElements)
                target.requiredElements.Add(new UIElementContract
                {
                    elementId = req.elementId,
                    required = req.required,
                    requiredCapabilities = new List<string>(req.requiredCapabilities)
                });
        }
    }
}
