using UnityEngine;
#if UNITY_EDITOR
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor;
#endif

namespace redd096.Attributes
{ 
    /// <summary>
    /// Change display name shown in inspector
    /// </summary>
    public class RenameAttribute : PropertyAttribute
    {
        public readonly string name;
        public readonly string value;

        /// <summary>
        /// Change display name shown in inspector
        /// </summary>
        /// <param name="name"></param>
        public RenameAttribute(string name)
        {
            this.name = name;
            this.value = string.Empty;
        }

        /// <summary>
        /// Change display name shown in inspector
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">field, property or method to add to name. Example -> "PlayerHealth: " + value</param>
        public RenameAttribute(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameDrawer : PropertyDrawer
    {
        RenameAttribute at;
        string newLabel;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return base.GetPropertyHeight(property, label);
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //name + value
            at = attribute as RenameAttribute;
            newLabel = at.name;            
            if (string.IsNullOrEmpty(at.value) == false)
                newLabel += property.GetValue(at.value).ToString();

            //draw property
            EditorGUI.PropertyField(position, property, new GUIContent(newLabel), true);
        }
    }

#endif

    #endregion
}