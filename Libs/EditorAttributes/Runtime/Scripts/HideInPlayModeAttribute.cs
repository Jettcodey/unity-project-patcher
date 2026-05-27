using UnityEngine;

namespace EditorAttributes
{
    public class HideInPlayModeAttribute : PropertyAttribute 
    {
        /// <summary>
        /// Attribute to hide a field when entering play mode
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        public HideInPlayModeAttribute() : base(true)
        { }
#else
        public HideInPlayModeAttribute()
        { }
#endif
    }
}