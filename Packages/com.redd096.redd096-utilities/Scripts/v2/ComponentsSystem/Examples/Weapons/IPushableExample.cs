using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// Use this interface on every object that can be pushed
    /// </summary>
    public interface IPushableExample
    {
        void Push(Vector3 force);
    }
}