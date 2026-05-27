using UnityEngine;

namespace EditorAttributes
{
    public class HideFieldAttribute : PropertyAttribute, IConditionalAttribute
    {
        public string ConditionName { get; private set; }
        public int EnumValue { get; private set; }

        /// <summary>
        /// Attribute to hide fields based on a condition
        /// </summary>
        /// <param name="conditionName">The name of the condition to evaluate</param>
#if UNITY_6000_0_OR_NEWER
        public HideFieldAttribute(string conditionName) : base(true)
            => ConditionName = conditionName;
#else
        public HideFieldAttribute(string conditionName)
            => ConditionName = conditionName;
#endif

        /// <summary>
        /// Attribute to hide fields based on a condition
        /// </summary>
        /// <param name="conditionName">The name of the condition to evaluate</param>
        /// <param name="enumValue">The value of the enum</param>
#if UNITY_6000_0_OR_NEWER
        public HideFieldAttribute(string conditionName, object enumValue) : base(true)
        {
            ConditionName = conditionName;
            EnumValue = (int)enumValue;
        }
#else
        public HideFieldAttribute(string conditionName, object enumValue)
        {
            ConditionName = conditionName;
            EnumValue = (int)enumValue;
        }
#endif
    }
}