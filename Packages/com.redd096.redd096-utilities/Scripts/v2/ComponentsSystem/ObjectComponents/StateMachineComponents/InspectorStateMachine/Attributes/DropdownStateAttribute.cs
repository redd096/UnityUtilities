﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.v2.ComponentsSystem
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(DropdownStateAttribute))]
    public class DropdownStateDrawer : PropertyDrawer
    {
        string[] statesNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //get states
            GetStates(property);

            //show dropdown to select and set property
            if (statesNames != null && statesNames.Length > 0)
            {
                //get current index
                int selectedIndex = GetSelectedIndex(property);

                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, statesNames);
                property.stringValue = statesNames[selectedIndex];
            }
            //else show NONE
            else
            {
                EditorGUI.Popup(position, label.text, 0, new string[1] { "NONE" });
                property.stringValue = "NONE";
            }

            EditorGUI.EndProperty();
        }

        void GetStates(SerializedProperty property)
        {
            //get statemachine from this target object, or from owner
            IStateMachineInspector stateMachine = null;
            if (GetTransform(property.serializedObject.targetObject).TryGetComponent(out IStateMachineInspector sm))
                stateMachine = sm;
            else if (GetTransform(property.serializedObject.targetObject).TryGetComponent(out IGameObjectRD goRD))
                stateMachine = goRD.GetComponentRD<IStateMachineInspector>();

            if (stateMachine == null)
            {
                Debug.LogError("This attribute can be used only on IStateMachineInspector: " + GetType().Name, property.serializedObject.targetObject);
                return;
            }

            //get states list
            InspectorState[] states = stateMachine.States;

            //set arrays for dropdown
            if (states != null && states.Length > 0)
            {
                statesNames = new string[states.Length + 1];

                //set NONE as first state
                statesNames[0] = "NONE";

                for (int i = 0; i < states.Length; i++)
                {
                    statesNames[i + 1] = states[i].StateName;
                }

                return;
            }

            //if there are no states, set NONE as unique name
            statesNames = new string[1] { "NONE" };
        }

        int GetSelectedIndex(SerializedProperty property)
        {
            //return current index
            for (int i = 0; i < statesNames.Length; i++)
            {
                if (statesNames[i] == property.stringValue)
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// Return Transform (cast obj as GameObject or Component)
        /// </summary>
        private Transform GetTransform(Object obj)
        {
            if (obj is GameObject)
                return (obj as GameObject).transform;
            else
                return (obj as Component).transform;
        }
    }

#endif

    #endregion

    /// <summary>
    /// Attribute to draw a dropdown for every state in this state machine
    /// </summary>
    public class DropdownStateAttribute : PropertyAttribute
    {
    }
}