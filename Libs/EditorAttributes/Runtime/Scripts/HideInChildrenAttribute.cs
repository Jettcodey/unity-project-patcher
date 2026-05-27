using UnityEngine;

namespace EditorAttributes
{
    public class HideInChildrenAttribute : PropertyAttribute 
    {
        /// <summary>
        /// Attribute to hide the inherited field in the child classes
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        public HideInChildrenAttribute() : base(true)
        { }
#else
        public HideInChildrenAttribute()
        { }
#endif
    }
}