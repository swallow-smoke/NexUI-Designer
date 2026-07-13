using System;
using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.Productivity
{
    public enum DesignerTextPreset { Empty, Short, Long, Multiline, MaxLength, Korean, English, Japanese, Number, SpecialCharacters }
    public enum DesignerValuePreset { Zero, Minimum, Middle, Maximum, OverRange }

    /// <summary>Applies common edge-case preview values without creating a second binding runtime.</summary>
    public static class DesignerMockDataPresetService
    {
        public static void ApplyText(NexUIDesignerContext context, DesignerTextPreset preset)
        {
            var value = Text(preset);
            Apply(context, e => e.text = value, "Apply Text Test Preset");
        }

        public static void ApplyValue(NexUIDesignerContext context, DesignerValuePreset preset)
        {
            var value = preset == DesignerValuePreset.Zero || preset == DesignerValuePreset.Minimum ? 0f
                : preset == DesignerValuePreset.Middle ? 50f : preset == DesignerValuePreset.Maximum ? 100f : 125f;
            Apply(context, e => e.previewValue = value, "Apply Value Test Preset");
        }

        public static void ApplyCollection(NexUIDesignerContext context, int count)
        {
            Apply(context, e => e.previewItemCount = Math.Max(0, count), "Apply Collection Test Preset");
        }

        private static void Apply(NexUIDesignerContext context, Action<DesignerElementMetadata> change, string undo)
        {
            if (context == null || context.SelectedElements.Count == 0) return;
            var selection = new List<DesignerElementMetadata>(context.SelectedElements);
            NexUIDesignerUndo.Group(undo, () =>
            {
                foreach (var element in selection) context.UpdateElement(element, change, undo);
            });
        }

        private static string Text(DesignerTextPreset preset)
        {
            switch (preset)
            {
                case DesignerTextPreset.Empty: return string.Empty;
                case DesignerTextPreset.Short: return "OK";
                case DesignerTextPreset.Long: return "This is a deliberately long preview string used to expose clipping and overflow problems.";
                case DesignerTextPreset.Multiline: return "첫 번째 줄\n두 번째 줄\n세 번째 줄";
                case DesignerTextPreset.MaxLength: return new string('가', 256);
                case DesignerTextPreset.Korean: return "한국어 길이와 줄바꿈을 확인합니다";
                case DesignerTextPreset.English: return "English preview text";
                case DesignerTextPreset.Japanese: return "日本語のプレビューテキスト";
                case DesignerTextPreset.Number: return "123,456,789.00";
                case DesignerTextPreset.SpecialCharacters: return "!@#$%^&*()_+-=[]{}<>/\\";
                default: return string.Empty;
            }
        }
    }
}
