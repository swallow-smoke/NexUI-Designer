using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Localization;

namespace emiteat.NexUI.Designer.Editor.Common
{
    /// <summary>
    /// Base class for the standalone NexUI Designer advanced tool windows. Provides a
    /// localized header + description, a scroll body, and Korean-aware tooltip helpers.
    /// Subclasses implement <see cref="DrawBody"/> with IMGUI.
    /// </summary>
    public abstract class NexUIToolWindow : EditorWindow
    {
        private Vector2 _scroll;

        /// <summary>Localization key for the window title.</summary>
        protected abstract string TitleKey { get; }

        /// <summary>Localization key for the window description / tab tooltip.</summary>
        protected abstract string TooltipKey { get; }

        protected static string T(string key) => DesignerLocalization.T(key);
        protected static string T(string key, params object[] args) => DesignerLocalization.T(key, args);

        /// <summary>Localized GUIContent with a tooltip (label key + tooltip key).</summary>
        protected static GUIContent LC(string labelKey, string tooltipKey)
            => new GUIContent(DesignerLocalization.T(labelKey), DesignerLocalization.T(tooltipKey));

        /// <summary>Localized GUIContent (label only).</summary>
        protected static GUIContent LC(string labelKey)
            => new GUIContent(DesignerLocalization.T(labelKey));

        protected virtual void OnEnable()
        {
            UpdateTitle();
            DesignerLocalization.LanguageChanged += UpdateTitle;
        }

        protected virtual void OnDisable()
        {
            DesignerLocalization.LanguageChanged -= UpdateTitle;
        }

        private void UpdateTitle()
        {
            titleContent = new GUIContent(DesignerLocalization.T(TitleKey), DesignerLocalization.T(TooltipKey));
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(LC(TitleKey, TooltipKey), EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(DesignerLocalization.T(TooltipKey), MessageType.None);
            EditorGUILayout.Space(4);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawBody();
            EditorGUILayout.EndScrollView();
        }

        protected abstract void DrawBody();

        /// <summary>Draws a bold localized section header.</summary>
        protected static void Section(string titleKey)
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField(DesignerLocalization.T(titleKey), EditorStyles.boldLabel);
        }

        /// <summary>Marks a Unity object dirty after an edit so the change is saved.</summary>
        protected static void MarkDirty(Object target)
        {
            if (target != null)
                EditorUtility.SetDirty(target);
        }
    }
}
