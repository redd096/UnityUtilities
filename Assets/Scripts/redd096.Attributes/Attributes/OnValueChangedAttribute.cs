using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
    public class OnValueChangedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //draw property
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property);

            //if changed something
            if (EditorGUI.EndChangeCheck())
            {
                //apply modifications, so new value is updated in serialized object
                property.serializedObject.ApplyModifiedProperties();

                OnValueChangedAttribute at = attribute as OnValueChangedAttribute;

                //in target object, find method with same name and invoke
                MethodInfo method = property.serializedObject.targetObject.GetMethod(at.methodName);

                //only if with 0 or only optional parameters
                if (method != null && method.HasZeroParameterOrOnlyOptional())
                {
                    method.Invoke(property.serializedObject.targetObject, method.GetDefaultParameters());
                }
                else
                {
                    Debug.LogWarning(at.GetType().Name + "can invoke only methods with 0 parameters", property.serializedObject.targetObject);
                }
            }
        }
    }

#endif

    #endregion

    /// <summary>
    /// Attribute to call a method when value is changed
    /// </summary>
    public class OnValueChangedAttribute : PropertyAttribute
    {
        public readonly string methodName;

        /// <summary>
        /// Attribute to call a method when value is changed
        /// </summary>
        /// <param name="methodNameNoParameters"></param>
        public OnValueChangedAttribute(string methodNameNoParameters)
        {
            methodName = methodNameNoParameters;
        }
    }
}