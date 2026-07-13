using System;
using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Designer.Editor.Common;
using emiteat.NexUI.Designer.Editor.Components;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Scenario
{
    /// <summary>
    /// Standalone authoring window for <see cref="DesignerScenarioAsset"/> (brief §28, §35). Edits a
    /// scenario's mock binding values, forced state and language, then applies them to — or captures
    /// them from — the screen currently open in the main Designer. Follows the same standalone-window
    /// convention as the Motion Clip / Motion Graph editors.
    /// </summary>
    public sealed class ScenarioEditorWindow : NexUIToolWindow
    {
        private const string NoneChoice = "(None)";

        [SerializeField] private DesignerScenarioAsset _scenario;
        private ScenarioApplyReport _lastReport;
        private bool _hasReport;

        // Timeline playback / scrub state. Preview values are mutated in place during scrub, so the
        // pre-scrub values are snapshotted and restored on Stop / disable (never persisted - §43).
        private bool _timelinePlaying;
        private float _timelineTime;
        private double _lastEditorTime;
        private struct ElementPreviewSnapshot { public float PreviewValue; public string Text; public bool Hidden; }
        private readonly Dictionary<string, ElementPreviewSnapshot> _timelineSnapshot = new Dictionary<string, ElementPreviewSnapshot>();
        private bool _timelineSnapshotTaken;

        private readonly ScenarioRecorder _recorder = new ScenarioRecorder();

        protected override string TitleKey => "scenario.window.title";
        protected override string TooltipKey => "scenario.window.description";

        [MenuItem("Tools/NexUI/Designer/Advanced/Scenario Editor")]
        public static void Open()
        {
            var window = GetWindow<ScenarioEditorWindow>();
            window.minSize = new Vector2(460f, 420f);
            window.Show();
        }

        private static NexUIDesignerContext ResolveContext()
            => Resources.FindObjectsOfTypeAll<NexUIDesignerWindow>().FirstOrDefault()?.Context;

        protected override void DrawBody()
        {
            var context = ResolveContext();
            DrawConnectionStatus(context);

            EditorGUILayout.Space(4);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var picked = (DesignerScenarioAsset)EditorGUILayout.ObjectField(
                    LC("scenario.field.asset", "tooltip.scenario.asset"), _scenario, typeof(DesignerScenarioAsset), false);
                if (check.changed) { _scenario = picked; _hasReport = false; }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(LC("scenario.button.create", "tooltip.scenario.create")))
                    _scenario = ScenarioService.CreateAsset();
            }

            if (_scenario == null)
            {
                EditorGUILayout.HelpBox(T("scenario.help.noAsset"), MessageType.Info);
                return;
            }

            DrawIdentity();
            DrawBindings();
            DrawStateAndLanguage();
            DrawTimeline(context);
            DrawRecording(context);
            DrawActions(context);
            DrawReport();
            DrawValidation(context);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopTimeline(ResolveContext());
            if (_recorder.IsRecording) { _recorder.Stop(); EditorApplication.update -= Repaint; }
        }

        private void DrawRecording(NexUIDesignerContext context)
        {
            Section("scenario.section.recording");

            if (_recorder.IsRecording)
            {
                Badge($"{T("scenario.record.recording")} · {_recorder.ElapsedTime:0.0}s · {_recorder.RecordedKeyCount} keys", BadgeKind.Warning);
                if (GUILayout.Button(LC("scenario.record.stop", "tooltip.scenario.record.stop")))
                {
                    _recorder.Stop();
                    EditorApplication.update -= Repaint;
                }
                return;
            }

            using (new EditorGUI.DisabledScope(context?.Metadata == null))
            {
                if (GUILayout.Button(LC("scenario.record.start", "tooltip.scenario.record.start")))
                {
                    _recorder.Start(_scenario, context);
                    if (_recorder.IsRecording) EditorApplication.update += Repaint;
                }
            }
            EditorGUILayout.LabelField(T("scenario.record.help"), EditorStyles.wordWrappedMiniLabel);
        }

        private void DrawConnectionStatus(NexUIDesignerContext context)
        {
            var screen = context?.Metadata != null ? context.Metadata.screenId : null;
            if (!string.IsNullOrEmpty(screen))
                Badge($"{T("scenario.status.target")}: {screen}", BadgeKind.Ok);
            else
            {
                Badge(T("scenario.status.disconnected"), BadgeKind.Warning);
                if (GUILayout.Button(T("scenario.status.openDesigner"), GUILayout.ExpandWidth(false)))
                    EditorApplication.ExecuteMenuItem("Tools/NexUI/Designer");
            }
        }

        private void DrawIdentity()
        {
            Section("scenario.section.identity");
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var name = EditorGUILayout.TextField(LC("scenario.field.name", "tooltip.scenario.name"), _scenario.scenarioName);
                var desc = EditorGUILayout.TextField(LC("scenario.field.description", "tooltip.scenario.description"), _scenario.description);
                if (check.changed)
                {
                    Undo.RecordObject(_scenario, "Edit Scenario");
                    _scenario.scenarioName = name;
                    _scenario.description = desc;
                    MarkDirty(_scenario);
                }
            }
        }

        private void DrawBindings()
        {
            Section("scenario.section.bindings");

            int removeIndex = -1;
            for (int i = 0; i < _scenario.bindings.Count; i++)
            {
                var binding = _scenario.bindings[i];
                if (binding == null) continue;

                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var key = EditorGUILayout.TextField(binding.key, GUILayout.MinWidth(120f));
                        var kind = (DesignerScenarioValueKind)EditorGUILayout.EnumPopup(binding.kind, GUILayout.Width(80f));
                        bool boolValue = binding.boolValue;
                        float numberValue = binding.numberValue;
                        string textValue = binding.textValue;
                        switch (kind)
                        {
                            case DesignerScenarioValueKind.Bool:
                                boolValue = EditorGUILayout.Toggle(binding.boolValue, GUILayout.Width(40f));
                                break;
                            case DesignerScenarioValueKind.Number:
                                numberValue = EditorGUILayout.FloatField(binding.numberValue, GUILayout.Width(90f));
                                break;
                            case DesignerScenarioValueKind.Text:
                                textValue = EditorGUILayout.TextField(binding.textValue, GUILayout.MinWidth(90f));
                                break;
                        }
                        if (check.changed)
                        {
                            Undo.RecordObject(_scenario, "Edit Scenario Binding");
                            binding.key = key;
                            binding.kind = kind;
                            binding.boolValue = boolValue;
                            binding.numberValue = numberValue;
                            binding.textValue = textValue;
                            MarkDirty(_scenario);
                        }
                    }
                    if (GUILayout.Button("✕", GUILayout.Width(24f))) removeIndex = i;
                }
            }

            if (removeIndex >= 0)
            {
                Undo.RecordObject(_scenario, "Remove Scenario Binding");
                _scenario.bindings.RemoveAt(removeIndex);
                MarkDirty(_scenario);
            }

            if (GUILayout.Button(LC("scenario.button.addBinding", "tooltip.scenario.addBinding")))
            {
                Undo.RecordObject(_scenario, "Add Scenario Binding");
                _scenario.bindings.Add(new DesignerScenarioBinding(string.Empty, DesignerScenarioValueKind.Number));
                MarkDirty(_scenario);
            }
        }

        private void DrawStateAndLanguage()
        {
            Section("scenario.section.context");

            var stateChoices = new List<string> { NoneChoice };
            stateChoices.AddRange(Enum.GetNames(typeof(DesignerComponentState)).Where(n => n != "None"));
            DrawPopup(LC("scenario.field.forcedState", "tooltip.scenario.forcedState"), stateChoices, _scenario.forcedState,
                value => { _scenario.forcedState = value; });

            var langChoices = new List<string> { NoneChoice };
            langChoices.AddRange(Enum.GetNames(typeof(DesignerLanguage)));
            DrawPopup(LC("scenario.field.language", "tooltip.scenario.language"), langChoices, _scenario.language,
                value => { _scenario.language = value; });
        }

        private void DrawPopup(GUIContent label, List<string> choices, string current, Action<string> apply)
        {
            int index = string.IsNullOrEmpty(current) ? 0 : Mathf.Max(0, choices.IndexOf(current));
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                int newIndex = EditorGUILayout.Popup(label, index, choices.ToArray());
                if (check.changed)
                {
                    Undo.RecordObject(_scenario, "Edit Scenario");
                    apply(newIndex == 0 ? string.Empty : choices[newIndex]);
                    MarkDirty(_scenario);
                }
            }
        }

        private void DrawTimeline(NexUIDesignerContext context)
        {
            Section("scenario.section.timeline");

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var use = EditorGUILayout.Toggle(LC("scenario.timeline.use", "tooltip.scenario.timeline.use"), _scenario.useTimeline);
                if (check.changed)
                {
                    Undo.RecordObject(_scenario, "Edit Scenario Timeline");
                    _scenario.useTimeline = use;
                    if (!use) StopTimeline(context);
                    MarkDirty(_scenario);
                }
            }

            if (!_scenario.useTimeline) return;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var duration = EditorGUILayout.FloatField(LC("scenario.timeline.duration", "tooltip.scenario.timeline.duration"), _scenario.timelineDuration);
                var loop = EditorGUILayout.Toggle(LC("scenario.timeline.loop", "tooltip.scenario.timeline.loop"), _scenario.timelineLoop);
                if (check.changed)
                {
                    Undo.RecordObject(_scenario, "Edit Scenario Timeline");
                    _scenario.timelineDuration = Mathf.Max(0.01f, duration);
                    _scenario.timelineLoop = loop;
                    MarkDirty(_scenario);
                }
            }

            DrawTimelineKeys();

            // Scrubber + transport.
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                var time = EditorGUILayout.Slider(LC("scenario.timeline.time", "tooltip.scenario.timeline.scrub"),
                    _timelineTime, 0f, _scenario.timelineDuration);
                if (check.changed) ScrubTo(context, time);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(context?.Metadata == null))
                {
                    if (GUILayout.Button(_timelinePlaying ? T("scenario.timeline.pause") : T("scenario.timeline.play")))
                        ToggleTimelinePlayback(context);
                    if (GUILayout.Button(T("scenario.timeline.stop")))
                        StopTimeline(context);
                }
                GUILayout.Label($"{_timelineTime:0.00}s / {_scenario.timelineDuration:0.00}s", EditorStyles.miniLabel);
            }
        }

        private void DrawTimelineKeys()
        {
            int removeIndex = -1;
            for (int i = 0; i < _scenario.timelineKeys.Count; i++)
            {
                var key = _scenario.timelineKeys[i];
                if (key == null) continue;

                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        var time = EditorGUILayout.FloatField(key.time, GUILayout.Width(50f));
                        var bindingKey = EditorGUILayout.TextField(key.key, GUILayout.MinWidth(100f));
                        var kind = (DesignerScenarioValueKind)EditorGUILayout.EnumPopup(key.kind, GUILayout.Width(72f));
                        bool boolValue = key.boolValue;
                        float numberValue = key.numberValue;
                        string textValue = key.textValue;
                        switch (kind)
                        {
                            case DesignerScenarioValueKind.Bool: boolValue = EditorGUILayout.Toggle(key.boolValue, GUILayout.Width(36f)); break;
                            case DesignerScenarioValueKind.Number: numberValue = EditorGUILayout.FloatField(key.numberValue, GUILayout.Width(80f)); break;
                            case DesignerScenarioValueKind.Text: textValue = EditorGUILayout.TextField(key.textValue, GUILayout.MinWidth(80f)); break;
                        }
                        if (check.changed)
                        {
                            Undo.RecordObject(_scenario, "Edit Scenario Timeline Key");
                            key.time = Mathf.Max(0f, time);
                            key.key = bindingKey;
                            key.kind = kind;
                            key.boolValue = boolValue;
                            key.numberValue = numberValue;
                            key.textValue = textValue;
                            MarkDirty(_scenario);
                        }
                    }
                    if (GUILayout.Button("✕", GUILayout.Width(24f))) removeIndex = i;
                }
            }

            if (removeIndex >= 0)
            {
                Undo.RecordObject(_scenario, "Remove Scenario Timeline Key");
                _scenario.timelineKeys.RemoveAt(removeIndex);
                MarkDirty(_scenario);
            }

            if (GUILayout.Button(LC("scenario.timeline.addKey", "tooltip.scenario.timeline.addKey")))
            {
                Undo.RecordObject(_scenario, "Add Scenario Timeline Key");
                _scenario.timelineKeys.Add(new DesignerScenarioTimelineKey(_timelineTime, string.Empty, DesignerScenarioValueKind.Number));
                MarkDirty(_scenario);
            }
        }

        private void ScrubTo(NexUIDesignerContext context, float time)
        {
            _timelineTime = Mathf.Clamp(time, 0f, _scenario.timelineDuration);
            if (context?.Metadata == null) return;
            EnsureTimelineSnapshot(context);
            ScenarioService.ApplyAtTime(_scenario, context, _timelineTime);
        }

        private void ToggleTimelinePlayback(NexUIDesignerContext context)
        {
            if (_timelinePlaying) { _timelinePlaying = false; EditorApplication.update -= OnTimelineUpdate; return; }
            if (context?.Metadata == null) return;
            if (_timelineTime >= _scenario.timelineDuration) _timelineTime = 0f;
            EnsureTimelineSnapshot(context);
            _timelinePlaying = true;
            _lastEditorTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += OnTimelineUpdate;
        }

        private void OnTimelineUpdate()
        {
            var context = ResolveContext();
            if (_scenario == null || context?.Metadata == null) { StopTimeline(context); return; }

            var now = EditorApplication.timeSinceStartup;
            _timelineTime += (float)(now - _lastEditorTime);
            _lastEditorTime = now;

            if (_timelineTime >= _scenario.timelineDuration)
            {
                if (_scenario.timelineLoop) _timelineTime %= Mathf.Max(_scenario.timelineDuration, 0.0001f);
                else { _timelineTime = _scenario.timelineDuration; _timelinePlaying = false; EditorApplication.update -= OnTimelineUpdate; }
            }

            ScenarioService.ApplyAtTime(_scenario, context, _timelineTime);
            Repaint();
        }

        /// <summary>Ends playback, rewinds, and restores the pre-scrub preview values (§43: preview
        /// mutation must never persist to the asset).</summary>
        private void StopTimeline(NexUIDesignerContext context)
        {
            _timelinePlaying = false;
            EditorApplication.update -= OnTimelineUpdate;
            _timelineTime = 0f;
            RestoreTimelineSnapshot(context);
        }

        private void EnsureTimelineSnapshot(NexUIDesignerContext context)
        {
            if (_timelineSnapshotTaken || context?.Metadata == null) return;
            _timelineSnapshot.Clear();
            foreach (var element in context.Metadata.elements)
            {
                if (element == null || string.IsNullOrEmpty(element.elementId)) continue;
                _timelineSnapshot[element.elementId] = new ElementPreviewSnapshot
                {
                    PreviewValue = element.previewValue,
                    Text = element.text,
                    Hidden = element.hiddenInDesigner
                };
            }
            _timelineSnapshotTaken = true;
        }

        private void RestoreTimelineSnapshot(NexUIDesignerContext context)
        {
            if (!_timelineSnapshotTaken) return;
            if (context?.Metadata != null)
            {
                foreach (var element in context.Metadata.elements)
                {
                    if (element == null || !_timelineSnapshot.TryGetValue(element.elementId, out var snap)) continue;
                    element.previewValue = snap.PreviewValue;
                    element.text = snap.Text;
                    element.hiddenInDesigner = snap.Hidden;
                }
                context.NotifyPreviewValuesChanged();
            }
            _timelineSnapshot.Clear();
            _timelineSnapshotTaken = false;
        }

        private void DrawActions(NexUIDesignerContext context)
        {
            Section("scenario.section.actions");
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(context?.Metadata == null))
                {
                    if (GUILayout.Button(LC("scenario.button.apply", "tooltip.scenario.apply")))
                    {
                        _lastReport = ScenarioService.Apply(_scenario, context);
                        _hasReport = true;
                    }
                    if (GUILayout.Button(LC("scenario.button.capture", "tooltip.scenario.capture")))
                    {
                        ScenarioService.Capture(_scenario, context);
                        _hasReport = false;
                    }
                }
                if (GUILayout.Button(LC("scenario.button.save", "tooltip.scenario.save")))
                    AssetDatabase.SaveAssets();
            }
        }

        private void DrawReport()
        {
            if (!_hasReport || _lastReport.Messages == null) return;
            EditorGUILayout.HelpBox(string.Join("\n", _lastReport.Messages), MessageType.Info);
        }

        private void DrawValidation(NexUIDesignerContext context)
        {
            var issues = ScenarioService.Validate(_scenario, context);
            if (issues.Count == 0)
            {
                Badge(T("scenario.validation.passed"), BadgeKind.Ok);
                return;
            }
            foreach (var issue in issues)
                EditorGUILayout.HelpBox(issue.Message, issue.IsWarning ? MessageType.Warning : MessageType.Info);
        }
    }
}
