using UnityEngine;
using System.Reflection;
using System;
#if UNITY_EDITOR
using redd096.Attributes.AttributesEditorUtility;
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show a vector3 or vector2 as a drabble point in scene. It's the same as HandlePointAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class DraggablePointAttribute : PropertyAttribute
    {
    }

    #region editor

#if UNITY_EDITOR

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class DraggablePointEditor : Editor
    {
        readonly GUIStyle style = new GUIStyle();

        void OnEnable()
        {
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
        }

        private void OnSceneGUI()
        {
            //iterate every property in serializedObject
            SerializedProperty property = serializedObject.GetIterator();
            while (property.Next(true))
            {
                //make sure it is decorated by our custom attribute
                FieldInfo field = serializedObject.targetObject.GetField(property.name); // serializedObject.targetObject.GetType().GetField(property.name);
                if (field == null)
                    continue;

                DraggablePointAttribute draggablePoint = field.GetCustomAttribute<DraggablePointAttribute>(true);
                if (draggablePoint != null)
                {
                    //draw PositionHandle
                    if (property.propertyType == SerializedPropertyType.Vector3)
                    {
                        Handles.Label(property.vector3Value + Vector3.down * 0.08f, property.name);
                        property.vector3Value = Handles.PositionHandle(property.vector3Value, Quaternion.identity);
                        serializedObject.ApplyModifiedProperties();
                    }
                    else if (property.propertyType == SerializedPropertyType.Vector2)
                    {
                        Handles.Label(property.vector2Value + Vector2.down * 0.08f, property.name);
                        property.vector2Value = Handles.PositionHandle(property.vector2Value, Quaternion.identity);
                        serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        Debug.LogWarning($"{serializedObject.targetObject} - {draggablePoint.GetType().Name} can't be used on '{property.name}'. It can be used only on Vector3 or Vector2 variables", serializedObject.targetObject);
                    }
                }
            }
        }
    }

#endif

    #endregion
}