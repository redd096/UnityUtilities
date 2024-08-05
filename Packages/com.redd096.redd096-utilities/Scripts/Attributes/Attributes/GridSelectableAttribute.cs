using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Draw a grid. Return Vector2Int for every selected button. If this attribute is on an Integer or Vector2Int, can use it as size
    /// </summary>
    public class GridSelectableAttribute : PropertyAttribute
    {
        public readonly string vector2IntArrayProperty;
        [Tooltip("Show coordinates (x, y) on the button?")] public bool showButtonName;
        [Tooltip("use center as zero, left down button will be negative (in grid 3x3, left down is -1,-1). If false, use left down button as zero")] public bool useCenterAsZero;
        public int sizeX { get; private set; }
        public int sizeY { get; private set; }

        /// <summary>
        /// Draw a grid. Return Vector2Int for every selected button. If this attribute is on an Integer or Vector2Int, can use it as size
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        public GridSelectableAttribute(string vector2IntArrayProperty, int sizeX = 3, int sizeY = 3)
        {
            this.vector2IntArrayProperty = vector2IntArrayProperty;

            SetSize(sizeX, sizeY);
        }

        public void SetSize(int sizeX, int sizeY)
        {
            //can't be negative
            this.sizeX = Mathf.Max(1, sizeX);
            this.sizeY = Mathf.Max(1, sizeY);
        }
    }

    #region editor
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(GridSelectableAttribute))]
    public class GridSelectableDrawer : PropertyDrawer
    {
        GridSelectableAttribute at;
        int propertyHeight = 18;
        int littleSpace = 2;
        int sizeButton = 40;
        float spaceBetweenButtons = 2;
        Color colorSelectedButton = Color.red;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //size to draw first property (if int or vector2 int)
            float propertySize = GetPropertySize(property);

            //property height + little space + every row * (size square + space between)
            int y = (attribute as GridSelectableAttribute).sizeY;
            return propertySize + y * (sizeButton + spaceBetweenButtons);

            //return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //get vector2IntArrayProperty value
            at = attribute as GridSelectableAttribute;
            Vector2Int[] arrayValues = property.GetValue(at.vector2IntArrayProperty) as Vector2Int[];

            //get grid size (number of rows and columns)
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = Mathf.Max(1, property.intValue);
                at.SetSize(property.intValue, property.intValue);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                property.vector2IntValue = new Vector2Int(Mathf.Max(1, property.vector2IntValue.x), Mathf.Max(1, property.vector2IntValue.y));
                at.SetSize(property.vector2IntValue.x, property.vector2IntValue.y);
            }

            //show property to set grid size
            if (CanDrawPropertyToSetSize(property))
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, propertyHeight), property, new GUIContent(property.name));



            //==============================================================
            //show grid
            bool someValueIsChanged = false;    //check if update values
            Vector2 startPosition = new Vector2(position.x, position.y + GetPropertySize(property));

            //get start coordinates (left down button is zero, or center button is zero and left down is negative?)
            ValueStruct[,] values = new ValueStruct[at.sizeX, at.sizeY];
            GetStartCoordinates(out bool xIsEven, out bool yIsEven, out int coordinatesX, out int coordinatesY);
            int defaultY = coordinatesY;

            for (int x = 0; x < at.sizeX; x++)
            {
                //if even, skip 0
                if (xIsEven && coordinatesX == 0)
                    coordinatesX++;

                for (int y = at.sizeY - 1; y >= 0; y--)     //inverse, because EditorGUI has 0,0 on top left instead of down left
                {
                    //if even, skip 0
                    if (yIsEven && coordinatesY == 0)
                        coordinatesY++;

                    //add to list and check if was already in array
                    Vector2Int coordinates = new Vector2Int(coordinatesX, coordinatesY);
                    values[x,y] = new ValueStruct { coordinates = coordinates, isUsed = ContainsValue(arrayValues, coordinates) };

                    //set button name and color
                    string buttonName = at.showButtonName ? coordinates.x + "," + coordinates.y : "";
                    Color previousGUIColor = GUI.backgroundColor;
                    GUI.backgroundColor = values[x,y].isUsed ? colorSelectedButton : previousGUIColor;

                    //when click, change value
                    if (GUI.Button(new Rect(startPosition.x + x * (sizeButton + spaceBetweenButtons), startPosition.y + y * (sizeButton + spaceBetweenButtons), 
                        sizeButton, sizeButton), buttonName))
                    {
                        someValueIsChanged = true;
                        values[x,y].isUsed = !values[x,y].isUsed;

                        //if keep pressed Shift, reset value
                        if (Event.current.shift)
                            values[x,y].isUsed = false;
                    }

                    //reset color
                    GUI.backgroundColor = previousGUIColor;

                    coordinatesY++;
                }

                //when reach last column, change row and restart columns
                coordinatesY = defaultY;
                coordinatesX++;
            }

            //update property only if some value is changed
            if (someValueIsChanged == false)
                return;

            //set values in property
            List<Vector2Int> v = new List<Vector2Int>();
            for (int x = 0; x < at.sizeX; x++)
                for (int y = at.sizeY - 1; y >= 0; y--)     //inverse, because EditorGUI has 0,0 on top left instead of down left
                    if (values[x,y].isUsed)
                        v.Add(values[x,y].coordinates);

            //get vector2IntArrayProperty serialized property
            SerializedProperty arrayProperty = property.FindCorrectProperty(at.vector2IntArrayProperty);

            arrayProperty.arraySize = v.Count;
            for (int i = 0; i < v.Count; i++)
                arrayProperty.GetArrayElementAtIndex(i).vector2IntValue = v[i];

            arrayProperty.serializedObject.ApplyModifiedProperties();
        }

        bool CanDrawPropertyToSetSize(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.Vector2Int;
        }

        float GetPropertySize(SerializedProperty property)
        {
            return CanDrawPropertyToSetSize(property) ? propertyHeight + littleSpace : 0;
        }

        void GetStartCoordinates(out bool xIsEven, out bool yIsEven, out int coordinatesX, out int coordinatesY)
        {
            //use center as zero, left down button will be negative (in grid 3x3, left down is -1,-1)
            if (at.useCenterAsZero)
            {
                xIsEven = at.sizeX % 2 == 0;    //if even skip coordinate 0
                yIsEven = at.sizeY % 2 == 0;    //if even skip coordinate 0
                coordinatesX = -at.sizeX / 2;   //example odd with size 5 => -2, -1, 0, 1, 2
                coordinatesY = -at.sizeY / 2;   //example even with size 4 => -2, -1, 1, 2
            }
            //else, use left down button as zero
            else
            {
                xIsEven = false;
                yIsEven = false;
                coordinatesX = 0;
                coordinatesY = 0;
            }
        }

        bool ContainsValue(Vector2Int[] array, Vector2Int value)
        {
            if (array == null)
                return false;

            //check if value is inside array
            foreach (Vector2Int v in array)
                if (v == value)
                    return true;

            return false;
        }

        public struct ValueStruct
        {
            public bool isUsed;
            public Vector2Int coordinates;
        }
    }

#endif
    #endregion
}