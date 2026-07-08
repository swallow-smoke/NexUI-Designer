using emiteat.NexUI.State;
using emiteat.NexUI.Theme;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed class NexUIDesignerPreviewRuntime
    {
        public UIStateStore StateStore { get; private set; } = new UIStateStore();
        public ThemeRegistry ThemeRegistry { get; private set; } = new ThemeRegistry();
        public void Reset() => StateStore.Clear();
    }
}
