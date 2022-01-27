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
                foreach(MethodInfo method in property.serializedObject.targetObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if(method.Name == at.methodName)
                    {
                        //only if with 0 parameters
                        if (method != null && method.GetParameters().Length == 0)
                        {
                            method.Invoke(property.serializedObject.targetObject, null);

                            //repaint scene
                            SceneView.RepaintAll();                            
                        }
                        else
                        {
                            Debug.LogWarning(at.GetType().Name + "can invoke only methods with 0 parameters", property.serializedObject.targetObject);
                        }

                        break;
                    }
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