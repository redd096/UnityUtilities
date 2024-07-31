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
                FieldInfo field = property.GetField(); //serializedObject.targetObject.GetType().GetField(property.name);
                if (field == null)
                    continue;

                DraggablePointAttribute draggablePoint = field.GetCustomAttribute<DraggablePointAttribute>(true);
                if (draggablePoint != null)
                {
                    //draw PositionHandle
                    if (property.propertyType == SerializedPropertyType.Vector3 || property.propertyType == SerializedPropertyType.Vector2)
                    {
                        DrawHandle(property, property.propertyType == SerializedPropertyType.Vector3);
                    }
                }
            }
        }

        void DrawHandle(SerializedProperty property, bool isVector3)
        {
            //get value (world or local)
            Vector3 pos = isVector3 ? property.vector3Value : property.vector2Value;

            //draw handle
            Handles.Label(pos + Vector3.down * 0.08f, property.name);
            pos = Handles.PositionHandle(pos, Quaternion.identity);

            //update property value
            if (isVector3) property.vector3Value = pos;
            else property.vector2Value = pos;

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion
}