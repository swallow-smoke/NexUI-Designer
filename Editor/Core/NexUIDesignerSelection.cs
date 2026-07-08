using emiteat.NexUI.Abstractions;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed class NexUIDesignerSelection
    {
        public IUIElementHandle Current { get; private set; }
        public void Set(IUIElementHandle handle) => Current = handle;
        public void Clear() => Current = null;
    }
}
