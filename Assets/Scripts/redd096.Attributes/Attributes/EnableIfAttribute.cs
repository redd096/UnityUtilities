using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfDrawer : PropertyDrawer
    {
        //used to compare float
        float floatingPoint = 0.05f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //update height when open a list/array/struct in inspector
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //disable GUI for this property if CanEnable is false
            GUI.enabled = CanEnable(property);

            EditorGUI.PropertyField(position, property, label, true);

            //re-enable GUI
            GUI.enabled = true;
        }

        #region private API

        bool CanEnable(SerializedProperty property)
        {
            EnableIfAttribute enableIf = attribute as EnableIfAttribute;

            SerializedProperty comparedProperty = property.FindCorrectProperty(enableIf.propertyA);

            //compare property based on propertyType
            if (comparedProperty.propertyType == SerializedPropertyType.Boolean)
                return CompareBool(enableIf, comparedProperty.boolValue);
            else if (comparedProperty.propertyType == SerializedPropertyType.Enum)
                return CompareInt(enableIf, comparedProperty.enumValueIndex);
            else if (comparedProperty.propertyType == SerializedPropertyType.Integer)
                return CompareInt(enableIf, comparedProperty.intValue);
            else if (comparedProperty.propertyType == SerializedPropertyType.Float)
                return CompareFloat(enableIf, comparedProperty.floatValue);

            return false;
        }

        bool CompareBool(EnableIfAttribute enableIf, bool comparedProperty)
        {
            //equal
            if (enableIf.comparisonType == EnableIfAttribute.EComparisonType.isEqual)
                return comparedProperty == (bool)enableIf.valueToCompare;
            //not equal
            else if (enableIf.comparisonType == EnableIfAttribute.EComparisonType.isNotEqual)
                return comparedProperty != (bool)enableIf.valueToCompare;
            else
                return false;
        }

        bool CompareInt(EnableIfAttribute enableIf, int comparedProperty)
        {
            //compare property A and value
            switch (enableIf.comparisonType)
            {
                case EnableIfAttribute.EComparisonType.isEqual:
                    return comparedProperty == (int)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (int)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (int)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (int)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (int)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isLowerEqual:
                    return comparedProperty <= (int)enableIf.valueToCompare;
                default:
                    return comparedProperty == (int)enableIf.valueToCompare;
            }
        }

        bool CompareFloat(EnableIfAttribute enableIf, float comparedProperty)
        {
            //compare property A and value
            switch (enableIf.comparisonType)
            {
                case EnableIfAttribute.EComparisonType.isEqual:
                    return Mathf.Abs(comparedProperty - (float)enableIf.valueToCompare) <= floatingPoint;
                case EnableIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (float)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (float)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (float)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (float)enableIf.valueToCompare;
                case EnableIfAttribute.EComparisonType.isLowerEqual:
                    return comparedProperty <= (float)enableIf.valueToCompare;
                default:
                    return Mathf.Abs(comparedProperty - (float)enableIf.valueToCompare) <= floatingPoint;
            }
        }

        #endregion
    }

#endif

    #endregion

    //attribute
    public class EnableIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// How to compare variables (default is Equal)
        /// </summary>
        public EComparisonType comparisonType = EComparisonType.isEqual;

        public readonly string propertyA;
        public readonly object valueToCompare;

        /// <summary>
        /// Attribute to show variable writable only if comparison return true
        /// </summary>
        /// <param name="propertyA"></param>
        /// <param name="valueToCompare"></param>
        public EnableIfAttribute(string propertyA, object valueToCompare)
        {
            this.propertyA = propertyA;
            this.valueToCompare = valueToCompare;
        }

        public enum EComparisonType
        {
            isEqual, isNotEqual, isGreater, isGreaterEqual, isLower, isLowerEqual
        }
    }
}