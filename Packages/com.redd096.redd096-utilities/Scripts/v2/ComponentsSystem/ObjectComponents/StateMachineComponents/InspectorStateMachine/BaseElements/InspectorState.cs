using System.Collections.Generic;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Every state has a list of Actions and Transitions
    /// </summary>
    [System.Serializable]
    public class InspectorState
    {
        [OnStateNameChanged] public string StateName;
        [DropdownTask(typeof(ActionTask))] public ActionTask[] Actions;
        public Transition[] Transitions;
    }

    /// <summary>
    /// When EVERY or ANY condition is true, move to Destination State
    /// </summary>
    [System.Serializable]
    public class Transition
    {
        [DropdownState] public string DestinationState;
        public ETransitionCheck TransitionCheck;
        [DropdownTask(typeof(ConditionTask))] public List<ConditionTask> Conditions;
    }

    public enum ETransitionCheck { AllTrueRequired, AnyTrueSuffice }
}