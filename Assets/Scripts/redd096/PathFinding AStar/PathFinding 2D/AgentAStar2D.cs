using UnityEngine;
using redd096.Attributes;

namespace redd096
{
    /// <summary>
    /// Used to know size of the agent. When call PathFinding you can pass it as parameter
    /// </summary>
    [AddComponentMenu("redd096/Path Finding A Star/Agent A Star 2D")]
    public class AgentAStar2D : MonoBehaviour
    {
        enum ETypeCollider { circle, box }

        [Header("Collider Agent")]
        [SerializeField] Vector2 offset = Vector2.zero;
        [SerializeField] ETypeCollider typeCollider = ETypeCollider.box;
        [EnableIf("typeCollider", ETypeCollider.box)] [SerializeField] Vector2 sizeCollider = Vector2.one;
        [EnableIf("typeCollider", ETypeCollider.circle)] [SerializeField] float radiusCollider = 1;

        [Header("If this object is an obstacle, ignore self (default get from this gameObject)")]
        [SerializeField] ObstacleAStar2D obstacleAStar = default;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        //vars
        Node2D node;
        GridAStar2D grid;
        Vector2 halfCollider;

        //nodes to calculate
        Node2D leftNode;
        Node2D rightNode;
        Node2D upNode;
        Node2D downNode;

        void OnDrawGizmos()
        {
            if (drawDebug)
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
            if (obstacleAStar == null)
                obstacleAStar = GetComponent<ObstacleAStar2D>();
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

            //box
            if (typeCollider == ETypeCollider.box)
            {
                halfCollider = sizeCollider * 0.5f;
                return CanMove_Box();
            }
            //circle
            else
            {
                return CanMove_Circle();
            }
        }

        #region private API

        bool CanMove_Box()
        {
            //calculate nodes
            grid.GetNodesExtremesOfABox(node,
                new Vector2(node.worldPosition.x + offset.x - halfCollider.x, node.worldPosition.y + offset.y - halfCollider.y),
                new Vector2(node.worldPosition.x + offset.x + halfCollider.x, node.worldPosition.y + offset.y + halfCollider.y),
                out leftNode, out rightNode, out downNode, out upNode);

            //check every node
            Node2D nodeToCheck;
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //if agent can not move through OR there are obstacles, return false
                    if (nodeToCheck.agentCanMoveThrough == false || ThereAreObstacles(nodeToCheck))
                        return false;
                }
            }

            return true;
        }

        bool CanMove_Circle()
        {
            //calculate nodes
            grid.GetNodesExtremesOfABox(node,
                new Vector2(node.worldPosition.x + offset.x - radiusCollider, node.worldPosition.y + offset.y - radiusCollider),
                new Vector2(node.worldPosition.x + offset.x + radiusCollider, node.worldPosition.y + offset.y + radiusCollider),
                out leftNode, out rightNode, out downNode, out upNode);

            //check every node
            Node2D nodeToCheck;
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //if inside radius
                    if (Vector2.Distance(node.worldPosition, nodeToCheck.worldPosition) <= radiusCollider)
                    {
                        //if agent can not move through OR there are obstacles, return false
                        if (nodeToCheck.agentCanMoveThrough == false || ThereAreObstacles(nodeToCheck))
                            return false;
                    }
                }
            }

            return true;
        }

        bool ThereAreObstacles(Node2D nodeToCheck)
        {
            //if there are obstacles
            if (nodeToCheck.obstaclesOnThisNode.Count > 0)
            {
                //if there is only one obstacle and is self, return false
                if (obstacleAStar)
                {
                    if (nodeToCheck.obstaclesOnThisNode.Count == 1 && nodeToCheck.obstaclesOnThisNode.Contains(obstacleAStar))
                        return false;
                }

                //if there are others obstacles, return true
                return true;
            }

            //if there aren't obstacles, return false
            return false;
        }

        #endregion
    }
}