namespace redd096
{
    using UnityEngine;

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
                state.Exit();

            //set new one
            state = stateToSet;

            //enter in new one
            if (state != null)
            {
                state.AwakeState(this);
                state.Enter();
            }
        }
    }
}