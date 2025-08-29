using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.Attributes
{
    /// <summary>
    /// Show help box above property. It's the same as InfoBoxAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public enum EMessageType { None, Info, Warning, Error }

        public readonly EMessageType messageType;
        public readonly string message;

        /// <summary>
        /// Show help box above property
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public HelpBoxAttribute(string message, EMessageType messageType = EMessageType.Info)
        {
            this.message = message;
            this.messageType = messageType;
        }
    }

    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HelpBoxAttribute), true)]
    public class HelpBoxDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            HelpBoxAttribute at = attribute as HelpBoxAttribute;

            //get the width of the property drawing area
            float width = EditorGUIUtility.currentViewWidth - 38f; // 38f is a constant for the horizontal margins/padding

            //calculate the height of the HelpBox with the correct width
            float height = EditorStyles.helpBox.CalcHeight(new GUIContent(at != null ? at.message : ""), width);

            return base.GetHeight() + height;
        }

        public override void OnGUI(Rect position)
        {
            //base.OnGUI(position);
            HelpBoxAttribute at = attribute as HelpBoxAttribute;

            EditorGUI.HelpBox(position, at.message, GetMessageType(at.messageType));
        }

        MessageType GetMessageType(HelpBoxAttribute.EMessageType messageType)
        {
            switch (messageType)
            {
                case HelpBoxAttribute.EMessageType.None:
                    return MessageType.None;
                case HelpBoxAttribute.EMessageType.Info:
                    return MessageType.Info;
                case HelpBoxAttribute.EMessageType.Warning:
                    return MessageType.Warning;
                case HelpBoxAttribute.EMessageType.Error:
                    return MessageType.Error;
                default:
                    return MessageType.Info;
            }
        }
    }

#endif

    #endregion
}