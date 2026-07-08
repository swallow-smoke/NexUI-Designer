using UnityEditor;
using UnityEngine;

namespace emiteat.NexUI.Designer.Editor
{
    public static class NexUIDesignerUndo
    {
        public static void Record(Object target, string action)
        {
            if (target != null)
                Undo.RecordObject(target, action);
        }

        public static void RegisterCreated(Object target, string action)
        {
            if (target != null)
                Undo.RegisterCreatedObjectUndo(target, action);
        }

        public static void Destroy(Object target)
        {
            if (target != null)
                Undo.DestroyObjectImmediate(target);
        }
    }
}
