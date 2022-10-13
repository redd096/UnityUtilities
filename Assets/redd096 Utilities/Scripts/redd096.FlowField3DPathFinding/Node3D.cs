using UnityEngine;

namespace redd096.FlowField3DPathFinding
{
    public class Node3D
    {
        public Vector3 worldPos;
        public Vector2Int gridIndex;
        public byte cost;
        public ushort bestCost;
        public GridDirection bestDirection;

        public Node3D(Vector3 _worldPos, Vector2Int _gridIndex)
        {
            worldPos = _worldPos;
            gridIndex = _gridIndex;
            cost = 1;
            bestCost = ushort.MaxValue;
            bestDirection = GridDirection.None;
        }

        public void IncreaseCost(int amnt)
        {
            if (cost == byte.MaxValue) { return; }
            if (amnt + cost >= 255) { cost = byte.MaxValue; }
            else { cost += (byte)amnt; }
        }
    }
}