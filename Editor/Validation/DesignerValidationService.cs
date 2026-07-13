using System.Collections.Generic;
using emiteat.NexUI.Abstractions;
using emiteat.NexUI.Core;
using emiteat.NexUI.Designer.Editor.Backend;
using emiteat.NexUI.Designer.Editor.Components;
using emiteat.NexUI.Designer.Editor.Serialization;
using emiteat.NexUI.MotionClip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace emiteat.NexUI.Designer.Editor.Validation
{
    /// <summary>
    /// Produces structured, actionable validation issues for a screen + metadata pair.
    /// Cheap metadata / screen rules run unconditionally; backend-asset cross-checks read
    /// the backend asset directly (VisualTreeAsset clone or prefab-asset transform walk)
    /// without instantiating a live surface.
    /// </summary>
    public static class DesignerValidationService
    {
        public static List<DesignerValidationIssue> Validate(UIScreenDefinition screen, DesignerMetadataAsset metadata)
        {
            var issues = new List<DesignerValidationIssue>();
            var screenId = screen != null ? screen.ScreenId : null;

            if (screen == null)
            {
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "no-screen",
                    "No screen is open.", "Assign a UIScreenDefinition in the toolbar Screen field."));
                return issues;
            }

            ValidateScreen(screen, screenId, issues);

            var backendNames = CollectBackendElementNames(screen);

            if (metadata == null)
            {
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "no-metadata",
                    "No Designer metadata asset is assigned.",
                    "Assign or create a DesignerMetadataAsset with the 'New' button.", screenId));
                return issues;
            }

            if (!string.IsNullOrEmpty(metadata.screenId) && !string.IsNullOrEmpty(screenId) && metadata.screenId != screenId)
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "metadata-screen-mismatch",
                    $"Metadata screenId '{metadata.screenId}' differs from screen '{screenId}'.",
                    "Set the metadata screenId to match the screen, or open the correct screen.", screenId));

            ValidateElements(screen, metadata, screenId, backendNames, issues);
            ValidateHierarchy(metadata, screenId, issues);
            ValidateOrphans(metadata, screenId, backendNames, issues);
            ValidateReferences(metadata, screenId, issues);
            ValidateMotion(metadata, screenId, issues);
            ValidatePrefabComponents(screen, metadata, screenId, issues);

            return issues;
        }

        private static void ValidateScreen(UIScreenDefinition screen, string screenId, List<DesignerValidationIssue> issues)
        {
            if (string.IsNullOrEmpty(screenId))
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "empty-screen-id",
                    "Screen has an empty screenId.", "Set identity.screenId on the UIScreenDefinition."));

            var backend = screen.backendAsset.backend;
            var asset = screen.backendAsset.asset;

            if (!DesignerBackendRegistry.TryGet(backend, out _))
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "unsupported-backend",
                    $"No designer backend is registered for '{backend}'.",
                    "Use a supported backend (UIToolkit or UGUI).", screenId));

            if (asset == null)
            {
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "backend-asset-missing",
                    "The screen has no backend asset assigned.",
                    backend == UIRenderBackend.UIToolkit
                        ? "Assign a UXML VisualTreeAsset to backendAsset.asset."
                        : "Assign a uGUI prefab to backendAsset.asset.", screenId));
            }
            else if (backend == UIRenderBackend.UIToolkit && !(asset is VisualTreeAsset))
            {
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "backend-type-mismatch",
                    $"UI Toolkit backend requires a VisualTreeAsset but '{asset.name}' is {asset.GetType().Name}.",
                    "Assign a UXML VisualTreeAsset.", screenId));
            }
            else if (backend == UIRenderBackend.UGUI && !(asset is GameObject))
            {
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "backend-type-mismatch",
                    $"uGUI backend requires a GameObject prefab but '{asset.name}' is {asset.GetType().Name}.",
                    "Assign a uGUI prefab.", screenId));
            }
        }

        private static void ValidateElements(UIScreenDefinition screen, DesignerMetadataAsset metadata, string screenId,
            HashSet<string> backendNames, List<DesignerValidationIssue> issues)
        {
            var backend = screen.backendAsset.backend;
            var ids = new HashSet<string>();
            foreach (var element in metadata.elements)
            {
                if (element == null) continue;
                var id = element.elementId;

                if (string.IsNullOrEmpty(id))
                {
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "empty-element-id",
                        "An element has an empty id.", "Give every element a unique elementId.", screenId));
                    continue;
                }

                if (!ids.Add(id))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "duplicate-element-id",
                        $"Element id '{id}' is used more than once.", "Rename one of the duplicates.", screenId, id));

                if (!DesignerMetadataUtility.IsValidElementId(id))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "invalid-element-id",
                        $"Element id '{id}' is not a safe identifier.",
                        "Use letters, digits, '_' or '-' and start with a letter/underscore.", screenId, id));

                if (backendNames != null && !backendNames.Contains(id))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "missing-backend-element",
                        $"No element named '{id}' exists in the backend asset.",
                        backend == UIRenderBackend.UIToolkit
                            ? "Add name=\"" + id + "\" in UI Builder, or save to create it (uGUI)."
                            : "Save the screen to create the GameObject, or rename to match.", screenId, id));

                ValidateElementDetails(element, screenId, issues);
            }
        }

        private static void ValidateElementDetails(DesignerElementMetadata element, string screenId, List<DesignerValidationIssue> issues)
        {
            var id = element.elementId;
            var type = element.elementType ?? "Panel";
            bool isButton = Is(type, "Button") || Is(type, "IconButton");

            if (isButton && (element.binding == null || string.IsNullOrEmpty(element.binding.commandKey)))
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "button-without-command",
                    $"{type} '{id}' has no command key.", "Set a Command Key in the Binding inspector.", screenId, id));

            if (isButton && string.IsNullOrEmpty(element.text) &&
                (element.binding == null || string.IsNullOrEmpty(element.binding.textKey)))
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "button-without-text",
                    $"{type} '{id}' has neither text nor a text binding.", "Set text or a Text Key.", screenId, id));

            if (element.rect.width < 32f || element.rect.height < 32f)
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "small-touch-target",
                    $"'{id}' is {element.rect.width:0}x{element.rect.height:0}; below the 32x32 minimum touch target.",
                    "Increase width/height to at least 32x32.", screenId, id));

            if (element.hiddenInDesigner && element.binding != null &&
                (!string.IsNullOrEmpty(element.binding.commandKey) || !string.IsNullOrEmpty(element.binding.interactableKey)))
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "hidden-but-interactive",
                    $"'{id}' is hidden in designer yet declares interactive bindings.",
                    "Unhide it, or remove the command/interactable binding.", screenId, id));
        }

        /// <summary>
        /// Parent/child hierarchy integrity: missing/self/cyclic parents, leaf types holding
        /// children, and excessive nesting depth. The source of truth is parentId + siblingIndex.
        /// </summary>
        private static void ValidateHierarchy(DesignerMetadataAsset metadata, string screenId, List<DesignerValidationIssue> issues)
        {
            const int MaxDepth = 20;
            foreach (var element in metadata.elements)
            {
                if (element == null || string.IsNullOrEmpty(element.elementId)) continue;
                var id = element.elementId;

                if (!string.IsNullOrEmpty(element.parentId))
                {
                    if (element.parentId == id)
                    {
                        issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "self-parent",
                            $"'{id}' is its own parent.", "Move it to root or set a different parent.", screenId, id));
                    }
                    else if (metadata.Find(element.parentId) == null)
                    {
                        issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "missing-parent",
                            $"'{id}' references parent '{element.parentId}' which does not exist.",
                            "Move it to root or repoint it at an existing element.", screenId, id));
                    }
                    else if (DesignerHierarchyUtility.IsDescendant(metadata, element.parentId, id))
                    {
                        issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "circular-parent",
                            $"'{id}' is part of a circular parent chain via '{element.parentId}'.",
                            "Move one node in the cycle to root.", screenId, id));
                        continue; // depth walk below would loop
                    }
                }

                // Leaf-type element holding children ⇒ warn (allowed, but usually unintended).
                if (!DesignerComponentRegistry.CanHaveChildren(element.elementType) &&
                    DesignerHierarchyUtility.CountChildren(metadata, element) > 0)
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "leaf-with-children",
                        $"'{id}' is a {element.elementType} (a leaf type) but has children.",
                        "Wrap the children in a Panel/Container, or change this element's type.", screenId, id));

                // Binding key set on a channel the component doesn't support ⇒ warn (never deleted;
                // shown so the user can move it to the Advanced/Legacy area or remove it).
                ValidateBindingSupport(element, screenId, issues);

                if (DesignerHierarchyUtility.GetDepth(metadata, element) > MaxDepth)
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "excessive-depth",
                        $"'{id}' is nested deeper than {MaxDepth} levels.",
                        "Flatten the hierarchy to keep layout predictable.", screenId, id));

                // Slot integrity: a non-default parentSlotId must name a real slot on the parent's
                // descriptor, and template slots hold at most one child.
                if (!string.IsNullOrEmpty(element.parentId) && !string.IsNullOrEmpty(element.parentSlotId) &&
                    element.parentSlotId != DesignerComponentSlot.Content)
                {
                    var parent = metadata.Find(element.parentId);
                    if (parent != null)
                    {
                        var parentDesc = DesignerComponentRegistry.Get(parent.elementType);
                        if (!parentDesc.IsGeneric && parentDesc.GetSlot(element.parentSlotId) == null)
                            issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "invalid-slot",
                                $"'{id}' targets slot '{element.parentSlotId}' which '{parent.elementId}' ({parentDesc.DisplayName}) does not have.",
                                "Move it to a valid slot or the content slot.", screenId, id));
                    }
                }
            }

            // Template slots must not contain more than one authored child.
            foreach (var parent in metadata.elements)
            {
                if (parent == null || string.IsNullOrEmpty(parent.elementId)) continue;
                var desc = DesignerComponentRegistry.Get(parent.elementType);
                foreach (var slot in desc.Slots)
                {
                    if (!slot.IsTemplateSlot) continue;
                    var count = 0;
                    foreach (var child in metadata.elements)
                        if (child != null && child.parentId == parent.elementId &&
                            (child.parentSlotId ?? DesignerComponentSlot.Content) == slot.SlotId)
                            count++;
                    if (count > 1)
                        issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "template-slot-multiple",
                            $"'{parent.elementId}' template slot '{slot.SlotId}' has {count} children; only the first is used as the item template.",
                            "Keep a single element in the template slot.", screenId, parent.elementId));
                }
            }
        }

        /// <summary>
        /// Flags binding keys set on channels the element's component descriptor does not support.
        /// Reported as info (not error) and never mutated - the value stays in the data so it can be
        /// surfaced in the Inspector's Legacy/Unsupported area. Unknown/Generic types support all
        /// channels, so they never trip this.
        /// </summary>
        private static void ValidateBindingSupport(DesignerElementMetadata element, string screenId, List<DesignerValidationIssue> issues)
        {
            var b = element.binding;
            if (b == null) return;
            var d = DesignerComponentRegistry.Get(element.elementType);
            var id = element.elementId;

            void Check(string key, DesignerBindingChannel channel, string label)
            {
                if (!string.IsNullOrEmpty(key) && !d.SupportsBinding(channel))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "unsupported-binding",
                        $"'{id}' ({d.DisplayName}) sets a {label} binding, which this component does not use.",
                        "Remove it, or move it to the Advanced/Legacy bindings area.", screenId, id));
            }

            Check(b.textKey, DesignerBindingChannel.Text, "text");
            Check(b.valueKey, DesignerBindingChannel.Value, "value");
            Check(b.commandKey, DesignerBindingChannel.Command, "command");
            Check(b.interactableKey, DesignerBindingChannel.Interactable, "interactable");
        }

        private static void ValidateOrphans(DesignerMetadataAsset metadata, string screenId,
            HashSet<string> backendNames, List<DesignerValidationIssue> issues)
        {
            if (backendNames == null) return;
            var metaIds = new HashSet<string>();
            foreach (var e in metadata.elements)
                if (e != null && !string.IsNullOrEmpty(e.elementId)) metaIds.Add(e.elementId);

            foreach (var name in backendNames)
                if (!metaIds.Contains(name))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "orphan-backend-element",
                        $"Backend element '{name}' has no Designer metadata.",
                        "Use 'Sync Metadata From Backend' or ignore if it is decorative.", screenId, name));
        }

        private static void ValidateReferences(DesignerMetadataAsset metadata, string screenId, List<DesignerValidationIssue> issues)
        {
            var ids = new HashSet<string>();
            foreach (var e in metadata.elements)
                if (e != null && !string.IsNullOrEmpty(e.elementId)) ids.Add(e.elementId);

            if (metadata.localization != null)
                foreach (var link in metadata.localization.links)
                {
                    if (link == null) continue;
                    if (!string.IsNullOrEmpty(link.elementId) && !ids.Contains(link.elementId))
                        issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "localization-target-missing",
                            $"Localization link targets missing element '{link.elementId}'.",
                            "Point the link at an existing element or remove it.", screenId, link.elementId));
                    if (!string.IsNullOrEmpty(link.elementId) && string.IsNullOrEmpty(link.localizationKey))
                        issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "localization-key-missing",
                            $"Element '{link.elementId}' has a localization link with no key.",
                            "Set a localization key or remove the link.", screenId, link.elementId));
                }

            if (metadata.variants != null)
                foreach (var v in metadata.variants)
                {
                    if (v == null) continue;
                    foreach (var ov in v.overrides)
                        if (ov != null && !string.IsNullOrEmpty(ov.targetElementId) && !ids.Contains(ov.targetElementId))
                            issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "variant-target-missing",
                                $"Variant '{v.variantId}' overrides missing element '{ov.targetElementId}'.",
                                "Fix the target elementId or remove the override.", screenId, ov.targetElementId));
                }

            if (metadata.responsiveRules != null)
                foreach (var r in metadata.responsiveRules)
                {
                    if (r == null) continue;
                    foreach (var ov in r.overrides)
                        if (ov != null && !string.IsNullOrEmpty(ov.elementId) && !ids.Contains(ov.elementId))
                            issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "responsive-target-missing",
                                $"Responsive rule '{r.ruleId}' overrides missing element '{ov.elementId}'.",
                                "Fix the target elementId or remove the override.", screenId, ov.elementId));
                }
        }

        private static void ValidateMotion(DesignerMetadataAsset metadata, string screenId, List<DesignerValidationIssue> issues)
        {
            var motion = metadata.screenMotion;
            if (motion == null) return;
            var ids = new HashSet<string>();
            foreach (var element in metadata.elements)
                if (element != null && !string.IsNullOrEmpty(element.elementId)) ids.Add(element.elementId);

            var validatedClips = new HashSet<UIMotionClip>();
            ValidateClip(motion.entryClip, screenId, issues, validatedClips);
            ValidateClip(motion.exitClip, screenId, issues, validatedClips);
            foreach (var binding in motion.bindings ?? new List<DesignerMotionBinding>())
            {
                if (binding == null) continue;
                var isScreenTrigger = binding.trigger == DesignerMotionTrigger.ScreenEnter || binding.trigger == DesignerMotionTrigger.ScreenExit;
                if (!isScreenTrigger && (string.IsNullOrEmpty(binding.targetElementId) || !ids.Contains(binding.targetElementId)))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "motion-target-missing",
                        $"Motion binding '{binding.bindingId}' targets missing element '{binding.targetElementId}'.",
                        "Choose an existing element or remove the binding.", screenId, binding.targetElementId));
                if (isScreenTrigger && !string.IsNullOrEmpty(binding.targetElementId))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "screen-motion-has-target",
                        $"Screen trigger '{binding.trigger}' is connected to element '{binding.targetElementId}'.",
                        "Clear the target or use an element trigger.", screenId, binding.targetElementId));
                if (binding.clip == null)
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "motion-clip-missing",
                        $"Motion binding '{binding.bindingId}' has no clip or its asset is missing.",
                        "Assign an existing UIMotionClip asset.", screenId, binding.targetElementId));
                if ((binding.trigger == DesignerMotionTrigger.StateEnter || binding.trigger == DesignerMotionTrigger.StateExit) && string.IsNullOrEmpty(binding.stateId))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Error, "motion-state-id-missing",
                        $"Motion binding '{binding.bindingId}' requires a state id.", "Set a valid State Id.", screenId, binding.targetElementId));
                if ((binding.trigger == DesignerMotionTrigger.CommandStarted || binding.trigger == DesignerMotionTrigger.CommandCompleted || binding.trigger == DesignerMotionTrigger.CommandFailed) && string.IsNullOrEmpty(binding.commandId))
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "motion-command-id-missing",
                        $"Motion binding '{binding.bindingId}' requires a command id.", "Set a valid Command Id.", screenId, binding.targetElementId));
                ValidateClip(binding.clip, screenId, issues, validatedClips);
                ValidateClip(binding.reducedMotionClip, screenId, issues, validatedClips);
            }
        }

        private static void ValidateClip(UIMotionClip clip, string screenId, List<DesignerValidationIssue> issues, HashSet<UIMotionClip> validated)
        {
            if (clip == null || !validated.Add(clip)) return;
            var targets = new HashSet<string>();
            foreach (var track in clip.tracks ?? System.Array.Empty<UIMotionClipTrack>())
            {
                if (track == null) continue;
                if (!targets.Add(track.targetElementId ?? string.Empty))
                    AddClipIssue(DesignerValidationSeverity.Error, "motion-duplicate-track-target",
                        $"Clip '{clip.name}' contains duplicate track target '{track.targetElementId}'.",
                        "Merge properties into one target track.", clip, screenId, issues);
                foreach (var propertyTrack in track.propertyTracks ?? System.Array.Empty<UIMotionClipPropertyTrack>())
                {
                    if (propertyTrack?.keyframes == null) continue;
                    var previous = float.NegativeInfinity;
                    foreach (var keyframe in propertyTrack.keyframes)
                    {
                        if (keyframe.time < 0f)
                            AddClipIssue(DesignerValidationSeverity.Error, "motion-negative-keyframe",
                                $"Clip '{clip.name}' has a keyframe at negative time {keyframe.time:0.###}.", "Move it to time 0 or later.", clip, screenId, issues);
                        if (keyframe.time > clip.duration)
                            AddClipIssue(DesignerValidationSeverity.Error, "motion-keyframe-after-duration",
                                $"Clip '{clip.name}' has a keyframe at {keyframe.time:0.###}, after duration {clip.duration:0.###}.", "Extend duration or move the keyframe.", clip, screenId, issues);
                        if (keyframe.time < previous)
                            AddClipIssue(DesignerValidationSeverity.Error, "motion-keyframes-unsorted",
                                $"Clip '{clip.name}' has unsorted keyframes in {propertyTrack.propertyType}.", "Sort keyframes by time.", clip, screenId, issues);
                        previous = keyframe.time;
                    }
                }
            }
        }

        private static void AddClipIssue(DesignerValidationSeverity severity, string code, string message, string fix,
            UIMotionClip clip, string screenId, List<DesignerValidationIssue> issues)
        {
            var issue = new DesignerValidationIssue(severity, code, message, fix, screenId) { Asset = clip };
            issues.Add(issue);
        }

        // ---- Backend-asset inspection ------------------------------------------------

        private static HashSet<string> CollectBackendElementNames(UIScreenDefinition screen)
        {
            var asset = screen.backendAsset.asset;
            switch (screen.backendAsset.backend)
            {
                case UIRenderBackend.UIToolkit:
                    return asset is VisualTreeAsset vta ? UIToolkitAssetSerializer.CollectElementNames(vta) : null;
                case UIRenderBackend.UGUI:
                    return asset is GameObject go ? CollectPrefabNames(go) : null;
                default:
                    return null;
            }
        }

        private static HashSet<string> CollectPrefabNames(GameObject prefab)
        {
            var names = new HashSet<string>();
            foreach (var t in prefab.GetComponentsInChildren<Transform>(true))
                names.Add(t.name);
            return names;
        }

        private static void ValidatePrefabComponents(UIScreenDefinition screen, DesignerMetadataAsset metadata,
            string screenId, List<DesignerValidationIssue> issues)
        {
            if (screen.backendAsset.backend != UIRenderBackend.UGUI) return;
            if (!(screen.backendAsset.asset is GameObject prefab)) return;

            // Duplicate GameObject names make name-based prefab matching unpredictable.
            var seen = new HashSet<string>();
            var dupes = new HashSet<string>();
            foreach (var t in prefab.GetComponentsInChildren<Transform>(true))
                if (!seen.Add(t.name)) dupes.Add(t.name);
            foreach (var name in dupes)
                issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "duplicate-gameobject-name",
                    $"Prefab contains multiple GameObjects named '{name}'.",
                    "Rename duplicates so element matching stays reliable.", screenId, name));

            foreach (var element in metadata.elements)
            {
                if (element == null || string.IsNullOrEmpty(element.elementId)) continue;
                var child = FindChild(prefab.transform, element.elementId);
                if (child == null) continue; // missing-backend-element already reported.

                var type = element.elementType ?? "Panel";
                var go = child.gameObject;

                if ((Is(type, "Button") || Is(type, "IconButton")) && go.GetComponent<UnityEngine.UI.Button>() == null)
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "ugui-missing-button",
                        $"'{element.elementId}' is a {type} but has no Button component.",
                        "Save the screen to add one, or add Button manually.", screenId, element.elementId));

                if ((Is(type, "Label") || Is(type, "Toast") || Is(type, "Tooltip")) &&
                    go.GetComponentInChildren<TMP_Text>(true) == null && go.GetComponentInChildren<Text>(true) == null)
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "ugui-missing-text",
                        $"'{element.elementId}' is a {type} but has no TMP_Text/Text component.",
                        "Save the screen to add text, or add a text component manually.", screenId, element.elementId));

                if (Is(type, "Image") && go.GetComponent<Graphic>() == null)
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Warning, "ugui-missing-graphic",
                        $"'{element.elementId}' is an Image but has no Graphic/Image component.",
                        "Save the screen to add an Image, or add one manually.", screenId, element.elementId));

                if (Is(type, "Modal") && go.GetComponent<CanvasGroup>() == null)
                    issues.Add(new DesignerValidationIssue(DesignerValidationSeverity.Info, "ugui-modal-without-canvasgroup",
                        $"Modal '{element.elementId}' has no CanvasGroup.",
                        "Add a CanvasGroup so the modal can fade / block input.", screenId, element.elementId));
            }
        }

        private static Transform FindChild(Transform root, string name)
        {
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                var found = FindChild(root.GetChild(i), name);
                if (found != null) return found;
            }
            return null;
        }

        private static bool Is(string type, string other)
            => string.Equals(type, other, System.StringComparison.OrdinalIgnoreCase);
    }
}
