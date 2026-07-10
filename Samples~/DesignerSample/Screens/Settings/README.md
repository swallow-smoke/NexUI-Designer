# Settings Template Screen

A settings menu window with volume rows and Back/Apply buttons. Demonstrates a Window-layer, single-instance screen with default focus and an Apply command stub.

## Files

- `Settings.UIToolkit.asset` / `Settings.UGUI.asset` — `UIScreenDefinition` variants, one per backend. Both point at the same `Settings.Metadata.asset` element layout.
- `Settings.Metadata.asset` — Designer element metadata (open with `Tools/NexUI/Designer`).
- `UIToolkit/Settings.uxml` + `.uss` — UI Toolkit backend asset.
- `UGUI/Settings.prefab` — uGUI backend asset.

## Try it

1. Open `Tools/NexUI/Designer`.
2. Assign `Settings.UIToolkit.asset` (or the `.UGUI` variant) as the open screen, and `Settings.Metadata.asset` as its metadata.
3. Click **Rebuild Preview** / **Validate** to see the layout.
4. Command bindings on interactive elements are wired to no-op stub commands in `TemplateCommands.cs` (see `Scripts/TemplateCommands.cs`) — replace `CommandId` handlers with real gameplay logic in your own project.
