using UnityEngine;
using redd096.Attributes;

namespace redd096
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

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        //vars
        Node3D node;
        GridAStar3D grid;
        Vector3 halfCollider;

        //nodes to calculate
        Node3D leftNode;
        Node3D rightNode;
        Node3D forwardNode;
        Node3D backNode;

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

        /// <summary>
        /// Agent can move on this node or hit some wall?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public bool CanMoveOnThisNode(Node3D node, GridAStar3D grid)
        {
            this.node = node;
            this.grid = grid;
            halfCollider = sizeCollider * 0.5f;

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
            CalculateNodes(
                (node.worldPosition + offset).x - halfCollider.x,
                (node.worldPosition + offset).x + halfCollider.x,
                (node.worldPosition + offset).z + halfCollider.z,
                (node.worldPosition + offset).z - halfCollider.z);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                {
                    //if agent can not move through, return false
                    if (grid.GetNodeByCoordinates(x, y).agentCanMoveThrough == false)
                        return false;
                }
            }

            return true;
        }

        bool CanMove_Sphere()
        {
            //calculate nodes
            CalculateNodes(
                (node.worldPosition + offset).x - radiusCollider,
                (node.worldPosition + offset).x + radiusCollider,
                (node.worldPosition + offset).z + radiusCollider,
                (node.worldPosition + offset).z - radiusCollider);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = backNode.gridPosition.y; y <= forwardNode.gridPosition.y; y++)
                {
                    //if inside radius
                    if (Vector3.Distance(node.worldPosition, grid.GetNodeByCoordinates(x, y).worldPosition) <= radiusCollider)
                    {
                        //if agent can not move through, return false
                        if (grid.GetNodeByCoordinates(x, y).agentCanMoveThrough == false)
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