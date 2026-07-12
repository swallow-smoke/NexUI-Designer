using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// Produces the component-specific Inspector section for a selected element. Resolved per type
    /// through <see cref="DesignerComponentInspectorRegistry"/> so each component can present its own
    /// UI without a shared switch. The built-in <see cref="DescriptorInspectorProvider"/> renders a
    /// full section straight from the registry descriptor (identity, preview-state picker, supported
    /// states / bindings / events, slots, per-backend support), which every type gets for free.
    /// </summary>
    public interface IUIDesignerComponentInspectorProvider
    {
        VisualElement Build(NexUIDesignerContext context, DesignerElementMetadata element, DesignerComponentDescriptor descriptor);
    }

    public static class DesignerComponentInspectorRegistry
    {
        private static readonly Dictionary<string, IUIDesignerComponentInspectorProvider> _byId =
            new Dictionary<string, IUIDesignerComponentInspectorProvider>();
        private static readonly IUIDesignerComponentInspectorProvider _default = new DescriptorInspectorProvider();

        static DesignerComponentInspectorRegistry()
        {
            // Bespoke providers (each composes the descriptor summary under its own fields).
            var value = new ValueComponentInspectorProvider();
            Register("ProgressBar", value);
            Register("StatBar", value);
            Register("RadialFill", value);
            Register("Spinner", value);
            Register("ChoiceList", new ChoiceListInspectorProvider());
            var collection = new CollectionInspectorProvider();
            Register("List", collection);
            Register("Grid", collection);
            Register("Hotbar", collection);
            Register("Slot", new SlotInspectorProvider());
        }

        /// <summary>Registers a bespoke provider for a type (overrides the descriptor-driven default).</summary>
        public static void Register(string typeId, IUIDesignerComponentInspectorProvider provider)
        {
            if (!string.IsNullOrEmpty(typeId) && provider != null) _byId[typeId] = provider;
        }

        public static IUIDesignerComponentInspectorProvider Get(string typeId)
            => !string.IsNullOrEmpty(typeId) && _byId.TryGetValue(typeId, out var p) ? p : _default;
    }

    /// <summary>The Design-tab section that hosts the per-component inspector. Rebuilds on selection change.</summary>
    public sealed class ComponentInspector : DesignerInspectorBase
    {
        private readonly VisualElement _host;

        public ComponentInspector(NexUIDesignerContext context) : base(context, "panel.inspector")
        {
            _host = new VisualElement();
            Add(_host);
            context.MultiSelectionChanged += _ => Rebuild();
            context.PreviewSettingsChanged += Rebuild;
            Rebuild();
        }

        private void Rebuild()
        {
            _host.Clear();
            var element = Context.SelectedMetadata;
            if (element == null) { style.display = DisplayStyle.None; return; }
            style.display = DisplayStyle.Flex;
            var descriptor = DesignerComponentRegistry.Get(element.elementType);
            _host.Add(DesignerComponentInspectorRegistry.Get(element.elementType).Build(Context, element, descriptor));
        }
    }

    /// <summary>Descriptor-driven per-type inspector - used for every type unless a bespoke provider is registered.</summary>
    public sealed class DescriptorInspectorProvider : IUIDesignerComponentInspectorProvider
    {
        public VisualElement Build(NexUIDesignerContext context, DesignerElementMetadata element, DesignerComponentDescriptor d)
        {
            var root = new VisualElement();

            // Identity
            var title = new Label($"{d.DisplayName}  ·  {d.Category}");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(title);
            if (!string.IsNullOrEmpty(d.Description))
            {
                var desc = new Label(d.Description) { style = { whiteSpace = WhiteSpace.Normal, opacity = 0.75f } };
                root.Add(desc);
            }
            if (d.IsGeneric)
                root.Add(Hint("Unknown/custom type — shown generically. Its data is preserved."));

            // Parent-slot picker: when this element's parent exposes named slots, let the user
            // assign which slot it occupies (spec §22-3 slot assignment, without a canvas rework).
            AddParentSlotPicker(root, context, element);

            // Preview-state picker (only states this component supports).
            var states = SupportedStatesList(d);
            if (states.Count > 1)
            {
                var current = states.Contains(context.ForcedPreviewState) ? context.ForcedPreviewState : DesignerComponentState.Normal;
                var picker = new PopupField<DesignerComponentState>("Preview State", states, current)
                {
                    tooltip = "Force a state to preview it on the canvas. Preview-only; not saved on the element."
                };
                picker.RegisterValueChangedCallback(evt => context.SetForcedPreviewState(evt.newValue));
                root.Add(picker);
            }

            // Capability chips
            root.Add(ChipRow("Supported states", StateNames(d)));
            root.Add(ChipRow("Bindings", BindingNames(d)));
            if (d.SupportedEvents.Count > 0)
            {
                root.Add(ChipRow("Events", d.SupportedEvents));
                // Example simulated payload per event (spec §29).
                foreach (var evt in d.SupportedEvents)
                    root.Add(new Label($"{evt}: {DesignerEventPayloads.Example(d.TypeId, evt)}")
                        { style = { fontSize = 10, opacity = 0.55f, whiteSpace = WhiteSpace.Normal, marginLeft = 8 } });
            }

            // Slots
            if (d.Slots.Count > 0)
            {
                root.Add(SubHeader("Slots"));
                foreach (var s in d.Slots)
                {
                    var max = s.MaximumChildren == int.MaxValue ? "∞" : s.MaximumChildren.ToString();
                    var flags = (s.IsTemplateSlot ? " [template]" : "") + (s.IsGeneratedContentSlot ? " [generated]" : "");
                    root.Add(new Label($"• {s.DisplayName}  ({s.SlotId})  {s.MinimumChildren}–{max}{flags}")
                        { style = { fontSize = 11, opacity = 0.85f } });
                }
            }

            // Backend support levels
            root.Add(SubHeader("Backend support"));
            root.Add(BackendRow("uGUI", d.UGUISupport));
            root.Add(BackendRow("UI Toolkit", d.UIToolkitSupport));
            if (d.UGUISupport != DesignerBackendSupport.Full || d.UIToolkitSupport != DesignerBackendSupport.Full)
                root.Add(Hint("Partial / Preview-only values are shown in the canvas but reported in the Save Report rather than fully written."));

            return root;
        }

        private static void AddParentSlotPicker(VisualElement root, NexUIDesignerContext context, DesignerElementMetadata element)
        {
            if (context.Metadata == null || string.IsNullOrEmpty(element.parentId)) return;
            var parent = context.Metadata.Find(element.parentId);
            if (parent == null) return;
            var parentDesc = DesignerComponentRegistry.Get(parent.elementType);
            if (parentDesc.Slots.Count < 2) return; // single/no slot ⇒ nothing to choose

            var ids = new List<string>();
            foreach (var s in parentDesc.Slots) ids.Add(s.SlotId);
            var current = string.IsNullOrEmpty(element.parentSlotId) ? DesignerComponentSlot.Content : element.parentSlotId;
            if (!ids.Contains(current)) current = ids[0];

            var picker = new PopupField<string>($"Parent Slot ({parent.elementId})", ids, current)
            {
                tooltip = "Which named slot of the parent this element occupies."
            };
            picker.RegisterValueChangedCallback(evt =>
                context.UpdateElement(element, e => e.parentSlotId = evt.newValue, "Set Parent Slot"));
            root.Add(picker);
        }

        private static List<DesignerComponentState> SupportedStatesList(DesignerComponentDescriptor d)
        {
            var list = new List<DesignerComponentState> { DesignerComponentState.Normal };
            foreach (DesignerComponentState s in Enum.GetValues(typeof(DesignerComponentState)))
                if (s != DesignerComponentState.None && s != DesignerComponentState.Normal && d.SupportsState(s))
                    list.Add(s);
            return list;
        }

        private static List<string> StateNames(DesignerComponentDescriptor d)
        {
            var list = new List<string>();
            foreach (DesignerComponentState s in Enum.GetValues(typeof(DesignerComponentState)))
                if (s != DesignerComponentState.None && d.SupportsState(s)) list.Add(s.ToString());
            return list;
        }

        private static List<string> BindingNames(DesignerComponentDescriptor d)
        {
            var list = new List<string>();
            foreach (DesignerBindingChannel c in Enum.GetValues(typeof(DesignerBindingChannel)))
                if (c != DesignerBindingChannel.None && d.SupportsBinding(c)) list.Add(c.ToString());
            if (list.Count == 0) list.Add("—");
            return list;
        }

        private static VisualElement SubHeader(string text)
        {
            var l = new Label(text) { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 6, opacity = 0.9f } };
            return l;
        }

        private static VisualElement Hint(string text)
            => new Label(text) { style = { whiteSpace = WhiteSpace.Normal, fontSize = 11, opacity = 0.6f, marginTop = 2 } };

        private static VisualElement ChipRow(string label, List<string> values)
        {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, flexWrap = Wrap.Wrap, alignItems = Align.Center, marginTop = 4 } };
            row.Add(new Label(label + ":") { style = { fontSize = 11, opacity = 0.7f, marginRight = 4 } });
            foreach (var v in values)
            {
                var chip = new Label(v);
                chip.style.fontSize = 10;
                chip.style.paddingLeft = 5; chip.style.paddingRight = 5;
                chip.style.paddingTop = 1; chip.style.paddingBottom = 1;
                chip.style.marginRight = 3; chip.style.marginBottom = 2;
                chip.style.backgroundColor = new Color(1f, 1f, 1f, 0.08f);
                chip.style.borderTopLeftRadius = 3; chip.style.borderTopRightRadius = 3;
                chip.style.borderBottomLeftRadius = 3; chip.style.borderBottomRightRadius = 3;
                row.Add(chip);
            }
            return row;
        }

        private static VisualElement BackendRow(string backend, DesignerBackendSupport support)
        {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center, marginTop = 2 } };
            row.Add(new Label(backend) { style = { width = 80, fontSize = 11 } });
            var dot = new VisualElement { style = { width = 8, height = 8, marginRight = 5 } };
            dot.style.borderTopLeftRadius = 4; dot.style.borderTopRightRadius = 4;
            dot.style.borderBottomLeftRadius = 4; dot.style.borderBottomRightRadius = 4;
            dot.style.backgroundColor = SupportColor(support);
            row.Add(dot);
            row.Add(new Label(support.ToString()) { style = { fontSize = 11, opacity = 0.85f } });
            return row;
        }

        private static Color SupportColor(DesignerBackendSupport s)
        {
            switch (s)
            {
                case DesignerBackendSupport.Full: return new Color(0.30f, 0.72f, 0.40f);
                case DesignerBackendSupport.Partial: return new Color(0.92f, 0.68f, 0.22f);
                case DesignerBackendSupport.PreviewOnly: return new Color(0.35f, 0.6f, 0.95f);
                default: return new Color(0.86f, 0.28f, 0.28f);
            }
        }
    }
}
