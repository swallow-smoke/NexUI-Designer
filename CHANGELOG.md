# Changelog

## [Unreleased]

### Stabilized
- Added a focus-aware `DesignerSessionRegistry` and removed satellite-window context discovery through `Resources.FindObjectsOfTypeAll`.
- Added panel-lifetime event subscriptions so rebuilt/closed VisualElements do not accumulate Context callbacks.
- Persisted screen and element Motion Clip bindings, Reduced Motion alternatives, Motion State Machine and Motion Graph references in Designer metadata.
- Added Motion binding Undo/Redo, element-id reference migration, save synchronization and validation for missing targets/clips and invalid keyframes.
- Added dirty-state handling, Ctrl+S and Undo/Redo preview refresh.
- Restored recent screen, metadata, valid selection and canvas scroll state by asset GUID after reload.
- Avoided constructing `RectOffset` while Unity is serializing metadata during domain reload.
- Added a transactional generated-asset writer for UXML/USS with validation, marker protection, dry run, VCS checkout, rollback and targeted imports.
- Added Session, lifecycle, Motion persistence, Undo consistency, generated-writer and sample smoke EditMode tests plus GitHub Actions EditMode/PlayMode workflow.
- Updated Korean documentation, architecture, implementation status, installation and testing guides.

### Added
- **Motion Clip Editor**: new standalone `Tools/NexUI/Designer/Motion Clip Editor` window for
  authoring multi-element, multi-property, keyframe-based `UIMotionClip` assets, with a Designer
  selection-linked entry point ("Open Motion Clip Editor") from the Motion inspector. Includes a
  minimal timeline view (draggable keyframes), live preview against the Designer's preview
  surface, and Play/Stop. See `Documentation~/motion-clip-editor.md`.
- `UnityAnimationClipAdapter` (preview an existing `AnimationClip` via `SampleAnimation`);
  `UIMotionClipImporter`/`UIMotionClipExporter` interfaces (stubbed, not yet implemented).
- Motion Graph Editor: `Tools/NexUI/Designer/Motion Graph` menu entry so it can be opened
  standalone (with its own Preset picker) instead of only from the Motion inspector; new
  documentation (`Documentation~/motion-graph-editor.md`, previously undocumented); "Auto
  Layout" and "Duplicate Node" context menu actions; brand-new (empty) graphs are now seeded
  with a connected `start`/`end` node pair.
- Shared IMGUI chrome for all `NexUIToolWindow`-based satellite tool windows (header band,
  accent section headers, status badges) driven by an expanded `DesignerColors` token set, so
  their look now tracks the main Designer's dark UI Toolkit theme instead of default Editor
  styling.

### Fixed
- Main Designer window's bottom panel (`State`/`Command`/`Screen Graph` cards) was clipped at a
  fixed 34px/28px height that didn't fit its own content; increased to 64px/56px.
- `MotionGraphWindow` (Motion Graph popout) now applies the shared `NexUIDesigner.uss`
  stylesheet and button classes, matching the rest of the Designer.

### Known limitations
- Motion Clip `AnchoredPosition`/`LocalPosition` currently resolve to the same underlying value.
- Motion trigger data is persisted for both Backends, but automatic runtime wiring for every trigger remains Partial.
- AnimationClip Import/Export remains a disabled Stub.

## 0.1.0

- Initial NexUI Designer extension package.
- Added metadata assets, localized Editor window shell, backend abstraction, tools, inspectors, graph panels, serializers, and documentation.
