using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Utilities
{
    /// <summary>
    /// A single, searchable home for the small NexUI editor tools. The tools remain
    /// independent windows so their existing workflows and serialized state are preserved.
    /// </summary>
    public sealed class NexUIUtilitiesWindow : EditorWindow
    {
        private const string StylePath = "Packages/com.emiteat.nexui.designer/Editor/Styles/NexUIDesigner.uss";

        private readonly List<ToolEntry> _tools = new()
        {
            Header("Workspace", "Project-wide setup and maintenance"),
            Window("Project Setup", "Create and verify the recommended project defaults.", "emiteat.NexUI.Editor.ProjectSetup.NexUIProjectSetupWindow, emiteat.NexUI.Editor.ProjectSetup", "setup defaults install"),
            Action("Project Settings", "Configure runtime, theme, input, and integration defaults.", () => SettingsService.OpenProjectSettings("Project/NexUI"), "settings preferences"),
            Window("Validator", "Scan screen definitions and project configuration.", "emiteat.NexUI.Editor.Validator.NexUIValidatorWindow, emiteat.NexUI.Editor.Validator", "validate errors warnings"),
            Window("ID Generator", "Generate stable UI identifiers and source files.", "emiteat.NexUI.Editor.IDGenerator.NexUIIDGeneratorWindow, emiteat.NexUI.Editor.IDGenerator", "id code generate"),
            Window("Debug Snapshot", "Inspect current runtime UI state and diagnostics.", "emiteat.NexUI.Editor.DebugTools.NexUIDebugWindow, emiteat.NexUI.Editor.DebugTools", "debug runtime snapshot"),
            Window("Migration", "Upgrade older NexUI assets to the current format.", "emiteat.NexUI.Editor.Migration.NexUIMigrationWindow, emiteat.NexUI.Editor.Migration", "migrate upgrade legacy"),

            Header("Design", "Reusable systems that shape screens and content"),
            Window("Design Tokens", "Edit shared color, type, spacing, and motion tokens.", "emiteat.NexUI.Designer.Editor.Tokens.DesignerTokenWindow, emiteat.NexUI.Designer.Editor", "theme style tokens"),
            Window("UI Recipes", "Create common UI structures from compact recipes.", "emiteat.NexUI.Designer.Editor.Recipes.RecipesWindow, emiteat.NexUI.Designer.Editor", "template create"),
            Window("Screen Variants", "Manage state and platform variants for screens.", "emiteat.NexUI.Designer.Editor.Variants.VariantEditorWindow, emiteat.NexUI.Designer.Editor", "variant platform state"),
            Window("Responsive Rules", "Define layout behavior across resolutions.", "emiteat.NexUI.Designer.Editor.Responsive.ResponsiveEditorWindow, emiteat.NexUI.Designer.Editor", "responsive resolution layout"),
            Window("Prompt Glyphs", "Map device prompts and controller glyphs.", "emiteat.NexUI.Designer.Editor.PromptGlyph.PromptGlyphWindow, emiteat.NexUI.Designer.Editor", "input glyph gamepad"),
            Window("Game Localization", "Review game-facing localized UI content.", "emiteat.NexUI.Designer.Editor.GameLocalization.GameLocalizationWindow, emiteat.NexUI.Designer.Editor", "language locale text"),
            Window("UI Contract", "Define and generate typed screen contracts.", "emiteat.NexUI.Designer.Editor.Contracts.ContractEditorWindow, emiteat.NexUI.Designer.Editor", "contract code binding"),
            Window("Generate UXML + USS", "Publish the current design as UI Toolkit assets.", "emiteat.NexUI.Designer.Editor.Backend.UIToolkitGenerationWindow, emiteat.NexUI.Designer.Editor", "uitoolkit export backend"),

            Header("Motion & Flow", "Animation, navigation, and scenario authoring"),
            Window("Motion Clip Editor", "Author reusable motion clips on a timeline.", "emiteat.NexUI.Designer.Editor.MotionClipEditor.MotionClipEditorWindow, emiteat.NexUI.Designer.Editor", "animation timeline clip"),
            Window("Motion Graph", "Build relationships between motion clips.", "emiteat.NexUI.Designer.Editor.GraphV2.MotionGraphV2Window, emiteat.NexUI.Designer.Editor", "animation graph"),
            Window("Motion State Machine", "Connect motion states and transitions.", "emiteat.NexUI.Designer.Editor.StateMachine.MotionStateMachineWindow, emiteat.NexUI.Designer.Editor", "animation state transition"),
            Window("Motion Budget", "Audit animation cost and motion policy.", "emiteat.NexUI.Designer.Editor.MotionBudget.MotionBudgetWindow, emiteat.NexUI.Designer.Editor", "animation performance"),
            Window("Screen Flow", "Edit navigation between UI screens.", "emiteat.NexUI.Designer.Editor.ScreenFlow.ScreenFlowWindow, emiteat.NexUI.Designer.Editor", "navigation graph screen"),
            Window("Scenario Editor", "Author repeatable preview and test scenarios.", "emiteat.NexUI.Designer.Editor.Scenario.ScenarioEditorWindow, emiteat.NexUI.Designer.Editor", "scenario timeline test"),
            Window("Input Mode Preview", "Preview keyboard, gamepad, touch, and handheld modes.", "emiteat.NexUI.Designer.Editor.InputPreview.InputPreviewWindow, emiteat.NexUI.Designer.Editor", "input device preview"),
            Window("Loading Strategy", "Review loading behavior for screens and assets.", "emiteat.NexUI.Designer.Editor.LoadingStrategy.LoadingStrategyWindow, emiteat.NexUI.Designer.Editor", "load addressables performance"),
            Window("Sync & Publish", "Synchronize metadata and publish designer output.", "emiteat.NexUI.Designer.Editor.Sync.SyncPublishWindow, emiteat.NexUI.Designer.Editor", "sync publish export"),

            Header("Quality", "Focused checks and diagnostics"),
            Window("Accessibility Preview", "Audit focus, labels, scale, and accessibility metadata.", "emiteat.NexUI.Designer.Editor.Accessibility.AccessibilityWindow, emiteat.NexUI.Designer.Editor", "accessibility a11y focus"),
            Window("Contrast Checker", "Check token and color combinations against WCAG ratios.", "emiteat.NexUI.Designer.Editor.Contrast.ContrastWindow, emiteat.NexUI.Designer.Editor", "contrast wcag color"),
            Window("Font Glyph Checker", "Find missing glyphs before localized content ships.", "emiteat.NexUI.Designer.Editor.FontCheck.FontCheckWindow, emiteat.NexUI.Designer.Editor", "font glyph locale"),
            Window("UI Profiler", "Inspect element count and designer-side UI cost.", "emiteat.NexUI.Designer.Editor.Profiler.ProfilerWindow, emiteat.NexUI.Designer.Editor", "profile performance"),
            Window("Binding Profiler", "Inspect binding volume, state, and update pressure.", "emiteat.NexUI.Designer.Editor.BindingProfiler.BindingProfilerWindow, emiteat.NexUI.Designer.Editor", "binding profile performance"),
            Window("Flow Simulator", "Exercise navigation paths without entering Play mode.", "emiteat.NexUI.Designer.Editor.FlowSimulator.FlowSimulatorWindow, emiteat.NexUI.Designer.Editor", "navigation simulate test"),
            Window("Metadata Diff", "Compare designer metadata assets side by side.", "emiteat.NexUI.Designer.Editor.Diff.DiffEditorWindow, emiteat.NexUI.Designer.Editor", "compare diff metadata"),
            Window("Snapshot", "Capture and compare designer metadata snapshots.", "emiteat.NexUI.Designer.Editor.Snapshot.SnapshotEditorWindow, emiteat.NexUI.Designer.Editor", "snapshot compare"),
            Window("Rename Refactor", "Rename element IDs while preserving references.", "emiteat.NexUI.Designer.Editor.Refactor.RenameEditorWindow, emiteat.NexUI.Designer.Editor", "rename refactor id"),
            Window("Reference Cleaner", "Find and remove dead metadata references.", "emiteat.NexUI.Designer.Editor.Cleaner.CleanerEditorWindow, emiteat.NexUI.Designer.Editor", "clean dead reference"),
        };

        private TextField _search;
        private ScrollView _content;

        [MenuItem("Tools/NexUI/Utilities", priority = 1)]
        public static void Open()
        {
            var window = GetWindow<NexUIUtilitiesWindow>();
            window.titleContent = new GUIContent("NexUI Utilities");
            window.minSize = new Vector2(520, 420);
            window.Show();
        }

        public void CreateGUI()
        {
            titleContent = new GUIContent("NexUI Utilities");
            rootVisualElement.Clear();
            rootVisualElement.AddToClassList("nexui-designer-root");
            rootVisualElement.AddToClassList("nexui-utilities-root");

            var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
            if (styles != null) rootVisualElement.styleSheets.Add(styles);

            var header = new VisualElement();
            header.AddToClassList("nexui-utilities-header");
            var title = new Label("Utilities");
            title.AddToClassList("nexui-utilities-title");
            header.Add(title);
            var subtitle = new Label("One place for project setup, design helpers, motion, and quality tools.");
            subtitle.AddToClassList("nexui-utilities-subtitle");
            header.Add(subtitle);
            rootVisualElement.Add(header);

            _search = new TextField { tooltip = "Filter utilities by name or purpose." };
            _search.AddToClassList("nexui-utilities-search");
            _search.RegisterValueChangedCallback(_ => RebuildTools());
            rootVisualElement.Add(_search);

            _content = new ScrollView();
            _content.AddToClassList("nexui-utilities-content");
            rootVisualElement.Add(_content);
            RebuildTools();

            rootVisualElement.schedule.Execute(() => _search.Focus());
        }

        private void RebuildTools()
        {
            _content.Clear();
            var query = (_search.value ?? string.Empty).Trim();
            VisualElement grid = null;
            var visible = 0;

            foreach (var tool in _tools)
            {
                if (tool.IsHeader)
                {
                    if (!CategoryHasMatch(tool.Name, query)) continue;
                    var section = new VisualElement();
                    section.AddToClassList("nexui-utilities-section");
                    var heading = new Label(tool.Name);
                    heading.AddToClassList("nexui-utilities-section-title");
                    section.Add(heading);
                    var detail = new Label(tool.Description);
                    detail.AddToClassList("nexui-utilities-section-subtitle");
                    section.Add(detail);
                    _content.Add(section);

                    grid = new VisualElement();
                    grid.AddToClassList("nexui-utilities-grid");
                    _content.Add(grid);
                    continue;
                }

                if (!tool.Matches(query)) continue;
                if (grid == null)
                {
                    grid = new VisualElement();
                    grid.AddToClassList("nexui-utilities-grid");
                    _content.Add(grid);
                }

                var card = new Button(tool.Open);
                card.AddToClassList("nexui-utility-card");
                var label = new Label(tool.Name);
                label.AddToClassList("nexui-utility-card-title");
                card.Add(label);
                var description = new Label(tool.Description);
                description.AddToClassList("nexui-utility-card-description");
                card.Add(description);
                grid.Add(card);
                visible++;
            }

            if (visible != 0) return;
            var empty = new Label("No utilities match this search.");
            empty.AddToClassList("nexui-utilities-empty");
            _content.Add(empty);
        }

        private bool CategoryHasMatch(string category, string query)
        {
            if (string.IsNullOrEmpty(query)) return true;
            var inCategory = false;
            foreach (var entry in _tools)
            {
                if (entry.IsHeader)
                {
                    if (inCategory) break;
                    inCategory = entry.Name == category;
                    continue;
                }
                if (inCategory && entry.Matches(query)) return true;
            }
            return false;
        }

        private static ToolEntry Header(string name, string description) => new(name, description, null, null, true);
        private static ToolEntry Action(string name, string description, Action open, string keywords) => new(name, description, open, keywords, false);
        private static ToolEntry Window(string name, string description, string typeName, string keywords)
            => Action(name, description, () => OpenWindow(typeName, name), keywords);

        private static void OpenWindow(string typeName, string title)
        {
            var type = Type.GetType(typeName);
            if (type == null || !typeof(EditorWindow).IsAssignableFrom(type))
            {
                EditorUtility.DisplayDialog("NexUI Utilities", $"{title} is not available in the installed package set.", "OK");
                return;
            }

            var window = GetWindow(type);
            window.Show();
            window.Focus();
        }

        private sealed class ToolEntry
        {
            public readonly string Name;
            public readonly string Description;
            public readonly Action Open;
            public readonly string Keywords;
            public readonly bool IsHeader;

            public ToolEntry(string name, string description, Action open, string keywords, bool isHeader)
            {
                Name = name;
                Description = description;
                Open = open;
                Keywords = keywords ?? string.Empty;
                IsHeader = isHeader;
            }

            public bool Matches(string query)
            {
                if (string.IsNullOrEmpty(query)) return true;
                return Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                    || Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                    || Keywords.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
    }
}
