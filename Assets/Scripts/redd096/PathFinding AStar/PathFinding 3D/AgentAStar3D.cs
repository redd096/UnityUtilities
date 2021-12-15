using UnityEngine;
using redd096.Attributes;

namespace redd096
{
    [AddComponentMenu("redd096/Path Finding A Star/Agent A Star 3D")]
    public class AgentAStar3D : MonoBehaviour
    {
        enum ETypeOverlap { sphere, box }

        [Header("Collider Agent")]
        [SerializeField] ETypeOverlap typeOverlap = ETypeOverlap.box;
        [EnableIf("typeOverlap", ETypeOverlap.box)] [SerializeField] Vector3 sizeCollider = Vector3.one;
        [EnableIf("typeOverlap", ETypeOverlap.sphere)] [SerializeField] float radiusCollider = 1;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        Node3D node;
        GridAStar3D grid;
        Node3D leftNode;
        Node3D rightNode;
        Node3D forwardNode;
        Node3D backNode;

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
        public bool CanMoveOnThisNode(Node3D node, GridAStar3D grid)
        {
            this.node = node;
            this.grid = grid;

            //overlap box
            if (typeOverlap == ETypeOverlap.box)
            {
                return OverlapBox();
            }
            //overlap sphere
            else
            {
                return OverlapSphere();
            }
        }

        #region private API

        bool OverlapBox()
        {
            //calculate nodes
            CalculateNodes(
                transform.position.x - sizeCollider.x,
                transform.position.x + sizeCollider.x,
                transform.position.z + sizeCollider.z,
                transform.position.z - sizeCollider.z);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                {
                    //if agent can not overlap, return false
                    if (grid.GetNodeByCoordinates(x, y).agentCanOverlap == false)
                        return false;
                }
            }

            return true;
        }

        bool OverlapSphere()
        {
            //calculate nodes
            CalculateNodes(
                transform.position.x - radiusCollider,
                transform.position.x + radiusCollider,
                transform.position.z + radiusCollider,
                transform.position.z - radiusCollider);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                {
                    //if inside radius
                    if (Vector3.Distance(transform.position, grid.GetNodeByCoordinates(x, y).worldPosition) <= radiusCollider)
                    {
                        //if agent can not overlap, return false
                        if (grid.GetNodeByCoordinates(x, y).agentCanOverlap == false)
                            return false;
                    }
                }
            }

            return true;
        }

        void CalculateNodes(float left, float right, float forward, float back)
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
            forwardNode = node;
            for (int y = node.gridPosition.y; y < grid.GridSize.y; y++)
            {
                if (grid.GetNodeByCoordinates(node.gridPosition.x, y).worldPosition.z <= forward)
                    forwardNode = grid.GetNodeByCoordinates(node.gridPosition.x, y);
                else
                    break;
            }
            //set down node
            backNode = node;
            for (int y = node.gridPosition.y; y >= 0; y--)
            {
                if (grid.GetNodeByCoordinates(node.gridPosition.x, y).worldPosition.z >= back)
                    backNode = grid.GetNodeByCoordinates(node.gridPosition.x, y);
                else
                    break;
            }
        }

        #endregion
    }
}