using System.Collections.Generic;
using System.Linq;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.MotionClip;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>
    /// Standalone timeline/keyframe editor for <see cref="UIMotionClip"/> assets. Opened either
    /// via Tools/NexUI/Designer/Motion Clip Editor (pick any clip from the toolbar) or from the
    /// Motion inspector's "Open Motion Clip Editor" button (preview surface + selected element
    /// pre-filled), matching the standalone-window convention used by <c>MotionGraphWindow</c>.
    /// MVP: buttons-first UI, not a polished drag-and-drop timeline.
    /// </summary>
    public sealed class MotionClipEditorWindow : EditorWindow
    {
        [SerializeField] private UIMotionClip _clip;
        [SerializeField] private string _targetElementId;
        [SerializeField] private float _previewTime;

        private IUISurface _previewSurface;
        private readonly UIMotionClipPlayer _player = new UIMotionClipPlayer();
        private bool _isPlaying;
        private double _lastEditorTime;

        private Slider _scrubber;
        private VisualElement _tracksHost;
        private Button _playButton;

        [MenuItem("Tools/NexUI/Designer/Advanced/Motion Clip Editor")]
        public static void OpenFromMenu()
        {
            var window = GetWindow<MotionClipEditorWindow>();
            window.titleContent = new GUIContent("Motion Clip Editor");
            window.minSize = new Vector2(640f, 420f);
            window.BuildUI();
        }

        /// <summary>Opened from the Designer's Motion inspector with the live preview surface and selected element pre-filled.</summary>
        public static void Open(IUISurface previewSurface, string targetElementId)
        {
            var window = GetWindow<MotionClipEditorWindow>();
            window.titleContent = new GUIContent("Motion Clip Editor");
            window.minSize = new Vector2(640f, 420f);
            window._previewSurface = previewSurface;
            window._targetElementId = targetElementId;
            window.BuildUI();
        }

        private void CreateGUI() => BuildUI();

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            _player.Stop();
        }

        private void BuildUI()
        {
            var root = rootVisualElement;
            root.Clear();
            root.AddToClassList("nexui-designer-root");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.emiteat.nexui.designer/Editor/Styles/NexUIDesigner.uss");
            if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
                root.styleSheets.Add(styleSheet);

            root.Add(BuildToolbar());
            root.Add(BuildTimeRow());

            _tracksHost = new VisualElement { name = "TracksHost" };
            _tracksHost.style.flexGrow = 1;
            var scroll = new ScrollView();
            scroll.style.flexGrow = 1;
            scroll.Add(_tracksHost);
            root.Add(scroll);

            RefreshTracks();
        }

        private VisualElement BuildToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.AddToClassList("nexui-toolbar");

            var clipField = new ObjectField("Clip") { objectType = typeof(UIMotionClip), allowSceneObjects = false, value = _clip };
            clipField.AddToClassList("nexui-metadata-field");
            clipField.RegisterValueChangedCallback(evt =>
            {
                _clip = evt.newValue as UIMotionClip;
                _previewTime = 0f;
                BuildUI();
            });
            toolbar.Add(clipField);

            toolbar.Add(MakeButton(CreateClip, "Create Motion Clip", "nexui-button-secondary"));
            toolbar.Add(MakeButton(AddTrackFromSelection, "Add Track From Selection", "nexui-button-secondary"));

            if (_clip != null)
            {
                var duration = new FloatField("Duration") { value = _clip.duration };
                duration.AddToClassList("nexui-compact-popup");
                duration.RegisterValueChangedCallback(evt =>
                {
                    Undo.RecordObject(_clip, "Edit Motion Clip Duration");
                    _clip.duration = Mathf.Max(0.01f, evt.newValue);
                    EditorUtility.SetDirty(_clip);
                    _scrubber.highValue = _clip.duration;
                });
                toolbar.Add(duration);

                var loop = new Toggle("Loop") { value = _clip.loop };
                loop.AddToClassList("nexui-toolbar-toggle");
                loop.RegisterValueChangedCallback(evt =>
                {
                    Undo.RecordObject(_clip, "Edit Motion Clip Loop");
                    _clip.loop = evt.newValue;
                    EditorUtility.SetDirty(_clip);
                });
                toolbar.Add(loop);

                _playButton = MakeButton(TogglePlay, "Play", "nexui-button-primary");
                toolbar.Add(_playButton);
                toolbar.Add(MakeButton(() => AssetDatabase.SaveAssets(), "Save", "nexui-button-secondary"));
            }

            return toolbar;
        }

        private VisualElement BuildTimeRow()
        {
            var row = new VisualElement { name = "TimeRow" };
            row.AddToClassList("nexui-toolbar-row");

            _scrubber = new Slider("Time", 0f, _clip != null ? _clip.duration : 1f) { value = _previewTime };
            _scrubber.style.flexGrow = 1;
            _scrubber.RegisterValueChangedCallback(evt =>
            {
                _previewTime = evt.newValue;
                EvaluatePreview();
            });
            row.Add(_scrubber);

            return row;
        }

        private void CreateClip()
        {
            var asset = ScriptableObject.CreateInstance<UIMotionClip>();
            asset.clipName = "NewMotionClip";
            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewMotionClip.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            _clip = asset;
            EditorGUIUtility.PingObject(asset);
            BuildUI();
        }

        private void AddTrackFromSelection()
        {
            if (_clip == null) return;

            var elementId = _targetElementId;
            if (string.IsNullOrEmpty(elementId))
            {
                var designer = Resources.FindObjectsOfTypeAll<NexUIDesignerWindow>().FirstOrDefault();
                if (designer != null && designer.Context?.SelectedMetadata != null)
                {
                    elementId = designer.Context.SelectedMetadata.elementId;
                    _previewSurface = designer.Context.PreviewSurface;
                }
            }

            if (string.IsNullOrEmpty(elementId))
            {
                EditorUtility.DisplayDialog("Motion Clip Editor",
                    "No element selected. Select an element in the NexUI Designer first.", "OK");
                return;
            }

            var tracks = new List<UIMotionClipTrack>(_clip.tracks ?? System.Array.Empty<UIMotionClipTrack>());
            if (tracks.Any(t => t.targetElementId == elementId))
            {
                RefreshTracks();
                return;
            }

            Undo.RecordObject(_clip, "Add Motion Clip Track");
            tracks.Add(new UIMotionClipTrack { targetElementId = elementId });
            _clip.tracks = tracks.ToArray();
            EditorUtility.SetDirty(_clip);
            RefreshTracks();
        }

        private void RefreshTracks()
        {
            if (_tracksHost == null) return;
            _tracksHost.Clear();

            if (_clip == null)
            {
                _tracksHost.Add(new HelpBox("Create or assign a Motion Clip to begin.", HelpBoxMessageType.Info));
                return;
            }

            if (_clip.tracks == null || _clip.tracks.Length == 0)
            {
                _tracksHost.Add(new HelpBox("No tracks yet. Select an element in the Designer and click \"Add Track From Selection\".", HelpBoxMessageType.Info));
                return;
            }

            foreach (var track in _clip.tracks)
                _tracksHost.Add(BuildTrackView(track));
        }

        private VisualElement BuildTrackView(UIMotionClipTrack track)
        {
            var section = new VisualElement();
            section.AddToClassList("nexui-inspector-section");

            var header = new Label(track.targetElementId) { name = "SectionTitle" };
            section.Add(header);

            var addPropertyRow = new VisualElement();
            addPropertyRow.AddToClassList("nexui-inline-row");
            var propertyPopup = new EnumField("Property", UIMotionClipPropertyType.AnchoredPosition);
            propertyPopup.style.flexGrow = 1;
            addPropertyRow.Add(propertyPopup);
            addPropertyRow.Add(MakeButton(() => AddPropertyTrack(track, (UIMotionClipPropertyType)propertyPopup.value), "Add Property Track", "nexui-button-secondary"));
            section.Add(addPropertyRow);

            if (track.propertyTracks != null)
                foreach (var propertyTrack in track.propertyTracks)
                    section.Add(BuildPropertyTrackView(track, propertyTrack));

            return section;
        }

        private void AddPropertyTrack(UIMotionClipTrack track, UIMotionClipPropertyType propertyType)
        {
            var propertyTracks = new List<UIMotionClipPropertyTrack>(track.propertyTracks ?? System.Array.Empty<UIMotionClipPropertyTrack>());
            if (propertyTracks.Any(p => p.propertyType == propertyType))
            {
                RefreshTracks();
                return;
            }

            Undo.RecordObject(_clip, "Add Motion Clip Property Track");
            propertyTracks.Add(new UIMotionClipPropertyTrack { propertyType = propertyType });
            track.propertyTracks = propertyTracks.ToArray();
            EditorUtility.SetDirty(_clip);
            RefreshTracks();
        }

        private VisualElement BuildPropertyTrackView(UIMotionClipTrack track, UIMotionClipPropertyTrack propertyTrack)
        {
            var box = new VisualElement();
            box.AddToClassList("nexui-panel");

            var titleRow = new VisualElement();
            titleRow.AddToClassList("nexui-inline-row");
            titleRow.Add(new Label(propertyTrack.propertyType.ToString()) { name = "PanelTitle" });
            titleRow.Add(MakeButton(() => AddKeyframe(track, propertyTrack), "Add Keyframe At Time", "nexui-button-secondary"));
            box.Add(titleRow);

            var timeline = new MotionClipTimelineView(propertyTrack, () => _clip != null ? _clip.duration : 1f,
                onChanged: () => { EditorUtility.SetDirty(_clip); EvaluatePreview(); },
                onDelete: index => DeleteKeyframe(track, propertyTrack, index));
            box.Add(timeline);

            return box;
        }

        private void AddKeyframe(UIMotionClipTrack track, UIMotionClipPropertyTrack propertyTrack)
        {
            var value = DefaultValueFor(propertyTrack.propertyType);
            var keyframes = new List<UIMotionClipKeyframe>(propertyTrack.keyframes ?? System.Array.Empty<UIMotionClipKeyframe>())
            {
                new UIMotionClipKeyframe(_previewTime, value)
            };
            keyframes.Sort((a, b) => a.time.CompareTo(b.time));

            Undo.RecordObject(_clip, "Add Motion Clip Keyframe");
            propertyTrack.keyframes = keyframes.ToArray();
            EditorUtility.SetDirty(_clip);
            RefreshTracks();
        }

        private void DeleteKeyframe(UIMotionClipTrack track, UIMotionClipPropertyTrack propertyTrack, int index)
        {
            var keyframes = new List<UIMotionClipKeyframe>(propertyTrack.keyframes);
            if (index < 0 || index >= keyframes.Count) return;

            Undo.RecordObject(_clip, "Delete Motion Clip Keyframe");
            keyframes.RemoveAt(index);
            propertyTrack.keyframes = keyframes.ToArray();
            EditorUtility.SetDirty(_clip);
            RefreshTracks();
        }

        private static UIMotionClipValue DefaultValueFor(UIMotionClipPropertyType propertyType)
        {
            switch (propertyType)
            {
                case UIMotionClipPropertyType.LocalRotationZ:
                case UIMotionClipPropertyType.CanvasGroupAlpha:
                    return UIMotionClipValue.Float(propertyType == UIMotionClipPropertyType.CanvasGroupAlpha ? 1f : 0f);
                case UIMotionClipPropertyType.LocalScale:
                    return UIMotionClipValue.FromVector3(Vector3.one);
                default:
                    return UIMotionClipValue.FromVector2(Vector2.zero);
            }
        }

        // ---- Preview -------------------------------------------------------

        private IUISurface ResolvePreviewSurface()
        {
            if (_previewSurface != null) return _previewSurface;
            var designer = Resources.FindObjectsOfTypeAll<NexUIDesignerWindow>().FirstOrDefault();
            return designer?.Context?.PreviewSurface;
        }

        private void EvaluatePreview()
        {
            if (_clip == null) return;
            var surface = ResolvePreviewSurface();
            if (surface == null) return;
            _player.Evaluate(surface, _clip, _previewTime);
        }

        private void TogglePlay()
        {
            if (_isPlaying) StopPreview();
            else StartPreview();
        }

        private void StartPreview()
        {
            if (_clip == null) return;
            _isPlaying = true;
            _lastEditorTime = EditorApplication.timeSinceStartup;
            if (_playButton != null) _playButton.text = "Stop";
            EditorApplication.update += OnEditorUpdate;
        }

        private void StopPreview()
        {
            _isPlaying = false;
            if (_playButton != null) _playButton.text = "Play";
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            var now = EditorApplication.timeSinceStartup;
            var delta = (float)(now - _lastEditorTime);
            _lastEditorTime = now;

            _previewTime += delta;
            var duration = _clip != null ? _clip.duration : 0f;
            if (_previewTime >= duration)
            {
                if (_clip != null && _clip.loop)
                    _previewTime %= Mathf.Max(duration, 0.0001f);
                else
                {
                    _previewTime = duration;
                    StopPreview();
                }
            }

            if (_scrubber != null) _scrubber.SetValueWithoutNotify(_previewTime);
            EvaluatePreview();
        }

        private static Button MakeButton(System.Action action, string text, string className)
        {
            var button = new Button(action) { text = text };
            button.AddToClassList("nexui-toolbar-button");
            button.AddToClassList(className);
            return button;
        }
    }
}
