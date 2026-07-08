using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.Cleaner
{
    /// <summary>A broken reference plus a delegate that removes/clears it.</summary>
    public sealed class DeadReference
    {
        public string description;
        public Action fix;
    }

    /// <summary>
    /// Finds references inside a <see cref="DesignerMetadataAsset"/> that point at
    /// element ids which no longer exist (parents, variant/responsive overrides,
    /// contract requirements, localization links) and offers a per-item fix.
    /// </summary>
    public static class CleanerService
    {
        public static List<DeadReference> Find(DesignerMetadataAsset asset)
        {
            var dead = new List<DeadReference>();
            if (asset == null) return dead;

            foreach (var e in asset.elements)
            {
                if (e == null) continue;
                if (!string.IsNullOrEmpty(e.parentId) && asset.Find(e.parentId) == null)
                {
                    var el = e;
                    dead.Add(new DeadReference
                    {
                        description = $"{el.elementId}.parentId → missing '{el.parentId}'",
                        fix = () => el.parentId = null
                    });
                }
            }

            foreach (var v in asset.variants)
                foreach (var o in new List<DesignerVariantOverrideMetadata>(v.overrides))
                    if (string.IsNullOrEmpty(o.targetElementId) || asset.Find(o.targetElementId) == null)
                    {
                        var (vv, oo) = (v, o);
                        dead.Add(new DeadReference
                        {
                            description = $"variant '{vv.variantId}' → missing element '{oo.targetElementId}'",
                            fix = () => vv.overrides.Remove(oo)
                        });
                    }

            foreach (var r in asset.responsiveRules)
                foreach (var o in new List<DesignerResponsiveOverrideMetadata>(r.overrides))
                    if (string.IsNullOrEmpty(o.elementId) || asset.Find(o.elementId) == null)
                    {
                        var (rr, oo) = (r, o);
                        dead.Add(new DeadReference
                        {
                            description = $"rule '{rr.ruleId}' → missing element '{oo.elementId}'",
                            fix = () => rr.overrides.Remove(oo)
                        });
                    }

            if (asset.contract != null)
                foreach (var req in new List<DesignerContractElementMetadata>(asset.contract.requiredElements))
                    if (!string.IsNullOrEmpty(req.elementId) && asset.Find(req.elementId) == null)
                    {
                        var rq = req;
                        dead.Add(new DeadReference
                        {
                            description = $"contract → missing element '{rq.elementId}'",
                            fix = () => asset.contract.requiredElements.Remove(rq)
                        });
                    }

            if (asset.localization != null)
                foreach (var link in new List<DesignerLocalizationLink>(asset.localization.links))
                    if (!string.IsNullOrEmpty(link.elementId) && asset.Find(link.elementId) == null)
                    {
                        var lk = link;
                        dead.Add(new DeadReference
                        {
                            description = $"localization link → missing element '{lk.elementId}'",
                            fix = () => asset.localization.links.Remove(lk)
                        });
                    }

            return dead;
        }

        /// <summary>Applies every fix and returns how many were removed.</summary>
        public static int CleanAll(List<DeadReference> dead)
        {
            int count = 0;
            foreach (var d in dead) { d.fix?.Invoke(); count++; }
            return count;
        }
    }
}
