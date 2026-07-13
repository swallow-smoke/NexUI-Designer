using emiteat.NexUI.Core;
using emiteat.NexUI.State;

namespace emiteat.NexUI.Designer.Samples
{
    /// <summary>
    /// Shows the Command Pipeline wiring pattern used by the starter template screens
    /// (Settings, Inventory, ConfirmDialog, Loading, HUD): a Designer element's
    /// <c>binding.commandKey</c> is a plain string action key. At runtime it is registered
    /// against a <see cref="UIActionResolver"/>, which a <c>UICommandBinder</c> resolves and
    /// invokes when the element is clicked. The sample handlers mutate a small UIStateStore model
    /// and use the NexUI screen stack, so the imported sample works without game-specific systems.
    /// </summary>
    public static class TemplateCommands
    {
        /// <summary>Registers every action key used by the template screens' Designer metadata.</summary>
        public static TemplateSampleModel RegisterAll(UIActionResolver resolver, UIStateStore state = null)
        {
            if (resolver == null) return null;
            var model = new TemplateSampleModel(state ?? new UIStateStore());

            resolver.Register("nexui.close", model.CloseTopScreen);

            // Settings.UIToolkit/.UGUI: Apply button.
            resolver.Register("template.settings.apply", model.ApplySettings);

            // ConfirmDialog.UIToolkit/.UGUI: Confirm button.
            resolver.Register("template.confirmDialog.confirm", model.ConfirmDialog);
            return model;
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
