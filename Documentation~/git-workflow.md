# Git-Friendly Screen Collaboration (B8)

`DesignerMetadataAsset` (`*.Metadata.asset`) stays Unity's own YAML - swapping its underlying
storage format would touch every code path that creates/loads/writes one (Designer Context,
every inspector, the sample screens), so this doesn't replace it. Instead, every metadata save
also writes a companion `*.Metadata.json` file next to the `.asset`, kept in sync automatically.

- **Review PRs against the `.json`, not the `.asset`.** The JSON has a fixed field order (same
  order every save, for the same data) and uses persistent asset GUIDs instead of instance IDs
  for object references (motion preset, theme) - it diffs and merges the way a normal text file
  does. `element.parentId` is a plain string id, not a Unity object reference, so there's no
  GUID-churn-on-merge problem for the element hierarchy either.
- **After resolving a merge conflict in the `.json`**, run `Tools/NexUI/Designer/Backend/Sync
  Metadata From JSON` (or `NexUIDesignerContext.SyncMetadataFromJson()`) to push the merged
  result back into the `.asset`. This is Undo-tracked like every other Designer mutation.
- The JSON is a byproduct of saving, not a separate thing to remember to export - open the
  screen in the Designer, hit **Save**, and both files update together.

See the 5 starter template screens under `Samples~/DesignerSample/Screens/` for real
`.asset`/`.json` pairs to look at.
