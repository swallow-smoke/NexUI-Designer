namespace emiteat.NexUI.Designer.Editor.MotionClipEditor
{
    /// <summary>Controls whether canvas edits in the main NexUI Designer (drag/resize) write keyframes into the open Motion Clip at the current scrub time.</summary>
    public enum MotionClipAutoKeyMode
    {
        /// <summary>Canvas edits only change <c>DesignerElementMetadata.rect</c> (the normal, non-recording behavior).</summary>
        Off,

        /// <summary>Canvas edits update a keyframe only on property tracks that already exist for the edited element.</summary>
        ExistingTracks,

        /// <summary>Canvas edits create the element's track/property track (if missing) and add or update a keyframe.</summary>
        AllChanges
    }
}
