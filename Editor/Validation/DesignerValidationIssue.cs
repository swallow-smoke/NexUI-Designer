namespace emiteat.NexUI.Designer.Editor.Validation
{
    public enum DesignerValidationSeverity
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// A single, actionable validation result. Carries enough context (severity, stable
    /// code, screen / element ids, a human message and a suggested fix) to be understood
    /// without reading source code.
    /// </summary>
    public sealed class DesignerValidationIssue
    {
        public DesignerValidationSeverity Severity;
        public string Code;
        public string ScreenId;
        public string ElementId;
        public string Message;
        public string Fix;

        public DesignerValidationIssue(DesignerValidationSeverity severity, string code, string message, string fix,
            string screenId = null, string elementId = null)
        {
            Severity = severity;
            Code = code;
            Message = message;
            Fix = fix;
            ScreenId = screenId;
            ElementId = elementId;
        }

        /// <summary>Compact one-line rendering, e.g. "[Error] duplicate-id (loginButton): ... → Fix: ...".</summary>
        public override string ToString()
        {
            var scope = string.IsNullOrEmpty(ElementId) ? Code : $"{Code} ({ElementId})";
            var text = $"[{Severity}] {scope}: {Message}";
            if (!string.IsNullOrEmpty(Fix)) text += $"  →  Fix: {Fix}";
            return text;
        }
    }
}
