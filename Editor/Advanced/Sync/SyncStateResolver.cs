namespace emiteat.NexUI.Designer.Editor.Sync
{
    /// <summary>Three-way sync state between the Designer's freshly generated output, the on-disk
    /// generated file, and the baseline last written by a publish (brief §32).</summary>
    public enum SyncState
    {
        /// <summary>No generated file on disk yet.</summary>
        New,
        /// <summary>Generated output already matches the file.</summary>
        InSync,
        /// <summary>Designer changed since last publish; the file is still the published baseline (safe to write).</summary>
        DesignerChanged,
        /// <summary>The file was edited outside the Designer; the Designer output still matches the baseline (file diverged).</summary>
        BackendChanged,
        /// <summary>Both the Designer output and the file diverged from the baseline (needs resolution).</summary>
        Conflict
    }

    /// <summary>
    /// Pure three-way sync-state resolution (brief §32). Given hashes of the current Designer-generated
    /// content, the on-disk file, and the baseline recorded at the last publish, classifies the state so
    /// the UI can offer Use Designer / Use Backend / Review Diff. No Unity or file dependency, so every
    /// case is unit-tested.
    /// </summary>
    public static class SyncStateResolver
    {
        public static SyncState Resolve(bool fileExists, string designerHash, string fileHash, string baseHash)
        {
            if (!fileExists) return SyncState.New;
            if (Same(designerHash, fileHash)) return SyncState.InSync;

            var hasBase = !string.IsNullOrEmpty(baseHash);
            if (hasBase && Same(fileHash, baseHash)) return SyncState.DesignerChanged;
            if (hasBase && Same(designerHash, baseHash)) return SyncState.BackendChanged;
            return SyncState.Conflict;
        }

        /// <summary>Whether the state is safe to auto-write during a changed-only publish (New or a
        /// clean Designer edit). InSync means nothing to do; BackendChanged/Conflict need a decision.</summary>
        public static bool IsAutoPublishable(SyncState state)
            => state == SyncState.New || state == SyncState.DesignerChanged;

        private static bool Same(string a, string b) => string.Equals(a ?? string.Empty, b ?? string.Empty);
    }
}
