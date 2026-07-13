using System.IO;
using UnityEditor;
using UnityEngine;
using emiteat.NexUI.Designer.Editor.Common;

namespace emiteat.NexUI.Designer.Editor.AgentHandoff
{
    /// <summary>Full UI for the AI agent handoff manifest export (§23).</summary>
    public sealed class AgentHandoffWindow : NexUIToolWindow
    {
        private DesignerAgentHandoffMetadata _manifest;
        private string _json;

        protected override string TitleKey => "panel.agentHandoff";
        protected override string TooltipKey => "tooltip.agentHandoff";

        public static void Open() => GetWindow<AgentHandoffWindow>();

        protected override void DrawBody()
        {
            Section("panel.agentHandoff");
            if (GUILayout.Button("Build from project", GUILayout.Height(24)))
            {
                _manifest = AgentHandoffService.Collect();
                _json = AgentHandoffService.ToJson(_manifest);
            }

            if (_manifest == null) return;

            EditorGUILayout.LabelField($"screens: {_manifest.screens.Count}   state: {_manifest.stateKeys.Count}   actions: {_manifest.actionKeys.Count}");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(LC("button.exportManifest", "tooltip.exportManifest"), GUILayout.Height(22)))
                Export();
            if (GUILayout.Button(LC("button.copyCliPrompt", "tooltip.copyCliPrompt"), GUILayout.Height(22)))
                CopyCliPrompt();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("nexui-agent-manifest.json", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(_json,
                EditorStyles.textArea, GUILayout.Height(260));
        }

        private void Export()
        {
            string folder = EditorUtility.SaveFolderPanel("Export Agent Manifest", "", "");
            if (string.IsNullOrEmpty(folder)) return;
            File.WriteAllText(Path.Combine(folder, "nexui-agent-manifest.json"), _json);
            File.WriteAllText(Path.Combine(folder, "nexui-agent-brief.md"), AgentHandoffService.ToMarkdown(_manifest));
            EditorUtility.RevealInFinder(folder);
        }

        private void CopyCliPrompt()
        {
            EditorGUIUtility.systemCopyBuffer =
                "You are working on the NexUI Unity UI framework. Project state:\n\n" +
                AgentHandoffService.ToMarkdown(_manifest) +
                "\nFollow the forbidden rules. Propose the next change as a minimal, compiling diff.";
        }
    }
}
