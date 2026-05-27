using UnityEngine;

namespace EditorAttributes
{
    public class HideInEditModeAttribute : PropertyAttribute 
    {
        /// <summary>
        /// Attribute to hide a field when outside of play mode
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        public HideInEditModeAttribute() : base(true)
        { }
#else
        public HideInEditModeAttribute()
        { }
#endif
    }
}