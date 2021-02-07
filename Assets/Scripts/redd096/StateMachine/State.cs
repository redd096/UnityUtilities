namespace redd096
{
    public abstract class State
    {
        protected StateMachine stateMachine;

        /// <summary>
        /// Function called when enter in this state (before of Enter). This one is used by default to set stateMachine
        /// </summary>
        public virtual void AwakeState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        /// <summary>
        /// Function called when enter in this state
        /// </summary>
        public virtual void Enter()
        {

        }

        /// <summary>
        /// Function to call in Update()
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Function to call in FixedUpdate()
        /// </summary>
        public virtual void FixedUpdate()
        {

        }

        /// <summary>
        /// Function called when exit from this state
        /// </summary>
        public virtual void Exit()
        {

        }
    }
}