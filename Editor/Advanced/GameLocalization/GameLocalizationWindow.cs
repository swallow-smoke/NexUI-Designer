using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Localization;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.GameLocalization
{
    /// <summary>Full UI for the game UI localization table editor (§14).</summary>
    public sealed class GameLocalizationWindow : NexUIToolWindow
    {
        [SerializeField] private UIGameLocalizationTable _table;
        [SerializeField] private int _maxChars = 40;
        private List<string> _issues;

        protected override string TitleKey => "panel.gameLocalization";
        protected override string TooltipKey => "tooltip.gameLocalization";

        public static void Open() => GetWindow<GameLocalizationWindow>();

        protected override void DrawBody()
        {
            EditorGUILayout.BeginHorizontal();
            _table = (UIGameLocalizationTable)EditorGUILayout.ObjectField("table", _table, typeof(UIGameLocalizationTable), false);
            if (GUILayout.Button(LC("button.create"), GUILayout.Width(80)))
                CreateTable();
            EditorGUILayout.EndHorizontal();
            if (_table == null)
            {
                EditorGUILayout.HelpBox("Assign or create a UIGameLocalizationTable.", MessageType.Info);
                return;
            }

            Section("panel.gameLocalization");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("key", GUILayout.Width(140));
            EditorGUILayout.LabelField("ko-KR");
            EditorGUILayout.LabelField("en-US");
            GUILayout.Space(26);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < _table.entries.Count; i++)
            {
                var e = _table.entries[i];
                EditorGUILayout.BeginHorizontal();
                e.key = EditorGUILayout.TextField(e.key, GUILayout.Width(140));
                e.koKR = EditorGUILayout.TextField(e.koKR);
                e.enUS = EditorGUILayout.TextField(e.enUS);
                if (GUILayout.Button("×", GUILayout.Width(22))) { Undo.RecordObject(_table, "Remove Entry"); _table.entries.RemoveAt(i); MarkDirty(_table); break; }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ entry", GUILayout.Width(90)))
            {
                Undo.RecordObject(_table, "Add Entry");
                GameLocalizationService.Add(_table, "key." + (_table.entries.Count + 1));
                MarkDirty(_table);
            }

            Section("panel.validation");
            _maxChars = EditorGUILayout.IntField("overflow max chars", _maxChars);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find missing", GUILayout.Width(120)))
                _issues = GameLocalizationService.FindMissing(_table);
            if (GUILayout.Button("Find overflow", GUILayout.Width(120)))
                _issues = GameLocalizationService.FindOverflow(_table, _maxChars);
            EditorGUILayout.EndHorizontal();
            if (_issues != null)
                EditorGUILayout.HelpBox(_issues.Count == 0 ? T("message.validationPassed") : string.Join("\n", _issues),
                    _issues.Count == 0 ? MessageType.Info : MessageType.Warning);
        }

        private void CreateTable()
        {
            string path = EditorUtility.SaveFilePanelInProject("Create Localization Table", "GameLocalizationTable.asset", "asset", "");
            if (string.IsNullOrEmpty(path)) return;
            var t = ScriptableObject.CreateInstance<UIGameLocalizationTable>();
            AssetDatabase.CreateAsset(t, path);
            AssetDatabase.SaveAssets();
            _table = t;
        }
    }
}
