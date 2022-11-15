using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.PathFinding.FlowField2D
{
    /// <summary>
    /// Grid used for pathfinding
    /// </summary>
    [AddComponentMenu("redd096/.PathFinding/FlowField 2D/Grid FlowField 2D")]
    public class GridFlowField : MonoBehaviour
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

        [Header("Layer Mask Unwalkable (default penalty is 1)")]
        [SerializeField] protected LayerMask unwalkableMask = default;
        [Tooltip("If not inside these regions, penalty is 1")][SerializeField] protected TerrainType[] penaltyRegions = default;

        [Header("Grid")]
        [SerializeField] protected bool updateOnAwake = true;
        [SerializeField] protected Vector2 gridWorldSize = Vector2.one;
        [SerializeField][Min(0.1f)] protected float nodeDiameter = 1;

        [Header("Extensions")]
        [SerializeField] AgentSize_FlowField agentSize = default;
        [SerializeField] bool canMoveDiagonal = true;

        [Header("Gizmos - cyan GridArea - red unwalkable - magenta obstacle - green walkable")]
        [SerializeField] protected bool drawGridArea = false;
        [SerializeField] protected bool drawUnwalkableNodes = false;
        [SerializeField] protected bool drawObstacles = false;
        [SerializeField] protected bool drawWalkableNodes = false;
        [SerializeField] protected bool drawCost = false;
        [SerializeField] protected bool drawArrow = false;
        [SerializeField] protected float alphaNodes = 1f;

        //grid
        Node[,] grid;

        float nodeRadius;
        float overlapRadius;                        //node radius - 0.05f to not hit adjacent colliders
        Vector2Int gridSize;                        //rows and columns (number of nodes)
        LayerMask penaltyRegionsMask;               //layerMask with every penalty region

        //public properties
        public virtual Vector2 GridWorldPosition => transform.position;
        public Vector2 GridWorldSize => gridWorldSize;
        public float NodeRadius => nodeRadius;

        #endregion

        void Awake()
        {
            //create grid
            if (updateOnAwake && IsGridCreated() == false)
                BuildGrid();
        }

        void OnDrawGizmos()
        {
            if (drawGridArea)
            {
                //draw area
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(GridWorldPosition, new Vector2(gridWorldSize.x, gridWorldSize.y));
            }

            //draw every node in grid
            if (drawWalkableNodes || drawUnwalkableNodes || drawObstacles || drawCost || drawArrow)
            {
                if (grid != null)
                {
                    foreach (Node node in grid)
                    {
                        //set color if walkable or not (red = not walkable, green = walkable, magenta = walkable but with obstacles)
                        //Gizmos.color = new Color(1, 1, 1, alphaNodes) * (node.isWalkable ? (drawObstacles && node.GetObstaclesOnThisNode().Count > 0 ? Color.magenta : Color.green) : Color.red);
                        //Gizmos.DrawSphere(node.worldPosition, overlapRadius);

                        //draw if unwalkable
                        if (node.isWalkable == false && drawUnwalkableNodes)
                        {
                            Gizmos.color = new Color(1, 1, 1, alphaNodes) * Color.red;
                            Gizmos.DrawCube(node.worldPosition, Vector2.one * (nodeDiameter - 0.1f));
                        }
                        //draw if obstacle
                        else if (node.GetObstaclesOnThisNode().Count > 0 && drawObstacles)
                        {
                            Gizmos.color = new Color(1, 1, 1, alphaNodes) * Color.magenta;
                            Gizmos.DrawCube(node.worldPosition, Vector2.one * (nodeDiameter - 0.1f));
                        }
                        //draw if walkable
                        else if (node.isWalkable && drawWalkableNodes)
                        {
                            Gizmos.color = new Color(1, 1, 1, alphaNodes) * Color.green;
                            Gizmos.DrawCube(node.worldPosition, Vector2.one * (nodeDiameter - 0.1f));
                        }

#if UNITY_EDITOR
                        //draw cost
                        if (drawCost)
                            Handles.Label(node.worldPosition, node.bestCost.ToString());

                        //draw arrow
                        if (drawArrow)
                        {
                            Handles.ArrowHandleCap(0, node.worldPosition,
                                Quaternion.LookRotation(node.worldPosition + new Vector2(node.bestDirection.x, node.bestDirection.y)), overlapRadius, EventType.Repaint);
                        }
#endif
                    }
                }
            }

            Gizmos.color = Color.white;

            //draw agent size
            agentSize.OnDrawGizmos(transform);
        }

        #region create grid

        protected virtual void SetGrid()
        {
            //set radius for every node
            nodeRadius = nodeDiameter * 0.5f;
            overlapRadius = nodeRadius - 0.05f;

            //set grid size
            gridSize.x = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSize.y = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            //add every walkable regions to a single layer mask, to use when raycast to calculate penalty
            penaltyRegionsMask.value = default;
            foreach (TerrainType terrain in penaltyRegions)
            {
                penaltyRegionsMask.value = penaltyRegionsMask | terrain.TerrainLayer.value;
            }
        }

        void CreateGrid()
        {
            //reset grid and find bottom left world position
            grid = new Node[gridSize.x, gridSize.y];
            Vector2 worldBottomLeft = GridWorldPosition + (Vector2.left * gridWorldSize.x * 0.5f) + (Vector2.down * gridWorldSize.y * 0.5f);

            //create grid
            Vector2 worldPosition;
            bool isWalkable;
            bool agentCanMoveThrough;
            int movementPenalty;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    //find world position and if walkable
                    worldPosition = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                    isWalkable = IsWalkable(worldPosition, out agentCanMoveThrough);

                    //if walkable, calculate movement penalty
                    movementPenalty = 1;
                    if (isWalkable)
                    {
                        CalculateMovementPenalty(worldPosition, out movementPenalty);
                    }

                    //set new node in grid
                    grid[x, y] = new Node(isWalkable, agentCanMoveThrough, worldPosition, x, y, movementPenalty);
                }
            }
        }

        protected virtual bool IsWalkable(Vector2 worldPosition, out bool agentCanMoveThrough)
        {
            //overlap circle (agent can move through only on walkable nodes)
            agentCanMoveThrough = gameObject.scene.GetPhysicsScene2D().OverlapCircle(worldPosition, overlapRadius, unwalkableMask) == false;
            return agentCanMoveThrough;
        }

        void CalculateMovementPenalty(Vector2 worldPosition, out int movementPenalty)
        {
            //raycast to check terrain
            RaycastHit2D hit;
            movementPenalty = 1;
            if (hit = Physics2D.Raycast((Vector3)worldPosition - Vector3.forward, Vector3.forward, 1.1f, penaltyRegionsMask))
            {
                int hittedLayer = hit.collider.gameObject.layer;

                //find terrain inside walkable regions
                foreach (TerrainType region in penaltyRegions)
                {
                    if (region.TerrainLayer == (region.TerrainLayer | (1 << hittedLayer)))  //if this region contains layer
                    {
                        //set this terrain movement penalty
                        movementPenalty = region.TerrainPenalty;
                        break;
                    }
                }
            }
        }

        void SetNeighbours()
        {
            //set neighbours for every node
            int checkX;
            int checkY;
            foreach (Node node in grid)
            {
                node.neighboursCardinalDirections.Clear();
                node.neighbours.Clear();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        //this is the node we are using as parameter
                        if (x == 0 && y == 0)
                            continue;

                        //find grid position
                        checkX = node.gridPosition.x + x;
                        checkY = node.gridPosition.y + y;

                        //if that position is inside the grid, add to neighbours
                        if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                        {
                            node.neighbours.Add(grid[checkX, checkY]);

                            //set another list with neighbours only in cardinal directions (up, down, left, right)
                            if (x == 0 || y == 0)
                                node.neighboursCardinalDirections.Add(grid[checkX, checkY]);
                        }
                    }
                }
            }
        }

        #endregion

        #region flow field

        void ResetFlowFieldGrid()
        {
            //reset every node in the grid (not neighbours or penalty, just reset best cost and direction used for FlowField Pathfinding)
            foreach (Node node in grid)
            {
                //set default values
                node.bestCost = short.MaxValue;
                node.bestDirection = Vector2Int.zero;
            }
        }

        void SetBestCostToThisNode(TargetRequest[] targetRequests)
        {
            foreach (TargetRequest targetRequest in targetRequests)
            {
                Node targetNode = GetNodeFromWorldPosition(targetRequest.savedPosition);

                //set target node at 0 or lower
                targetNode.bestCost = (short)-targetRequest.weight;

                //start from target node
                Queue<Node> cellsToCheck = new Queue<Node>();
                cellsToCheck.Enqueue(targetNode);

                while (cellsToCheck.Count > 0)
                {
                    //get every neighbour in cardinal directions
                    Node currentNode = cellsToCheck.Dequeue();
                    foreach (Node currentNeghbour in currentNode.neighboursCardinalDirections)
                    {
                        //if not walkable, ignore
                        if (currentNeghbour.isWalkable == false) { continue; }

                        //if using agent and can't move on this node, skip to next Neighbour
                        if (agentSize.CanMoveOnThisNode(currentNeghbour, this) == false)
                            continue;

                        //else, calculate best cost
                        if (currentNeghbour.movementPenalty + currentNode.bestCost < currentNeghbour.bestCost)
                        {
                            currentNeghbour.bestCost = (short)(currentNeghbour.movementPenalty + currentNode.bestCost);
                            cellsToCheck.Enqueue(currentNeghbour);
                        }
                    }
                }
            }
        }

        void SetBestDirections()
        {
            //foreach node in the grid
            foreach (Node currentNode in grid)
            {
                //calculate best direction from this node to neighbours
                int bestCost = currentNode.bestCost;
                foreach (Node neighbour in canMoveDiagonal ? currentNode.neighbours : currentNode.neighboursCardinalDirections)
                {
                    //if this best cost is lower then found one, this is the best node to move to
                    if (neighbour.bestCost < bestCost)
                    {
                        //save best cost and set direction
                        bestCost = neighbour.bestCost;
                        currentNode.bestDirection = neighbour.gridPosition - currentNode.gridPosition;
                    }
                }
            }
        }

        #endregion

        #region public API

        /// <summary>
        /// Recreate grid (set which node is walkable and which not)
        /// </summary>
        public void BuildGrid()
        {
            SetGrid();
            CreateGrid();
            SetNeighbours();
        }

        /// <summary>
        /// Set best direction for every node in the grid, to target node
        /// </summary>
        /// <param name="targetRequests"></param>
        public void SetFlowField(TargetRequest[] targetRequests)
        {
            ResetFlowFieldGrid();
            SetBestCostToThisNode(targetRequests);
            SetBestDirections();
        }

        /// <summary>
        /// Is grid created or is null?
        /// </summary>
        /// <returns></returns>
        public bool IsGridCreated()
        {
            //return if the grid was being created
            return grid != null;
        }

        /// <summary>
        /// Is world position inside the grid?
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool IsInsideGrid(Vector2 worldPosition)
        {
            //outside left or right
            if (worldPosition.x < GridWorldPosition.x - (gridWorldSize.x * 0.5f) || worldPosition.x > GridWorldPosition.x + (gridWorldSize.x * 0.5f))
                return false;

            //outside down or up
            if (worldPosition.y < GridWorldPosition.y - (gridWorldSize.y * 0.5f) || worldPosition.y > GridWorldPosition.y + (gridWorldSize.y * 0.5f))
                return false;

            //else is inside
            return true;
        }

        /// <summary>
        /// Get node from world position
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Node GetNodeFromWorldPosition(Vector2 worldPosition)
        {
            //be sure to get right result also if grid doesn't start at [0,0]
            worldPosition -= GridWorldPosition;

            //find percent
            float percentX = (worldPosition.x + gridWorldSize.x * 0.5f) / gridWorldSize.x;
            float percentY = (worldPosition.y + gridWorldSize.y * 0.5f) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            //get coordinates from it
            int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
            int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

            //return node
            return grid[x, y];
        }

        /// <summary>
        /// Get node at grid position
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Node GetNodeByCoordinates(int x, int y)
        {
            return grid[x, y];
        }

        /// <summary>
        /// From a start node, calculate node at the extremes of a box
        /// </summary>
        /// <param name="startNode">start node</param>
        /// <param name="center">center of the box</param>
        /// <param name="halfSize">half size of the box</param>
        /// <param name="leftNode"></param>
        /// <param name="rightNode"></param>
        /// <param name="downNode"></param>
        /// <param name="upNode"></param>
        public void GetNodesExtremesOfABox(Node startNode, Vector2 center, Vector2 halfSize, out Node leftNode, out Node rightNode, out Node downNode, out Node upNode)
        {
            //set left node
            leftNode = startNode;
            for (int x = startNode.gridPosition.x - 1; x >= 0; x--)
            {
                if (grid[x, startNode.gridPosition.y].worldPosition.x + nodeRadius >= (center - halfSize).x)
                    leftNode = grid[x, startNode.gridPosition.y];
                else
                    break;
            }
            //set right node
            rightNode = startNode;
            for (int x = startNode.gridPosition.x + 1; x < gridSize.x; x++)
            {
                if (grid[x, startNode.gridPosition.y].worldPosition.x - nodeRadius <= (center + halfSize).x)
                    rightNode = grid[x, startNode.gridPosition.y];
                else
                    break;
            }
            //set up node
            upNode = startNode;
            for (int y = startNode.gridPosition.y + 1; y < gridSize.y; y++)
            {
                if (grid[startNode.gridPosition.x, y].worldPosition.y - nodeRadius <= (center + halfSize).y)
                    upNode = grid[startNode.gridPosition.x, y];
                else
                    break;
            }
            //set down node
            downNode = startNode;
            for (int y = startNode.gridPosition.y - 1; y >= 0; y--)
            {
                if (grid[startNode.gridPosition.x, y].worldPosition.y + nodeRadius >= (center - halfSize).y)
                    downNode = grid[startNode.gridPosition.x, y];
                else
                    break;
            }
        }

        #endregion
    }

    #region unity editor

#if UNITY_EDITOR

    [CustomEditor(typeof(GridFlowField), true)]
    public class GridFlowFieldEditor : Editor
    {
        private GridFlowField grid;

        private void OnEnable()
        {
            grid = target as GridFlowField;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if (GUILayout.Button("Update Nodes"))
            {
                //update nodes
                grid.BuildGrid();

                //update position of every obstacle
                foreach (ObstacleFlowField obstacle in FindObjectsOfType<ObstacleFlowField>())
                {
                    if (obstacle)
                    {
                        obstacle.SetColliders_Editor();
                        obstacle.UpdatePositionOnGrid(grid);
                    }
                }

                //repaint scene and set undo
                SceneView.RepaintAll();
                Undo.RegisterFullObjectHierarchyUndo(target, "Update Nodes");
            }
        }
    }

#endif

    #endregion
}