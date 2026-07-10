using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Integrations.Figma
{
    /// <summary>
    /// C5 (first slice): connection + auth UI for the Figma bridge. Lets the user store a
    /// personal access token and verify it, and pull a file's raw JSON to confirm access before
    /// the frame -&gt; NexUI mapping engine (not yet built) consumes it.
    /// </summary>
    public sealed class FigmaWindow : EditorWindow
    {
        private string _token;
        private string _fileKey = "";
        private string _statusMessage;
        private MessageType _statusType = MessageType.None;
        private string _lastFileJsonPreview;
        private bool _busy;

        public static void Open() => GetWindow<FigmaWindow>("NexUI Figma Bridge");

        private void OnEnable() => _token = FigmaCredentials.Token;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("NexUI Figma Bridge", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "C5 (in progress): personal access token auth + connection check are implemented. " +
                "The Figma frame -> NexUI screen mapping engine (Auto Layout conversion, text/font " +
                "mapping, coordinate conversion, nested components, auto-binding by name) is not " +
                "built yet - this window currently only proves your token/file access works.",
                MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Personal Access Token", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                _token = EditorGUILayout.PasswordField(_token);
                if (GUILayout.Button("Save", GUILayout.Width(60)))
                {
                    FigmaCredentials.Token = _token;
                    SetStatus("Token saved locally (EditorPrefs, not committed to version control).", MessageType.Info);
                }
                if (GUILayout.Button("Clear", GUILayout.Width(60)))
                {
                    FigmaCredentials.Clear();
                    _token = string.Empty;
                    SetStatus("Token cleared.", MessageType.Info);
                }
            }

            using (new EditorGUI.DisabledScope(_busy || string.IsNullOrEmpty(_token)))
            {
                if (GUILayout.Button("Test Connection", GUILayout.Height(24)))
                    TestConnection();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("File Key", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The id in a Figma file URL: figma.com/file/<fileKey>/...", MessageType.None);
            _fileKey = EditorGUILayout.TextField(_fileKey);

            using (new EditorGUI.DisabledScope(_busy || string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_fileKey)))
            {
                if (GUILayout.Button("Fetch File", GUILayout.Height(24)))
                    FetchFile();
            }

            if (!string.IsNullOrEmpty(_statusMessage))
                EditorGUILayout.HelpBox(_statusMessage, _statusType);

            if (!string.IsNullOrEmpty(_lastFileJsonPreview))
            {
                EditorGUILayout.LabelField("Response preview (first 500 chars)", EditorStyles.boldLabel);
                EditorGUILayout.TextArea(_lastFileJsonPreview, GUILayout.Height(120));
            }
        }

        private async void TestConnection()
        {
            _busy = true;
            SetStatus("Connecting...", MessageType.None);
            try
            {
                var user = await FigmaApiClient.GetAuthenticatedUserAsync(_token);
                SetStatus($"Connected as {user.handle} ({user.email}).", MessageType.Info);
            }
            catch (System.Exception ex)
            {
                SetStatus($"Connection failed: {ex.Message}", MessageType.Error);
            }
            finally
            {
                _busy = false;
                Repaint();
            }
        }

        private async void FetchFile()
        {
            _busy = true;
            SetStatus("Fetching file...", MessageType.None);
            try
            {
                var json = await FigmaApiClient.GetFileJsonAsync(_token, _fileKey);
                _lastFileJsonPreview = json.Length > 500 ? json.Substring(0, 500) + "..." : json;
                SetStatus($"Fetched {json.Length} bytes. Treat this as a draft source, not a final asset - review it in the Designer before shipping.", MessageType.Info);
            }
            catch (System.Exception ex)
            {
                SetStatus($"Fetch failed: {ex.Message}", MessageType.Error);
            }
            finally
            {
                _busy = false;
                Repaint();
            }
        }

        private void SetStatus(string message, MessageType type)
        {
            _statusMessage = message;
            _statusType = type;
            Repaint();
        }
    }
}
