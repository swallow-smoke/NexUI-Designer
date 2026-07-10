# ConfirmDialog Template Screen

A modal confirm/cancel popup. Demonstrates Modal layer, trap-focus policy, additive open policy, and two command bindings (confirm stub / nexui.close).

## Files

- `ConfirmDialog.UIToolkit.asset` / `ConfirmDialog.UGUI.asset` — `UIScreenDefinition` variants, one per backend. Both point at the same `ConfirmDialog.Metadata.asset` element layout.
- `ConfirmDialog.Metadata.asset` — Designer element metadata (open with `Tools/NexUI/Designer`).
- `UIToolkit/ConfirmDialog.uxml` + `.uss` — UI Toolkit backend asset.
- `UGUI/ConfirmDialog.prefab` — uGUI backend asset.

## Try it

1. Open `Tools/NexUI/Designer`.
2. Assign `ConfirmDialog.UIToolkit.asset` (or the `.UGUI` variant) as the open screen, and `ConfirmDialog.Metadata.asset` as its metadata.
3. Click **Rebuild Preview** / **Validate** to see the layout.
4. Command bindings on interactive elements are wired to no-op stub commands in `TemplateCommands.cs` (see `Scripts/TemplateCommands.cs`) — replace `CommandId` handlers with real gameplay logic in your own project.
