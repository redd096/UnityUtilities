using UnityEngine;

namespace redd096.v2.PathFinding
{
    /// <summary>
    /// This is used by path finding algorithms to know which one is the target
    /// </summary>
    public struct FTargetRequest
    {
        [Tooltip("If there are more targets nearby, go to the one with the highest Weight")] public short Weight;
        [Tooltip("This is the node to reach")] public Node TargetNode;
    }
}