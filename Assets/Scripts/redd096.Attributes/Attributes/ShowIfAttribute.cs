using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        //used to compare float
        float floatingPoint = 0.05f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //update height only if can show (to hide space where there are hidden variables) - necessary also to update height when open a list/array/struct in inspector
            if (CanShow(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            //else hide space
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //show only if CanShow return true
            if (CanShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        #region private API

        bool CanShow(SerializedProperty property)
        {
            ShowIfAttribute at = attribute as ShowIfAttribute;

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

        bool CheckValues(ShowIfAttribute at, SerializedProperty property)
        {
            //foreach value to check
            foreach (string value in at.values)
            {
                bool check = property.FindCorrectProperty(value).boolValue;

                //if check AND, when find one false, return false
                if (at.conditionOperator == ShowIfAttribute.EConditionOperator.AND && check == false)
                {
                    return false;
                }
                //if check OR, when find one true, return true
                else if (at.conditionOperator == ShowIfAttribute.EConditionOperator.OR && check)
                {
                    return true;
                }
            }

            //else return if check is AND, cause it mean every value is true if check AND, or every value is false if check OR
            return at.conditionOperator == ShowIfAttribute.EConditionOperator.AND;
        }

        bool CompareEqual<T>(ShowIfAttribute at, T comparedProperty)
        {
            //equal
            if (at.comparisonType == ShowIfAttribute.EComparisonType.isEqual)
                return comparedProperty.Equals((T)at.valueToCompare);
            //not equal
            else if (at.comparisonType == ShowIfAttribute.EComparisonType.isNotEqual)
                return comparedProperty.Equals((T)at.valueToCompare) == false;
            else
                return false;
        }

        bool CompareInt(ShowIfAttribute at, int comparedProperty)
        {
            //compare property A and value
            switch (at.comparisonType)
            {
                case ShowIfAttribute.EComparisonType.isEqual:
                    return comparedProperty == (int)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (int)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (int)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (int)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (int)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLowerEqual:
                    return comparedProperty <= (int)at.valueToCompare;
                default:
                    return comparedProperty == (int)at.valueToCompare;
            }
        }

        bool CompareFloat(ShowIfAttribute at, float comparedProperty)
        {
            //compare property A and value
            switch (at.comparisonType)
            {
                case ShowIfAttribute.EComparisonType.isEqual:
                    return Mathf.Abs(comparedProperty - (float)at.valueToCompare) <= floatingPoint;
                case ShowIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (float)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (float)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (float)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (float)at.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLowerEqual:
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
    /// Attribute to show variable only if condition is true
    /// </summary>
    public class ShowIfAttribute : PropertyAttribute
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

        public ShowIfAttribute(params string[] values)
        {
            comparing = false;

            //by default condition is AND
            conditionOperator = EConditionOperator.AND;
            this.values = values;
        }

        public ShowIfAttribute(EConditionOperator conditionOperator, params string[] values)
        {
            comparing = false;

            this.conditionOperator = conditionOperator;
            this.values = values;
        }

        public ShowIfAttribute(string propertyA, object valueToCompare, EComparisonType comparisonType = EComparisonType.isEqual)
        {
            comparing = true;

            this.propertyA = propertyA;
            this.valueToCompare = valueToCompare;
            this.comparisonType = comparisonType;
        }
    }
}