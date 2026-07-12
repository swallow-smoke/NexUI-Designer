using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor
{
    public enum DesignerPreviewLogKind { Command, State, Interaction, Info }

    public readonly struct DesignerPreviewLogEntry
    {
        public readonly DateTime Time;
        public readonly DesignerPreviewLogKind Kind;
        public readonly string ElementId;
        public readonly string Message;
        public readonly string Payload;

        public DesignerPreviewLogEntry(DesignerPreviewLogKind kind, string elementId, string message, string payload)
        {
            Time = DateTime.Now;
            Kind = kind;
            ElementId = elementId;
            Message = message;
            Payload = payload;
        }

        public override string ToString()
        {
            var head = $"[{Time:HH:mm:ss}] {Kind}: {Message}";
            if (!string.IsNullOrEmpty(ElementId)) head = $"[{Time:HH:mm:ss}] {Kind} ({ElementId}): {Message}";
            return string.IsNullOrEmpty(Payload) ? head : head + "  " + Payload;
        }
    }

    /// <summary>
    /// Session-only log of Interactive-Preview activity: <b>simulated</b> command invocations,
    /// forced state changes and interactions. Nothing here executes real game commands - the log is
    /// the safe record of "what would have fired". Newest entries are first; capped to avoid growth.
    /// </summary>
    public sealed class DesignerPreviewLog
    {
        private readonly List<DesignerPreviewLogEntry> _entries = new List<DesignerPreviewLogEntry>();
        private const int Max = 200;

        public IReadOnlyList<DesignerPreviewLogEntry> Entries => _entries;
        public event Action Changed;

        /// <summary>Raised when a command is simulated, so the shell can auto-open the Preview dock.</summary>
        public event Action<DesignerPreviewLogEntry> CommandSimulated;

        public void Log(DesignerPreviewLogKind kind, string elementId, string message, string payload = null)
        {
            var entry = new DesignerPreviewLogEntry(kind, elementId, message, payload);
            _entries.Insert(0, entry);
            if (_entries.Count > Max) _entries.RemoveAt(_entries.Count - 1);
            Changed?.Invoke();
            if (kind == DesignerPreviewLogKind.Command) CommandSimulated?.Invoke(entry);
        }

        public void Clear()
        {
            if (_entries.Count == 0) return;
            _entries.Clear();
            Changed?.Invoke();
        }
    }
}
