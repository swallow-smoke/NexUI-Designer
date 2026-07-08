using System.Collections.Generic;

namespace emiteat.NexUI.Designer.Editor.Recipes
{
    /// <summary>
    /// Generates common game-UI structures into a <see cref="DesignerMetadataAsset"/>:
    /// elements (with binding-derived capabilities), parenting, and contract requirements
    /// for interactive elements (§22).
    /// </summary>
    public static class RecipeService
    {
        public static readonly string[] RecipeNames =
        {
            "Pause Menu", "Settings Menu", "Inventory Grid", "Toast Queue", "Dialogue Choice",
            "Quest Tracker", "HUD Stat Bar", "Loading Overlay", "Confirm Modal", "Tab Menu"
        };

        private struct Node
        {
            public string id, parent, command, text;
            public Node(string id, string parent, string command, string text)
            { this.id = id; this.parent = parent; this.command = command; this.text = text; }
        }

        public static int Generate(DesignerMetadataAsset asset, string recipe, string prefix)
        {
            int created = 0;
            foreach (var n in Blueprint(recipe))
            {
                string id = Prefixed(prefix, n.id);
                if (asset.Find(id) != null) continue;

                var e = new DesignerElementMetadata
                {
                    elementId = id,
                    parentId = string.IsNullOrEmpty(n.parent) ? null : Prefixed(prefix, n.parent)
                };
                if (!string.IsNullOrEmpty(n.command)) e.binding.commandKey = n.command;
                if (!string.IsNullOrEmpty(n.text)) e.binding.textKey = n.text;
                asset.elements.Add(e);
                created++;

                if (!string.IsNullOrEmpty(n.command) && asset.contract != null)
                    asset.contract.requiredElements.Add(new DesignerContractElementMetadata
                    {
                        elementId = id,
                        requiredCapabilities = new List<string> { "IUIClickCapability" }
                    });
            }
            return created;
        }

        private static string Prefixed(string prefix, string id)
            => string.IsNullOrEmpty(prefix) ? id : prefix + id;

        private static List<Node> Blueprint(string recipe)
        {
            switch (recipe)
            {
                case "Pause Menu": return new List<Node>
                {
                    new Node("PausePanel", "", "", ""),
                    new Node("TitleLabel", "PausePanel", "", "pause.title"),
                    new Node("ResumeButton", "PausePanel", "ui.resume", "button.resume"),
                    new Node("SettingsButton", "PausePanel", "open:Settings", "button.settings"),
                    new Node("QuitButton", "PausePanel", "open:Title", "button.quit"),
                };
                case "Settings Menu": return new List<Node>
                {
                    new Node("SettingsPanel", "", "", ""),
                    new Node("AudioTab", "SettingsPanel", "settings.tab.audio", "settings.audio"),
                    new Node("VideoTab", "SettingsPanel", "settings.tab.video", "settings.video"),
                    new Node("BackButton", "SettingsPanel", "ui.back", "button.back"),
                };
                case "Inventory Grid": return new List<Node>
                {
                    new Node("InventoryPanel", "", "", ""),
                    new Node("Grid", "InventoryPanel", "", ""),
                    new Node("SidePanel", "InventoryPanel", "", ""),
                    new Node("CloseButton", "InventoryPanel", "ui.back", "button.close"),
                };
                case "Toast Queue": return new List<Node>
                {
                    new Node("ToastRoot", "", "", ""),
                    new Node("ToastItem", "ToastRoot", "", "toast.message"),
                };
                case "Dialogue Choice": return new List<Node>
                {
                    new Node("DialoguePanel", "", "", ""),
                    new Node("DialogueText", "DialoguePanel", "", "dialogue.body"),
                    new Node("Choice1", "DialoguePanel", "dialogue.choice.1", "dialogue.choice1"),
                    new Node("Choice2", "DialoguePanel", "dialogue.choice.2", "dialogue.choice2"),
                };
                case "Quest Tracker": return new List<Node>
                {
                    new Node("QuestTracker", "", "", ""),
                    new Node("QuestTitle", "QuestTracker", "", "quest.title"),
                    new Node("QuestObjective", "QuestTracker", "", "quest.objective"),
                };
                case "HUD Stat Bar": return new List<Node>
                {
                    new Node("StatBar", "", "", ""),
                    new Node("HealthBar", "StatBar", "", ""),
                    new Node("ManaBar", "StatBar", "", ""),
                };
                case "Loading Overlay": return new List<Node>
                {
                    new Node("LoadingOverlay", "", "", ""),
                    new Node("Spinner", "LoadingOverlay", "", ""),
                    new Node("LoadingLabel", "LoadingOverlay", "", "loading.message"),
                };
                case "Confirm Modal": return new List<Node>
                {
                    new Node("ConfirmModal", "", "", ""),
                    new Node("ConfirmText", "ConfirmModal", "", "confirm.message"),
                    new Node("ConfirmButton", "ConfirmModal", "ui.confirm", "button.confirm"),
                    new Node("CancelButton", "ConfirmModal", "ui.cancel", "button.cancel"),
                };
                case "Tab Menu": return new List<Node>
                {
                    new Node("TabMenu", "", "", ""),
                    new Node("Tab1", "TabMenu", "tab.select.1", "tab.one"),
                    new Node("Tab2", "TabMenu", "tab.select.2", "tab.two"),
                    new Node("TabContent", "TabMenu", "", ""),
                };
                default: return new List<Node> { new Node("Root", "", "", "") };
            }
        }
    }
}
