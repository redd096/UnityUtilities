using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer
    {
        HelpBoxAttribute at;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //add help box height
            return EditorGUI.GetPropertyHeight(property, label, true) + 
                EditorStyles.helpBox.CalcSize(new GUIContent(at != null ? at.message : "")).y;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            at = attribute as HelpBoxAttribute;

            //draw help box above
            if (at.isAbove)
                EditorGUILayout.HelpBox(at.message, at.messageType, at.wide);

            //draw property
            EditorGUILayout.PropertyField(property, label, true);

            //draw help box below
            if (at.isAbove == false)
                EditorGUILayout.HelpBox(at.message, at.messageType, at.wide);
        }
    }

#endif

    #endregion

    /// <summary>
    /// Show help box above property
    /// </summary>
    public class HelpBoxAttribute : PropertyAttribute
    {
        public readonly MessageType messageType;
        public readonly string message;
        public readonly bool wide;
        public readonly bool isAbove;

        /// <summary>
        /// Show help box above property
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="wide">If true, the box will cover the whole width of the window; otherwise it will cover the controls part only</param>
        public HelpBoxAttribute(string message, MessageType messageType = MessageType.Info, bool wide = true)
        {
            this.message = message;
            this.messageType = messageType;
            this.wide = wide;
            isAbove = true;
        }

        /// <summary>
        /// Show help box above or below property
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isAbove">help box will be above or below property?</param>
        /// <param name="messageType"></param>
        /// <param name="wide">If true, the box will cover the whole width of the window; otherwise it will cover the controls part only</param>
        public HelpBoxAttribute(string message, bool isAbove, MessageType messageType = MessageType.Info, bool wide = true)
        {
            this.message = message;
            this.isAbove = isAbove;
            this.messageType = messageType;
            this.wide = wide;
        }
    }
}