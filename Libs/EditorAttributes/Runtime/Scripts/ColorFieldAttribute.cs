using System;
using UnityEngine;

namespace EditorAttributes
{
    public enum GUIColor
    {
        White,
        Black,
        Gray,
        Red,
        Green,
        Lime,
        Blue,
        Cyan,
        Yellow,
        Orange,
        Brown,
        Magenta,
        Purple,
        Pink
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ColorFieldAttribute : PropertyAttribute, IColorAttribute
    {
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
        
        public bool UseRGB { get; private set; }
        public string HexColor { get; private set; }
        public string ColorFieldName { get; private set; }

        public GUIColor Color { get; private set; }

        /// <summary>
        /// Attribute to color a field in the inspector
        /// </summary>
        /// <param name="fieldColor">The color of the field</param>
#if UNITY_6000_0_OR_NEWER
        public ColorFieldAttribute(GUIColor fieldColor) : base(true)
            => Color = fieldColor;
#else
        public ColorFieldAttribute(GUIColor fieldColor)
            => Color = fieldColor;
#endif

        /// <summary>
        /// Attribute to color a field in the inspector
        /// </summary>
        /// <param name="r">Red amount</param>
        /// <param name="g">Green amount</param>
        /// <param name="b">Blue amount</param>
#if UNITY_6000_0_OR_NEWER
        public ColorFieldAttribute(float r, float g, float b) : base(true)
        {
            UseRGB = true;
            R = r;
            G = g;
            B = b;
        }
#else
        public ColorFieldAttribute(float r, float g, float b)
        {
            UseRGB = true;
            R = r;
            G = g;
            B = b;
        }
#endif

        /// <summary>
        /// Attribute to color a field in the inspector
        /// </summary>
        /// <param name="hexColor">The color in hexadecimal</param>
#if UNITY_6000_0_OR_NEWER
        public ColorFieldAttribute(string hexColor) : base(true)
            => HexColor = hexColor;
#else
        public ColorFieldAttribute(string hexColor)
            => HexColor = hexColor;
#endif
    }
}