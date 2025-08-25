using UnityEngine;
using redd096.v1.InspectorStateMachine;
using redd096.OLD;

namespace redd096.v1.GameTopDown2D
{
    [AddComponentMenu("redd096/v1/GameTopDown2D/Feedbacks/StateMachine Feedback")]
    public class StateMachineFeedback : FeedbackRedd096
    {
        [Header("Necessary Components - default get in child and parent")]
        [SerializeField] Animator anim = default;
        [SerializeField] InspectorStateMachine.StateMachine stateMachine = default;

        [Header("When enter in these states, call trigger in animator")]
        [SerializeField] AnimTriggerStateStruct[] animTriggers = default;

        [Header("When enter in these states, set bool in animator true or false")]
        [SerializeField] AnimBoolStateStruct[] animBools = default;

        protected override void OnEnable()
        {
            //get references
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (stateMachine == null)
            {
                Redd096Main main = GetComponentInParent<Redd096Main>();
                if (main) stateMachine = main.GetComponentInChildren<InspectorStateMachine.StateMachine>();
            }

            base.OnEnable();
        }

        protected override void AddEvents()
        {
            base.AddEvents();

            //add events
            if (stateMachine)
            {
                stateMachine.onSetState += OnSetState;
            }
        }

        protected override void RemoveEvents()
        {
            base.RemoveEvents();

            //remove events
            if (stateMachine)
            {
                stateMachine.onSetState -= OnSetState;
            }
        }

        void OnSetState(State state)
        {
            if (state == null)
                return;

            string stateName = state.StateName;

            foreach (AnimTriggerStateStruct animTrigger in animTriggers)
            {
                //if enter in this state
                if (stateName.Equals(animTrigger.State))
                {
                    //set trigger
                    if (anim)
                        anim.SetTrigger(animTrigger.TriggerToSet);

                    break;
                }
            }

            foreach (AnimBoolStateStruct animBool in animBools)
            {
                //if enter in this state
                if (stateName.Equals(animBool.State))
                {
                    //set bool
                    if (anim)
                        anim.SetBool(animBool.BoolToSet, animBool.Value);

                    break;
                }
            }
        }
    }

    [System.Serializable]
    public struct AnimTriggerStateStruct
    {
        public string State;
        [Tooltip("Trigger's name in the animator")] public string TriggerToSet;
    }

    [System.Serializable]
    public struct AnimBoolStateStruct
    {
        public string State;
        [Tooltip("Bool's name in the animator")] public string BoolToSet;
        [Tooltip("Value to set when enter in this state")] public bool Value;
    }
}