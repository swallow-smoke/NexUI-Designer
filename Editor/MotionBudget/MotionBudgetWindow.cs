using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Motion;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.MotionBudget
{
    /// <summary>Full UI for the motion budget system (§18).</summary>
    public sealed class MotionBudgetWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        [SerializeField] private int _maxConcurrent = 32;
        [SerializeField] private bool _reduceMotion;
        [SerializeField] private bool _skipLowPriority;
        private List<string> _issues;

        protected override string TitleKey => "panel.motionBudget";
        protected override string TooltipKey => "tooltip.motionBudget";

        [MenuItem("Tools/NexUI/Designer/Motion Budget")]
        public static void Open() => GetWindow<MotionBudgetWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);

            Section("panel.motionBudget");
            _maxConcurrent = EditorGUILayout.IntField("max concurrent", _maxConcurrent);
            _reduceMotion = EditorGUILayout.Toggle("reduce motion", _reduceMotion);
            _skipLowPriority = EditorGUILayout.Toggle("skip low priority", _skipLowPriority);

            if (_asset != null)
            {
                int count = MotionBudgetService.CountMotions(_asset);
                EditorGUILayout.HelpBox($"active motions on screen: {count} / {_maxConcurrent}",
                    count > _maxConcurrent ? MessageType.Warning : MessageType.None);

                if (GUILayout.Button(LC("toolbar.validate"), GUILayout.Width(120)))
                {
                    var budget = new UIMotionBudget
                    {
                        maxConcurrentMotions = _maxConcurrent,
                        reduceMotion = _reduceMotion,
                        skipLowPriorityMotions = _skipLowPriority
                    };
                    _issues = MotionBudgetService.Validate(_asset, budget);
                }
                if (_issues != null && _issues.Count > 0)
                    EditorGUILayout.HelpBox(T("validation.motionBudgetExceeded") + "\n" + string.Join("\n", _issues), MessageType.Warning);
            }
        }
    }
}
