using System;
using System.Collections.Generic;
using emiteat.NexUI.Designer.Editor.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// Shown instead of/above the single-element inspectors whenever more than one element is
    /// selected. Reports common values for position/size/layer, or "Mixed" when they differ -
    /// per-element editing still happens through the single-element inspectors below (which
    /// continue to operate on the "primary" selection).
    /// </summary>
    public sealed class MultiSelectionInspector : DesignerInspectorBase
    {
        private readonly Label _summary;
        private readonly Label _position;
        private readonly Label _size;
        private readonly Label _layer;

        public MultiSelectionInspector(NexUIDesignerContext context) : base(context, "inspector.multiSelection")
        {
            _summary = new Label { tooltip = DesignerLocalization.T("tooltip.multiSelection.summary") };
            _summary.AddToClassList("nexui-multiselect-summary");
            Add(_summary);
            _position = new Label { tooltip = DesignerLocalization.T("tooltip.multiSelection.position") };
            Add(_position);
            _size = new Label { tooltip = DesignerLocalization.T("tooltip.multiSelection.size") };
            Add(_size);
            _layer = new Label { tooltip = DesignerLocalization.T("tooltip.multiSelection.layer") };
            Add(_layer);

            context.MultiSelectionChanged += _ => Refresh();
            Refresh();
        }

        private void Refresh()
        {
            var selection = Context.SelectedElements;
            if (selection.Count <= 1)
            {
                style.display = DisplayStyle.None;
                return;
            }

            style.display = DisplayStyle.Flex;
            _summary.text = selection.Count + " elements selected";
            _position.text = "Position: " + (AllEqual(selection, e => e.rect.position) ? selection[0].rect.position.ToString() : "Mixed");
            _size.text = "Size: " + (AllEqual(selection, e => e.rect.size) ? selection[0].rect.size.ToString() : "Mixed");
            _layer.text = "Layer: " + LayerSummary(selection);
        }

        private static bool AllEqual(IReadOnlyList<DesignerElementMetadata> elements, Func<DesignerElementMetadata, Vector2> selector)
        {
            var first = selector(elements[0]);
            for (int i = 1; i < elements.Count; i++)
                if (selector(elements[i]) != first) return false;
            return true;
        }

        private string LayerSummary(IReadOnlyList<DesignerElementMetadata> selection)
        {
            if (Context.Metadata == null) return "Mixed";
            var indices = new List<int>();
            foreach (var e in selection)
                indices.Add(Context.Metadata.elements.IndexOf(e));
            indices.Sort();

            var contiguous = true;
            for (int i = 1; i < indices.Count; i++)
            {
                if (indices[i] != indices[i - 1] + 1) { contiguous = false; break; }
            }
            return contiguous ? indices[0] + "-" + indices[indices.Count - 1] : "Mixed";
        }
    }
}
