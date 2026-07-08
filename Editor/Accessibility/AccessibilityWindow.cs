using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Accessibility;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.Accessibility
{
    /// <summary>Full UI for accessibility preference preview (§19).</summary>
    public sealed class AccessibilityWindow : NexUIToolWindow
    {
        [SerializeField] private bool _reduceMotion;
        [SerializeField] private bool _highContrast;
        [SerializeField] private bool _largeText;

        protected override string TitleKey => "panel.accessibility";
        protected override string TooltipKey => "tooltip.accessibility";

        [MenuItem("Tools/NexUI/Designer/Accessibility")]
        public static void Open() => GetWindow<AccessibilityWindow>();

        protected override void DrawBody()
        {
            Section("panel.accessibility");
            _reduceMotion = EditorGUILayout.Toggle("reduce motion", _reduceMotion);
            _highContrast = EditorGUILayout.Toggle("high contrast", _highContrast);
            _largeText = EditorGUILayout.Toggle("large text", _largeText);

            var pref = new UIAccessibilityPreference
            {
                reduceMotion = _reduceMotion,
                highContrast = _highContrast,
                largeText = _largeText
            };

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Preview effects", EditorStyles.boldLabel);
            foreach (var effect in AccessibilityService.Effects(pref))
                EditorGUILayout.LabelField("• " + effect);
        }
    }
}
