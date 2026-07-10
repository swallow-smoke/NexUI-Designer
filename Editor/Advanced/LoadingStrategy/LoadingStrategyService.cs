using System.Collections.Generic;
using emiteat.NexUI.Core;

namespace emiteat.NexUI.Designer.Editor.LoadingStrategy
{
    /// <summary>
    /// Editor-side advice for a screen's load strategy (§21), mirroring the runtime
    /// <c>ScreenLoadingStrategyValidator</c> but for a single definition being edited.
    /// </summary>
    public static class LoadingStrategyService
    {
        public static List<string> Advise(UIScreenDefinition def)
        {
            var messages = new List<string>();
            if (def == null) return messages;

            switch (def.loadStrategy)
            {
                case UIScreenLoadStrategy.Addressable:
                    if (def.backendAsset.asset == null)
                        messages.Add("• Addressable: no referenced asset/key set on this screen.");
                    break;
                case UIScreenLoadStrategy.KeepAlive:
                    messages.Add("• KeepAlive: this screen stays resident for the whole session (memory cost).");
                    break;
                case UIScreenLoadStrategy.Pool:
                    if (def.policy.lifetimePolicy != UILifetimePolicy.Pool)
                        messages.Add("• Pool loading pairs best with a Pool lifetime policy.");
                    break;
                case UIScreenLoadStrategy.Preload:
                    messages.Add("• Preload: instantiated at startup — keep these few.");
                    break;
            }
            return messages;
        }
    }
}
