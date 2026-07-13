using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor;

namespace emiteat.NexUI.Integrations.Figma
{
    /// <summary>
    /// Connects to Figma and imports the first top-level frame into the active Designer metadata.
    /// </summary>
    public sealed class FigmaWindow : EditorWindow
    {
        private string _token;
        private string _fileKey = "";
        private string _statusMessage;
        private MessageType _statusType = MessageType.None;
        private string _lastFileJsonPreview;
        private string _lastFileJson;
        private bool _busy;

        public static void Open() => GetWindow<FigmaWindow>("NexUI Figma Bridge");

        private void OnEnable() => _token = FigmaCredentials.Token;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("NexUI Figma Bridge", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "가져오기는 첫 번째 Frame의 계층, 좌표, Text, Solid Fill, Auto Layout을 현재 Designer Metadata로 변환합니다. " +
                "가져온 결과는 저장 전에 Designer와 Validation에서 검토하세요.", MessageType.Info);

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
                var context = DesignerSessions.ActiveContext;
                using (new EditorGUI.DisabledScope(_busy || context?.Metadata == null))
                {
                    if (GUILayout.Button("현재 Designer로 첫 Frame 가져오기", GUILayout.Height(28)))
                        ImportIntoDesigner(context);
                }
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
                _lastFileJson = json;
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

        private void ImportIntoDesigner(NexUIDesignerContext context)
        {
            if (context?.Metadata == null || string.IsNullOrEmpty(_lastFileJson)) return;
            if (!EditorUtility.DisplayDialog("Figma Frame 가져오기",
                    "현재 Metadata의 Element를 Figma 첫 Frame으로 교체합니다. 이 작업은 Undo할 수 있습니다.", "가져오기", "취소")) return;
            try
            {
                Undo.RecordObject(context.Metadata, "Import Figma Frame");
                var count = FigmaDocumentImporter.ImportFirstFrame(_lastFileJson, context.Metadata);
                EditorUtility.SetDirty(context.Metadata);
                context.SetMetadata(context.Metadata);
                SetStatus($"{count}개 노드를 가져왔습니다. Validation 후 저장하세요.", MessageType.Info);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                SetStatus($"Import failed: {ex.Message}", MessageType.Error);
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
