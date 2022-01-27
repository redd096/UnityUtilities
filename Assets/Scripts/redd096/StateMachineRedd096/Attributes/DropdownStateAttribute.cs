using UnityEngine;

namespace redd096
{
    #region editor property drawer
#if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(DropdownStateAttribute))]
    public class DropdownStateDrawer : PropertyDrawer
    {
        State[] possibleStates;
        string[] statesNames;

        const float percentageWidthlabel = 0.4f;
        Rect dropdownRect;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            //get states
            GetStates(property);

            //show label if necessary, and set dropdown rect based on this
            ShowLabelAndSetDropdownRect(rect);

            //show dropdown to select and set property
            if (possibleStates != null && possibleStates.Length > 0)
            {                
                property.intValue = EditorGUI.Popup(dropdownRect, property.intValue, statesNames);
            }
            //else show NONE and don't set anything
            else
            {
                EditorGUI.Popup(dropdownRect, 0, new string[1] { "NONE" });
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

                //set arrays for dropdown
                if (possibleStates != null && possibleStates.Length > 0)
                {
                    statesNames = new string[possibleStates.Length];

                    for (int i = 0; i < possibleStates.Length; i++)
                    {
                        statesNames[i] = possibleStates[i].StateName;
                    }
                }
            }
        }

        void ShowLabelAndSetDropdownRect(Rect rect)
        {
            //if dropdown attribute and string is not empty
            if (attribute is DropdownStateAttribute dropdownState && string.IsNullOrEmpty(dropdownState.nameValue) == false)
            {
                //show name value to the left and set dropdown rect to the right
                float widthLabel = rect.width * percentageWidthlabel;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, widthLabel, rect.height), dropdownState.nameValue);
                dropdownRect = new Rect(rect.x + widthLabel, rect.y, rect.width - widthLabel, rect.height);
            }
            //else use normal rect for dropdown, and do not visualize label
            else
            {
                dropdownRect = rect;
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