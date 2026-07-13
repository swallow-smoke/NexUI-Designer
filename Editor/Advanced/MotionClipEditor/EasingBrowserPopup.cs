using System;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Designer.Editor.Localization;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Categorized easing picker (brief §5.7): every <see cref="UIMotionEasing"/>, grouped, each with
    /// a live curve thumbnail and a tooltip describing when to use it, plus a "Custom Curve" entry
    /// that switches the keyframe to its own <see cref="AnimationCurve"/> override instead. Built on
    /// <see cref="PopupWindowContent"/>, which is IMGUI-only, so thumbnails are drawn with
    /// <see cref="Handles"/> rather than the UI-Toolkit <c>Painter2D</c> preview used inline in the
    /// timeline row (<see cref="EasingCurvePreview"/>) - same underlying <see cref="UIMotionClipEvaluator.Ease"/>
    /// samples either way, just two renderers for two different GUI systems.
    /// </summary>
    public sealed class EasingBrowserPopup : PopupWindowContent
    {
        private readonly Action<UIMotionEasing> _onSelectEasing;
        private readonly Action _onSelectCustomCurve;
        private Vector2 _scroll;

        public EasingBrowserPopup(Action<UIMotionEasing> onSelectEasing, Action onSelectCustomCurve)
        {
            _onSelectEasing = onSelectEasing;
            _onSelectCustomCurve = onSelectCustomCurve;
        }

        public override Vector2 GetWindowSize() => new Vector2(260f, 380f);

        public override void OnGUI(Rect rect)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);

            foreach (EasingCategory category in Enum.GetValues(typeof(EasingCategory)))
            {
                GUILayout.Label(DesignerLocalization.T(EasingCatalog.CategoryLocalizationKey(category)), EditorStyles.boldLabel);
                foreach (var easing in EasingCatalog.All())
                {
                    if (EasingCatalog.CategoryOf(easing) != category) continue;
                    DrawEasingRow(easing, DesignerLocalization.T(EasingCatalog.CategoryHintKey(category)));
                }
                GUILayout.Space(4f);
            }

            GUILayout.Space(4f);
            GUILayout.Label(DesignerLocalization.T("easing.category.Custom"), EditorStyles.boldLabel);
            if (GUILayout.Button(new GUIContent(DesignerLocalization.T("easing.custom"), DesignerLocalization.T("easing.hint.Custom"))))
            {
                _onSelectCustomCurve?.Invoke();
                editorWindow.Close();
            }

            GUILayout.EndScrollView();
        }

        private void DrawEasingRow(UIMotionEasing easing, string hint)
        {
            GUILayout.BeginHorizontal();

            var curveRect = GUILayoutUtility.GetRect(36f, 20f, GUILayout.Width(36f), GUILayout.Height(20f));
            DrawCurveThumbnail(curveRect, easing);

            if (GUILayout.Button(new GUIContent(ObjectNames.NicifyVariableName(easing.ToString()), hint), GUILayout.ExpandWidth(true)))
            {
                _onSelectEasing?.Invoke(easing);
                editorWindow.Close();
            }

            GUILayout.EndHorizontal();
        }

        private static void DrawCurveThumbnail(Rect rect, UIMotionEasing easing)
        {
            const int samples = 16;
            Handles.BeginGUI();
            Handles.color = new Color(0.66f, 0.63f, 1f, 0.9f);
            Vector3? previous = null;
            for (var i = 0; i <= samples; i++)
            {
                var t = i / (float)samples;
                var eased = Mathf.Clamp01(UIMotionClipEvaluator.Ease(easing, t));
                var point = new Vector3(rect.x + t * rect.width, rect.y + rect.height - eased * rect.height, 0f);
                if (previous.HasValue) Handles.DrawLine(previous.Value, point);
                previous = point;
            }
            Handles.EndGUI();
        }
    }
}
