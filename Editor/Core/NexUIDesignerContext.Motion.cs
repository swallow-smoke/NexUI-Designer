using System;
using emiteat.NexUI.MotionClip;

namespace emiteat.NexUI.Designer.Editor
{
    public sealed partial class NexUIDesignerContext
    {
        /// <summary>Adds one persistent element or screen motion binding as a single Undo step.</summary>
        public DesignerMotionBinding AddMotionBinding(DesignerMotionTrigger trigger, UIMotionClip clip = null)
        {
            if (Metadata == null) return null;
            EnsureScreenMotion();
            RecordMetadata("Add NexUI Motion Binding");
            var binding = new DesignerMotionBinding
            {
                bindingId = Guid.NewGuid().ToString("N"),
                targetElementId = trigger == DesignerMotionTrigger.ScreenEnter || trigger == DesignerMotionTrigger.ScreenExit
                    ? string.Empty
                    : SelectedMetadata?.elementId ?? string.Empty,
                trigger = trigger,
                clip = clip
            };
            Metadata.screenMotion.bindings.Add(binding);
            MarkMetadataDirty();
            return binding;
        }

        /// <summary>Updates screen-level motion references as one metadata Undo step.</summary>
        public void UpdateScreenMotion(Action<DesignerScreenMotionMetadata> change, string undoName)
        {
            if (Metadata == null || change == null) return;
            EnsureScreenMotion();
            RecordMetadata(undoName);
            change(Metadata.screenMotion);
            MarkMetadataDirty();
        }

        /// <summary>Updates a motion binding on the metadata asset as a single Undo step.</summary>
        public void UpdateMotionBinding(DesignerMotionBinding binding, Action<DesignerMotionBinding> change)
        {
            if (Metadata == null || binding == null || change == null) return;
            EnsureScreenMotion();
            if (!Metadata.screenMotion.bindings.Contains(binding)) return;
            RecordMetadata("Edit NexUI Motion Binding");
            change(binding);
            MarkMetadataDirty();
        }

        /// <summary>Removes a motion binding from the metadata asset as a single Undo step.</summary>
        public void RemoveMotionBinding(DesignerMotionBinding binding)
        {
            if (Metadata == null || binding == null) return;
            EnsureScreenMotion();
            if (!Metadata.screenMotion.bindings.Contains(binding)) return;
            RecordMetadata("Remove NexUI Motion Binding");
            Metadata.screenMotion.bindings.Remove(binding);
            MarkMetadataDirty();
        }

        /// <summary>
        /// Renames an element and all Designer-owned references to it. This avoids creating
        /// dangling parent, focus, variant, and motion targets during an ordinary rename.
        /// </summary>
        public void RenameElementId(DesignerElementMetadata element, string newElementId)
        {
            if (Metadata == null || element == null || string.IsNullOrWhiteSpace(newElementId)) return;
            newElementId = newElementId.Trim();
            var oldElementId = element.elementId;
            if (oldElementId == newElementId) return;
            if (Metadata.Find(newElementId) != null) return;

            RecordMetadata("Rename NexUI Element Id");
            element.elementId = newElementId;
            foreach (var candidate in Metadata.elements)
            {
                if (candidate == null) continue;
                if (candidate.parentId == oldElementId) candidate.parentId = newElementId;
                if (candidate.focus.upElementId == oldElementId) candidate.focus.upElementId = newElementId;
                if (candidate.focus.downElementId == oldElementId) candidate.focus.downElementId = newElementId;
                if (candidate.focus.leftElementId == oldElementId) candidate.focus.leftElementId = newElementId;
                if (candidate.focus.rightElementId == oldElementId) candidate.focus.rightElementId = newElementId;
            }

            foreach (var variant in Metadata.variants)
            {
                if (variant?.overrides == null) continue;
                foreach (var variantOverride in variant.overrides)
                    if (variantOverride != null && variantOverride.targetElementId == oldElementId)
                        variantOverride.targetElementId = newElementId;
            }

            EnsureScreenMotion();
            foreach (var binding in Metadata.screenMotion.bindings)
                if (binding != null && binding.targetElementId == oldElementId)
                    binding.targetElementId = newElementId;

            if (CurrentScreen != null && CurrentScreen.focus.defaultFocusElementId == oldElementId)
            {
                UnityEditor.Undo.RecordObject(CurrentScreen, "Rename NexUI Element Id");
                var focus = CurrentScreen.focus;
                focus.defaultFocusElementId = newElementId;
                CurrentScreen.focus = focus;
                UnityEditor.EditorUtility.SetDirty(CurrentScreen);
            }

            MarkMetadataDirty();
            ElementChanged?.Invoke(element);
        }

        private void EnsureScreenMotion()
        {
            if (Metadata != null && Metadata.screenMotion == null)
                Metadata.screenMotion = new DesignerScreenMotionMetadata();
        }

        private void SynchronizeScreenMotionReferences()
        {
            if (Metadata == null || CurrentScreen == null) return;
            EnsureScreenMotion();
            var motion = CurrentScreen.motion;
            if (motion.openMotion == Metadata.screenMotion.entryClip && motion.closeMotion == Metadata.screenMotion.exitClip)
                return;
            motion.openMotion = Metadata.screenMotion.entryClip;
            motion.closeMotion = Metadata.screenMotion.exitClip;
            CurrentScreen.motion = motion;
            UnityEditor.EditorUtility.SetDirty(CurrentScreen);
        }
    }
}
