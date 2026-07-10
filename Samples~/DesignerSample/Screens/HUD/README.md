# HUD Template Screen

A health/resource bar HUD using StatBar elements (background + fill pairs). Demonstrates an always-additive, non-input-blocking HUD-layer screen intended to be preloaded and kept open.

## Files

- `HUD.UIToolkit.asset` / `HUD.UGUI.asset` — `UIScreenDefinition` variants, one per backend. Both point at the same `HUD.Metadata.asset` element layout.
- `HUD.Metadata.asset` — Designer element metadata (open with `Tools/NexUI/Designer`).
- `UIToolkit/HUD.uxml` + `.uss` — UI Toolkit backend asset.
- `UGUI/HUD.prefab` — uGUI backend asset.

## Try it

1. Open `Tools/NexUI/Designer`.
2. Assign `HUD.UIToolkit.asset` (or the `.UGUI` variant) as the open screen, and `HUD.Metadata.asset` as its metadata.
3. Click **Rebuild Preview** / **Validate** to see the layout.
4. Command bindings on interactive elements are wired to no-op stub commands in `TemplateCommands.cs` (see `Scripts/TemplateCommands.cs`) — replace `CommandId` handlers with real gameplay logic in your own project.
