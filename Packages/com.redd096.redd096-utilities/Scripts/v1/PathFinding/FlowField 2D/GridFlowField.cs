using UnityEngine;
using redd096.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace redd096.v1.PathFinding.FlowField2D
{
    /// <summary>
    /// Grid used for pathfinding
    /// </summary>
    [AddComponentMenu("redd096/v1/PathFinding/FlowField 2D/Grid FlowField 2D")]
    public class GridFlowField : GridBASE
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

        [Header("Grid")]
        [ShowIf("isGrid_Editor")][SerializeField] bool usedByComposite = false;                         //show only on grids, not on composite
        [SerializeField] bool updateOnAwake = true;
        [EnableIf("isGrid_Editor")][SerializeField] protected Vector2 gridWorldSize = Vector2.one;      //enable only on grids, composite size is based on every grid
        [SerializeField][Min(0.1f)] float nodeDiameter = 1;

        [Header("Layer Mask Unwalkable (default penalty is 1)")]
        [SerializeField] LayerMask unwalkableMask = default;
        [Tooltip("If not inside these regions, penalty is 1")][SerializeField] TerrainType[] penaltyRegions = default;

        [Header("Extensions")]
        [SerializeField] AgentSize_FlowField agentSize = default;

        [Header("Gizmos - cyan GridArea - red unwalkable - magenta obstacle - green walkable")]
        [SerializeField] bool drawGridArea = false;
        [Tooltip("Calculate agent size to check if a node is walkable")][SerializeField] bool calculateAgentForUnwalkableNodes = true;
        [SerializeField] bool drawUnwalkableNodes = false;
        [SerializeField] bool drawObstacles = false;
        [SerializeField] bool drawWalkableNodes = false;
        [SerializeField] bool drawCosts = false;
        [SerializeField] bool drawDirections = false;
        [Tooltip("Enable gizmos also on agent")][SerializeField] bool drawAgentSizeOnEveryNode = false;
        [SerializeField] float alphaNodes = 1f;

        float nodeRadius;
        float overlapRadius;                        //node radius - 0.05f to not hit adjacent colliders
        Vector2Int gridSize;                        //rows and columns (number of nodes)
        LayerMask penaltyRegionsMask;               //layerMask with every penalty region

        //public properties
        public bool UsedByComposite => usedByComposite;
        public Vector2 GridWorldSize => gridWorldSize;
        public float NodeRadius => nodeRadius;

        //private properties, used only by editor
        bool isGrid_Editor => GetType() == typeof(GridFlowField);

        #endregion

        void Awake()
        {
            //doesn't need to create grid, if used by a composite
            if (usedByComposite)
                return;

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

            //don't draw other gizmos if this grid is used by a composite
            if (usedByComposite)
                return;

            //draw every node in grid
            if (drawWalkableNodes || drawUnwalkableNodes || drawObstacles || drawCosts || drawDirections || drawAgentSizeOnEveryNode)
            {
                if (grid != null)
                {
                    foreach (Node node in grid)
                    {
                        //set color if walkable or not (red = not walkable, green = walkable, magenta = walkable but with obstacles)
                        //Gizmos.color = new Color(1, 1, 1, alphaNodes) * (node.isWalkable ? (drawObstacles && node.GetObstaclesOnThisNode().Count > 0 ? Color.magenta : Color.green) : Color.red);
                        //Gizmos.DrawSphere(node.worldPosition, overlapRadius);

                        //draw if unwalkable
                        if (drawUnwalkableNodes && (node.IsWalkable == false || (calculateAgentForUnwalkableNodes && agentSize.CanMoveOnThisNode(node, this) == false)))
                        {
                            Gizmos.color = new Color(1, 1, 1, alphaNodes) * Color.red;
                            Gizmos.DrawCube(node.worldPosition, Vector2.one * (nodeDiameter - (nodeDiameter * 0.1f)));
                        }
                        //draw if obstacle
                        else if (drawObstacles && node.GetObstaclesOnThisNode().Count > 0)
                        {
                            Gizmos.color = new Color(1, 1, 1, alphaNodes) * Color.magenta;
                            Gizmos.DrawCube(node.worldPosition, Vector2.one * (nodeDiameter - (nodeDiameter * 0.1f)));
                        }
                        //draw if walkable
                        else if (drawWalkableNodes && node.IsWalkable && (calculateAgentForUnwalkableNodes == false || agentSize.CanMoveOnThisNode(node, this)))
                        {
                            Gizmos.color = new Color(1, 1, 1, alphaNodes) * Color.green;
                            Gizmos.DrawCube(node.worldPosition, Vector2.one * (nodeDiameter - (nodeDiameter * 0.1f)));
                        }

                        Gizmos.color = Color.white;

#if UNITY_EDITOR
                        //draw cost
                        if (drawCosts)
                            Handles.Label(node.worldPosition, node.bestCost.ToString());

                        //draw arrow direction
                        if (drawDirections)
                        {
                            Gizmos.DrawWireCube(node.worldPosition + new Vector2(node.bestDirection.x, node.bestDirection.y) * overlapRadius, Vector2.one * 0.05f);
                            if (node.bestDirection != Vector2Int.zero)
                                Gizmos.DrawLine(node.worldPosition, node.worldPosition + new Vector2(node.bestDirection.x, node.bestDirection.y) * overlapRadius);
                            //Handles.ArrowHandleCap(0, node.worldPosition, Quaternion.LookRotation(new Vector2(node.bestDirection.x, node.bestDirection.y)), overlapRadius, EventType.Repaint);
                        }
#endif
                        //draw agent on every node
                        if (drawAgentSizeOnEveryNode)
                            agentSize.OnDrawGizmos(node.worldPosition);
                    }
                }
            }

            //draw agent size
            agentSize.OnDrawGizmos(transform.position);
        }

        #region create grid

        protected override void SetGrid()
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

        protected override void CreateGrid()
        {
            //reset grid and find bottom left world position
            grid = new Node[gridSize.x, gridSize.y];
            Vector2 worldBottomLeft = GridWorldPosition + (Vector2.left * gridWorldSize.x * 0.5f) + (Vector2.down * gridWorldSize.y * 0.5f);

            //create grid
            Vector2 worldPosition;
            bool isWalkable;
            bool agentCanOverlap;
            int movementPenalty;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    //find world position and if walkable
                    worldPosition = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                    isWalkable = IsWalkable(worldPosition, out agentCanOverlap);

                    //if walkable, calculate movement penalty
                    movementPenalty = 1;
                    if (isWalkable)
                    {
                        CalculateMovementPenalty(worldPosition, out movementPenalty);
                    }

                    //set new node in grid
                    grid[x, y] = new Node(isWalkable, agentCanOverlap, worldPosition, x, y, movementPenalty);
                }
            }
        }

        protected virtual bool IsWalkable(Vector2 worldPosition, out bool agentCanOverlap)
        {
            //overlap circle (normally, agent can move overlap only with walkable nodes)
            agentCanOverlap = gameObject.scene.GetPhysicsScene2D().OverlapCircle(worldPosition, overlapRadius, unwalkableMask) == false;
            return agentCanOverlap;
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

        protected override void SetNeighbours()
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

        #region public API

        /// <summary>
        /// Set best direction for every node in the grid, to target node
        /// </summary>
        /// <param name="targetRequests"></param>
        /// <param name="canMoveDiagonal">can move diagonal or only horizontal and vertical?</param>
        public override void SetFlowField(TargetRequest[] targetRequests, bool canMoveDiagonal)
        {
            ResetFlowFieldGrid();
            SetBestCosts(targetRequests, (Node currentNeighbour) => agentSize.CanMoveOnThisNode(currentNeighbour, this));
            SetBestDirections(canMoveDiagonal);
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
        /// Get node from world position (or nearest one)
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public override Node GetNodeFromWorldPosition(Vector2 worldPosition)
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
        private ObstacleFlowField[] obstacles;
        SerializedProperty usedByComposite;
        SerializedProperty gridWorldSize;
        SerializedProperty drawGridArea;

        private void OnEnable()
        {
            grid = target as GridFlowField;
            usedByComposite = serializedObject.FindProperty("usedByComposite");
            gridWorldSize = serializedObject.FindProperty("gridWorldSize");
            drawGridArea = serializedObject.FindProperty("drawGridArea");
        }

        public override void OnInspectorGUI()
        {
            //when this grid is used by a composite, show only few variables
            if (usedByComposite.boolValue)
            {
                //show script on top (as base.OnInspectorGUI())
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(grid), serializedObject.GetType(), false);
                GUI.enabled = true;

                //show only few variables
                EditorGUILayout.PropertyField(usedByComposite);
                EditorGUILayout.PropertyField(gridWorldSize);

                EditorGUILayout.PropertyField(drawGridArea);
                EditorGUILayout.HelpBox("Update nodes on CompositeGrid to see nodes or obstacles", MessageType.Info);

                //apply changes to serialized properties
                serializedObject.ApplyModifiedProperties();
                return;
            }

            //==============================================

            //else show normally everything
            base.OnInspectorGUI();

            //==============================================

            GUILayout.Space(10);

            if (GUILayout.Button("Update Nodes"))
            {
                //set undo
                obstacles = FindObjectsOfType<ObstacleFlowField>();
                System.Collections.Generic.List<Object> objs = new System.Collections.Generic.List<Object>(obstacles);  //obstacles
                objs.Add(target);                                                                                       //grid
                Undo.RecordObjects(objs.ToArray(), "Update Nodes");

                //update nodes
                grid.BuildGrid();

                //update position of every obstacle
                foreach (ObstacleFlowField obstacle in obstacles)
                {
                    if (obstacle)
                    {
                        obstacle.SetColliders_Editor();
                        obstacle.UpdatePositionOnGrid(grid);
                    }
                }

                //repaint scene
                SceneView.RepaintAll();
            }
        }
    }

#endif

    #endregion
}