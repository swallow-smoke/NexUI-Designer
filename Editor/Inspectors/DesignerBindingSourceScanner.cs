using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using emiteat.NexUI.State;

namespace emiteat.NexUI.Designer.Editor.Inspectors
{
    /// <summary>
    /// Scans loaded assemblies for public types exposing <see cref="IBindableProperty{T}"/>
    /// members (fields or properties, instance or static) and offers them as pick-list
    /// candidates for <see cref="BindingInspector"/>, so a user can connect a UI element to a
    /// data-source property visually instead of hand-typing a binding key string.
    /// </summary>
    internal static class DesignerBindingSourceScanner
    {
        public readonly struct Candidate
        {
            /// <summary>"TypeName.MemberName" - written into the binding key field.</summary>
            public readonly string Key;

            /// <summary>"Namespace.TypeName/MemberName (ValueType)" - shown in the picker menu.</summary>
            public readonly string DisplayPath;

            /// <summary>The T in IBindableProperty&lt;T&gt;.</summary>
            public readonly Type ValueType;

            public Candidate(string key, string displayPath, Type valueType)
            {
                Key = key;
                DisplayPath = displayPath;
                ValueType = valueType;
            }
        }

        private static readonly Type BindablePropertyInterface = typeof(IBindableProperty<>);
        private static List<Candidate> _cache;

        /// <summary>
        /// All discovered candidates, optionally filtered to a value type assignable to
        /// <paramref name="valueTypeFilter"/> (e.g. typeof(float) for a Value Key picker).
        /// Pass null to list everything.
        /// </summary>
        public static IReadOnlyList<Candidate> Find(Type valueTypeFilter = null)
        {
            if (_cache == null) _cache = Scan();
            if (valueTypeFilter == null) return _cache;
            return _cache.Where(c => valueTypeFilter.IsAssignableFrom(c.ValueType)).ToList();
        }

        /// <summary>Drops the cached scan. Call if bindable source types were added after the first scan (e.g. script recompile already triggers this via domain reload).</summary>
        public static void Invalidate() => _cache = null;

        private static List<Candidate> Scan()
        {
            var results = new List<Candidate>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }

                foreach (var type in types)
                {
                    if (type == null || !type.IsClass) continue;

                    foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                        TryAdd(results, type, field.FieldType, field.Name);

                    foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                    {
                        if (prop.GetIndexParameters().Length > 0) continue;
                        TryAdd(results, type, prop.PropertyType, prop.Name);
                    }
                }
            }

            return results;
        }

        private static void TryAdd(List<Candidate> list, Type owner, Type memberType, string memberName)
        {
            var iface = FindBindablePropertyInterface(memberType);
            if (iface == null) return;

            var valueType = iface.GetGenericArguments()[0];
            var key = $"{owner.Name}.{memberName}";
            var displayPath = $"{owner.Namespace}.{owner.Name}/{memberName} ({valueType.Name})";
            list.Add(new Candidate(key, displayPath, valueType));
        }

        private static Type FindBindablePropertyInterface(Type memberType)
        {
            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == BindablePropertyInterface)
                return memberType;

            return Array.Find(memberType.GetInterfaces(),
                i => i.IsGenericType && i.GetGenericTypeDefinition() == BindablePropertyInterface);
        }
    }
}
