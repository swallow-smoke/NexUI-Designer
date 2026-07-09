using System;
using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    /// <summary>
    /// Thin, correct wrappers over Unity's Undo API for the designer. Prefer
    /// <see cref="Group"/> to collapse a multi-step edit into a single undo entry.
    /// </summary>
    public static class NexUIDesignerUndo
    {
        public static void Record(UnityEngine.Object target, string action)
        {
            if (target != null)
                Undo.RecordObject(target, action);
        }

        public static void Record(UnityEngine.Object[] targets, string action)
        {
            if (targets != null && targets.Length > 0)
                Undo.RecordObjects(targets, action);
        }

        public static void RegisterCreated(UnityEngine.Object target, string action)
        {
            if (target != null)
                Undo.RegisterCreatedObjectUndo(target, action);
        }

        /// <summary>Undo-tracked reparent for GameObjects.</summary>
        public static void SetTransformParent(Transform child, Transform parent, string action)
        {
            if (child != null)
                Undo.SetTransformParent(child, parent, action);
        }

        public static void Destroy(UnityEngine.Object target)
        {
            if (target != null)
                Undo.DestroyObjectImmediate(target);
        }

        /// <summary>
        /// Runs <paramref name="body"/> as a single collapsed undo group. Any Undo calls made
        /// inside are merged so one Ctrl+Z reverts the whole operation.
        /// </summary>
        public static void Group(string name, Action body)
        {
            if (body == null) return;
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(name);
            try
            {
                body();
            }
            finally
            {
                Undo.CollapseUndoOperations(group);
            }
        }
    }
}
