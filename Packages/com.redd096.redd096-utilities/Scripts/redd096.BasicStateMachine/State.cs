using UnityEngine;

namespace redd096.BasicStateMachine
{
    public abstract class State
    {
        [Header("Basic State")]
        public string StateName = "";

        private StateMachine _stateMachine;
        public StateMachine StateMachine { get => _stateMachine; set => _stateMachine = value; }
        private bool _isActive;
        public bool IsActive { get => _isActive; set => _isActive = value; }

        bool isInitialized;

        /// <summary>
        /// Return StateMachine casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetStateMachine<T>() where T : StateMachine
        {
            return _stateMachine as T;
        }

        #region protected

        //as transform use statemachine if possible
        protected Transform transformState => _stateMachine ? _stateMachine.transform : null;

        /// <summary>
        /// Get component in stateMachine or parent. If not found, show warning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="showWarningIfNotFound"></param>
        /// <returns></returns>
        protected T GetStateMachineComponent<T>(bool showWarningIfNotFound = true)
        {
            //get in parent
            T component = _stateMachine ? _stateMachine.GetComponentInParent<T>() : default;

            //show warning if not found
            if (showWarningIfNotFound && component == null)
                Debug.LogWarning($"Miss {typeof(T).Name} on {_stateMachine}");

            return component;
        }

        /// <summary>
        /// Get blackboard or normal value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        protected T GetValue<T>(VarOrBlackboard<T> v) => v != null ? v.GetValue(_stateMachine) : default;

        #endregion

        #region statemachine functions

        /// <summary>
        /// Called by StateMachine, to set owner and to be sure to call OnInit() only one time
        /// </summary>
        /// <param name="stateMachine"></param>
        public void Initialize(StateMachine stateMachine)
        {
            if (isInitialized)
                return;

            isInitialized = true;

            //set state machine and init
            _stateMachine = stateMachine;
            OnInit();
        }

        /// <summary>
        /// Called by StateMachine, to set active and call OnEnter()
        /// </summary>
        public void Enter()
        {
            _isActive = true;
            OnEnter();
        }

        /// <summary>
        /// Called by StateMachine, to set not active and call OnExit()
        /// </summary>
        public void Exit()
        {
            _isActive = false;
            OnExit();
        }

        /// <summary>
        /// Called by StateMachine, to call OnUpdate()
        /// </summary>
        public void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// Called by StateMachine, to call OnFixedUpdate()
        /// </summary>
        public void FixedUpdate()
        {
            OnFixedUpdate();
        }

        #endregion

        /// <summary>
        /// Called only one time, first time enter in this state
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// Called every time enter in this state
        /// </summary>
        protected virtual void OnEnter() { }
        /// <summary>
        /// Called every time exit from this state
        /// </summary>
        protected virtual void OnExit() { }
        /// <summary>
        /// Called every time update this state
        /// </summary>
        protected virtual void OnUpdate() { }
        /// <summary>
        /// Called every time fixed update this state
        /// </summary>
        protected virtual void OnFixedUpdate() { }
    }
}