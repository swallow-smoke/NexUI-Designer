using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Components;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>Example simulated payloads per component event (spec §29) - shown in the inspector and used by the Preview Log.</summary>
    public static class DesignerEventPayloads
    {
        public static string Example(string typeId, string evt)
        {
            switch (evt)
            {
                case "Dropped": return "{ \"sourceSlotId\": \"inventory.slot.3\", \"targetSlotId\": \"hotbar.slot.1\", \"itemId\": \"…\", \"count\": 1 }";
                case "Selected":
                case "ItemSelected": return "{ \"id\": \"…\", \"index\": 0 }";
                case "SelectionChanged": return "{ \"selectedIds\": [\"…\"] }";
                case "ActiveIndexChanged": return "{ \"activeIndex\": 0 }";
                case "ValueChanged": return "{ \"value\": 0.0 }";
                case "Opened":
                case "Closed":
                case "Dismissed": return "{ \"screenId\": \"…\" }";
                default: return "{ }";
            }
        }
    }

    /// <summary>Small helpers shared by the bespoke inspector providers.</summary>
    internal static class InspectorProviderUtil
    {
        public static VisualElement Header(string text)
            => new Label(text) { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 4, marginBottom = 2 } };

        /// <summary>Appends the descriptor-driven summary section under a bespoke provider's own fields.</summary>
        public static void AppendDescriptorSection(VisualElement root, NexUIDesignerContext context,
            DesignerElementMetadata element, DesignerComponentDescriptor d)
        {
            var sep = new VisualElement { style = { height = 1, marginTop = 6, marginBottom = 4, backgroundColor = new Color(1, 1, 1, 0.08f) } };
            root.Add(sep);
            root.Add(new DescriptorInspectorProvider().Build(context, element, d));
        }
    }

    /// <summary>Value components (ProgressBar/StatBar/RadialFill/Spinner): range, value slider, direction.</summary>
    public sealed class ValueComponentInspectorProvider : IUIDesignerComponentInspectorProvider
    {
        public VisualElement Build(NexUIDesignerContext context, DesignerElementMetadata element, DesignerComponentDescriptor d)
        {
            var root = new VisualElement();
            root.Add(InspectorProviderUtil.Header("Value"));

            var min = new FloatField("Min") { value = element.fill.minValue };
            min.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.fill.minValue = evt.newValue, "Set Min"));
            root.Add(min);

            var max = new FloatField("Max") { value = element.fill.maxValue };
            max.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.fill.maxValue = evt.newValue, "Set Max"));
            root.Add(max);

            var slider = new Slider("Value", element.fill.minValue, Mathf.Max(element.fill.minValue + 0.01f, element.fill.maxValue))
            { value = element.previewValue, showInputField = true };
            slider.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.previewValue = evt.newValue, "Set Preview Value"));
            root.Add(slider);

            if (element.elementType == "ProgressBar" || element.elementType == "StatBar")
            {
                var dir = new EnumField("Fill Direction", element.fill.direction);
                dir.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.fill.direction = (DesignerFillDirection)evt.newValue, "Set Fill Direction"));
                root.Add(dir);
            }
            else // Radial/Spinner
            {
                var cw = new Toggle("Clockwise") { value = element.fill.clockwise };
                cw.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.fill.clockwise = evt.newValue, "Set Clockwise"));
                root.Add(cw);
            }

            if (d.SupportsState(DesignerComponentState.Indeterminate))
                root.Add(StateButton(context, "Preview Indeterminate", DesignerComponentState.Indeterminate));

            InspectorProviderUtil.AppendDescriptorSection(root, context, element, d);
            return root;
        }

        internal static Button StateButton(NexUIDesignerContext context, string label, DesignerComponentState state)
        {
            var b = new Button(() => context.SetForcedPreviewState(context.ForcedPreviewState == state ? DesignerComponentState.Normal : state)) { text = label };
            b.AddToClassList("nexui-toolbar-button");
            return b;
        }
    }

    /// <summary>Choice List: inline option editor bound to previewOptions.</summary>
    public sealed class ChoiceListInspectorProvider : IUIDesignerComponentInspectorProvider
    {
        public VisualElement Build(NexUIDesignerContext context, DesignerElementMetadata element, DesignerComponentDescriptor d)
        {
            var root = new VisualElement();
            root.Add(InspectorProviderUtil.Header("Options"));

            var listHost = new VisualElement();
            root.Add(listHost);

            void Rebuild()
            {
                listHost.Clear();
                var options = element.previewOptions ?? new List<string>();
                for (int i = 0; i < options.Count; i++)
                {
                    var index = i;
                    var row = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
                    var field = new TextField { value = options[i], style = { flexGrow = 1 } };
                    field.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.previewOptions[index] = evt.newValue, "Edit Option"));
                    row.Add(field);
                    var remove = new Button(() => { context.UpdateElement(element, e => e.previewOptions.RemoveAt(index), "Remove Option"); Rebuild(); }) { text = "✕", tooltip = "Remove option." };
                    remove.AddToClassList("nexui-layer-icon-button");
                    row.Add(remove);
                    listHost.Add(row);
                }
            }
            Rebuild();

            var add = new Button(() =>
            {
                context.UpdateElement(element, e => { e.previewOptions ??= new List<string>(); e.previewOptions.Add("Option " + (e.previewOptions.Count + 1)); }, "Add Option");
                Rebuild();
            }) { text = "+ Add Option" };
            add.AddToClassList("nexui-toolbar-button");
            root.Add(add);

            InspectorProviderUtil.AppendDescriptorSection(root, context, element, d);
            return root;
        }
    }

    /// <summary>List/Grid/Hotbar: generated preview item count + state quick-previews.</summary>
    public sealed class CollectionInspectorProvider : IUIDesignerComponentInspectorProvider
    {
        public VisualElement Build(NexUIDesignerContext context, DesignerElementMetadata element, DesignerComponentDescriptor d)
        {
            var root = new VisualElement();
            root.Add(InspectorProviderUtil.Header(element.elementType == "Hotbar" ? "Slots" : "Data"));

            var label = element.elementType == "Hotbar" ? "Slot Count" : "Preview Item Count";
            var count = new IntegerField(label) { value = element.previewItemCount };
            count.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.previewItemCount = Mathf.Max(0, evt.newValue), "Set Preview Item Count"));
            root.Add(count);

            if (element.elementType == "Hotbar")
            {
                var active = new IntegerField("Active Index") { value = Mathf.RoundToInt(element.previewValue) };
                active.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.previewValue = Mathf.Max(0, evt.newValue), "Set Active Index"));
                root.Add(active);
            }

            var states = new VisualElement { style = { flexDirection = FlexDirection.Row, flexWrap = Wrap.Wrap, marginTop = 2 } };
            foreach (var s in new[] { DesignerComponentState.Normal, DesignerComponentState.Empty, DesignerComponentState.Loading, DesignerComponentState.Error })
                if (d.SupportsState(s))
                    states.Add(ValueComponentInspectorProvider.StateButton(context, s.ToString(), s));
            root.Add(states);

            InspectorProviderUtil.AppendDescriptorSection(root, context, element, d);
            return root;
        }
    }

    /// <summary>Slot: item key, stack count, and (shared) parent-slot picker.</summary>
    public sealed class SlotInspectorProvider : IUIDesignerComponentInspectorProvider
    {
        public VisualElement Build(NexUIDesignerContext context, DesignerElementMetadata element, DesignerComponentDescriptor d)
        {
            var root = new VisualElement();
            root.Add(InspectorProviderUtil.Header("Item"));

            var itemKey = new TextField("Item Key") { value = element.binding?.valueKey ?? "" };
            itemKey.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.binding.valueKey = evt.newValue, "Set Item Key"));
            root.Add(itemKey);

            var count = new IntegerField("Stack Count (preview)") { value = Mathf.RoundToInt(element.previewValue) };
            count.RegisterValueChangedCallback(evt => context.UpdateElement(element, e => e.previewValue = Mathf.Max(0, evt.newValue), "Set Stack Count"));
            root.Add(count);

            InspectorProviderUtil.AppendDescriptorSection(root, context, element, d);
            return root;
        }
    }
}
