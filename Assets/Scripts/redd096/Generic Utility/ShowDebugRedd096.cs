using UnityEngine;

namespace redd096
{
    #region editor
#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(ShowDebugRedd096))]
    public class ShowDebugRedd096Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            EditorGUI.BeginProperty(position, label, property);

            //draw in horizontal
            EditorGUILayout.BeginHorizontal();
            Vector2 size = EditorStyles.label.CalcSize(label);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ShowDebug"), GUIContent.none, GUILayout.Width(30));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ColorDebug"), GUIContent.none);

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndProperty();
        }
    }

#endif
    #endregion

    [System.Serializable]
    public class ShowDebugRedd096
    {
        public bool ShowDebug = false;
        public Color ColorDebug = Color.cyan;
    }
}