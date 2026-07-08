using emiteat.NexUI.Designer.Editor.Viewport;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Tools
{
    public interface INexUIDesignerTool
    {
        string ToolId { get; }
        string DisplayNameKey { get; }
        void OnActivate(NexUIDesignerContext context);
        void OnDeactivate(NexUIDesignerContext context);
        void OnGUI(NexUIDesignerViewport viewport);
        void HandleInput(Event e, NexUIDesignerContext context);
    }

    public abstract class NexUIDesignerToolBase : INexUIDesignerTool
    {
        public abstract string ToolId { get; }
        public abstract string DisplayNameKey { get; }
        public virtual void OnActivate(NexUIDesignerContext context) { }
        public virtual void OnDeactivate(NexUIDesignerContext context) { }
        public virtual void OnGUI(NexUIDesignerViewport viewport) { }
        public virtual void HandleInput(Event e, NexUIDesignerContext context) { }
    }
}
