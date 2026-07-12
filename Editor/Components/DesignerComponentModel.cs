using System;
using System.Collections.Generic;
using emiteat.NexUI.Accessibility;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Components
{
    /// <summary>High-level grouping used by the Palette / Assets browser.</summary>
    public enum DesignerComponentCategory
    {
        Container,
        Text,
        Media,
        Input,
        Feedback,
        Overlay,
        Data,
        Generic
    }

    /// <summary>
    /// Preview/interaction states a component can express. Not every component supports every
    /// state - each <see cref="DesignerComponentDescriptor"/> declares its supported subset.
    /// Forced preview state is a per-screen preview setting, never persisted on the element.
    /// </summary>
    [Flags]
    public enum DesignerComponentState
    {
        None          = 0,
        Normal        = 1 << 0,
        Hover         = 1 << 1,
        Pressed       = 1 << 2,
        Focused       = 1 << 3,
        Selected      = 1 << 4,
        Disabled      = 1 << 5,
        Loading       = 1 << 6,
        Empty         = 1 << 7,
        Error         = 1 << 8,
        Success       = 1 << 9,
        Warning       = 1 << 10,
        Indeterminate = 1 << 11
    }

    /// <summary>
    /// The binding channels a component supports. These map 1:1 onto the existing serialized
    /// <see cref="DesignerBindingMetadata"/> keys (no new serialized fields), so the Inspector can
    /// show only the channels a component actually supports and Validation can flag a key set on an
    /// unsupported channel without ever deleting user data.
    /// </summary>
    [Flags]
    public enum DesignerBindingChannel
    {
        None         = 0,
        Text         = 1 << 0, // DesignerBindingMetadata.textKey
        Value        = 1 << 1, // valueKey
        Visibility   = 1 << 2, // visibilityKey
        Class        = 1 << 3, // classKey
        Command      = 1 << 4, // commandKey
        Interactable = 1 << 5  // interactableKey
    }

    /// <summary>How completely a property/behavior can be written to a given backend.</summary>
    public enum DesignerBackendSupport
    {
        Full,
        Partial,
        PreviewOnly,
        Unsupported
    }

    /// <summary>
    /// A named region of a component into which the user may place authored children (distinct
    /// from Virtual Preview Parts, which the renderer draws and are never stored as elements).
    /// The default slot id is <c>"content"</c>; elements with no explicit <c>parentSlotId</c> are
    /// treated as belonging to it.
    /// </summary>
    public sealed class DesignerComponentSlot
    {
        public const string Content = "content";

        public string SlotId;
        public string DisplayName;
        public string LocalizationKey;
        public int MinimumChildren;
        public int MaximumChildren = int.MaxValue; // int.MaxValue ⇒ unbounded
        public string[] AcceptedComponentTypes;    // null ⇒ any type accepted
        public bool IsTemplateSlot;                 // holds a single item template (List/Grid/Hotbar)
        public bool IsGeneratedContentSlot;         // filled by generated preview items, no direct authoring
        public bool AllowReorder = true;

        public bool Accepts(string typeId)
            => AcceptedComponentTypes == null || Array.IndexOf(AcceptedComponentTypes, typeId) >= 0;
    }

    /// <summary>
    /// Single source of truth for one component type: its identity, defaults, capabilities, slots,
    /// supported states/bindings and per-backend support. Palette, Inspector, Preview, Validation,
    /// Hierarchy and the backend serializers all read from here instead of duplicating type
    /// switch statements (see <see cref="DesignerComponentRegistry"/>).
    /// </summary>
    public sealed class DesignerComponentDescriptor
    {
        // Identity
        public string TypeId;
        public string DisplayName;
        public string LocalizationKey;
        public DesignerComponentCategory Category = DesignerComponentCategory.Generic;
        public string Icon;            // DesignerIcons key or unicode glyph
        public string Description;

        // Defaults (Palette creation)
        public Vector2 DefaultSize = new Vector2(240, 96);
        public Vector2 MinimumSize = new Vector2(24, 24);
        public DesignerElementShape DefaultShape = DesignerElementShape.Rounded;
        public Color DefaultColor = new Color(0.13f, 0.18f, 0.26f, 1f);
        public string DefaultText;

        // Capabilities
        public bool CanResize = true;
        public bool CanHaveChildren;
        public bool IsContainer;
        public bool IsInteractive;
        public bool IsValueComponent;
        public bool IsCollectionComponent;
        public bool IsOverlayComponent;

        public AccessibilityRole DefaultAccessibilityRole = AccessibilityRole.None;

        // Structure & contracts
        public List<DesignerComponentSlot> Slots = new List<DesignerComponentSlot>();
        public DesignerComponentState SupportedStates = DesignerComponentState.Normal;
        public DesignerBindingChannel SupportedBindings = DesignerBindingChannel.Visibility;
        public List<string> SupportedEvents = new List<string>();

        // Backend support levels
        public DesignerBackendSupport UGUISupport = DesignerBackendSupport.Full;
        public DesignerBackendSupport UIToolkitSupport = DesignerBackendSupport.Full;

        public bool IsGeneric => Category == DesignerComponentCategory.Generic;

        public bool SupportsBinding(DesignerBindingChannel channel) => (SupportedBindings & channel) != 0;
        public bool SupportsState(DesignerComponentState state) => (SupportedStates & state) != 0;

        /// <summary>The slot children default to when they declare no explicit slot (always "content" when present, else the first slot, else null).</summary>
        public string DefaultSlotId
        {
            get
            {
                foreach (var s in Slots)
                    if (s.SlotId == DesignerComponentSlot.Content) return s.SlotId;
                return Slots.Count > 0 ? Slots[0].SlotId : null;
            }
        }

        public DesignerComponentSlot GetSlot(string slotId)
        {
            if (string.IsNullOrEmpty(slotId)) slotId = DesignerComponentSlot.Content;
            foreach (var s in Slots)
                if (s.SlotId == slotId) return s;
            return null;
        }

        /// <summary>Maps a serialized binding key name to its channel, for support/validation checks.</summary>
        public static DesignerBindingChannel ChannelForKeyName(string keyFieldName)
        {
            switch (keyFieldName)
            {
                case "textKey": return DesignerBindingChannel.Text;
                case "valueKey": return DesignerBindingChannel.Value;
                case "visibilityKey": return DesignerBindingChannel.Visibility;
                case "classKey": return DesignerBindingChannel.Class;
                case "commandKey": return DesignerBindingChannel.Command;
                case "interactableKey": return DesignerBindingChannel.Interactable;
                default: return DesignerBindingChannel.None;
            }
        }
    }
}
