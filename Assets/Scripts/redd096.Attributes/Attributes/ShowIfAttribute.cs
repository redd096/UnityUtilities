﻿using UnityEngine;
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
            ShowIfAttribute showIf = attribute as ShowIfAttribute;

            SerializedProperty comparedProperty = property.FindCorrectProperty(showIf.propertyA);

            //compare property based on propertyType
            if (comparedProperty.propertyType == SerializedPropertyType.Boolean)
                return CompareBool(showIf, comparedProperty.boolValue);
            else if (comparedProperty.propertyType == SerializedPropertyType.Enum)
                return CompareInt(showIf, comparedProperty.enumValueIndex);
            else if (comparedProperty.propertyType == SerializedPropertyType.Integer)
                return CompareInt(showIf, comparedProperty.intValue);
            else if (comparedProperty.propertyType == SerializedPropertyType.Float)
                return CompareFloat(showIf, comparedProperty.floatValue);

            return false;
        }

        bool CompareBool(ShowIfAttribute showIf, bool comparedProperty)
        {
            //equal
            if (showIf.comparisonType == ShowIfAttribute.EComparisonType.isEqual)
                return comparedProperty == (bool)showIf.valueToCompare;
            //not equal
            else if (showIf.comparisonType == ShowIfAttribute.EComparisonType.isNotEqual)
                return comparedProperty != (bool)showIf.valueToCompare;
            else
                return false;
        }

        bool CompareInt(ShowIfAttribute showIf, int comparedProperty)
        {
            //compare property A and value
            switch (showIf.comparisonType)
            {
                case ShowIfAttribute.EComparisonType.isEqual:
                    return comparedProperty == (int)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (int)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (int)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (int)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (int)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLowerEqual:
                    return comparedProperty <= (int)showIf.valueToCompare;
                default:
                    return comparedProperty == (int)showIf.valueToCompare;
            }
        }

        bool CompareFloat(ShowIfAttribute showIf, float comparedProperty)
        {
            //compare property A and value
            switch (showIf.comparisonType)
            {
                case ShowIfAttribute.EComparisonType.isEqual:
                    return Mathf.Abs(comparedProperty - (float)showIf.valueToCompare) <= floatingPoint;
                case ShowIfAttribute.EComparisonType.isNotEqual:
                    return comparedProperty != (float)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreater:
                    return comparedProperty > (float)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isGreaterEqual:
                    return comparedProperty >= (float)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLower:
                    return comparedProperty < (float)showIf.valueToCompare;
                case ShowIfAttribute.EComparisonType.isLowerEqual:
                    return comparedProperty <= (float)showIf.valueToCompare;
                default:
                    return Mathf.Abs(comparedProperty - (float)showIf.valueToCompare) <= floatingPoint;
            }
        }

        #endregion
    }

#endif

    #endregion

    //attribute
    public class ShowIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// How to compare variables (default is Equal)
        /// </summary>
        public EComparisonType comparisonType = EComparisonType.isEqual;

        public readonly string propertyA;
        public readonly object valueToCompare;

        /// <summary>
        /// Attribute to show variable only if comparison return true
        /// </summary>
        /// <param name="propertyA"></param>
        /// <param name="valueToCompare"></param>
        public ShowIfAttribute(string propertyA, object valueToCompare)
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