using System;
using System.Collections.Generic;
using UnityEngine;

namespace emiteat.NexUI.Designer
{
    [Serializable]
    public sealed class DesignerElementMetadata
    {
        public string elementId;
        public string parentId;
        public string displayName;
        public string elementType = "Panel";
        public Rect rect = new Rect(64, 64, 240, 96);
        public string text;
        public Color tint = new Color(0.15f, 0.22f, 0.34f, 1f);
        public Color textColor = Color.white;
        public int fontSize = 14;
        public List<string> classes = new List<string>();
        public DesignerBindingMetadata binding = new DesignerBindingMetadata();
        public DesignerMotionMetadata motion = new DesignerMotionMetadata();
        public DesignerThemeMetadata theme = new DesignerThemeMetadata();
        public bool locked;
        public bool hiddenInDesigner;
    }
}
