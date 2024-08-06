using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.PathFinding
{
    [System.Serializable]
    public class GridPathfinding
    {
        [Tooltip("Size of this grid")] public Vector2 GridWorldSize = Vector2.one;
        [Tooltip("Diameter for every node")] public float NodeDiameter = 0.5f;

        [Tooltip("Check this layer on every node. If hit something, it's not walkable")] public LayerMask UnwalkableMask = default;
        [Tooltip("If node node is walkable, check its surface layer for movement penalty")] public FTerrainType[] PenaltyRegions = default;
    }
}