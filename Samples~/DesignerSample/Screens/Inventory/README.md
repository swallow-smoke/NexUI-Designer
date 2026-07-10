# Inventory Template Screen

A 3x2 inventory grid with a Grid container and Slot elements. Demonstrates List/Grid/Slot element types and a Close command binding.

## Files

- `Inventory.UIToolkit.asset` / `Inventory.UGUI.asset` — `UIScreenDefinition` variants, one per backend. Both point at the same `Inventory.Metadata.asset` element layout.
- `Inventory.Metadata.asset` — Designer element metadata (open with `Tools/NexUI/Designer`).
- `UIToolkit/Inventory.uxml` + `.uss` — UI Toolkit backend asset.
- `UGUI/Inventory.prefab` — uGUI backend asset.

## Try it

1. Open `Tools/NexUI/Designer`.
2. Assign `Inventory.UIToolkit.asset` (or the `.UGUI` variant) as the open screen, and `Inventory.Metadata.asset` as its metadata.
3. Click **Rebuild Preview** / **Validate** to see the layout.
4. Command bindings on interactive elements are wired to no-op stub commands in `TemplateCommands.cs` (see `Scripts/TemplateCommands.cs`) — replace `CommandId` handlers with real gameplay logic in your own project.
