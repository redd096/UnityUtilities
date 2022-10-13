using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.FlowField3DPathFinding
{
    /// <summary>
    /// Grid used for pathfinding
    /// </summary>
    [AddComponentMenu("redd096/.FlowField3DPathFinding/Grid FlowField 3D")]
    public class GridFlowField3D : MonoBehaviour
    {
        #region structs

        [System.Serializable]
        public struct TerrainType
        {
            public LayerMask TerrainLayer;
            public int TerrainPenalty;
        }

        #endregion

        #region variables

        [Header("Layer Mask Unwalkable")]
        [SerializeField] protected LayerMask unwalkableMask = default;
        [Tooltip("If not inside these regions, penalty is 0")][SerializeField] protected TerrainType[] penaltyRegions = default;

        [Header("Grid")]
        [SerializeField] protected bool updateOnAwake = true;
        [SerializeField] protected Vector2 gridWorldSize = Vector2.one;
        [SerializeField][Min(0.1f)] protected float nodeDiameter = 1;

        [Header("Gizmos - cyan Area - green/red walkable node - magenta obstacle")]
        [SerializeField] protected bool drawGridArea = false;
        [SerializeField] protected bool drawObstacles = false;
        [SerializeField] protected float alphaNodes = 0.3f;

        //grid
        Node3D[,] grid;

        float nodeRadius;
        float overlapRadius;                        //node radius - 0.05f to not hit adjacent colliders
        Vector2Int gridSize;                        //rows and columns (number of nodes)
        LayerMask penaltyRegionsMask;               //layerMask with every penalty region

        //public properties
        public int MaxSize => gridSize.x * gridSize.y;
        public virtual Vector3 GridWorldPosition => transform.position;
        public Vector2 GridWorldSize => gridWorldSize;
        public float NodeRadius => nodeRadius;

        #endregion

        public Node3D destinationCell;

        public void CreateGrid()
        {
            grid = new Node3D[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPos = new Vector3(nodeDiameter * x + nodeRadius, 0, nodeDiameter * y + nodeRadius);
                    grid[x, y] = new Node3D(worldPos, new Vector2Int(x, y));
                }
            }
        }

        public void CreateCostField()
        {
            Vector3 cellHalfExtents = Vector3.one * nodeRadius;
            int terrainMask = LayerMask.GetMask("Impassible", "RoughTerrain");
            foreach (Node3D curCell in grid)
            {
                Collider[] obstacles = Physics.OverlapBox(curCell.worldPos, cellHalfExtents, Quaternion.identity, terrainMask);
                bool hasIncreasedCost = false;
                foreach (Collider col in obstacles)
                {
                    if (col.gameObject.layer == 8)
                    {
                        curCell.IncreaseCost(255);
                        continue;
                    }
                    else if (!hasIncreasedCost && col.gameObject.layer == 9)
                    {
                        curCell.IncreaseCost(3);
                        hasIncreasedCost = true;
                    }
                }
            }
        }

        public void CreateIntegrationField(Node3D _destinationCell)
        {
            destinationCell = _destinationCell;

            destinationCell.cost = 0;
            destinationCell.bestCost = 0;

            Queue<Node3D> cellsToCheck = new Queue<Node3D>();

            cellsToCheck.Enqueue(destinationCell);

            while (cellsToCheck.Count > 0)
            {
                Node3D curCell = cellsToCheck.Dequeue();
                List<Node3D> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);
                foreach (Node3D curNeighbor in curNeighbors)
                {
                    if (curNeighbor.cost == byte.MaxValue) { continue; }
                    if (curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
                    {
                        curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost);
                        cellsToCheck.Enqueue(curNeighbor);
                    }
                }
            }
        }

        public void CreateFlowField()
        {
            foreach (Node3D curCell in grid)
            {
                List<Node3D> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);

                int bestCost = curCell.bestCost;

                foreach (Node3D curNeighbor in curNeighbors)
                {
                    if (curNeighbor.bestCost < bestCost)
                    {
                        bestCost = curNeighbor.bestCost;
                        curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
                    }
                }
            }
        }

        private List<Node3D> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
        {
            List<Node3D> neighborCells = new List<Node3D>();

            foreach (Vector2Int curDirection in directions)
            {
                Node3D newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
                if (newNeighbor != null)
                {
                    neighborCells.Add(newNeighbor);
                }
            }
            return neighborCells;
        }

        private Node3D GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
        {
            Vector2Int finalPos = orignPos + relativePos;

            if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
            {
                return null;
            }

            else { return grid[finalPos.x, finalPos.y]; }
        }

        public Node3D GetCellFromWorldPos(Vector3 worldPos)
        {
            float percentX = worldPos.x / (gridSize.x * nodeDiameter);
            float percentY = worldPos.z / (gridSize.y * nodeDiameter);

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);
            return grid[x, y];
        }
    }
}