using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/StateMachine Feedback")]
    public class StateMachineFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in child and parent")]
        [SerializeField] Animator anim = default;
        [SerializeField] StateMachineRedd096 stateMachine = default;

        [Header("When enter in these states, call trigger in animator")]
        [SerializeField] AnimTriggerStateStruct[] animTriggers = default;

        [Header("When enter in these states, set bool in animator true or false")]
        [SerializeField] AnimBoolStateStruct[] animBools = default;

        void OnEnable()
        {
            //get references
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (stateMachine == null)
            {
                Redd096Main main = GetComponentInParent<Redd096Main>();
                if (main) stateMachine = main.GetComponentInChildren<StateMachineRedd096>();
            }

            //add events
            if (stateMachine)
            {
                stateMachine.onSetState += OnSetState;
            }
        }

        void OnDisable()
        {
            //remove events
            if (stateMachine)
            {
                stateMachine.onSetState -= OnSetState;
            }
        }

        void OnSetState(string stateName)
        {
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
        public string TriggerToSet;
    }

    [System.Serializable]
    public struct AnimBoolStateStruct
    {
        public string State;
        public string BoolToSet;
        public bool Value;
    }
}