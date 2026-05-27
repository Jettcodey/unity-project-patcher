using UnityEngine;

namespace EditorAttributes
{
    public class DisableInEditModeAttribute : PropertyAttribute 
    {
        /// <summary>
        /// Attribute to disable a field when outside of play mode
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        public DisableInEditModeAttribute() : base(true)
        { }
#else
        public DisableInEditModeAttribute()
        { }
#endif
    }
}