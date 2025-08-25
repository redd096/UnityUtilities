namespace redd096.StateMachine
{
    /// <summary>
    /// Basic interface for a State, where T is its StateMachine
    /// </summary>
    /// <typeparam name="T">StateMachine that controls this State</typeparam>
    public interface IState<T> where T : StateMachine<T>
    {
        T StateMachine { get; set; }

        void Enter();
        void UpdateState();
        void Exit();
    }
}
