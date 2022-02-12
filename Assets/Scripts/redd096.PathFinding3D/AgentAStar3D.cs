using UnityEngine;
using redd096.Attributes;

namespace redd096.PathFinding3D
{
    /// <summary>
    /// Used to know size of the agent. When call PathFinding you can pass it as parameter
    /// </summary>
    [AddComponentMenu("redd096/Path Finding A Star/Agent A Star 3D")]
    public class AgentAStar3D : MonoBehaviour
    {
        enum ETypeCollider { sphere, box }

        [Header("Collider Agent")]
        [SerializeField] Vector3 offset = Vector3.zero;
        [SerializeField] ETypeCollider typeCollider = ETypeCollider.box;
        [EnableIf("typeCollider", ETypeCollider.box)] [SerializeField] Vector3 sizeCollider = Vector3.one;
        [EnableIf("typeCollider", ETypeCollider.sphere)] [SerializeField] float radiusCollider = 1;

        [Header("If this object is an obstacle, ignore self (default get from this gameObject)")]
        [SerializeField] ObstacleAStar3D obstacleAStar = default;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        //vars
        Node3D node;
        GridAStar3D grid;
        bool isWaitingPath;
        PathRequest lastPathRequest;

        //nodes to calculate
        Node3D leftNode;
        Node3D rightNode;
        Node3D forwardNode;
        Node3D backNode;
        Node3D nodeToCheck;

        void OnDrawGizmos()
        {
            if (drawDebug)
            {
                Gizmos.color = Color.cyan;

                //draw box
                if (typeCollider == ETypeCollider.box)
                {
                    Gizmos.DrawWireCube(transform.position + offset, sizeCollider);
                }
                //draw sphere
                else
                {
                    Gizmos.DrawWireSphere(transform.position + offset, radiusCollider);
                }

                Gizmos.color = Color.white;
            }
        }

        void Awake()
        {
            //get references
            if (obstacleAStar == null)
                obstacleAStar = GetComponent<ObstacleAStar3D>();
        }

        /// <summary>
        /// Agent can move on this node or hit some wall?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public bool CanMoveOnThisNode(Node3D node, GridAStar3D grid)
        {
            if (node == null || grid == null)
                return false;

            //set vars
            this.node = node;
            this.grid = grid;

            //box
            if (typeCollider == ETypeCollider.box)
            {
                return CanMove_Box();
            }
            //sphere
            else
            {
                return CanMove_Sphere();
            }
        }

        #region private API

        bool CanMove_Box()
        {
            //calculate nodes
            grid.GetNodesExtremesOfABox(node, node.worldPosition + offset, sizeCollider * 0.5f, out leftNode, out rightNode, out backNode, out forwardNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
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

        bool CanMove_Sphere()
        {
            //calculate nodes
            grid.GetNodesExtremesOfABox(node, node.worldPosition + offset, Vector3.one * radiusCollider, out leftNode, out rightNode, out backNode, out forwardNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //if inside radius
                    if (Vector3.Distance(node.worldPosition, nodeToCheck.worldPosition) <= radiusCollider)
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

        bool ThereAreObstacles(Node3D nodeToCheck)
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
            if (PathFindingAStar3D.instance)
            {
                //if still waiting previous path, stop that request
                if (isWaitingPath)
                {
                    PathFindingAStar3D.instance.CancelRequest(lastPathRequest);
                }

                isWaitingPath = true;                                                                   //set is waiting path
                lastPathRequest = new PathRequest(startPosition, targetPosition, func, this);           //save last path request
                PathFindingAStar3D.instance.FindPath(lastPathRequest);
            }
        }

        /// <summary>
        /// Remove last path request from queue. If already processing, do nothiing
        /// </summary>
        public void CancelLastPathRequest()
        {
            //stop request
            if (PathFindingAStar3D.instance)
            {
                if (PathFindingAStar3D.instance.CancelRequest(lastPathRequest))
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