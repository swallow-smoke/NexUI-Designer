using UnityEngine;

namespace emiteat.NexUI.Designer.Editor.Graph
{
    public static class GraphLayoutUtility
    {
        public static Vector2 PlaceOnCircle(int index, int count, float radius)
        {
            if (count <= 0) return Vector2.zero;
            var angle = Mathf.PI * 2f * index / count;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }
    }
}
