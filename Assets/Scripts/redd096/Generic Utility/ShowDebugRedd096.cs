using UnityEngine;

namespace redd096
{
    #region editor
#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(ShowDebugRedd096))]
    public class ShowDebugRedd096Drawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return base.GetPropertyHeight(property, label);
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            EditorGUI.BeginProperty(position, label, property);

            //draw in horizontal
            EditorGUILayout.BeginHorizontal();
            Vector2 size = EditorStyles.label.CalcSize(label);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUILayout.LabelField(label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ShowDebug"), GUIContent.none, GUILayout.Width(30));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ColorDebug"), GUIContent.none, GUILayout.Width(100));

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

        public ShowDebugRedd096(Color color)
        {
            ColorDebug = color;
        }

        //implicit convertor to do this: ShowDebugRedd096 varName = Color.cyan;
        public static implicit operator ShowDebugRedd096(Color c) => new ShowDebugRedd096(c);

        //implicit convertor to do this: if(s)
        public static implicit operator bool(ShowDebugRedd096 s) => s.ShowDebug;
    }
}