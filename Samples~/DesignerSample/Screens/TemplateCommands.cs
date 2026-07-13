using emiteat.NexUI.Core;
using emiteat.NexUI.State;
using UnityEngine;

namespace emiteat.NexUI.Designer.Samples
{
    /// <summary>
    /// Shows the Command Pipeline wiring pattern used by the starter template screens
    /// (Settings, Inventory, ConfirmDialog, Loading, HUD): a Designer element's
    /// <c>binding.commandKey</c> is a plain string action key. At runtime it is registered
    /// against a <see cref="UIActionResolver"/>, which a <c>UICommandBinder</c> resolves and
    /// invokes when the element is clicked. The handlers below are no-ops (they just log) -
    /// replace them with real gameplay/UI calls (e.g. dispatching an
    /// <see cref="emiteat.NexUI.Abstractions.IUICommand"/> through your Command Pipeline) in
    /// your own project.
    /// </summary>
    public static class TemplateCommands
    {
        /// <summary>Registers every action key used by the template screens' Designer metadata.</summary>
        public static void RegisterAll(UIActionResolver resolver)
        {
            if (resolver == null) return;

            // Built-in pipeline command - closes whichever screen owns the clicked element.
            // Real projects typically resolve the owning screen id at bind time instead of
            // hardcoding it; shown here as a stub for the template's Back/Close/Cancel buttons.
            resolver.Register("nexui.close", () => Debug.Log("[NexUI Sample] nexui.close (no-op stub)"));

            // Settings.UIToolkit/.UGUI: Apply button.
            resolver.Register("template.settings.apply", () => Debug.Log("[NexUI Sample] Settings applied (no-op stub)"));

            // ConfirmDialog.UIToolkit/.UGUI: Confirm button.
            resolver.Register("template.confirmDialog.confirm", () => Debug.Log("[NexUI Sample] Confirm dialog accepted (no-op stub)"));
        }

        /// <summary>Registers the working Inventory vertical-slice commands against its sample state model.</summary>
        public static InventorySampleModel RegisterInventory(UIActionResolver resolver, UIStateStore state)
        {
            if (resolver == null || state == null) return null;
            var model = new InventorySampleModel(state);
            for (var slot = 1; slot <= 6; slot++)
            {
                var captured = slot;
                resolver.Register("inventory.select.slot" + captured, () => model.Select(captured));
            }
            resolver.Register("inventory.open", model.Open);
            resolver.Register("inventory.close", model.Close);
            resolver.Register("inventory.equip", model.EquipSelected);
            return model;
        }
    }
}
