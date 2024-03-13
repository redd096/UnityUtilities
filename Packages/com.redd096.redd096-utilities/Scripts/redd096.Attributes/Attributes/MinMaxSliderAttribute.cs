using UnityEngine;

#if UNITY_EDITOR
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show a slider to specific a min and max value
    /// </summary>
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float minValue;
        public readonly string minValueName;
        public readonly float maxValue;
        public readonly string maxValueName;

        public MinMaxSliderAttribute(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.minValueName = string.Empty;
            this.maxValueName = string.Empty;
        }

        public MinMaxSliderAttribute(string minValueName, float maxValue)
        {
            this.maxValue = maxValue;
            this.minValueName = minValueName;
            this.maxValueName = string.Empty;
        }

        public MinMaxSliderAttribute(float minValue, string maxValueName)
        {
            this.minValue = minValue;
            this.minValueName = string.Empty;
            this.maxValueName = maxValueName;
        }

        public MinMaxSliderAttribute(string minValueName, string maxValueName)
        {
            this.minValueName = minValueName;
            this.maxValueName = maxValueName;
        }
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        MinMaxSliderAttribute at;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            at = attribute as MinMaxSliderAttribute;

            if (property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector2Int)
            {
                //get rects, values and limits
                GetRects(position, out Rect labelRect, out Rect sliderRect, out Rect minFieldRect, out Rect maxFieldRect);
                bool isVector2Int = property.propertyType == SerializedPropertyType.Vector2Int;
                Vector2 value = isVector2Int ? property.vector2IntValue : property.vector2Value;
                GetMinMaxLimits(property, out float minLimit, out float maxLimit);

                // Draw the label
                EditorGUI.LabelField(labelRect, label);

                EditorGUI.BeginChangeCheck();

                //draw min value
                if (isVector2Int) value.x = EditorGUI.IntField(minFieldRect, (int)value.x);
                else value.x = EditorGUI.FloatField(minFieldRect, value.x);
                value.x = Mathf.Clamp(value.x, minLimit, Mathf.Min(maxLimit, value.y));

                //draw slider
                EditorGUI.MinMaxSlider(sliderRect, ref value.x, ref value.y, minLimit, maxLimit);

                //draw max value
                if (isVector2Int) value.y = EditorGUI.IntField(maxFieldRect, (int)value.y);
                else value.y = EditorGUI.FloatField(maxFieldRect, value.y);
                value.y = Mathf.Clamp(value.y, Mathf.Max(minLimit, value.x), maxLimit);

                //update value
                if (EditorGUI.EndChangeCheck())
                {
                    if (isVector2Int)
                        property.vector2IntValue = new Vector2Int((int)value.x, (int)value.y);
                    else
                        property.vector2Value = value;
                }
            }
            else
            {
                Debug.LogWarning(property.serializedObject.targetObject + " - " + at.GetType().Name + " can't be used on '" + property.name + "'. It can be used only on Vector2 or Vector2Int variables", property.serializedObject.targetObject);
            }
        }

        void GetRects(Rect rect, out Rect labelRect, out Rect sliderRect, out Rect minFieldRect, out Rect maxFieldRect)
        {
            Rect indentRect = EditorGUI.IndentedRect(rect);
            float indentLength = indentRect.x - rect.x;
            float labelWidth = EditorGUIUtility.labelWidth + 2f;
            float fieldWidth = EditorGUIUtility.fieldWidth;
            float sliderWidth = rect.width - labelWidth - (fieldWidth * 2f);
            float sliderPadding = 5.0f;

            labelRect = new Rect(
                rect.x,
                rect.y,
                labelWidth,
                rect.height);

            minFieldRect = new Rect(
                rect.x + labelWidth - indentLength,
                rect.y,
                fieldWidth + indentLength,
                rect.height);

            sliderRect = new Rect(
                rect.x + labelWidth + fieldWidth + sliderPadding - indentLength,
                rect.y,
                sliderWidth - 2.0f * sliderPadding + indentLength,
                rect.height);

            maxFieldRect = new Rect(
                rect.x + labelWidth + fieldWidth + sliderWidth - indentLength,
                rect.y,
                fieldWidth + indentLength,
                rect.height);
        }

        void GetMinMaxLimits(SerializedProperty property, out float minLimit, out float maxLimit)
        {
            minLimit = (float)EditorAttributesUtility.GetObjectValue(property, at.minValueName, at.minValue, typeof(float), typeof(int));
            maxLimit = (float)EditorAttributesUtility.GetObjectValue(property, at.maxValueName, at.maxValue, typeof(float), typeof(int));
        }
    }

#endif

    #endregion
}