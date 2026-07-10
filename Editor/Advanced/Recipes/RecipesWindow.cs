using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.Recipes
{
    /// <summary>Full UI for the UI recipe / template system (§22).</summary>
    public sealed class RecipesWindow : NexUIToolWindow
    {
        [SerializeField] private DesignerMetadataAsset _asset;
        [SerializeField] private int _recipeIndex;
        [SerializeField] private string _prefix = "";
        private int _lastCreated = -1;

        protected override string TitleKey => "panel.recipes";
        protected override string TooltipKey => "tooltip.recipes";

        [MenuItem("Tools/NexUI/Designer/Advanced/UI Recipes")]
        public static void Open() => GetWindow<RecipesWindow>();

        protected override void DrawBody()
        {
            _asset = (DesignerMetadataAsset)EditorGUILayout.ObjectField(
                LC("panel.hierarchy"), _asset, typeof(DesignerMetadataAsset), false);
            if (_asset == null)
            {
                EditorGUILayout.HelpBox(T("message.noScreenSelected"), MessageType.Info);
                return;
            }

            Section("panel.recipes");
            _recipeIndex = EditorGUILayout.Popup("recipe", _recipeIndex, RecipeService.RecipeNames);
            _prefix = EditorGUILayout.TextField("id prefix", _prefix);

            if (GUILayout.Button(LC("button.create", "tooltip.recipes"), GUILayout.Height(24)))
            {
                Undo.RecordObject(_asset, "Generate Recipe");
                _lastCreated = RecipeService.Generate(_asset,
                    RecipeService.RecipeNames[Mathf.Clamp(_recipeIndex, 0, RecipeService.RecipeNames.Length - 1)], _prefix);
                MarkDirty(_asset);
            }

            if (_lastCreated >= 0)
                EditorGUILayout.HelpBox($"generated {_lastCreated} element(s) + contract requirements.", MessageType.Info);
        }
    }
}
