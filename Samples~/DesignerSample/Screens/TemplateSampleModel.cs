using System;
using emiteat.NexUI.Core;
using emiteat.NexUI.State;

namespace emiteat.NexUI.Designer.Samples
{
    /// <summary>Small working state model for Settings and ConfirmDialog sample commands.</summary>
    public sealed class TemplateSampleModel
    {
        private readonly UIStateStore state;

        public TemplateSampleModel(UIStateStore state)
        {
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            state.Set("settings.applied", false);
            state.Set("confirmDialog.result", string.Empty);
        }

        public void CloseTopScreen() => NexUIApp.Back();
        public void ApplySettings() => state.Set("settings.applied", true);
        public void ConfirmDialog()
        {
            state.Set("confirmDialog.result", "confirmed");
            NexUIApp.Close("ConfirmDialog");
        }
    }
}
