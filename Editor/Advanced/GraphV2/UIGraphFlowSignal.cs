namespace emiteat.NexUI.Designer.Editor.GraphV2
{
    /// <summary>
    /// Editor-only marker type used purely to give every Flow port the same GraphView
    /// <c>Port.portType</c>, distinct from data ports (typed <c>typeof(UIGraphValue)</c>) - so
    /// <see cref="MotionGraphV2View.GetCompatiblePorts"/> can reject a Flow-to-Data connection
    /// before even checking direction. Never instantiated.
    /// </summary>
    public sealed class UIGraphFlowSignal
    {
        private UIGraphFlowSignal() { }
    }
}
