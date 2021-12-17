using UnityEngine;
using redd096.Attributes;

namespace redd096
{
    [AddComponentMenu("redd096/Path Finding A Star/Agent A Star 2D")]
    public class AgentAStar2D : MonoBehaviour
    {
        enum ETypeCollider { circle, box }

        [Header("Collider Agent")]
        [SerializeField] Vector2 offset = Vector2.zero;
        [SerializeField] ETypeCollider typeCollider = ETypeCollider.box;
        [EnableIf("typeCollider", ETypeCollider.box)] [SerializeField] Vector2 sizeCollider = Vector2.one;
        [EnableIf("typeCollider", ETypeCollider.circle)] [SerializeField] float radiusCollider = 1;

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

        /// <summary>
        /// Agent can move on this node or hit some wall?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public bool CanMoveOnThisNode(Node2D node, GridAStar2D grid)
        {
            this.node = node;
            this.grid = grid;
            halfCollider = sizeCollider * 0.5f;

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

        #region private API

        bool CanMove_Box()
        {
            //calculate nodes
            CalculateNodes(
                transform.position.x + offset.x - halfCollider.x,
                transform.position.x + offset.x + halfCollider.x,
                transform.position.y + offset.y + halfCollider.y,
                transform.position.y + offset.y - halfCollider.y);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    //if agent can not move through, return false
                    if (grid.GetNodeByCoordinates(x, y).agentCanMoveThrough == false)
                        return false;
                }
            }

            return true;
        }

        bool CanMove_Circle()
        {
            //calculate nodes
            CalculateNodes(
                transform.position.x + offset.x - radiusCollider,
                transform.position.x + offset.x + radiusCollider,
                transform.position.y + offset.y + radiusCollider,
                transform.position.y + offset.y - radiusCollider);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    //if inside radius
                    if (Vector2.Distance(transform.position, grid.GetNodeByCoordinates(x, y).worldPosition) <= radiusCollider)
                    {
                        //if agent can not move through, return false
                        if (grid.GetNodeByCoordinates(x, y).agentCanMoveThrough == false)
                            return false;
                    }
                }
            }

            return true;
        }

        void CalculateNodes(float left, float right, float up, float down)
        {
            //set left node
            leftNode = node;
            for (int x = node.gridPosition.x; x >= 0; x--)
            {
                if (grid.GetNodeByCoordinates(x, node.gridPosition.y).worldPosition.x >= left)
                    leftNode = grid.GetNodeByCoordinates(x, node.gridPosition.y);
                else
                    break;
            }
            //set right node
            rightNode = node;
            for (int x = node.gridPosition.x; x < grid.GridSize.x; x++)
            {
                if (grid.GetNodeByCoordinates(x, node.gridPosition.y).worldPosition.x <= right)
                    rightNode = grid.GetNodeByCoordinates(x, node.gridPosition.y);
                else
                    break;
            }
            //set up node
            upNode = node;
            for (int y = node.gridPosition.y; y < grid.GridSize.y; y++)
            {
                if (grid.GetNodeByCoordinates(node.gridPosition.x, y).worldPosition.y <= up)
                    upNode = grid.GetNodeByCoordinates(node.gridPosition.x, y);
                else
                    break;
            }
            //set down node
            downNode = node;
            for (int y = node.gridPosition.y; y >= 0; y--)
            {
                if (grid.GetNodeByCoordinates(node.gridPosition.x, y).worldPosition.y >= down)
                    downNode = grid.GetNodeByCoordinates(node.gridPosition.x, y);
                else
                    break;
            }
        }

        #endregion
    }
}