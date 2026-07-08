using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Localization
{
    public static class DesignerLocalization
    {
        private const string PrefKey = "NexUI.Designer.Language";
        private const string PackageRoot = "Packages/com.emiteat.nexui.designer/Localization";
        private static readonly Dictionary<string, string> Current = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> English = new Dictionary<string, string>();
        private static DesignerLanguage _language;

        public static DesignerLanguage CurrentLanguage => _language;
        public static event Action LanguageChanged;

        static DesignerLocalization()
        {
            _language = (DesignerLanguage)EditorPrefs.GetInt(PrefKey, (int)DesignerLanguage.Korean);
            Load();
        }

        public static void SetLanguage(DesignerLanguage language)
        {
            _language = language;
            EditorPrefs.SetInt(PrefKey, (int)language);
            Load();
            LanguageChanged?.Invoke();
        }

        public static string T(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (Current.TryGetValue(key, out var value)) return value;
            if (English.TryGetValue(key, out value)) return value;
            return key;
        }

        public static string T(string key, params object[] args)
            => string.Format(T(key), args);

        private static void Load()
        {
            Current.Clear();
            English.Clear();
            LoadFile(Path.Combine(PackageRoot, "en-US.json"), English);
            LoadFile(Path.Combine(PackageRoot, _language == DesignerLanguage.Korean ? "ko-KR.json" : "en-US.json"), Current);
        }

        private static void LoadFile(string path, Dictionary<string, string> target)
        {
            if (!File.Exists(path)) return;
            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim().TrimEnd(',');
                if (!line.StartsWith("\"", StringComparison.Ordinal)) continue;
                var colon = line.IndexOf(':');
                if (colon <= 0) continue;

                var key = Unquote(line.Substring(0, colon).Trim());
                var value = Unquote(line.Substring(colon + 1).Trim());
                if (!string.IsNullOrEmpty(key))
                    target[key] = value;
            }
        }

        private static string Unquote(string value)
        {
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
                value = value.Substring(1, value.Length - 2);
            return value.Replace("\\\"", "\"").Replace("\\n", "\n");
        }
    }
}
