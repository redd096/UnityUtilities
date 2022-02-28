using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfDrawer : PropertyDrawer
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
            //disable GUI for this property if CanDisable is true
            GUI.enabled = CanDisable(property) == false;

            EditorGUI.PropertyField(position, property, label, true);

            //re-enable GUI
            GUI.enabled = true;
        }

        #region private API

        bool CanDisable(SerializedProperty property)
        {
            DisableIfAttribute at = attribute as DisableIfAttribute;

            //compare property
            if (at.comparing)
            {
                SerializedProperty comparedProperty = property.FindCorrectProperty(at.propertyA);

                //compare property based on propertyType
                if (comparedProperty.propertyType == SerializedPropertyType.Boolean)
                    return CompareEqual(at, comparedProperty.boolValue);
                else if (comparedProperty.propertyType == SerializedPropertyType.Enum)
                    return CompareEqual(at, comparedProperty.enumValueIndex);
                else if (comparedProperty.propertyType == SerializedPropertyType.Integer)
                    return CompareInt(at, comparedProperty.intValue);
                else if (comparedProperty.propertyType == SerializedPropertyType.Float)
                    return CompareFloat(at, comparedProperty.floatValue);
                else if (comparedProperty.propertyType == SerializedPropertyType.String)
                    return CompareEqual(at, comparedProperty.stringValue);

                return false;
            }
            //or normally check every value
            else
            {
                return CheckValues(at, property);
            }
        }

        bool CheckValues(DisableIfAttribute at, SerializedProperty property)
        {
            //foreach value to check
            foreach (string value in at.values)
            {
                bool check = property.FindCorrectProperty(value).boolValue;

                //if check AND, when find one false, return false
                if (at.conditionOperator == DisableIfAttribute.EConditionOperator.AND && check == false)
                {
                    return false;
                }
                //if check OR, when find one true, return true
                else if (at.conditionOperator == DisableIfAttribute.EConditionOperator.OR && check)
                {
                    return true;
                }
            }

            //else return if check is AND, cause it mean every value is true if check AND, or every value is false if check OR
            return at.conditionOperator == DisableIfAttribute.EConditionOperator.AND;
        }

        bool CompareEqual<T>(DisableIfAttribute at, T comparedProperty)
        {
            //equal
            if (at.comparisonType == DisableIfAttribute.EComparisonType.isEqual)
                return comparedProperty.Equals((T)at.valueToCompare);
            //not equal
            else if (at.comparisonType == DisableIfAttribute.EComparisonType.isNotEqual)
                return comparedProperty.Equals((T)at.valueToCompare) == false;
            else
                return false;
        }

        bool CompareInt(DisableIfAttribute at, int comparedProperty)
        {
            //compare property A and value
            switch (at.comparisonType)
            {
                case DisableIfAttribute.EComparisonType.isEqual:
                    return comparedProperty == (int)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (int)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (int)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (int)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (int)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isLowerEqual:
                    return comparedProperty <= (int)at.valueToCompare;
                default:
                    return comparedProperty == (int)at.valueToCompare;
            }
        }

        bool CompareFloat(DisableIfAttribute at, float comparedProperty)
        {
            //compare property A and value
            switch (at.comparisonType)
            {
                case DisableIfAttribute.EComparisonType.isEqual:
                    return Mathf.Abs(comparedProperty - (float)at.valueToCompare) <= floatingPoint;
                case DisableIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (float)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (float)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (float)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (float)at.valueToCompare;
                case DisableIfAttribute.EComparisonType.isLowerEqual:
                    return comparedProperty <= (float)at.valueToCompare;
                default:
                    return Mathf.Abs(comparedProperty - (float)at.valueToCompare) <= floatingPoint;
            }
        }

        #endregion
    }

#endif

    #endregion

    /// <summary>
    /// Attribute to show variable writable only if condition is false
    /// </summary>
    public class DisableIfAttribute : PropertyAttribute
    {
        public enum EComparisonType { isEqual, isNotEqual, isGreater, isGreaterEqual, isLower, isLowerEqual }
        public enum EConditionOperator { AND, OR }

        public readonly bool comparing;

        //check every value
        public readonly string[] values;
        public readonly EConditionOperator conditionOperator;

        //compare value
        public readonly string propertyA;
        public readonly object valueToCompare;
        public readonly EComparisonType comparisonType;

        public DisableIfAttribute(params string[] values)
        {
            comparing = false;

            //by default condition is AND
            conditionOperator = EConditionOperator.AND;
            this.values = values;
        }

        public DisableIfAttribute(EConditionOperator conditionOperator, params string[] values)
        {
            comparing = false;

            this.conditionOperator = conditionOperator;
            this.values = values;
        }

        public DisableIfAttribute(string propertyA, object valueToCompare, EComparisonType comparisonType = EComparisonType.isEqual)
        {
            comparing = true;

            this.propertyA = propertyA;
            this.valueToCompare = valueToCompare;
            this.comparisonType = comparisonType;
        }
    }
}