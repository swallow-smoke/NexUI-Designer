using System.Collections.Generic;
using System.Text;

namespace emiteat.NexUI.Designer.Editor.Serialization
{
    /// <summary>
    /// Result of a Designer save operation. Distinguishes what was actually persisted
    /// from what was skipped or is preview-only, so the tool never implies a change was
    /// written to disk when it was not (acceptance criterion).
    /// </summary>
    public sealed class DesignerSaveReport
    {
        /// <summary>Things that were written to disk.</summary>
        public readonly List<string> Changed = new List<string>();

        /// <summary>Things that were intentionally not written (preview-only, unsupported).</summary>
        public readonly List<string> Skipped = new List<string>();

        /// <summary>Non-fatal problems the user should know about.</summary>
        public readonly List<string> Warnings = new List<string>();

        /// <summary>Fatal problems that stopped part of the save.</summary>
        public readonly List<string> Errors = new List<string>();

        public bool HasErrors => Errors.Count > 0;
        public bool HasWarnings => Warnings.Count > 0;

        public void MarkChanged(string message) => Changed.Add(message);
        public void MarkSkipped(string message) => Skipped.Add(message);
        public void Warn(string message) => Warnings.Add(message);
        public void Error(string message) => Errors.Add(message);

        public void Merge(DesignerSaveReport other)
        {
            if (other == null) return;
            Changed.AddRange(other.Changed);
            Skipped.AddRange(other.Skipped);
            Warnings.AddRange(other.Warnings);
            Errors.AddRange(other.Errors);
        }

        /// <summary>One-line summary suitable for a toolbar status label.</summary>
        public string Summary()
        {
            if (HasErrors) return $"Save failed: {Errors.Count} error(s), {Changed.Count} change(s) written.";
            if (HasWarnings) return $"Saved with {Warnings.Count} warning(s). {Changed.Count} change(s) written.";
            return Changed.Count == 0 ? "Nothing to save (no changes)." : $"Saved. {Changed.Count} change(s) written.";
        }

        /// <summary>Full multi-line report for the console / a details panel.</summary>
        public string Details()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Summary());
            Append(sb, "Written", Changed);
            Append(sb, "Skipped / preview-only", Skipped);
            Append(sb, "Warnings", Warnings);
            Append(sb, "Errors", Errors);
            return sb.ToString().TrimEnd();
        }

        private static void Append(StringBuilder sb, string header, List<string> items)
        {
            if (items.Count == 0) return;
            sb.AppendLine();
            sb.AppendLine(header + ":");
            foreach (var item in items)
                sb.AppendLine("  - " + item);
        }
    }
}
