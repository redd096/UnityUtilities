using UnityEngine;
#if UNITY_EDITOR
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show a progress bar (property/max value)
    /// </summary>
    public class ProgressBarAttribute : PropertyAttribute
    {
        public readonly string barName;
        public readonly float maxValue;
        public readonly string maxValueName;
        public readonly AttributesUtility.EColor color;
        public readonly string colorValue;

        public bool canInteract;

		public ProgressBarAttribute(string barName, float maxValue, AttributesUtility.EColor color = AttributesUtility.EColor.Red)
		{
			this.barName = barName;
            this.maxValue = maxValue;
            this.maxValueName = string.Empty;
            this.color = color;
            this.colorValue = string.Empty;
        }

        public ProgressBarAttribute(string barName, float maxValue, string colorValue)
        {
            this.barName = barName;
            this.maxValue = maxValue;
            this.maxValueName = string.Empty;
            this.colorValue = colorValue;
        }

        public ProgressBarAttribute(string barName, string maxValueName, AttributesUtility.EColor color = AttributesUtility.EColor.Red)
        {
            this.barName = barName;
			this.maxValueName = maxValueName;
            this.color = color;
            this.colorValue = string.Empty;
        }

        public ProgressBarAttribute(string barName, string maxValueName, string colorValue)
        {
            this.barName = barName;
            this.maxValueName = maxValueName;
            this.colorValue = colorValue;
        }
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
	public class ProgressBarDrawer : PropertyDrawer
	{
        ProgressBarAttribute at;
        Color barColor;
        float maxValue;
        float value;
		string barLabel;
		bool canInteract;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			//get attribute and max value
			at = attribute as ProgressBarAttribute;
            SetBarColor(property);
            SetMaxValue(property, out bool maxValueIsInt);

			//be sure property is number
			if (property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.Float)
			{
				//set values
				value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
				string valueFormatted = property.propertyType == SerializedPropertyType.Integer ? value.ToString() : string.Format("{0:0.00}", value);
				string maxValueFormatted = maxValueIsInt ? maxValue.ToString() : string.Format("{0:0.00}", maxValue);
                string resultValue = valueFormatted + "/" + maxValueFormatted;
                barLabel = string.IsNullOrEmpty(at.barName) ? $"[{property.displayName}] {resultValue}" : $"[{at.barName}] {resultValue}";
				canInteract = at.canInteract;

				//draw bar
				DrawBar(property, position);
			}
			else
			{
				Debug.LogWarning(property.serializedObject.targetObject + " - " + at.GetType().Name + " can be used only on int or float properties", (property.serializedObject.targetObject as Component).gameObject);
			}
		}

        void SetBarColor(SerializedProperty property)
        {
            //get color from attribute or property
            barColor = default;
            if (string.IsNullOrEmpty(at.colorValue))
            {
                barColor = AttributesUtility.GetColor(at.color);
            }
            else
            {
                //property can be EColor or Color
                object obj = property.GetValue(at.colorValue, typeof(Color), typeof(Color32), typeof(AttributesUtility.EColor));
                if (obj is AttributesUtility.EColor eColor)
                {
                    barColor = AttributesUtility.GetColor(eColor);
                }
                else if (obj is Color32 color32)
                {
                    barColor = color32;
                }
                else if (obj is Color newColor)
                {
                    barColor = newColor;
                }
                else
                {
                    Debug.LogWarning(property.serializedObject.targetObject + " - " + typeof(ProgressBarAttribute).Name + " color error on property: '" + property.name + "'. It can be used only with Color, Color32 and AttributesUtility.EColor variables", property.serializedObject.targetObject);
                }
            }
        }

        void SetMaxValue(SerializedProperty property, out bool isInt)
        {
            //get max value from attribute or property
            maxValue = 0f;
            isInt = false;
            if (string.IsNullOrEmpty(at.maxValueName))
            {
                maxValue = at.maxValue;
            }
            else
            {
                //property can be float or int
                object obj = property.GetValue(at.maxValueName, typeof(float), typeof(int));
                if (obj is float floatValue)
                {
                    maxValue = floatValue;
                }
                else if (obj is int intValue)
                {
                    maxValue = intValue;
                    isInt = true;
                }
                else
                {
                    Debug.LogWarning(property.serializedObject.targetObject + " - " + typeof(ProgressBarAttribute).Name + " maxValue error on property: '" + property.name + "'. It can be used only with float and int variables", property.serializedObject.targetObject);
                }
            }
        }

		void DrawBar(SerializedProperty property, Rect position)
		{
			Rect fillRect = new Rect(position.x, position.y, position.width * (value / maxValue), position.height);

			//draw bar
			EditorGUI.DrawRect(position, new Color(0.13f, 0.13f, 0.13f));
			EditorGUI.DrawRect(fillRect, barColor);

			//save GUI default colors and alignment
			Color contentColor = GUI.contentColor;
			Color color = GUI.color;
			TextAnchor align = GUI.skin.label.alignment;

			//set alignment and color
			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			GUI.contentColor = Color.white;

			//draw label
			Rect labelRect = new Rect(position.x, position.y - 2, position.width, position.height);
			EditorGUI.DropShadowLabel(labelRect, barLabel);

			//draw invisible slider just to edit bar value
			if (canInteract)
			{
				GUI.color = Color.clear;
				if (property.propertyType == SerializedPropertyType.Integer)
					property.intValue = Mathf.FloorToInt(GUI.HorizontalSlider(position, property.intValue, 0, maxValue));
				else
					property.floatValue = GUI.HorizontalSlider(position, property.floatValue, 0, maxValue);
			}

			//reset color and alignment
			GUI.contentColor = contentColor;
			GUI.color = color;
			GUI.skin.label.alignment = align;
		}
	}

#endif

    #endregion
}