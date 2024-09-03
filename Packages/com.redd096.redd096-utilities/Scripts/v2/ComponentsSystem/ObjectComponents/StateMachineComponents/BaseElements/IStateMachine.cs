using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    public interface IStateMachine : IBlackboard
    {
        public Transform transform { get; }
    }
}