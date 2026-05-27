using UnityEngine;

namespace EditorAttributes
{
    public class DisableInPlayModeAttribute : PropertyAttribute 
    {
        /// <summary>
        /// Attribute to disable a field when entering play mode
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        public DisableInPlayModeAttribute() : base(true)
        { }
#else
        public DisableInPlayModeAttribute()
        { }
#endif
    }
}