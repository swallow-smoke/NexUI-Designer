using System;
using emiteat.NexUI.State;

namespace emiteat.NexUI.Designer.Samples
{
    /// <summary>Small state-only model used to demonstrate the complete Inventory binding path.</summary>
    public sealed class InventorySampleModel
    {
        private readonly UIStateStore _state;
        public string SelectedSlotId { get; private set; } = string.Empty;

        public InventorySampleModel(UIStateStore state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            SetSlot(1, "Iron Sword", 1);
            SetSlot(2, "Health Potion", 5);
            SetSlot(3, "Leather Boots", 1);
            SetSlot(4, "", 0);
            SetSlot(5, "Mana Potion", 3);
            SetSlot(6, "", 0);
            _state.Set("inventory.isOpen", false);
            _state.Set("inventory.selectedSlotId", string.Empty);
        }

        public void Open() => _state.Set("inventory.isOpen", true);
        public void Close() => _state.Set("inventory.isOpen", false);

        public void Select(int slot)
        {
            SelectedSlotId = "slot" + slot;
            _state.Set("inventory.selectedSlotId", SelectedSlotId);
            for (var i = 1; i <= 6; i++) _state.Set($"inventory.slot{i}.selected", i == slot);
        }

        public void EquipSelected()
        {
            if (string.IsNullOrEmpty(SelectedSlotId) || _state.Get<bool>($"inventory.{SelectedSlotId}.empty")) return;
            _state.Set("inventory.equippedSlotId", SelectedSlotId);
        }

        private void SetSlot(int slot, string itemName, int count)
        {
            var prefix = $"inventory.slot{slot}";
            _state.Set(prefix + ".name", itemName);
            _state.Set(prefix + ".count", count);
            _state.Set(prefix + ".empty", string.IsNullOrEmpty(itemName));
            _state.Set(prefix + ".selected", false);
        }
    }
}
