﻿using System.Collections.Generic;
using UnityEngine;

namespace redd096
{
    #region unity editor

#if UNITY_EDITOR

    using UnityEditor;

    [CustomEditor(typeof(GridAStar), true)]
    public class GridAStarEditor : Editor
    {
        private GridAStar gridAStar;

        private void OnEnable()
        {
            gridAStar = target as GridAStar;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Update Nodes"))
            {
                //update nodes
                gridAStar.UpdateGrid();

                //repaint scene and set undo
                SceneView.RepaintAll();
                Undo.RegisterFullObjectHierarchyUndo(target, "Update Nodes");
            }
        }
    }

#endif

    #endregion

    [AddComponentMenu("redd096/Path Finding A Star/Grid A Star")]
    public class GridAStar : MonoBehaviour
    {
        #region variables

        [Header("Use Z instead of Y")]
        [SerializeField] protected bool useZ = true;

        [Header("Layer Mask Unwalkable")]
        [SerializeField] protected LayerMask unwalkableMask = default;

        [Header("Grid")]
        [SerializeField] protected bool updateOnAwake = true;
        [SerializeField] protected Vector2 gridWorldSize = Vector2.one;
        [SerializeField] protected float nodeDiameter = 1;
        [SerializeField] protected float overlapDiameter = 0.9f;

        [Header("Gizmos")]
        [SerializeField] protected float alphaNodes = 0.3f;

        //grid
        Node[,] grid;

        float nodeRadius;
        float overlapRadius;
        Vector2Int gridSize;

        //properties
        public int MaxSize => gridSize.x * gridSize.y;
        public virtual Vector3 GridWorldPosition => transform.position;
        public Vector2 GridWorldSize => gridWorldSize;

        #endregion

        void Awake()
        {
            //create grid
            if (updateOnAwake && IsGridCreated() == false)
                UpdateGrid();
        }

        public void UpdateGrid()
        {
            SetGridSize();
            CreateGrid();
        }

        void OnDrawGizmosSelected()
        {
            //draw area
            Gizmos.color = Color.cyan;
            if (useZ)
                Gizmos.DrawWireCube(GridWorldPosition, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));      //use z
            else
                Gizmos.DrawWireCube(GridWorldPosition, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));      //or y

            //draw every node in grid
            if (grid != null)
            {
                foreach (Node node in grid)
                {
                    //set color if walkable or not
                    Gizmos.color = new Color(1, 1, 1, alphaNodes) * (node.isWalkable ? Color.green : Color.red);
                    //Gizmos.DrawSphere(node.worldPosition, nodeRadius);
                    Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }

        #region create grid

        protected virtual void SetGridSize()
        {
            //set radius for every node
            nodeRadius = nodeDiameter * 0.5f;
            overlapRadius = overlapDiameter * 0.5f;

            //set grid size
            gridSize.x = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSize.y = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        }

        void CreateGrid()
        {
            //reset grid and find bottom left world position
            grid = new Node[gridSize.x, gridSize.y];
            Vector3 worldBottomLeft = useZ ?
                GridWorldPosition + (Vector3.left * gridWorldSize.x / 2) + (Vector3.back * gridWorldSize.y / 2) :      //use z
                GridWorldPosition + (Vector3.left * gridWorldSize.x / 2) + (Vector3.down * gridWorldSize.y / 2);       //or y

            //create grid
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    //find world position and if walkable
                    Vector3 worldPosition = useZ ?
                        worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius) :     //use z
                        worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);           //or y

                    //set new node in grid
                    grid[x, y] = new Node(IsWalkable(worldPosition), worldPosition, x, y);
                }
            }
        }

        protected virtual bool IsWalkable(Vector3 worldPosition)
        {
            return useZ ?
                gameObject.scene.GetPhysicsScene().OverlapSphere(worldPosition, overlapRadius, new Collider[1], unwalkableMask, QueryTriggerInteraction.UseGlobal) <= 0 :   //use 3d (z)
                gameObject.scene.GetPhysicsScene2D().OverlapCircle(worldPosition, overlapRadius, unwalkableMask) == false;                                                  //or 2d (y)
        }

        #endregion

        #region public API

        public bool IsGridCreated()
        {
            //return if the grid was being created
            return grid != null;
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //this is the node we are using as parameter
                    if (x == 0 && y == 0)
                        continue;

                    //find grid position
                    int checkX = node.gridPosition.x + x;
                    int checkY = node.gridPosition.y + y;

                    //if that position is inside the grid, add to neighbours
                    if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPosition(Vector3 worldPosition)
        {
            //be sure to get right result also if grid doesn't start at [0,0]
            worldPosition -= GridWorldPosition;

            //find percent
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = useZ ?
                (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y :     //use z
                (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;      //or y
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            //get coordinates from it
            int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
            int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

            //return node
            return grid[x, y];
        }

        public bool IsInsideGrid(Vector3 worldPosition)
        {
            //outside left or right
            if (worldPosition.x < GridWorldPosition.x - (gridWorldSize.x * 0.5f) || worldPosition.x > GridWorldPosition.x + (gridWorldSize.x * 0.5f))
                return false;

            //outside down or up (if useZ, use back or forward)
            if (useZ && worldPosition.z < GridWorldPosition.z - (gridWorldSize.y * 0.5f) || worldPosition.z > GridWorldPosition.z + (gridWorldSize.y * 0.5f))
                return false;
            else if (useZ == false && worldPosition.y < GridWorldPosition.y - (gridWorldSize.y * 0.5f) || worldPosition.y > GridWorldPosition.y + (gridWorldSize.y * 0.5f))
                return false;

            //else is inside
            return true;
        }

        #endregion
    }
}