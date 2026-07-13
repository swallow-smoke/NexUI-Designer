using System;
using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Tokens
{
    /// <summary>
    /// Design-token set manager (brief §33/§37). Create/edit tokens, alias one token to another, and
    /// see each token's live resolved value (colors as swatches, numbers as values). Validation lists
    /// broken references, cycles, duplicates and category mismatches. Standalone tool window.
    /// </summary>
    public sealed class DesignerTokenWindow : NexUIToolWindow
    {
        private const string LiteralChoice = "(Literal)";

        [SerializeField] private DesignerTokenSetAsset _asset;

        protected override string TitleKey => "tokens.window.title";
        protected override string TooltipKey => "tokens.window.description";

        public static void Open()
        {
            var window = GetWindow<DesignerTokenWindow>();
            window.minSize = new Vector2(500f, 460f);
            window.Show();
        }

        protected override void DrawBody()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var picked = (DesignerTokenSetAsset)EditorGUILayout.ObjectField(
                    LC("tokens.field.asset", "tooltip.tokens.asset"), _asset, typeof(DesignerTokenSetAsset), false);
                if (check.changed) _asset = picked;
            }

            if (GUILayout.Button(LC("tokens.button.create", "tooltip.tokens.create")))
                _asset = CreateAsset();

            if (_asset == null)
            {
                EditorGUILayout.HelpBox(T("tokens.help.noAsset"), MessageType.Info);
                return;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var name = EditorGUILayout.TextField(LC("tokens.field.setName", "tooltip.tokens.setName"), _asset.setName);
                if (check.changed) { Undo.RecordObject(_asset, "Edit Token Set"); _asset.setName = name; MarkDirty(_asset); }
            }

            Section("tokens.section.tokens");
            DrawTokens();

            if (GUILayout.Button(LC("tokens.button.add", "tooltip.tokens.add")))
            {
                Undo.RecordObject(_asset, "Add Token");
                _asset.tokens.Add(new DesignerToken("token" + (_asset.tokens.Count + 1), DesignerTokenCategory.Color));
                MarkDirty(_asset);
            }

            Section("tokens.section.validation");
            DrawValidation();
        }

        private void DrawTokens()
        {
            var names = _asset.tokens.Where(t => t != null && !string.IsNullOrEmpty(t.name)).Select(t => t.name).ToList();
            int removeIndex = -1;

            for (int i = 0; i < _asset.tokens.Count; i++)
            {
                var token = _asset.tokens[i];
                if (token == null) continue;

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var name = EditorGUILayout.TextField(token.name, GUILayout.MinWidth(110f));
                        var category = (DesignerTokenCategory)EditorGUILayout.EnumPopup(token.category, GUILayout.Width(110f));
                        if (check.changed)
                        {
                            Undo.RecordObject(_asset, "Edit Token");
                            token.name = name;
                            token.category = category;
                            MarkDirty(_asset);
                        }
                        if (GUILayout.Button("✕", GUILayout.Width(24f))) removeIndex = i;
                    }

                    DrawTokenValue(token, names);
                }
            }

            if (removeIndex >= 0)
            {
                Undo.RecordObject(_asset, "Remove Token");
                _asset.tokens.RemoveAt(removeIndex);
                MarkDirty(_asset);
            }
        }

        private void DrawTokenValue(DesignerToken token, List<string> allNames)
        {
            // Reference selector: (Literal) + every other token name.
            var choices = new List<string> { LiteralChoice };
            choices.AddRange(allNames.Where(n => n != token.name));
            int index = token.IsReference ? Mathf.Max(0, choices.IndexOf(token.reference)) : 0;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                int newIndex = EditorGUILayout.Popup(T("tokens.field.reference"), index, choices.ToArray());
                if (check.changed)
                {
                    Undo.RecordObject(_asset, "Edit Token");
                    token.reference = newIndex == 0 ? string.Empty : choices[newIndex];
                    MarkDirty(_asset);
                }
            }

            if (token.IsReference)
            {
                DrawResolvedPreview(token);
                return;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (token.category == DesignerTokenCategory.Color)
                {
                    var color = EditorGUILayout.ColorField(T("tokens.field.value"), token.colorValue);
                    if (check.changed) { Undo.RecordObject(_asset, "Edit Token"); token.colorValue = color; MarkDirty(_asset); }
                }
                else if (token.IsNumeric)
                {
                    var value = EditorGUILayout.FloatField(T("tokens.field.value"), token.numberValue);
                    if (check.changed) { Undo.RecordObject(_asset, "Edit Token"); token.numberValue = value; MarkDirty(_asset); }
                }
                else
                {
                    var text = EditorGUILayout.TextField(T("tokens.field.value"), token.stringValue);
                    if (check.changed) { Undo.RecordObject(_asset, "Edit Token"); token.stringValue = text; MarkDirty(_asset); }
                }
            }
        }

        private void DrawResolvedPreview(DesignerToken token)
        {
            var resolved = DesignerTokenResolver.Resolve(_asset.tokens, token.name);
            if (resolved == null)
            {
                EditorGUILayout.LabelField(T("tokens.field.resolved"), T("tokens.resolved.unresolved"));
                return;
            }
            using (new EditorGUI.DisabledScope(true))
            {
                if (resolved.category == DesignerTokenCategory.Color)
                    EditorGUILayout.ColorField(T("tokens.field.resolved"), resolved.colorValue);
                else if (resolved.IsNumeric)
                    EditorGUILayout.FloatField(T("tokens.field.resolved"), resolved.numberValue);
                else
                    EditorGUILayout.TextField(T("tokens.field.resolved"), resolved.stringValue);
            }
        }

        private void DrawValidation()
        {
            var issues = DesignerTokenResolver.Validate(_asset);
            if (issues.Count == 0)
            {
                Badge(T("tokens.validation.passed"), BadgeKind.Ok);
                return;
            }
            foreach (var issue in issues)
            {
                var type = issue.Level == DesignerTokenIssue.Severity.Error ? MessageType.Error
                    : issue.Level == DesignerTokenIssue.Severity.Warning ? MessageType.Warning : MessageType.Info;
                EditorGUILayout.HelpBox(issue.Message, type);
            }
        }

        private static DesignerTokenSetAsset CreateAsset()
        {
            var asset = ScriptableObject.CreateInstance<DesignerTokenSetAsset>();
            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewTokenSet.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(asset);
            return asset;
        }
    }
}
