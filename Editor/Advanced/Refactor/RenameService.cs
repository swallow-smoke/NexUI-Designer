using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.Refactor
{
    public enum RenameKind
    {
        ElementId, ScreenId, StateKey, ActionKey, MotionId, ThemeId, ThemeToken,
        VariantId, ResponsiveRuleId, ContractId, LocalizationKey, PromptActionId
    }

    /// <summary>
    /// Safe rename of identifiers across a <see cref="DesignerMetadataAsset"/>, updating
    /// every reference. Find (dry-run) and Apply share one traversal; Apply is Undo-safe
    /// when the caller records the asset first.
    /// </summary>
    public static class RenameService
    {
        public static List<string> Find(DesignerMetadataAsset asset, RenameKind kind, string oldName)
        {
            var messages = new List<string>();
            Process(asset, kind, oldName, null, messages);
            return messages;
        }

        public static int Apply(DesignerMetadataAsset asset, RenameKind kind, string oldName, string newName)
        {
            var messages = new List<string>();
            Process(asset, kind, oldName, newName, messages);
            return messages.Count;
        }

        private static void Process(DesignerMetadataAsset asset, RenameKind kind, string oldName, string newName, List<string> hits)
        {
            if (asset == null || string.IsNullOrEmpty(oldName)) return;
            bool apply = newName != null;

            switch (kind)
            {
                case RenameKind.ElementId:
                    foreach (var e in asset.elements)
                    {
                        Hit(ref e.elementId, oldName, newName, apply, hits, $"element '{oldName}'");
                        Hit(ref e.parentId, oldName, newName, apply, hits, $"parentId on {e.elementId}");
                    }
                    foreach (var v in asset.variants)
                        foreach (var o in v.overrides)
                            Hit(ref o.targetElementId, oldName, newName, apply, hits, $"variant '{v.variantId}' override");
                    foreach (var r in asset.responsiveRules)
                        foreach (var o in r.overrides)
                            Hit(ref o.elementId, oldName, newName, apply, hits, $"rule '{r.ruleId}' override");
                    if (asset.contract != null)
                        foreach (var req in asset.contract.requiredElements)
                            Hit(ref req.elementId, oldName, newName, apply, hits, "contract requirement");
                    if (asset.localization != null)
                        foreach (var link in asset.localization.links)
                            Hit(ref link.elementId, oldName, newName, apply, hits, "localization link");
                    if (asset.snapshots != null)
                        foreach (var snap in asset.snapshots.baselines)
                            foreach (var el in snap.elements)
                                Hit(ref el.elementId, oldName, newName, apply, hits, $"snapshot '{snap.snapshotId}'");
                    break;

                case RenameKind.ScreenId:
                    Hit(ref asset.screenId, oldName, newName, apply, hits, "screenId");
                    if (asset.contract != null) Hit(ref asset.contract.screenId, oldName, newName, apply, hits, "contract.screenId");
                    if (asset.snapshots != null) Hit(ref asset.snapshots.screenId, oldName, newName, apply, hits, "snapshots.screenId");
                    if (asset.localization != null) Hit(ref asset.localization.screenId, oldName, newName, apply, hits, "localization.screenId");
                    break;

                case RenameKind.StateKey:
                    foreach (var e in asset.elements)
                    {
                        Hit(ref e.binding.textKey, oldName, newName, apply, hits, $"{e.elementId}.textKey");
                        Hit(ref e.binding.valueKey, oldName, newName, apply, hits, $"{e.elementId}.valueKey");
                        Hit(ref e.binding.visibilityKey, oldName, newName, apply, hits, $"{e.elementId}.visibilityKey");
                        Hit(ref e.binding.interactableKey, oldName, newName, apply, hits, $"{e.elementId}.interactableKey");
                        Hit(ref e.binding.classKey, oldName, newName, apply, hits, $"{e.elementId}.classKey");
                    }
                    break;

                case RenameKind.ActionKey:
                    foreach (var e in asset.elements)
                        Hit(ref e.binding.commandKey, oldName, newName, apply, hits, $"{e.elementId}.commandKey");
                    break;

                case RenameKind.MotionId:
                    foreach (var e in asset.elements)
                        Hit(ref e.motion.motionId, oldName, newName, apply, hits, $"{e.elementId}.motionId");
                    break;

                case RenameKind.ThemeId:
                    foreach (var e in asset.elements)
                        Hit(ref e.theme.themeId, oldName, newName, apply, hits, $"{e.elementId}.themeId");
                    break;

                case RenameKind.ThemeToken:
                    foreach (var e in asset.elements)
                        foreach (var tok in e.theme.tokenOverrides)
                            Hit(ref tok.key, oldName, newName, apply, hits, $"{e.elementId} token");
                    break;

                case RenameKind.VariantId:
                    foreach (var v in asset.variants)
                        Hit(ref v.variantId, oldName, newName, apply, hits, "variantId");
                    break;

                case RenameKind.ResponsiveRuleId:
                    foreach (var r in asset.responsiveRules)
                        Hit(ref r.ruleId, oldName, newName, apply, hits, "ruleId");
                    break;

                case RenameKind.ContractId:
                    if (asset.contract != null) Hit(ref asset.contract.contractId, oldName, newName, apply, hits, "contractId");
                    break;

                case RenameKind.LocalizationKey:
                    if (asset.localization != null)
                    {
                        foreach (var link in asset.localization.links)
                            Hit(ref link.localizationKey, oldName, newName, apply, hits, "localization link key");
                        foreach (var row in asset.localization.previewRows)
                            Hit(ref row.key, oldName, newName, apply, hits, "localization row key");
                    }
                    break;

                case RenameKind.PromptActionId:
                    if (asset.prompts != null)
                        foreach (var a in asset.prompts.actions)
                            Hit(ref a.actionId, oldName, newName, apply, hits, "prompt actionId");
                    break;
            }
        }

        private static void Hit(ref string field, string oldName, string newName, bool apply, List<string> hits, string where)
        {
            if (field != oldName) return;
            hits.Add($"• {where}");
            if (apply) field = newName;
        }
    }
}
