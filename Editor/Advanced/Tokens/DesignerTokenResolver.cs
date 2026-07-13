using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Tokens
{
    /// <summary>A token validation finding.</summary>
    public struct DesignerTokenIssue
    {
        public enum Severity { Info, Warning, Error }
        public Severity Level;
        public string TokenName;
        public string Message;
    }

    /// <summary>
    /// Pure resolution + validation of a <see cref="DesignerTokenSetAsset"/> (brief §33). Resolution
    /// follows <see cref="DesignerToken.reference"/> aliases to the final literal value, guarding
    /// against reference cycles. No Unity Editor dependency, so the whole ruleset is unit-tested.
    /// </summary>
    public static class DesignerTokenResolver
    {
        /// <summary>Resolves a token to its final literal token (following references). Returns null if
        /// the name is missing or a reference is broken/cyclic.</summary>
        public static DesignerToken Resolve(IReadOnlyList<DesignerToken> tokens, string name)
        {
            var byName = Index(tokens);
            return ResolveInternal(byName, name, new HashSet<string>());
        }

        public static bool TryResolveColor(IReadOnlyList<DesignerToken> tokens, string name, out Color color)
        {
            color = Color.white;
            var resolved = Resolve(tokens, name);
            if (resolved == null) return false;
            color = resolved.colorValue;
            return true;
        }

        public static bool TryResolveNumber(IReadOnlyList<DesignerToken> tokens, string name, out float value)
        {
            value = 0f;
            var resolved = Resolve(tokens, name);
            if (resolved == null) return false;
            value = resolved.numberValue;
            return true;
        }

        private static DesignerToken ResolveInternal(Dictionary<string, DesignerToken> byName, string name, HashSet<string> visiting)
        {
            if (string.IsNullOrEmpty(name) || !byName.TryGetValue(name, out var token)) return null;
            if (!token.IsReference) return token;
            if (!visiting.Add(name)) return null; // cycle
            var result = ResolveInternal(byName, token.reference, visiting);
            visiting.Remove(name);
            return result;
        }

        // ---- Validation ---------------------------------------------------------

        public static List<DesignerTokenIssue> Validate(DesignerTokenSetAsset asset)
        {
            var issues = new List<DesignerTokenIssue>();
            if (asset == null) return issues;

            var byName = new Dictionary<string, DesignerToken>();
            foreach (var token in asset.tokens)
            {
                if (token == null) continue;
                if (string.IsNullOrEmpty(token.name))
                {
                    issues.Add(Warn(null, "Token with empty name."));
                    continue;
                }
                if (byName.ContainsKey(token.name))
                    issues.Add(Error(token.name, $"Duplicate token name '{token.name}'."));
                else
                    byName[token.name] = token;
            }

            foreach (var token in byName.Values)
            {
                if (!token.IsReference) continue;

                if (!byName.TryGetValue(token.reference, out var target))
                {
                    issues.Add(Error(token.name, $"Token '{token.name}' references missing token '{token.reference}'."));
                    continue;
                }
                if (target.category != token.category)
                    issues.Add(Warn(token.name, $"Token '{token.name}' references '{token.reference}' of a different category ({target.category})."));

                if (ResolveInternal(byName, token.name, new HashSet<string>()) == null)
                    issues.Add(Error(token.name, $"Token '{token.name}' is part of a reference cycle."));
            }

            return issues;
        }

        private static Dictionary<string, DesignerToken> Index(IReadOnlyList<DesignerToken> tokens)
        {
            var byName = new Dictionary<string, DesignerToken>();
            if (tokens == null) return byName;
            foreach (var token in tokens)
                if (token != null && !string.IsNullOrEmpty(token.name) && !byName.ContainsKey(token.name))
                    byName[token.name] = token;
            return byName;
        }

        private static DesignerTokenIssue Warn(string name, string message) => new DesignerTokenIssue { Level = DesignerTokenIssue.Severity.Warning, TokenName = name, Message = message };
        private static DesignerTokenIssue Error(string name, string message) => new DesignerTokenIssue { Level = DesignerTokenIssue.Severity.Error, TokenName = name, Message = message };
    }
}
