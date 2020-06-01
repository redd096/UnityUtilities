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

            //enter in new one
            state = stateToSet;
            StartCoroutine(state.Enter());
        }
    }
}