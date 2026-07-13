using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.Designer.Editor.Styles;

namespace emiteat.NexUI.Designer.Editor.Common
{
    /// <summary>
    /// Base class for the standalone NexUI Designer advanced tool windows. Provides a
    /// localized header + description styled to match the main designer's dark UI Toolkit
    /// theme (see <see cref="DesignerColors"/>), a scroll body, and Korean-aware tooltip
    /// helpers. Subclasses implement <see cref="DrawBody"/> with IMGUI.
    /// </summary>
    public abstract class NexUIToolWindow : EditorWindow
    {
        /// <summary>Status kind for <see cref="Badge"/>, mirroring the UI Toolkit designer's
        /// <c>.nexui-toolbar-status.is-*</c> classes.</summary>
        protected enum BadgeKind { Ok, Warning, Muted }

        private Vector2 _scroll;
        private GUIStyle _headerBoxStyle;
        private GUIStyle _headerTitleStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _badgeStyleOk;
        private GUIStyle _badgeStyleWarning;
        private GUIStyle _badgeStyleMuted;

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
            minSize = new Vector2(380f, 280f);
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

        private void EnsureStyles()
        {
            if (_headerBoxStyle != null) return;

            var headerTex = MakeTex(DesignerColors.Toolbar);
            _headerBoxStyle = new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(8, 8, 6, 6) };
            _headerBoxStyle.normal.background = headerTex;

            _headerTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                normal = { textColor = DesignerColors.TextHeading }
            };

            _bodyStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(12, 12, 10, 12),
                margin = new RectOffset(6, 6, 2, 8)
            };
            _bodyStyle.normal.background = MakeTex(DesignerColors.Panel);

            _sectionStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = DesignerColors.Accent }
            };

            _badgeStyleOk = MakeBadgeStyle(DesignerColors.StatusOkBackground, DesignerColors.StatusOkText);
            _badgeStyleWarning = MakeBadgeStyle(DesignerColors.StatusWarningBackground, DesignerColors.StatusWarningText);
            _badgeStyleMuted = MakeBadgeStyle(DesignerColors.StatusMutedBackground, DesignerColors.StatusMutedText);
        }

        private static GUIStyle MakeBadgeStyle(Color background, Color foreground)
        {
            return new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(8, 8, 2, 2),
                fontSize = 10,
                normal = { textColor = foreground, background = MakeTex(background) }
            };
        }

        private static Texture2D MakeTex(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            tex.hideFlags = HideFlags.HideAndDontSave;
            return tex;
        }

        private void OnGUI()
        {
            EnsureStyles();
            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(new Rect(0f, 0f, position.width, position.height), DesignerColors.Background);

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginVertical(_headerBoxStyle);
            EditorGUILayout.LabelField(new GUIContent(DesignerLocalization.T(TitleKey)), _headerTitleStyle);
            EditorGUILayout.LabelField(DesignerLocalization.T(TooltipKey), EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.BeginVertical(_bodyStyle);
            DrawBody();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        protected abstract void DrawBody();

        /// <summary>Draws an accent-colored bold section header, matching the UI Toolkit
        /// designer's <c>#SectionTitle</c> look.</summary>
        protected void Section(string titleKey)
        {
            EnsureStyles();
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField(DesignerLocalization.T(titleKey), _sectionStyle);
        }

        /// <summary>Draws a small colored status badge, mirroring
        /// <c>.nexui-toolbar-status.is-ok/is-warning/is-muted</c>.</summary>
        protected void Badge(string text, BadgeKind kind)
        {
            EnsureStyles();
            var style = kind == BadgeKind.Ok ? _badgeStyleOk : kind == BadgeKind.Warning ? _badgeStyleWarning : _badgeStyleMuted;
            EditorGUILayout.LabelField(text, style, GUILayout.ExpandWidth(false));
        }

        /// <summary>Marks a Unity object dirty after an edit so the change is saved.</summary>
        protected static void MarkDirty(Object target)
        {
            if (target != null)
                EditorUtility.SetDirty(target);
        }
    }
}
