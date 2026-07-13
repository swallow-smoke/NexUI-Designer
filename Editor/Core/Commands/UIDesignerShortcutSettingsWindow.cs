using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Commands
{
    /// <summary>User-facing editor for the persisted Designer shortcut registry.</summary>
    public sealed class UIDesignerShortcutSettingsWindow : EditorWindow
    {
        private Vector2 scroll;

        [MenuItem("Tools/NexUI/Designer/Preferences/단축키 설정")]
        public static void Open() => GetWindow<UIDesignerShortcutSettingsWindow>("NexUI 단축키");

        private void OnGUI()
        {
            EditorGUILayout.LabelField("NexUI Designer 단축키", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("동일한 키 조합이 여러 Command에 지정되면 위쪽 항목이 먼저 실행됩니다.", MessageType.Info);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            var shortcuts = UIDesignerShortcutRegistry.Current;
            for (var i = 0; i < shortcuts.Count; i++)
            {
                var item = shortcuts[i];
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(item.commandId, GUILayout.Width(170));
                    item.key = (KeyCode)EditorGUILayout.EnumPopup(item.key, GUILayout.Width(140));
                    item.ctrl = GUILayout.Toggle(item.ctrl, "Ctrl", GUILayout.Width(48));
                    item.shift = GUILayout.Toggle(item.shift, "Shift", GUILayout.Width(52));
                    item.alt = GUILayout.Toggle(item.alt, "Alt", GUILayout.Width(42));
                }
            }
            EditorGUILayout.EndScrollView();

            var duplicates = shortcuts.GroupBy(Signature).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicates.Count > 0)
                EditorGUILayout.HelpBox("중복 조합: " + string.Join(", ", duplicates), MessageType.Warning);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("기본값 복원")) UIDesignerShortcutRegistry.ResetToDefaults();
                if (GUILayout.Button("저장"))
                {
                    UIDesignerShortcutRegistry.Save();
                    ShowNotification(new GUIContent("단축키를 저장했습니다."));
                }
            }
        }

        private static string Signature(UIDesignerShortcut x)
            => $"{(x.ctrl ? "Ctrl+" : "")}{(x.shift ? "Shift+" : "")}{(x.alt ? "Alt+" : "")}{x.key}";
    }
}
