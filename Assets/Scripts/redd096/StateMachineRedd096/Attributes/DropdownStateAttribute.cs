using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096
{
    #region editor property drawer

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(DropdownStateAttribute))]
    public class DropdownStateDrawer : PropertyDrawer
    {
        State[] possibleStates;
        string[] statesNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (attribute is DropdownStateAttribute at)
            {
                //get states
                GetStates(property);

                //show dropdown to select and set property
                if (possibleStates != null && possibleStates.Length > 0)
                {
                    property.intValue = EditorGUI.Popup(position, at.nameValue, property.intValue, statesNames);
                }
                //else show NONE and don't set anything
                else
                {
                    EditorGUI.Popup(position, at.nameValue, 0, new string[1] { "NONE" });
                }
            }

            EditorGUI.EndProperty();
        }

        void GetStates(SerializedProperty property)
        {
            //get owner of the property
            Component owner = property.serializedObject.targetObject as Component;
            StateMachineRedd096 stateMachine = owner as StateMachineRedd096;
            if (stateMachine)
            {
                //get tasks in children
                possibleStates = stateMachine.States;

                if (possibleStates != null && possibleStates.Length > 0)
                {
                    //set states name for dropdown
                    statesNames = new string[possibleStates.Length];
                    for (int i = 0; i < possibleStates.Length; i++)
                    {
                        statesNames[i] = possibleStates[i].StateName;
                    }
                }
            }
        }
    }

#endif

    #endregion

    public class DropdownStateAttribute : PropertyAttribute
    {
        public readonly string nameValue;

        /// <summary>
        /// Attribute to draw a dropdown for every state in this state machine
        /// </summary>
        /// <param name="nameValue">a string to show to the side of the dropdown</param>
        public DropdownStateAttribute(string nameValue = "")
        {
            this.nameValue = nameValue;
        }
    }
}