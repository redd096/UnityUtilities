using UnityEngine;

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(CanEnableAttribute))]
    public class CanEnableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //update height when open a list/array/struct in inspector
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //disable GUI for this property if CanEnable is false
            GUI.enabled = CanEnable(property);

            EditorGUI.PropertyField(position, property, label, true);

            //re-enable GUI
            GUI.enabled = true;
        }

        #region private API

        bool CanEnable(SerializedProperty property)
        {
            CanEnableAttribute canEnableAttribute = attribute as CanEnableAttribute;

            //foreach value to check
            foreach (string value in canEnableAttribute.values)
            {
                bool check = property.serializedObject.FindProperty(value).boolValue;

                //use NOT
                if (canEnableAttribute.NOT)
                    check = !check;

                //if checkAND, when find one false, return false
                if (canEnableAttribute.checkAND && !check)
                {
                    return false;
                }
                //if checkOR, when find one true, return true
                else if (!canEnableAttribute.checkAND && check)
                {
                    return true;
                }
            }

            //else return checkAND, cause it mean every value is true if checkAND, or every value is false if checkOR
            return canEnableAttribute.checkAND;
        }

        #endregion
    }

#endif

    #endregion

    //attribute
    public class CanEnableAttribute : PropertyAttribute
    {
        /// <summary>
        /// check every value as AND or as OR (default is true)
        /// </summary>
        public bool checkAND = true;

        /// <summary>
        /// set NOT for every value (default is false)
        /// </summary>
        public bool NOT = false;

        public readonly string[] values;

        /// <summary>
        /// Attribute to show writable variable only if specific values are true
        /// </summary>
        /// <param name="values">the values to determine if show or not</param>
        public CanEnableAttribute(params string[] values)
        {
            this.values = values;
        }
    }
}