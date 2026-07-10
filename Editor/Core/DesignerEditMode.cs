using System;
using UnityEditor;

namespace emiteat.NexUI.Designer.Editor
{
    public enum DesignerMode
    {
        Simple,
        Advanced
    }

    /// <summary>
    /// C3: a single Simple/Advanced toggle for the whole Designer window (GitLab's three-tier
    /// progressive-disclosure model - essential stays always visible, advanced-only sections
    /// gate on this instead of each carrying its own toggle). Persisted per-user in EditorPrefs
    /// so it survives domain reloads and is remembered across sessions/screens.
    /// </summary>
    public static class DesignerEditMode
    {
        private const string PrefKey = "NexUI.Designer.EditMode";

        public static event Action<DesignerMode> Changed;

        public static DesignerMode Current
        {
            get => EditorPrefs.GetInt(PrefKey, 0) == 1 ? DesignerMode.Advanced : DesignerMode.Simple;
            set
            {
                if (Current == value) return;
                EditorPrefs.SetInt(PrefKey, value == DesignerMode.Advanced ? 1 : 0);
                Changed?.Invoke(value);
            }
        }

        public static bool IsAdvanced => Current == DesignerMode.Advanced;
    }
}
