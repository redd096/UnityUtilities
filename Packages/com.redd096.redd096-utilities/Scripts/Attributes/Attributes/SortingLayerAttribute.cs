using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show dropdown of every SortingLayer in the project
    /// </summary>
    public class SortingLayerAttribute : PropertyAttribute
    {
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerDrawer : PropertyDrawer
    {
        string[] sortingLayers;
        int index;
        string newValue;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //only string or int
            if (property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.Integer)
            {
                //get sorting layers
                sortingLayers = GetSortingLayers();

                //find current selected index, then show dropdown to select
                index = GetCurrentIndex(property);
                index = EditorGUI.Popup(position, label.text, index, sortingLayers);
                newValue = index < sortingLayers.Length ? sortingLayers[index] : "";

                //if value is string
                if (property.propertyType == SerializedPropertyType.String)
                {
                    //set value
                    if (property.stringValue.Equals(newValue) == false)
                        property.stringValue = newValue;
                }
                //or int
                else
                {
                    int newLayerNumber = SortingLayer.NameToID(newValue);

                    if (property.intValue != newLayerNumber)
                    {
                        property.intValue = newLayerNumber;
                    }

                }
            }
            //else show warning
            else
            {
                Debug.LogWarning(property.serializedObject.targetObject + " - " + typeof(SortingLayerAttribute).Name + " can't be used on '" + property.name + "'. It can be used only on string and int variables", property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        private string[] GetSortingLayers()
        {
            //we need to call the function like this, because sortingLayerNames is internal instead of public
            System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        int GetCurrentIndex(SerializedProperty property)
        {
            //if string, use propertyString - if int, use IDToName
            string layerName;
            if (property.propertyType == SerializedPropertyType.String)
                layerName = property.stringValue;
            else
                layerName = SortingLayer.IDToName(property.intValue);

            for (int i = 0; i < sortingLayers.Length; i++)
                if (sortingLayers[i].Equals(layerName))
                    return i;

            return 0;
        }

        //private int GetCurrentIndexByString(string currentSortingLayer)
        //{
        //    //get in array
        //    var index = Array.IndexOf(sortingLayers, currentSortingLayer);
        //    return Mathf.Clamp(index, 0, sortingLayers.Length - 1);
        //}
    }

#endif

    #endregion
}