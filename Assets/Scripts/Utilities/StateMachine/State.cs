namespace redd096
{
    using System.Collections;

    public abstract class State
    {
        protected StateMachine stateMachine;

        public State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        /// <summary>
        /// Function called when enter in this state
        /// </summary>
        public virtual IEnumerator Enter()
        {
            yield break;
        }

        /// <summary>
        /// Function to call in Update()
        /// </summary>
        public virtual void Execution()
        {

        }

        /// <summary>
        /// Function called when exit from this state
        /// </summary>
        public virtual IEnumerator Exit()
        {
            yield break;
        }
    }
}