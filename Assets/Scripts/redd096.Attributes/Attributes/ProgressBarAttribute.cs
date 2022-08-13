using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace redd096.Attributes
{
	#region editor

#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(ProgressBarAttribute))]
	public class ProgressBarDrawer : PropertyDrawer
	{
		float value;
		float maxValue;
		string barLabel;
		Color barColor;
		bool canInteract;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			//get attribute and max value
			ProgressBarAttribute at = attribute as ProgressBarAttribute;
			object maxValueObject = property.GetValue(at.maxValueName, typeof(int), typeof(float));

			//be sure property and maxValue are numbers
			if (IsNumber(property) && IsNumber(maxValueObject))
			{
				//set values
				value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
				string valueFormatted = property.propertyType == SerializedPropertyType.Integer ? value.ToString() : string.Format("{0:0.00}", value);
				maxValue = maxValueObject is int ? (int)maxValueObject : (float)maxValueObject;
				string maxValueFormatted = maxValueObject is int ? maxValue.ToString() : string.Format("{0:0.00}", maxValue);
				barLabel = (!string.IsNullOrEmpty(at.name) ? "[" + at.name + "] " : "") + valueFormatted + "/" + maxValueFormatted;
				barColor = GetColor(at.color);
				canInteract = at.canInteract;

				//draw bar
				DrawBar(property, position);
			}
			else
			{
				Debug.LogWarning(at.GetType().Name + " can be used only on int or float properties", (property.serializedObject.targetObject as Component).gameObject);
			}
		}

		bool IsNumber(SerializedProperty property)
		{
			//be sure is int or float
			return property != null && (property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.Float);
		}

		bool IsNumber(object maxValueObject)
        {
			return maxValueObject != null && (maxValueObject is int || maxValueObject is float);
        }

		Color GetColor(ProgressBarAttribute.EColor color)
		{
			switch (color)
			{
				case ProgressBarAttribute.EColor.Clear:
					return Color.clear;
				case ProgressBarAttribute.EColor.Cyan:
					return Color.cyan;
				case ProgressBarAttribute.EColor.Grey:
					return Color.grey;
				case ProgressBarAttribute.EColor.Gray:
					return Color.gray;
				case ProgressBarAttribute.EColor.Magenta:
					return Color.magenta;
				case ProgressBarAttribute.EColor.Red:
					return Color.red;
				case ProgressBarAttribute.EColor.Yellow:
					return Color.yellow;
				case ProgressBarAttribute.EColor.Black:
					return Color.black;
				case ProgressBarAttribute.EColor.White:
					return Color.white;
				case ProgressBarAttribute.EColor.Green:
					return Color.green;
				case ProgressBarAttribute.EColor.Blue:
					return Color.blue;

				case ProgressBarAttribute.EColor.SmoothRed:
					return new Color32(255, 0, 63, 255);
				case ProgressBarAttribute.EColor.Pink:
					return new Color32(255, 152, 203, 255);
				case ProgressBarAttribute.EColor.Orange:
					return new Color32(255, 128, 0, 255);
				case ProgressBarAttribute.EColor.SmoothYellow:
					return new Color32(255, 211, 0, 255);
				case ProgressBarAttribute.EColor.SmoothGreen:
					return new Color32(98, 200, 79, 255);
				case ProgressBarAttribute.EColor.SmoothBlue:
					return new Color32(0, 135, 189, 255);
				case ProgressBarAttribute.EColor.Indigo:
					return new Color32(75, 0, 130, 255);
				case ProgressBarAttribute.EColor.Violet:
					return new Color32(128, 0, 255, 255);
				default:
					return Color.black;
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

	/// <summary>
	/// Show a progress bar (property/max value)
	/// </summary>
	public class ProgressBarAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly float maxValue;
		public readonly string maxValueName;
		public readonly EColor color;
		public readonly bool canInteract;

		public ProgressBarAttribute(string name, float maxValue, EColor color = EColor.Red, bool canInteract = true)
		{
			this.name = name;
			this.maxValue = maxValue;
			this.color = color;
			this.canInteract = canInteract;
		}

		public ProgressBarAttribute(string name, string maxValueName, EColor color = EColor.Red, bool canInteract = true)
		{
			this.name = name;
			this.maxValueName = maxValueName;
			this.color = color;
			this.canInteract = canInteract;
		}

		public ProgressBarAttribute(float maxValue, EColor color = EColor.Red, bool canInteract = true)
		{
			this.name = string.Empty;
			this.maxValue = maxValue;
			this.color = color;
			this.canInteract = canInteract;
		}

		public ProgressBarAttribute(string maxValueName, EColor color = EColor.Red, bool canInteract = true)
		{
			this.name = string.Empty;
			this.maxValueName = maxValueName;
			this.color = color;
			this.canInteract = canInteract;
		}

		public enum EColor
		{
			Clear,
			Cyan,
			Grey,
			Gray,
			Magenta,
			Red,
			Yellow,
			Black,
			White,
			Green,
			Blue,

			SmoothRed,
			Pink,
			Orange,
			SmoothYellow,
			SmoothGreen,
			SmoothBlue,
			Indigo,
			Violet
		}
	}
}