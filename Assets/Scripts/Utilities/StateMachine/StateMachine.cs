namespace redd096
{
    using UnityEngine;

    [AddComponentMenu("redd096/State Machine")]
    public class StateMachine : MonoBehaviour
    {
        protected State state;

        /// <summary>
        /// Call it to change state
        /// </summary>
        public void SetState(State stateToSet)
        {
            //exit from previous
            if (state != null)
                StartCoroutine(state.Exit());

            //set new one
            state = stateToSet;

            //enter in new one
            if (state != null)
            {
                state.Awake(this);
                StartCoroutine(state.Enter());
            }
        }

        //void Start()
        //{
        //    SetState(new PlayerState(this));  // create new one, with constructor
        //                                      // or
        //    SetState(playerState);            // use serialized state (use Awake instead of constructor)
        //}
        
        //void Update()
        //{
        //    state?.Execution();
        //}
    }
}