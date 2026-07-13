using System;
using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer
{
    /// <summary>Design-token category (brief §33/§37.1). Determines which value channel a token uses.</summary>
    public enum DesignerTokenCategory
    {
        Color,
        Spacing,
        Radius,
        Border,
        Shadow,
        MotionDuration,
        MotionEasing,
        Breakpoint,
        ZIndex,
        Typography
    }

    /// <summary>
    /// One design token (brief §33). A token is either a literal value or an <see cref="reference"/> to
    /// another token of the same category (an alias). Color-category tokens carry
    /// <see cref="colorValue"/>; numeric categories carry <see cref="numberValue"/>; the rest carry
    /// <see cref="stringValue"/> (easing name, font family, etc.).
    /// </summary>
    [Serializable]
    public sealed class DesignerToken
    {
        public string name;
        public DesignerTokenCategory category = DesignerTokenCategory.Color;

        /// <summary>Name of another token to alias, or empty for a literal value.</summary>
        public string reference = string.Empty;

        public Color colorValue = Color.white;
        public float numberValue;
        public string stringValue = string.Empty;

        public DesignerToken() { }

        public DesignerToken(string name, DesignerTokenCategory category)
        {
            this.name = name;
            this.category = category;
        }

        public bool IsReference => !string.IsNullOrEmpty(reference);

        /// <summary>Whether this token's category stores its value in <see cref="numberValue"/>.</summary>
        public bool IsNumeric =>
            category == DesignerTokenCategory.Spacing ||
            category == DesignerTokenCategory.Radius ||
            category == DesignerTokenCategory.Border ||
            category == DesignerTokenCategory.MotionDuration ||
            category == DesignerTokenCategory.Breakpoint ||
            category == DesignerTokenCategory.ZIndex;
    }

    /// <summary>
    /// A named set of design tokens (brief §33/§37) — colors, spacing, radius, motion durations,
    /// easings, etc., with aliasing between tokens. A standalone asset like a Theme; tokens resolve
    /// through <c>DesignerTokenResolver</c>. Referencing screens/elements by token name is a later
    /// integration step.
    /// </summary>
    [CreateAssetMenu(menuName = "NexUI/Designer/Token Set", fileName = "NexUITokenSet")]
    public sealed class DesignerTokenSetAsset : ScriptableObject
    {
        public string setName = "New Token Set";
        public List<DesignerToken> tokens = new List<DesignerToken>();

        public DesignerToken Find(string tokenName)
        {
            if (string.IsNullOrEmpty(tokenName)) return null;
            for (int i = 0; i < tokens.Count; i++)
                if (tokens[i] != null && tokens[i].name == tokenName)
                    return tokens[i];
            return null;
        }
    }
}
