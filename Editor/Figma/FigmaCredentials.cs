using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Integrations.Figma
{
    /// <summary>
    /// C5: stores the user's Figma personal access token in <see cref="EditorPrefs"/>, keyed
    /// per-project (via <see cref="Application.dataPath"/>) so it is scoped to this machine and
    /// this project only. Never written to any asset, scene, or other file that could end up in
    /// version control - EditorPrefs lives outside the project folder entirely.
    /// </summary>
    public static class FigmaCredentials
    {
        private static string Key => "NexUI.Figma.Token." + Application.dataPath.GetHashCode();

        public static string Token
        {
            get => EditorPrefs.GetString(Key, string.Empty);
            set => EditorPrefs.SetString(Key, value ?? string.Empty);
        }

        public static bool HasToken => !string.IsNullOrEmpty(Token);

        public static void Clear() => EditorPrefs.DeleteKey(Key);
    }
}
