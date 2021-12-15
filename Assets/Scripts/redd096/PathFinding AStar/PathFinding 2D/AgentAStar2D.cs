using UnityEngine;
using redd096.Attributes;

namespace redd096
{
    [AddComponentMenu("redd096/Path Finding A Star/Agent A Star 2D")]
    public class AgentAStar2D : MonoBehaviour
    {
        enum ETypeOverlap { circle, box }

        [Header("Collider Agent")]
        [SerializeField] ETypeOverlap typeOverlap = ETypeOverlap.box;
        [EnableIf("typeOverlap", ETypeOverlap.box)] [SerializeField] Vector2 sizeCollider = Vector2.one;
        [EnableIf("typeOverlap", ETypeOverlap.circle)] [SerializeField] float radiusCollider = 1;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        Node2D node;
        GridAStar2D grid;
        Node2D leftNode;
        Node2D rightNode;
        Node2D upNode;
        Node2D downNode;

        void OnDrawGizmos()
        {
            if (drawDebug)
            {
                Gizmos.color = Color.cyan;

                //draw box overlap
                if (typeOverlap == ETypeOverlap.box)
                {
                    Gizmos.DrawWireCube(transform.position, sizeCollider);
                }
                //draw circle overlap
                else
                {
                    Gizmos.DrawWireSphere(transform.position, radiusCollider);
                }

                Gizmos.color = Color.white;
            }
        }

        /// <summary>
        /// Agent can move on this node or overlap with some wall?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public bool CanMoveOnThisNode(Node2D node, GridAStar2D grid)
        {
            this.node = node;
            this.grid = grid;

            //overlap box
            if (typeOverlap == ETypeOverlap.box)
            {
                return OverlapBox();
            }
            //overlap circle
            else
            {
                return OverlapCircle();
            }
        }

        #region private API

        bool OverlapBox()
        {
            //calculate nodes
            CalculateNodes(
                transform.position.x - sizeCollider.x,
                transform.position.x + sizeCollider.x,
                transform.position.y + sizeCollider.y,
                transform.position.y - sizeCollider.y);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    //if agent can not overlap, return false
                    if (grid.GetNodeByCoordinates(x, y).agentCanOverlap == false)
                        return false;
                }
            }

            return true;
        }

        bool OverlapCircle()
        {
            //calculate nodes
            CalculateNodes(
                transform.position.x - radiusCollider,
                transform.position.x + radiusCollider,
                transform.position.y + radiusCollider,
                transform.position.y - radiusCollider);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    //if inside radius
                    if (Vector2.Distance(transform.position, grid.GetNodeByCoordinates(x, y).worldPosition) <= radiusCollider)
                    {
                        //if agent can not overlap, return false
                        if (grid.GetNodeByCoordinates(x, y).agentCanOverlap == false)
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