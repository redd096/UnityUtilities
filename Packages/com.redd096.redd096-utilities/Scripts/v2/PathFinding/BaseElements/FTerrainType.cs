using UnityEngine;

/// <summary>
/// Set movement penalty for the surfaces
/// </summary>
[System.Serializable]
public struct FTerrainType
{
    [Tooltip("Every node on surface with this layer will have this penalty")] public LayerMask TerrainLayer;
    [Tooltip("The penalty for this terrain")] public int TerrainPenalty;
}