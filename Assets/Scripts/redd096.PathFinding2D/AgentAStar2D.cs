using UnityEngine;
using redd096.Attributes;

namespace redd096.PathFinding2D
{
    /// <summary>
    /// Used to know size of the agent. When call PathFinding you can pass it as parameter
    /// </summary>
    [AddComponentMenu("redd096/Path Finding A Star/Agent A Star 2D")]
    public class AgentAStar2D : MonoBehaviour
    {
        enum ETypeCollider { circle, box }

        [Header("Collider Agent")]
        [SerializeField] bool useCustomCollider = false;
        [HideIf("useCustomCollider")] [SerializeField] Collider2D[] colliders = default;
        [ShowIf("useCustomCollider")] [SerializeField] Vector2 offset = Vector2.zero;
        [ShowIf("useCustomCollider")] [SerializeField] ETypeCollider typeCollider = ETypeCollider.box;
        [ShowIf("useCustomCollider")] [EnableIf("typeCollider", ETypeCollider.box)] [SerializeField] Vector2 sizeCollider = Vector2.one;
        [ShowIf("useCustomCollider")] [EnableIf("typeCollider", ETypeCollider.circle)] [SerializeField] float radiusCollider = 1;

        [Header("If this object is an obstacle, ignore self (default get from this gameObject)")]
        [SerializeField] ObstacleAStar2D obstacleAStar = default;

        [Header("DEBUG (only custom collider)")]
        [SerializeField] bool drawDebug = false;

        //vars
        Node2D node;
        GridAStar2D grid;
        bool isWaitingPath;
        PathRequest lastPathRequest;

        //nodes to calculate
        Node2D leftNode;
        Node2D rightNode;
        Node2D upNode;
        Node2D downNode;
        Node2D nodeToCheck;

        void OnDrawGizmos()
        {
            if (drawDebug && useCustomCollider)
            {
                Gizmos.color = Color.cyan;

                //draw box
                if (typeCollider == ETypeCollider.box)
                {
                    Gizmos.DrawWireCube((Vector2)transform.position + offset, sizeCollider);
                }
                //draw circle
                else
                {
                    Gizmos.DrawWireSphere((Vector2)transform.position + offset, radiusCollider);
                }

                Gizmos.color = Color.white;
            }
        }

        void Awake()
        {
            //get references
            if (obstacleAStar == null) obstacleAStar = GetComponent<ObstacleAStar2D>();
            if (colliders == null || colliders.Length <= 0) colliders = GetComponentsInChildren<Collider2D>();
        }

        /// <summary>
        /// Agent can move on this node or hit some wall?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public bool CanMoveOnThisNode(Node2D node, GridAStar2D grid)
        {
            if (node == null || grid == null)
                return false;

            //set vars
            this.node = node;
            this.grid = grid;

            if (useCustomCollider)
            {
                //box
                if (typeCollider == ETypeCollider.box)
                {
                    return CanMove_Box();
                }
                //circle
                else
                {
                    return CanMove_Circle();
                }
            }
            //else use colliders
            else
            {
                return CanMove_Colliders();
            }
        }

        #region private API

        bool CanMove_Box()
        {
            //calculate nodes
            grid.GetNodesExtremesOfABox(node, node.worldPosition + offset, sizeCollider * 0.5f, out leftNode, out rightNode, out downNode, out upNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    ////if agent can not move through OR there are obstacles, return false
                    //if (nodeToCheck.agentCanMoveThrough == false || ThereAreObstacles(nodeToCheck))
                    //if agent can not move through, return false
                    if (nodeToCheck.agentCanMoveThrough == false)
                        return false;
                }
            }

            return true;
        }

        bool CanMove_Circle()
        {
            //calculate nodes
            grid.GetNodesExtremesOfABox(node, node.worldPosition + offset, Vector2.one * radiusCollider, out leftNode, out rightNode, out downNode, out upNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //if inside radius
                    if (Vector2.Distance(node.worldPosition, nodeToCheck.worldPosition) <= radiusCollider)
                    {
                        ////if agent can not move through OR there are obstacles, return false
                        //if (nodeToCheck.agentCanMoveThrough == false || ThereAreObstacles(nodeToCheck))
                        //if agent can not move through, return false
                        if (nodeToCheck.agentCanMoveThrough == false)
                            return false;
                    }
                }
            }

            return true;
        }

        bool CanMove_Colliders()
        {
            //foreach collider
            foreach (Collider2D col in colliders)
            {
                if (col == null)
                    continue;

                //calculate nodes
                grid.GetNodesExtremesOfABox(node, col.bounds.center, col.bounds.extents, out leftNode, out rightNode, out downNode, out upNode);

                //check every node
                for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
                {
                    for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                    {
                        nodeToCheck = grid.GetNodeByCoordinates(x, y);

                        //if inside collider
                        if (Vector2.Distance(col.ClosestPoint(nodeToCheck.worldPosition), nodeToCheck.worldPosition) < Mathf.Epsilon)
                        {
                            ////if agent can not move through OR there are obstacles, return false
                            //if (nodeToCheck.agentCanMoveThrough == false || ThereAreObstacles(nodeToCheck))
                            //if agent can not move through, return false
                            if (nodeToCheck.agentCanMoveThrough == false)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        bool ThereAreObstacles(Node2D nodeToCheck)
        {
            //if there are obstacles
            if (nodeToCheck.GetObstaclesOnThisNode().Count > 0)
            {
                //if there is self obstacle, return false
                if (obstacleAStar)
                {
                    if (nodeToCheck.GetObstaclesOnThisNode().Contains(obstacleAStar))
                        return false;
                }

                //if there are others obstacles, return true
                return true;
            }

            //if there aren't obstacles, return false
            return false;
        }

        #endregion

        #region public API

        /// <summary>
        /// Calculate path, then call function passing the path as parameter. If called before receive path, will stop previous request. Call IsDone() to check when has finished
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="func">function to call when finish processing path. Will pass the path as parameter</param>
        public void FindPath(Vector2 startPosition, Vector2 targetPosition, System.Action<Path> func)
        {
            //call find path on Path Finding
            if (PathFindingAStar2D.instance)
            {
                //if still waiting previous path, stop that request
                if (isWaitingPath)
                {
                    PathFindingAStar2D.instance.CancelRequest(lastPathRequest);
                }

                isWaitingPath = true;                                                                   //set is waiting path
                lastPathRequest = new PathRequest(startPosition, targetPosition, func, this);           //save last path request
                PathFindingAStar2D.instance.FindPath(lastPathRequest);
            }
        }

        /// <summary>
        /// Remove last path request from queue. If already processing, do nothiing
        /// </summary>
        public void CancelLastPathRequest()
        {
            //stop request
            if (PathFindingAStar2D.instance)
            {
                if (PathFindingAStar2D.instance.CancelRequest(lastPathRequest))
                    isWaitingPath = false;                                                              //if succeeded, set is not waiting path
            }
        }

        /// <summary>
        /// Called from pathfinding, when finish processing path
        /// </summary>
        public void OnFinishProcessingPath(PathRequest pathRequest)
        {
            //if finish processing last request (if finish another request but not last, can't set isWaiting at false)
            if (pathRequest == lastPathRequest)
            {
                //set has finished to wait path
                isWaitingPath = false;
            }
        }

        /// <summary>
        /// Is not waiting path (already received or not requested)
        /// </summary>
        /// <returns></returns>
        public bool IsDone()
        {
            return !isWaitingPath;
        }

        #endregion
    }
}