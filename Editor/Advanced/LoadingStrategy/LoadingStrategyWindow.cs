using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.LoadingStrategy
{
    /// <summary>Full UI for the screen loading strategy editor (§21).</summary>
    public sealed class LoadingStrategyWindow : NexUIToolWindow
    {
        [SerializeField] private UIScreenDefinition _definition;
        private List<string> _advice;

        protected override string TitleKey => "panel.loadingStrategy";
        protected override string TooltipKey => "tooltip.loadingStrategy";

        public static void Open() => GetWindow<LoadingStrategyWindow>();

        protected override void DrawBody()
        {
            _definition = (UIScreenDefinition)EditorGUILayout.ObjectField(
                "Screen Definition", _definition, typeof(UIScreenDefinition), false);
            if (_definition == null)
            {
                EditorGUILayout.HelpBox(T("message.noScreenSelected"), MessageType.Info);
                return;
            }

            Section("panel.loadingStrategy");
            EditorGUI.BeginChangeCheck();
            var strategy = (UIScreenLoadStrategy)EditorGUILayout.EnumPopup("load strategy", _definition.loadStrategy);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_definition, "Change Load Strategy");
                _definition.loadStrategy = strategy;
                MarkDirty(_definition);
            }

            _advice = LoadingStrategyService.Advise(_definition);
            if (_advice.Count > 0)
                EditorGUILayout.HelpBox(string.Join("\n", _advice), MessageType.Info);
        }
    }
}
