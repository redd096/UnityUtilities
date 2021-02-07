namespace redd096
{
    using UnityEngine;

#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{
        //    return EditorGUI.GetPropertyHeight(property, label, true);
        //}

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //disable GUI for this property
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label, true);

            //re-enable GUI
            GUI.enabled = true;
        }
    }

#endif

    //attribute
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}