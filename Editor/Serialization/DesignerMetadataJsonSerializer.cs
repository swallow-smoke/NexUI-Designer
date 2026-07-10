using System;
using System.Collections.Generic;
using System.IO;
using emiteat.NexUI.Accessibility;
using emiteat.NexUI.Motion;
using emiteat.NexUI.Theme;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Serialization
{
    /// <summary>
    /// Companion git-friendly JSON export/import for <see cref="DesignerMetadataAsset"/> (B8).
    /// The <c>.asset</c> stays Unity's own YAML - this only adds a <c>.json</c> file written
    /// next to it that mirrors the same data through DTOs with a fixed, declaration-order field
    /// layout (so JsonUtility produces the same byte-for-byte output for the same data every
    /// time - a real diff/merge tool, unlike Unity's YAML). <see cref="UnityEngine.Object"/>
    /// references (motion preset, theme) are written as persistent asset GUIDs, never raw
    /// instance IDs, so the JSON stays valid across sessions and machines.
    ///
    /// Treat the <c>.json</c> file as the thing to diff/review in a PR; use
    /// <see cref="Import"/> ("Sync from JSON" in the Designer) after resolving a merge conflict
    /// in the JSON to push the merged result back into the <c>.asset</c>.
    /// </summary>
    public static class DesignerMetadataJsonSerializer
    {
        public static string CompanionPathFor(DesignerMetadataAsset asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(assetPath)) return null;
            return Path.ChangeExtension(assetPath, null) + ".json";
        }

        /// <summary>Writes the companion JSON next to the asset. Returns the written path, or null if the asset isn't saved to disk yet.</summary>
        public static string Export(DesignerMetadataAsset asset)
        {
            if (asset == null) return null;
            var path = CompanionPathFor(asset);
            if (string.IsNullOrEmpty(path)) return null;

            var dto = ToDto(asset);
            var json = JsonUtility.ToJson(dto, true);
            File.WriteAllText(path, json + "\n");
            AssetDatabase.ImportAsset(path);
            return path;
        }

        /// <summary>Applies a companion JSON file's contents onto <paramref name="asset"/> (Undo-tracked). Returns false if the file is missing or fails to parse.</summary>
        public static bool Import(DesignerMetadataAsset asset)
        {
            if (asset == null) return false;
            var path = CompanionPathFor(asset);
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return false;

            MetadataFileDto dto;
            try { dto = JsonUtility.FromJson<MetadataFileDto>(File.ReadAllText(path)); }
            catch (Exception ex)
            {
                Debug.LogError($"[NexUI] Failed to parse '{path}': {ex.Message}");
                return false;
            }
            if (dto == null) return false;

            Undo.RecordObject(asset, "Sync NexUI Metadata From JSON");
            ApplyDto(dto, asset);
            EditorUtility.SetDirty(asset);
            return true;
        }

        // ---- DTOs: fixed field order, no UnityEngine.Object references -----------------------

        [Serializable]
        private sealed class MetadataFileDto
        {
            public string screenId;
            public List<ElementDto> elements = new();
        }

        [Serializable]
        private sealed class ElementDto
        {
            public string elementId;
            public string parentId;
            public string displayName;
            public string elementType;
            public RectDto rect = new();
            public string anchorPreset;
            public string text;
            public ColorDto tint = new();
            public ColorDto textColor = new();
            public int fontSize;
            public List<string> classes = new();
            public BindingDto binding = new();
            public MotionDto motion = new();
            public ThemeDto theme = new();
            public bool locked;
            public bool hiddenInDesigner;
            public string accessibilityLabel;
            public string accessibilityRole;
        }

        [Serializable]
        private sealed class RectDto
        {
            public float x, y, width, height;
        }

        [Serializable]
        private sealed class ColorDto
        {
            public float r, g, b, a;
        }

        [Serializable]
        private sealed class BindingDto
        {
            public string textKey, valueKey, visibilityKey, classKey, commandKey, interactableKey;
        }

        [Serializable]
        private sealed class MotionDto
        {
            /// <summary>Asset GUID of the motion preset, or empty when none - never a raw instance/file ID.</summary>
            public string motionPresetGuid = "";
            public string motionId;
            public string initialVariant;
            public string animateVariant;
        }

        [Serializable]
        private sealed class ThemeDto
        {
            /// <summary>Asset GUID of the theme, or empty when none.</summary>
            public string themeRefGuid = "";
            public string themeId;
            public List<string> classes = new();
            public List<TokenOverrideDto> tokenOverrides = new();
        }

        [Serializable]
        private sealed class TokenOverrideDto
        {
            public string key, value;
        }

        // ---- Mapping ---------------------------------------------------------------------------

        private static MetadataFileDto ToDto(DesignerMetadataAsset asset)
        {
            var dto = new MetadataFileDto { screenId = asset.screenId };
            foreach (var e in asset.elements)
            {
                if (e == null) continue;
                dto.elements.Add(new ElementDto
                {
                    elementId = e.elementId,
                    parentId = e.parentId,
                    displayName = e.displayName,
                    elementType = e.elementType,
                    rect = new RectDto { x = e.rect.x, y = e.rect.y, width = e.rect.width, height = e.rect.height },
                    anchorPreset = e.anchorPreset.ToString(),
                    text = e.text,
                    tint = new ColorDto { r = e.tint.r, g = e.tint.g, b = e.tint.b, a = e.tint.a },
                    textColor = new ColorDto { r = e.textColor.r, g = e.textColor.g, b = e.textColor.b, a = e.textColor.a },
                    fontSize = e.fontSize,
                    classes = new List<string>(e.classes),
                    binding = new BindingDto
                    {
                        textKey = e.binding?.textKey, valueKey = e.binding?.valueKey,
                        visibilityKey = e.binding?.visibilityKey, classKey = e.binding?.classKey,
                        commandKey = e.binding?.commandKey, interactableKey = e.binding?.interactableKey,
                    },
                    motion = new MotionDto
                    {
                        motionPresetGuid = AssetGuid(e.motion?.motionPreset),
                        motionId = e.motion?.motionId, initialVariant = e.motion?.initialVariant,
                        animateVariant = e.motion?.animateVariant,
                    },
                    theme = new ThemeDto
                    {
                        themeRefGuid = AssetGuid(e.theme?.themeRef),
                        themeId = e.theme?.themeId,
                        classes = e.theme != null ? new List<string>(e.theme.classes) : new List<string>(),
                        tokenOverrides = ToTokenDtos(e.theme?.tokenOverrides),
                    },
                    locked = e.locked,
                    hiddenInDesigner = e.hiddenInDesigner,
                    accessibilityLabel = e.accessibilityLabel,
                    accessibilityRole = e.accessibilityRole.ToString(),
                });
            }
            return dto;
        }

        private static void ApplyDto(MetadataFileDto dto, DesignerMetadataAsset asset)
        {
            asset.screenId = dto.screenId;
            asset.elements.Clear();
            foreach (var d in dto.elements)
            {
                var e = new DesignerElementMetadata
                {
                    elementId = d.elementId,
                    parentId = d.parentId,
                    displayName = d.displayName,
                    elementType = d.elementType,
                    rect = new Rect(d.rect.x, d.rect.y, d.rect.width, d.rect.height),
                    anchorPreset = ParseEnum(d.anchorPreset, DesignerAnchorPreset.TopLeft),
                    text = d.text,
                    tint = new Color(d.tint.r, d.tint.g, d.tint.b, d.tint.a),
                    textColor = new Color(d.textColor.r, d.textColor.g, d.textColor.b, d.textColor.a),
                    fontSize = d.fontSize,
                    locked = d.locked,
                    hiddenInDesigner = d.hiddenInDesigner,
                    accessibilityLabel = d.accessibilityLabel,
                    accessibilityRole = ParseEnum(d.accessibilityRole, AccessibilityRole.None),
                };
                e.classes.AddRange(d.classes ?? new List<string>());

                if (d.binding != null)
                {
                    e.binding.textKey = d.binding.textKey;
                    e.binding.valueKey = d.binding.valueKey;
                    e.binding.visibilityKey = d.binding.visibilityKey;
                    e.binding.classKey = d.binding.classKey;
                    e.binding.commandKey = d.binding.commandKey;
                    e.binding.interactableKey = d.binding.interactableKey;
                }
                if (d.motion != null)
                {
                    e.motion.motionPreset = ResolveAsset<UIMotionPreset>(d.motion.motionPresetGuid);
                    e.motion.motionId = d.motion.motionId;
                    e.motion.initialVariant = d.motion.initialVariant;
                    e.motion.animateVariant = d.motion.animateVariant;
                }
                if (d.theme != null)
                {
                    e.theme.themeRef = ResolveAsset<UITheme>(d.theme.themeRefGuid);
                    e.theme.themeId = d.theme.themeId;
                    e.theme.classes.AddRange(d.theme.classes ?? new List<string>());
                    foreach (var t in d.theme.tokenOverrides ?? new List<TokenOverrideDto>())
                        e.theme.tokenOverrides.Add(new DesignerTokenOverride { key = t.key, value = t.value });
                }

                asset.elements.Add(e);
            }
        }

        private static List<TokenOverrideDto> ToTokenDtos(List<DesignerTokenOverride> overrides)
        {
            var list = new List<TokenOverrideDto>();
            if (overrides == null) return list;
            foreach (var o in overrides)
                list.Add(new TokenOverrideDto { key = o.key, value = o.value });
            return list;
        }

        private static string AssetGuid(UnityEngine.Object obj)
        {
            if (obj == null) return "";
            var path = AssetDatabase.GetAssetPath(obj);
            return string.IsNullOrEmpty(path) ? "" : AssetDatabase.AssetPathToGUID(path);
        }

        private static T ResolveAsset<T>(string guid) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(guid)) return null;
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static TEnum ParseEnum<TEnum>(string value, TEnum fallback) where TEnum : struct
            => !string.IsNullOrEmpty(value) && Enum.TryParse<TEnum>(value, out var parsed) ? parsed : fallback;
    }
}
