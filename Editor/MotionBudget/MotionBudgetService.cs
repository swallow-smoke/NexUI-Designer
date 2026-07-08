using System.Collections.Generic;
using emiteat.NexUI.Motion;

namespace emiteat.NexUI.Designer.Editor.MotionBudget
{
    /// <summary>
    /// Counts motions declared on a screen's elements and checks them against a
    /// <see cref="UIMotionBudget"/> (concurrent cap / reduce-motion).
    /// </summary>
    public static class MotionBudgetService
    {
        public static int CountMotions(DesignerMetadataAsset asset)
        {
            int count = 0;
            if (asset == null) return 0;
            foreach (var e in asset.elements)
                if (e?.motion != null && !string.IsNullOrEmpty(e.motion.motionId))
                    count++;
            return count;
        }

        public static List<string> Validate(DesignerMetadataAsset asset, UIMotionBudget budget)
        {
            var messages = new List<string>();
            int count = CountMotions(asset);
            if (budget == null) return messages;

            if (count > budget.maxConcurrentMotions)
                messages.Add($"• {count} motions exceed budget of {budget.maxConcurrentMotions}");
            if (budget.reduceMotion)
                messages.Add("• reduce-motion is ON: shake/scale motions will be simplified");
            return messages;
        }
    }
}
