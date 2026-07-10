# Loading Template Screen

A full-screen loading overlay with a ProgressBar placeholder and percent label. Demonstrates the Overlay layer and Preload-vs-LazyLoad load strategy field.

## Files

- `Loading.UIToolkit.asset` / `Loading.UGUI.asset` — `UIScreenDefinition` variants, one per backend. Both point at the same `Loading.Metadata.asset` element layout.
- `Loading.Metadata.asset` — Designer element metadata (open with `Tools/NexUI/Designer`).
- `UIToolkit/Loading.uxml` + `.uss` — UI Toolkit backend asset.
- `UGUI/Loading.prefab` — uGUI backend asset.

## Try it

1. Open `Tools/NexUI/Designer`.
2. Assign `Loading.UIToolkit.asset` (or the `.UGUI` variant) as the open screen, and `Loading.Metadata.asset` as its metadata.
3. Click **Rebuild Preview** / **Validate** to see the layout.
4. Command bindings on interactive elements are wired to no-op stub commands in `TemplateCommands.cs` (see `Scripts/TemplateCommands.cs`) — replace `CommandId` handlers with real gameplay logic in your own project.
