using UnityEngine;
using System.Collections.Generic;
using redd096.Attributes.AttributesEditorUtility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.v2.ComponentsSystem
{
    #region editor

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(OnStateNameChangedAttribute))]
    public class OnStateNameChangedDrawer : PropertyDrawer
    {
        const string StatesPropertyName = "States";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //save previous value
            string previousValue = property.stringValue;

            //draw property
            string newValue = property.stringValue;
            newValue = EditorGUI.TextField(position, label, newValue);

            //if changed something
            if (previousValue != newValue)
            {
                //get statemachine from this target object, or from owner
                IStateMachineInspector stateMachine;
                if (property.serializedObject.targetObject is IStateMachineInspector sm)
                    stateMachine = sm;
                else
                    stateMachine = ((IGameObjectRD)property.serializedObject.targetObject).GetComponentRD<IStateMachineInspector>();

                //get states list
                IEnumerable<InspectorState> states = null;
                if (stateMachine != null)
                    states = (IEnumerable<InspectorState>)stateMachine.GetValue(StatesPropertyName);

                //update every destination state in every transition
                if (states != null)
                {
                    foreach (InspectorState state in states)
                        foreach (Transition transition in state.Transitions)
                            if (transition.DestinationState == previousValue)
                                transition.DestinationState = newValue;
                }

                //update object (to update every destination state)
                property.serializedObject.Update();

                //apply to property
                property.stringValue = newValue;

                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }

#endif

    #endregion

    /// <summary>
    /// Attribute to call a method when value is changed. Method must have zero parameters, or optional. 
    /// Otherwise must have 2 variable: previous value and new value
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class OnStateNameChangedAttribute : PropertyAttribute
    {

    }
}