using UnityEngine;

namespace EditorAttributes
{
    public class ReadOnlyAttribute : PropertyAttribute 
    {
        /// <summary>
        /// Attribute to make a field readonly in the inspector
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        public ReadOnlyAttribute() : base(true)
        { }
#else
        public ReadOnlyAttribute()
        { }
#endif
    }
}